using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PackedMeat
{
    class ButcherTask
    {
        // Verse.Pawn
        // public override IEnumerable<Thing> ButcherProducts(Pawn butcher, float efficiency)
        [HarmonyPatch(typeof(Pawn), "ButcherProducts", new Type[] { typeof(Pawn), typeof(float) })]
        static class Pawn_ButcherProducts_RegularMeatPatch
        {

            static void Postfix(ref IEnumerable<Thing> __result, ref Pawn __instance)
            {

                if (!Settings.pack_on_butchering || __result == null || !__result.Any())
                    return;

                var things = __result.ToList();
                Thing meat = things.Find(x => x.def.IsIngestible && x.def.ingestible.foodType == FoodTypeFlags.Meat);

                if (meat == null || meat.def == ThingDefOf.Meat_Twisted || meat.def.ingestible.ateEvent != null)
                    return;

                ThingDef d;
                if (FoodUtility.GetMeatSourceCategory(meat.def) == MeatSourceCategory.Humanlike)
                    d = PackedMeat.MysteriousPackDef;
                else if (FoodUtility.GetMeatSourceCategory(meat.def) == MeatSourceCategory.Insect)
                    d = PackedMeat.OddPackDef;
                else
                    d = PackedMeat.RegularPackDef;

                ThingWithComps t = (ThingWithComps)ThingMaker.MakeThing(d, null);

                t.stackCount = meat.stackCount;

                CompIngredients ingredientsComp = t.TryGetComp<CompIngredients>();

                if (ingredientsComp != null)
                {
                    ingredientsComp.ingredients.Clear();
                    ingredientsComp.RegisterIngredient(meat.def);
                }

                things.Remove(meat);
                things.Insert(0, t);

                __result = things;
            }
        }
    }
}
