using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace PackedMeat
{
    [DefOf]
    public static class PackedMeatDefOf
    {
        public static ThingDef avRegularMeatPack;
        public static ThingDef avOddMeatPack;
        public static ThingDef avMysteriousMeatPack;
    }

    public class Settings : ModSettings
    {
        public static bool pack_on_butchering = false;
        //public static bool unusual_is_generic = false;
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

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("pack_on_butchering".Translate(), ref pack_on_butchering);


            listing_Standard.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref pack_on_butchering, "pack_on_butchering", false, false);
        }
    }
}
