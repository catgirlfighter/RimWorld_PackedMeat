﻿using Verse;
using RimWorld;

namespace PackedMeat
{
    public class avRecipeWorkerCounter_ButcherAnimals : RecipeWorkerCounter_ButcherAnimals
    {
        public static int CountEachCat(Bill_Production bill, ThingCategoryDef cat)
        {
            int num = 0;
            foreach (var i in (cat.childThingDefs))
                num += bill.Map.resourceCounter.GetCount(i);
            //
            foreach (var i in (cat.childCategories))
                num += CountEachCat(bill, i);
            //
            return num;
        }

        public override int CountProducts(Bill_Production bill)
        {
            return CountEachCat(bill, ThingCategoryDefOf.MeatRaw);
        }
    }
}
