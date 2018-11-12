using Harmony;
using System;
using System.Linq;
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

        public PackedMeat(ModContentPack content) : base(content)
        {
            var harmony = HarmonyInstance.Create("net.avilmask.rimworld.mod.PackedMeat");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            base.GetSettings<Settings>();
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
