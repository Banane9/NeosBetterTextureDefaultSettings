using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private static ModConfigurationKey<Filtering> MipMapFilter = new ModConfigurationKey<Filtering>("MipMapFilter", "Default MipMap Filter.", () => Filtering.Box);

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<TextureFilterMode> TextureFilterMode = new ModConfigurationKey<TextureFilterMode>("TextureFilterMode", "Default Texture Filter Mode.", () => FrooxEngine.TextureFilterMode.Anisotropic);

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<TextureWrapMode> TextureWrapMode = new ModConfigurationKey<TextureWrapMode>("TextureWrapMode", "Default Texture Wrap Mode.", () => FrooxEngine.TextureWrapMode.Clamp);

        public override string Author => "Banane9";
        public override string Link => "https://github.com/Banane9/NeosBetterTextureDefaultSettings";
        public override string Name => "BetterTextureDefaultSettings";
        public override string Version => "1.0.0";

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony($"{Author}.{Name}");
            Config = GetConfiguration();
            Config.OnThisConfigurationChanged += e => e.Config.Save(true);
            Config.Save(true);
            harmony.PatchAll();
        }

        [HarmonyPatch]
        private static class TextureComponentPatch
        {
            private static readonly Type indicatorFieldType = typeof(Sync<TextureWrapMode>);

            private static readonly Dictionary<Type, ModConfigurationKey> supportedDefaults = new Dictionary<Type, ModConfigurationKey>()
            {
                { typeof(Sync<TextureWrapMode>), TextureWrapMode },
                { typeof(Sync<TextureFilterMode>), TextureFilterMode },
                { typeof(Sync<Filtering>), MipMapFilter }
            };

            [HarmonyPostfix]
            private static void Postfix(object __instance)
            {
                var instanceFields = __instance.GetType().GetFields(AccessTools.all);

                foreach (var supportedDefault in supportedDefaults)
                {
                    foreach (var matchedField in instanceFields.Where(field => field.FieldType == supportedDefault.Key))
                    {
                        Traverse.Create(matchedField.GetValue(__instance)).Property("Value").SetValue(Config.GetValue(supportedDefault.Value));
                    }
                }
            }

            [HarmonyTargetMethods]
            private static IEnumerable<MethodBase> TargetMethods()
            {
                var types = AccessTools.GetTypesFromAssembly(AccessTools.AllAssemblies().First(assembly => assembly.GetName().Name == "FrooxEngine"))
                    .Where(type => !type.IsAbstract && type.GetFields(AccessTools.all).Any(field => field.FieldType == indicatorFieldType));

                Msg("Applying texture defaults to these Types:");
                foreach (var type in types)
                    Msg(type.Name);

                return types.Select(type => AccessTools.Method(type, "OnAwake"));
            }
        }
    }
}