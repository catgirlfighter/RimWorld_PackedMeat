using UnityEngine;
using Verse;
using System;

namespace PackedMeat
{
    class Settings : ModSettings
    {
        public static bool pack_on_butchering = false;
        public static bool unusual_is_generic = false;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("Pack meat on butchering", ref pack_on_butchering);
            listing_Standard.CheckboxLabeled("Treat meat with unusual properties (ex. insect) as 'regular meat'", ref unusual_is_generic);


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
