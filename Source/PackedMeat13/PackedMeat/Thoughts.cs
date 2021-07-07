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
        //Rimword.FoodUtiliy
        //public static bool IsHumanlikeMeat(ThingDef def)
        //simple ctrutch. 'HumanlikeMeat' is always humanlike meat
        //[HarmonyPatch(typeof(RimWorld.FoodUtility), "IsHumanlikeMeat", new Type[] { typeof(ThingDef) })]
        //static class FoodUtility_IsHumanlikeMeat_RegularMeatPatch
        //{
        //    static void Postfix(ref bool __result, ref ThingDef def)
        //    {
        //        __result = __result || def.defName == "avMysteriousMeatPack";
        //    }
        //}
        [HarmonyPatch(typeof(RimWorld.FoodUtility), "GetMeatSourceCategory", new Type[] { typeof(ThingDef) })]
        static class FoodUtility_GetMeatSourceCategory_PackedMeatPatch
        {
            static bool Prefix(ref MeatSourceCategory __result, ThingDef source)
            {
                if (source.defName == "avMysteriousMeatPack")
                {
                    __result = MeatSourceCategory.Humanlike;
                    return false;
                }
                //
                return true;
            }
        }

        //public static bool IsVeneratedAnimalMeatOrCorpse(ThingDef foodDef, Pawn ingester, Thing source = null)
        /*
        [HarmonyPatch(typeof(RimWorld.FoodUtility), "IsVeneratedAnimalMeatOrCorpse", new Type[] { typeof(ThingDef), typeof(Pawn), typeof(Thing) })]
        static class FoodUtility_IsVeneratedAnimalMeatOrCorpse_PackedMeatPatch
        {
            static void Postfix(ref bool __result, ThingDef foodDef, Pawn ingester, Thing source)
            {
                if (__result || source == null)
                    return;
                //
                CompIngredients ingredientsComp = source.TryGetComp<CompIngredients>();
                if (ingredientsComp == null || ingredientsComp.ingredients.NullOrEmpty())
                    return;
                //
                foreach (var def in ingredientsComp.ingredients)
                    if (FoodUtility.IsVeneratedAnimalMeatOrCorpse(def, ingester))
                    {
                        __result = true;
                        return;
                    }
            }
        }
        */

        //public static List<ThoughtDef> ThoughtsFromIngesting(Pawn ingester, Thing foodSource, ThingDef foodDef)
        //if parent food is raw then it's not cooked (IKR)
        [HarmonyPatch(typeof(RimWorld.FoodUtility), "ThoughtsFromIngesting", new Type[] { typeof(Pawn), typeof(Thing), typeof(ThingDef) })]
        static class FoodUtility_ThoughtsFromIngesting_PackedMeatPatch
        {
            //private static void TryAddIngestThought(Pawn ingester, ThoughtDef def, Precept fromPrecept, List<FoodUtility.ThoughtFromIngesting> ingestThoughts, ThingDef foodDef, MeatSourceCategory meatSourceCategory)
            static MethodInfo LTryAddIngestThought = null;

            static void Prepare()
            {
                LTryAddIngestThought = AccessTools.Method(typeof(FoodUtility), "TryAddIngestThought");
            }

            static void Postfix(ref List<FoodUtility.ThoughtFromIngesting> __result, Pawn ingester, Thing foodSource, ThingDef foodDef)
            {
                if (__result == null || !__result.Any())
                    return;

                CompIngredients compIngredients = foodSource.TryGetComp<CompIngredients>();
                if (compIngredients != null)
                {
                    if (foodDef.defName == "avOddMeatPack" && compIngredients.ingredients.Count > 0)
                        __result.RemoveAll(x => x.thought == foodDef.ingestible.specialThoughtDirect);

                    foreach (var ingredient in (compIngredients.ingredients))
                        if (foodDef.ingestible.tasteThought == ingredient.ingestible.tasteThought)
                        {
                            if (ingredient.ingestible.specialThoughtAsIngredient != null)
                                __result.RemoveAll(x => x.thought == ingredient.ingestible.specialThoughtAsIngredient);

                            if (ingredient.ingestible.specialThoughtDirect != null)
                            {
                                bool b = false;
                                foreach (var t in __result)
                                    if (b = t.thought == ingredient.ingestible.specialThoughtDirect)
                                        LTryAddIngestThought.Invoke(null, new object[] { ingester, ingredient.ingestible.specialThoughtDirect, null, __result, ingredient, FoodUtility.GetMeatSourceCategory(ingredient) } );

                            }
                        }
                }
            }
        }
    }
}