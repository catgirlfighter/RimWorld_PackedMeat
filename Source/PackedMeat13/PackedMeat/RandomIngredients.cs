using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Verse;
using RimWorld;


namespace PackedMeat
{
    /*
    class RandomIngredients
    {
        [HarmonyPatch(typeof(ThingMaker), "MakeThing", new Type[] { typeof(ThingDef), typeof(ThingDef) })]
        static class ThingMaker_MakeThing_PackedMeat
        {
            static IEnumerable<ThingDef> humanlikes = null;
            static IEnumerable<ThingDef> disgusting = null;
            static IEnumerable<ThingDef> regular = null;
            
            [HarmonyAfter(new string[] { "net.avilmask.rimworld.mod.CommonSense" })]
            static void Postfix(Thing __result, ThingDef def, ThingDef stuff)
            {
                if (Settings.CommonSenseMod == null || __result == null || !__result.def.IsIngestible)
                    return;

                CompIngredients ings = __result.TryGetComp<CompIngredients>();
                if (ings == null || ings.ingredients.Count == 0)
                    return;

                if (def == PackedMeat.MysteriousPackDef)
                {
                    ings.ingredients.Clear();
                    if (humanlikes == null)
                        humanlikes = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsIngestible && x.ingestible.foodType == FoodTypeFlags.Meat 
                        && x.ingestible.sourceDef != null && x.ingestible.sourceDef.race != null &&  x.ingestible.sourceDef.race.Humanlike);

                    ThingDef td = humanlikes.RandomElement();
                    if (td != null) ings.RegisterIngredient(td);
                }
                else if (def == PackedMeat.OddPackDef)
                {
                    ings.ingredients.Clear();
                    if (disgusting == null)
                        disgusting = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsIngestible && x.ingestible.foodType == FoodTypeFlags.Meat
                        && x.ingestible.sourceDef != null && x.ingestible.sourceDef.race != null && !x.ingestible.sourceDef.race.Humanlike
                        && x.ingestible.specialThoughtAsIngredient != null && x.ingestible.specialThoughtAsIngredient.stages[0].baseMoodEffect < 0);

                    ThingDef td = disgusting.RandomElement();
                    if (td != null) ings.RegisterIngredient(td);
                }
                else if (def == PackedMeat.RegularPackDef)
                {
                    ings.ingredients.Clear();
                    if (regular == null)
                        regular = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsIngestible && x.ingestible.foodType == FoodTypeFlags.Meat
                        && x.ingestible.sourceDef != null && x.ingestible.sourceDef.race != null && !x.ingestible.sourceDef.race.Humanlike
                        && x.ingestible.specialThoughtAsIngredient == null);

                    ThingDef td = regular.RandomElement();
                    if (td != null) ings.RegisterIngredient(td);
                }
            }

            static bool Prepare(ModContentPack instance)
            {
                return Settings.CommonSenseMod != null;
            }

        }
    }
    */
}
