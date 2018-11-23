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
            
            static bool Prepare(HarmonyInstance instance)
            {
                //Settings.CSLoaded = instance. ("net.avilmask.rimworld.mod.CommonSense");
                //Log.Message("CSLoaded" + Settings.CSLoaded.ToString());
                return Settings.CommonSenseMod != null;
            }

            [HarmonyBefore(new string[] { "net.avilmask.rimworld.mod.CommonSense" })]
            static void Postfix(Thing __result, ThingDef def, ThingDef stuff)
            {
                if (!Settings.add_meal_ingredients || __result == null || !__result.def.IsIngestible)
                    return;

                CompIngredients ings = __result.TryGetComp<CompIngredients>();
                if (ings == null || ings.ingredients.Count > 0)
                    return;

                if (def == PackedMeat.MysteriousPackDef)
                {
                    if (humanlikes == null)
                        humanlikes = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsIngestible && !x.comps.Any(y => y.compClass == typeof(CompIngredients)) &&
                        x.ingestible.foodType == FoodTypeFlags.Meat && FoodUtility.IsHumanlikeMeat(x));

                    ThingDef td = humanlikes.RandomElement();
                    if (td != null) ings.RegisterIngredient(td);
                }else if (def == PackedMeat.OddPackDef)
                {
                    if (disgusting == null)
                        disgusting = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsIngestible && !x.comps.Any(y => y.compClass == typeof(CompIngredients)) &&
                        x.ingestible.foodType == FoodTypeFlags.Meat && x.ingestible.specialThoughtAsIngredient != null && x.ingestible.specialThoughtAsIngredient.stages[0].baseMoodEffect < 0);

                    ThingDef td = disgusting.RandomElement();
                    if (td != null) ings.RegisterIngredient(td);
                }
                
            }

        }

    }
}
