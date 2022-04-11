using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FrooxEngine;
using FrooxEngine.LogiX;
using HarmonyLib;
using NeosModLoader;

namespace BetterTextureDefaultSettings
{
    public class BetterTextureDefaultSettings : NeosMod
    {
        public override string Author => "Banane9";
        public override string Link => "https://github.com/Banane9/NeosBetterTextureDefaultSettings";
        public override string Name => "BetterTextureDefaultSettings";
        public override string Version => "1.0.0";

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony($"{Author}.{Name}");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(StaticTexture2D))]
        private static class TextureComponentPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnAwake")]
            private static void Postfix(StaticTexture2D __instance)
            {
                __instance.WrapModeU.Value = TextureWrapMode.Clamp;
                __instance.WrapModeV.Value = TextureWrapMode.Clamp;
            }
        }
    }
}