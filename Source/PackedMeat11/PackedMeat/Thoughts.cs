using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PackedMeat
{
    class Thoughts
    {
        //Rimword.FoodUtiliy
        //public static bool IsHumanlikeMeat(ThingDef def)
        //simple ctrutch. 'HumanlikeMeat' is always humanlike meat
        [HarmonyPatch(typeof(RimWorld.FoodUtility), "IsHumanlikeMeat", new Type[] { typeof(ThingDef) })]
        static class FoodUtility_IsHumanlikeMeat_RegularMeatPatch
        {
            static void Postfix(ref bool __result, ref ThingDef def)
            {
                __result = __result || def.defName == "avMysteriousMeatPack";
            }
        }

        //public static List<ThoughtDef> ThoughtsFromIngesting(Pawn ingester, Thing foodSource, ThingDef foodDef)
        //if parent food is raw, then it's not cooked (IKR)
        [HarmonyPatch(typeof(RimWorld.FoodUtility), "ThoughtsFromIngesting", new Type[] { typeof(Pawn), typeof(Thing), typeof(ThingDef) })]
        static class FoodUtility_ThoughtsFromIngesting_RegularMeatPatch
        {
            static void Postfix(ref List<ThoughtDef> __result, Pawn ingester, Thing foodSource, ThingDef foodDef)
            {
                if (__result == null || !__result.Any())
                    return;

                CompIngredients compIngredients = foodSource.TryGetComp<CompIngredients>();
                if (compIngredients != null)
                {
                    if (foodDef.defName == "avOddMeatPack" && compIngredients.ingredients.Count > 0)
                        __result.Remove(foodDef.ingestible.specialThoughtDirect);

                    foreach (var ingredient in (compIngredients.ingredients))
                        if (foodDef.ingestible.tasteThought == ingredient.ingestible.tasteThought)
                        {
                            if (ingredient.ingestible.specialThoughtAsIngredient != null)
                                __result.Remove(ingredient.ingestible.specialThoughtAsIngredient);

                            if (ingredient.ingestible.specialThoughtDirect != null)
                                if (!__result.Contains(ingredient.ingestible.specialThoughtDirect))
                                    __result.Add(ingredient.ingestible.specialThoughtDirect);
                        }
                }
            }
        }
    }
}
