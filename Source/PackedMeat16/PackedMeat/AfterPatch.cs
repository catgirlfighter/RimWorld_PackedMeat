﻿using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using HarmonyLib;
using RimWorld;

namespace PackedMeat
{
    class AfterPatch
    { 
        [HarmonyPatch(typeof(StaticConstructorOnStartupUtility), "CallAll")]
        static class StaticConstructorOnStartupUtility_CallAll_PackedMeat
        {
            public static void Postfix()
            {
                //removing non-corpse based sources of meat
                IEnumerable<RecipeDef> l = DefDatabase<RecipeDef>.AllDefsListForReading.Where(x => x.defName.Contains("avPackMeat"));
                foreach (RecipeDef r in (l))
                    r.fixedIngredientFilter.SetDisallowAll(r.fixedIngredientFilter.AllowedThingDefs.Where(x => x.IsIngestible && x.ingestible.foodType == FoodTypeFlags.Meat
                           && x.ingestible.sourceDef != null && x.ingestible.sourceDef.race != null));
            }
        }
    }
}
