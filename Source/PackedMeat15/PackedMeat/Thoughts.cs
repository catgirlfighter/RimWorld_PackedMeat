using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection;

namespace PackedMeat
{
    class Thoughts
    {
        [HarmonyPatch(typeof(RimWorld.FoodUtility), "GetMeatSourceCategory", new Type[] { typeof(ThingDef) })]
        static class FoodUtility_GetMeatSourceCategory_PackedMeatPatch
        {
            static bool Prefix(ref MeatSourceCategory __result, ThingDef source)
            {
                if (source == PackedMeat.MysteriousPackDef)
                {
                    __result = MeatSourceCategory.Humanlike;
                    return false;
                }
                if (source == PackedMeat.OddPackDef)
                {
                    __result = MeatSourceCategory.Insect;
                    return false;
                }
                //
                return true;
            }
        }

        //public static List<ThoughtDef> ThoughtsFromIngesting(Pawn ingester, Thing foodSource, ThingDef foodDef)
        [HarmonyPatch(typeof(RimWorld.FoodUtility), "ThoughtsFromIngesting", new Type[] { typeof(Pawn), typeof(Thing), typeof(ThingDef) })]
        static class FoodUtility_ThoughtsFromIngesting_PackedMeatPatch
        {
            //private static void TryAddIngestThought(Pawn ingester, ThoughtDef def, Precept fromPrecept, List<FoodUtility.ThoughtFromIngesting> ingestThoughts, ThingDef foodDef, MeatSourceCategory meatSourceCategory)
            static MethodInfo LTryAddIngestThought = null;

            static void Prepare()
            {
                LTryAddIngestThought = AccessTools.Method(typeof(FoodUtility), "TryAddIngestThought");
            }

            static bool Prefix(ref List<FoodUtility.ThoughtFromIngesting> __result, Pawn ingester, Thing foodSource, ThingDef foodDef)
            {
                if (foodDef != PackedMeat.MysteriousPackDef && foodDef != PackedMeat.OddPackDef && foodDef != PackedMeat.RegularPackDef)
                    return true;
                //
                var compIngredients = foodSource.TryGetComp<CompIngredients>();
                if (compIngredients == null)
                    return true;
                //
                List<FoodUtility.ThoughtFromIngesting> l = new List<FoodUtility.ThoughtFromIngesting>();
                foreach (var ing in compIngredients.ingredients)
                {
                    Thing dummy = ThingMaker.MakeThing(ing);
                    l.AddRange(FoodUtility.ThoughtsFromIngesting(ingester, dummy, ing));
                }
                __result = l;
                return false;
            }
        }
    }
}