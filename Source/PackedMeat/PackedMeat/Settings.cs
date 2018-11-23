using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PackedMeat
{
    class Settings : ModSettings
    {
        public static bool pack_on_butchering = false;
        public static bool unusual_is_generic = false;
        private static bool CSLoaded = false;
        private static ModContentPack fCommonSense = null;
        public static ModContentPack CommonSenseMod
        {
            get
            {
                if (CSLoaded)
                    return fCommonSense;
                else
                {
                    fCommonSense = LoadedModManager.RunningMods.FirstOrDefault(x => x.Name == "Common Sense");
                    CSLoaded = true;
                    return fCommonSense;
                }
            }
        }
        public static bool add_meal_ingredients
        {
            get
            {
                return CommonSenseMod != null;// && CommonSense.Settings.add_meal_ingredients;
            }
        }

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("pack_on_butchering".Translate(), ref pack_on_butchering);
            listing_Standard.CheckboxLabeled("odd_is_fine".Translate(), ref unusual_is_generic);


            listing_Standard.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref pack_on_butchering, "pack_on_butchering", false, false);
            Scribe_Values.Look<bool>(ref unusual_is_generic, "unusual_is_generic", false, false);
        }
    }
}
