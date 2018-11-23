using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using RimWorld;


namespace PackedMeat
{
    class RandomIngredients
    {
        [HarmonyPatch(typeof(ThingMaker), "MakeThing", new Type[] { typeof(ThingDef), typeof(ThingDef) })]
        static class ThingMaker_MakeThing_PackedMeat
        {
            static IEnumerable<ThingDef> humanlikes = null;
            static IEnumerable<ThingDef> disgusting = null;
            static IEnumerable<ThingDef> regular = null;

            static bool Prepare(HarmonyInstance instance)
            {
                //Settings.CSLoaded = instance. ("net.avilmask.rimworld.mod.CommonSense");
                //Log.Message("CSLoaded" + Settings.CSLoaded.ToString());
                return Settings.CommonSenseMod != null;
            }

            [HarmonyAfter(new string[] { "net.avilmask.rimworld.mod.CommonSense" })]
            static void Postfix(Thing __result, ThingDef def, ThingDef stuff)
            {
                if (!Settings.add_meal_ingredients || __result == null || !__result.def.IsIngestible)
                    return;

                CompIngredients ings = __result.TryGetComp<CompIngredients>();
                if (ings == null || ings.ingredients.Count > 0)
                    return;

                if (def == PackedMeat.MysteriousPackDef)
                {   // !x.comps.Any(y => y.compClass == typeof(CompIngredients))
                    if (ings.ingredients.Count > 0)
                        ings.ingredients.Clear();
                    else
                        return;

                    if (humanlikes == null)
                        humanlikes = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsIngestible && x.ingestible.foodType == FoodTypeFlags.Meat 
                        && x.ingestible.sourceDef != null && x.ingestible.sourceDef.race != null &&  x.ingestible.sourceDef.race.Humanlike);

                    ThingDef td = humanlikes.RandomElement();
                    if (td != null) ings.RegisterIngredient(td);
                }
                else if (def == PackedMeat.OddPackDef)
                {
                    if (ings.ingredients.Count > 0)
                        ings.ingredients.Clear();
                    else
                        return;

                    if (disgusting == null)
                        disgusting = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsIngestible && x.ingestible.foodType == FoodTypeFlags.Meat
                        && x.ingestible.sourceDef != null && x.ingestible.sourceDef.race != null && !x.ingestible.sourceDef.race.Humanlike
                        && x.ingestible.specialThoughtAsIngredient != null && x.ingestible.specialThoughtAsIngredient.stages[0].baseMoodEffect < 0);

                    ThingDef td = disgusting.RandomElement();
                    if (td != null) ings.RegisterIngredient(td);
                }
                else if (def == PackedMeat.RegularPackDef)
                {
                    if (ings.ingredients.Count > 0)
                        ings.ingredients.Clear();
                    else
                        return;

                    if (regular == null)
                        regular = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsIngestible && x.ingestible.foodType == FoodTypeFlags.Meat
                        && x.ingestible.sourceDef != null && x.ingestible.sourceDef.race != null && !x.ingestible.sourceDef.race.Humanlike
                        && x.ingestible.specialThoughtAsIngredient == null);

                    ThingDef td = regular.RandomElement();
                    if (td != null) ings.RegisterIngredient(td);
                }
            }

        }

    }
}
