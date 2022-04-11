using CodeX;
using FrooxEngine;
using HarmonyLib;
using NeosModLoader;

namespace BetterTextureDefaultSettings
{
    public class BetterTextureDefaultSettings : NeosMod
    {
        public static ModConfiguration Config;
        
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<TextureWrapMode> TextureWrapMode = new ModConfigurationKey<TextureWrapMode>("TextureWrapMode", "Default Texture Wrap Mode.", ()=> FrooxEngine.TextureWrapMode.Clamp);
        
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<TextureFilterMode> TextureFilterMode = new ModConfigurationKey<TextureFilterMode>("TextureFilterMode", "Default Texture Filter Mode.", () => FrooxEngine.TextureFilterMode.Anisotropic);

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<Filtering> MipMapFilter = new ModConfigurationKey<Filtering>("MipMapFilter", "Default MipMap Filter.", () => Filtering.Box);


        public override string Author => "Banane9";
        public override string Link => "https://github.com/Banane9/NeosBetterTextureDefaultSettings";
        public override string Name => "BetterTextureDefaultSettings";
        public override string Version => "1.0.0";

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony($"{Author}.{Name}");
            Config = GetConfiguration();
            Config.Save(true);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(StaticTexture2D))]
        private static class TextureComponentPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnAwake")]
            private static void Postfix(StaticTexture2D __instance)
            {
                __instance.WrapModeU.Value = Config.GetValue(TextureWrapMode);
                __instance.WrapModeV.Value = Config.GetValue(TextureWrapMode);
                __instance.FilterMode.Value = Config.GetValue(TextureFilterMode);
                __instance.MipMapFilter.Value = Config.GetValue(MipMapFilter);
            }
        }
    }
}