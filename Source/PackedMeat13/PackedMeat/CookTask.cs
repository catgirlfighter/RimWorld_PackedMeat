using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PackedMeat
{
    class CookTask
    {
        static void StackIn(Thing thing, CompIngredients ingredients, Thing with)
        {
            ingredients.RegisterIngredient(with.def);
            CompRottable r = thing.TryGetComp<CompRottable>();
            CompRottable rr = with.TryGetComp<CompRottable>();

            if (r != null && rr != null)
                r.RotProgress = (r.RotProgress*thing.stackCount+rr.RotProgress*with.stackCount)/(thing.stackCount+with.stackCount);
            thing.stackCount += with.stackCount;
        }

        //Verse.GenRecipe
        //public static IEnumerable<Thing> MakeRecipeProducts(RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient, IBillGiver billGiver)
        //public static IEnumerable<Thing> MakeRecipeProducts(RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient, IBillGiver billGiver, Precept_ThingStyle precept = null)
        [HarmonyPatch(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts), new Type[] { typeof(RecipeDef), typeof(Pawn), typeof(List<Thing>), typeof(Thing), typeof(IBillGiver), typeof(Precept_ThingStyle) })]
        static class GenRecipe_MakeRecipeProducts_InheritancePatch
        {
            static void Postfix(ref IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker, List<Thing> ingredients)
            {
                if (__result == null)
                    return;

                var things = __result.ToList();

                if (recipeDef.defName.Contains("avPackMeat"))
                {
                    Thing reg = things.First(x => x.def == PackedMeat.RegularPackDef);
                    Thing odd = things.First(x => x.def == PackedMeat.OddPackDef);
                    Thing mys = things.First(x => x.def == PackedMeat.MysteriousPackDef);
                    reg.stackCount = 0;
                    odd.stackCount = 0;
                    mys.stackCount = 0;
                    CompIngredients regc = reg.TryGetComp<CompIngredients>();
                    CompIngredients oddc = odd.TryGetComp<CompIngredients>();
                    CompIngredients mysc = mys.TryGetComp<CompIngredients>();
                    regc.ingredients.Clear();
                    oddc.ingredients.Clear();
                    mysc.ingredients.Clear();
                    //
                    foreach (var ingredient in (ingredients))
                    {
                        if (FoodUtility.GetMeatSourceCategory(ingredient.def) == MeatSourceCategory.Humanlike)
                            StackIn(mys, mysc, ingredient);
                        else if (!Settings.unusual_is_generic && ingredient.def.ingestible.specialThoughtDirect != null && ingredient.def.ingestible.specialThoughtDirect.stages[0].baseMoodEffect < 0)
                            StackIn(odd, oddc, ingredient);
                        else
                            StackIn(reg, regc, ingredient);
                    }
                    //
                    if (reg.stackCount == 0)
                        things.Remove(reg);
                    if (odd.stackCount == 0)
                        things.Remove(odd);
                    if (mys.stackCount == 0)
                        things.Remove(mys);
                    //
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
                            if (subComp != null && subComp.ingredients.Count > 0)
                            {
                                int i = ingredientsComp.ingredients.IndexOf(ingredient.def);
                                
                                for (int counter = subComp.ingredients.Count - 1; counter >= 0; counter--)
                                    if (!ingredientsComp.ingredients.Contains(subComp.ingredients[counter]))
                                        ingredientsComp.ingredients.Insert(i+1, subComp.ingredients[counter]);
                                if (ingredient.def.ingestible == null || ingredient.def.ingestible.specialThoughtAsIngredient == null 
                                    || ingredient.def == PackedMeat.OddPackDef && subComp.ingredients.Count > 0)
                                    ingredientsComp.ingredients.Remove(ingredient.def);
                            }
                        }
                    }
                }

                __result = things;

            }
        }
    }
}
