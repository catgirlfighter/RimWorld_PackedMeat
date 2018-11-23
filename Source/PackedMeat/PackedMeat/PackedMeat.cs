using Harmony;
using System.Reflection;
using Verse;
using UnityEngine;

namespace PackedMeat
{

    [StaticConstructorOnStartup]
    class PackedMeat : Mod
    {
#pragma warning disable 0649
        public static Settings Settings;
#pragma warning restore 0649

        private static ThingDef mysDef = null;
        private static ThingDef oddDef = null;
        private static ThingDef regDef = null;
        public static ThingDef MysteriousPackDef
        {
            get
            {
                if (mysDef == null)
                    mysDef = DefDatabase<ThingDef>.GetNamed("avMysteriousMeatPack");

                return mysDef;
            }
        }
        public static ThingDef OddPackDef
        {
            get
            {
                if (oddDef == null)
                    oddDef = DefDatabase<ThingDef>.GetNamed("avOddMeatPack");

                return oddDef;
            }
        }
        public static ThingDef RegularPackDef
        
{
            get
            {
                if (regDef == null)
                    regDef = DefDatabase<ThingDef>.GetNamed("avRegularMeatPack");

                return regDef;
            }
        }


        public PackedMeat(ModContentPack content) : base(content)
        {
            var harmony = HarmonyInstance.Create("net.avilmask.rimworld.mod.PackedMeat");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            GetSettings<Settings>();
        }

        public void Save()
        {
            LoadedModManager.GetMod<PackedMeat>().GetSettings<Settings>().Write();
        }

        public override string SettingsCategory()
        {
            return "PackedMeat";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }
}
