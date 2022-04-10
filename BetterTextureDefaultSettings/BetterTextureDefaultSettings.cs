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

        [HarmonyPatch]
        private static class TextureComponentPatch
        {
            private static readonly Type syncTextureWrapModeType = typeof(Sync<TextureWrapMode>);
            private static readonly FieldInfo syncWrapModeValueField = syncTextureWrapModeType.GetField(nameof(Sync<TextureWrapMode>.Value));
            private static readonly string[] wrapModeFieldNames = new[] { "WrapModeU", "WrapModeV", "WrapModeW" };

            private static void Postfix(object __instance/*Sync<TextureWrapMode> ___WrapModeU, Sync<TextureWrapMode> ___WrapModeV/*, Sync<TextureWrapMode> ___WrapModeW*/)
            {
                //Msg("Called for " + __instance.ToString());

                foreach (var wrapModeFieldName in wrapModeFieldNames)
                {
                    Traverse.Create(__instance).Field(wrapModeFieldName).Property("Value").SetValue(TextureWrapMode.Clamp);
                    /*var syncWrapModeField = __instance.GetType().GetField(wrapModeFieldName);

                    if (syncWrapModeField == null)
                        continue;

                    syncWrapModeValueField.SetValue(syncWrapModeField.GetValue(__instance), TextureWrapMode.Clamp);*/
                }

                /*
                ___WrapModeU.Value = TextureWrapMode.Clamp;
                ___WrapModeV.Value = TextureWrapMode.Clamp;
                ___WrapModeW.Value = TextureWrapMode.Clamp;*/
            }

            private static IEnumerable<MethodBase> TargetMethods()
            {
                var types = AccessTools.GetTypesFromAssembly(AccessTools.AllAssemblies().First(assembly => assembly.GetName().Name == "FrooxEngine"))
                    .Where(type => type.GetFields(BindingFlags.Instance | BindingFlags.Public).Any(field => field.FieldType == syncTextureWrapModeType));

                Msg("Found Types to Patch:");
                foreach (var type in types)
                    Msg(type.Name);

                var methods = //types.SelectMany(type => type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                    types.SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    .Where(method => method.Name == "InitializeSyncMembers");

                Msg("Found Methods to Patch:");
                foreach (var method in methods)
                    Msg(method.FullDescription());

                return methods;
            }
        }
    }
}