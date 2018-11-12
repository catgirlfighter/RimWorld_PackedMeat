using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;

namespace PackedMeat
{
    class CookTask
    {
        //Verse.GenRecipe
        //public static IEnumerable<Thing> MakeRecipeProducts(RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient, IBillGiver billGiver)
        [HarmonyPatch(typeof(GenRecipe), "MakeRecipeProducts", new Type[] { typeof(RecipeDef), typeof(Pawn), typeof(List<Thing>), typeof(Thing), typeof(IBillGiver) })]
        static class GenRecipe_MakeRecipeProducts_InheritancePatch
        {
            static void Postfix(ref IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker, List<Thing> ingredients)
            {
                if (__result == null)
                    return;

                var things = __result.ToList();

                if (things.Count == 0 && (recipeDef.defName == "avPackMeat" || recipeDef.defName == "avPackMeat10" || recipeDef.defName == "avPackMeat40"))
                {
                    foreach (var ingredient in (ingredients))
                    {
                        ThingDef d;

                        if (FoodUtility.IsHumanlikeMeat(ingredient.def))
                            d = DefDatabase<ThingDef>.GetNamed("avMysteriousMeatPack");
                        else if (!Settings.unusual_is_generic && ingredient.def.ingestible.specialThoughtDirect != null && ingredient.def.ingestible.specialThoughtDirect.stages[0].baseMoodEffect < 0)
                            d = DefDatabase<ThingDef>.GetNamed("avOddMeatPack");
                        else
                            d = DefDatabase<ThingDef>.GetNamed("avRegularMeatPack");

                        ThingWithComps t = (ThingWithComps)ThingMaker.MakeThing(d, null);
                        
                        t.stackCount = ingredient.stackCount;
                        CompIngredients comp = t.TryGetComp<CompIngredients>();
                        if (comp != null)
                            comp.RegisterIngredient(ingredient.def);

                        CompRottable r = t.TryGetComp<CompRottable>();
                        CompRottable rr = ingredient.TryGetComp<CompRottable>();

                        if (r != null && r != null)
                            r.RotProgress = rr.RotProgress;

                        things.Add(t);
                    }
                    __result = things;
                    return;
                }

                foreach (var item in things)
                {
                    CompIngredients ingredientsComp = item.TryGetComp<CompIngredients>();
                    if (ingredientsComp != null)
                    {
                        foreach (var ingredient in (ingredients))
                        {
                            CompIngredients subComp = ingredient.TryGetComp<CompIngredients>();
                            if (subComp != null)
                            {
                                int i = ingredientsComp.ingredients.IndexOf(ingredient.def);
                                
                                for (int counter = subComp.ingredients.Count - 1; counter >= 0; counter--)
                                    if (!ingredientsComp.ingredients.Contains(subComp.ingredients[counter]))
                                        ingredientsComp.ingredients.Insert(i+1, subComp.ingredients[counter]);
                                if (ingredient.def.ingestible == null || ingredient.def.ingestible.specialThoughtAsIngredient == null 
                                    || ingredient.def.defName == "avOddMeatPack" && subComp.ingredients.Count > 0)
                                    ingredientsComp.ingredients.Remove(ingredient.def);
                            }
                        }

                        __result = things;
                    }
                }

            }
        }
    }
}
