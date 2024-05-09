using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PackedMeat
{
    [HarmonyPatch(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts))]
    static class GenRecipe_MakeRecipeProducts_PackedMeatPatch
    {
        static void StackIn(Thing thing, CompIngredients ingredients, Thing with)
        {
            ingredients.RegisterIngredient(with.def);
            CompRottable r = thing.TryGetComp<CompRottable>();
            CompRottable rr = with.TryGetComp<CompRottable>();

            if (r != null && rr != null)
                r.RotProgress = (r.RotProgress * thing.stackCount + rr.RotProgress * with.stackCount) / (thing.stackCount + with.stackCount);
            thing.stackCount += with.stackCount;
        }

        internal static void Postfix(ref IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker, List<Thing> ingredients)
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
                    else if (FoodUtility.GetMeatSourceCategory(ingredient.def) == MeatSourceCategory.Insect)
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
                                    ingredientsComp.ingredients.Insert(i + 1, subComp.ingredients[counter]);
                            //
                            if (ingredient.def.ingestible == null || subComp.ingredients.Count > 0)
                                ingredientsComp.ingredients.Remove(ingredient.def);
                        }
                    }
                }
            }

            __result = things;

        }
    }

    [HarmonyPatch(typeof(Building_NutrientPasteDispenser), nameof(Building_NutrientPasteDispenser.TryDispenseFood))]
    static class Building_NutrientPasteDispenser_NutrientPasteDispenser_PackedMeatPatch
    {
        /*
        internal static void Postfix(Thing __result)
        {
            CompIngredients ingredientsComp = __result.TryGetComp<CompIngredients>();
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
                                ingredientsComp.ingredients.Insert(i + 1, subComp.ingredients[counter]);
                        //
                        if (ingredient.def.ingestible == null || subComp.ingredients.Count > 0)
                            ingredientsComp.ingredients.Remove(ingredient.def);
                    }
                }
            }
        }
        */
        private static ThingDef GetDef(Thing thing)
        {
            CompIngredients subComp = thing.TryGetComp<CompIngredients>();
            if (subComp == null || subComp.ingredients.Count == 0)
                return thing.def;
            return subComp.ingredients[0];
        }

        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instrs)
        {
            bool b = false;
            FieldInfo Ldef = AccessTools.Field(typeof(Thing), nameof(Thing.def));
            MethodInfo LGetDef = AccessTools.Method(typeof(Building_NutrientPasteDispenser_NutrientPasteDispenser_PackedMeatPatch), nameof(GetDef));
            CodeInstruction prev = null;
            foreach (var i in (instrs))
            {
                if (i.opcode == OpCodes.Ldfld && i.operand == (object)Ldef
                    && prev?.opcode == OpCodes.Ldloc_2)
                {
                    yield return new CodeInstruction(OpCodes.Callvirt, LGetDef);
                    b = true;
                }
                else
                {
                    yield return i;
                }
                prev = i;
            }
            if (!b) Log.Message("[PackedMeat] cound't do patch 0 for Building_NutrientPasteDispenser.TryDispenseFood");
        }
    }
}
