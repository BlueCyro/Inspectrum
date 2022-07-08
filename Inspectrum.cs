using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;

//Idea by Gareth48

namespace Inspectrum
{
    public class Inspectrum : NeosMod
    {
        public override string Author => "Cyro";
        public override string Name => "Inspectrum";
        public override string Version => "1.2.0";
        public static ModConfiguration? Config;

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<color> CustomGrabColor = new ModConfigurationKey<color>("Custom Grab Color", "When alpha is greater than zero, use this color on grab instead of just inverting it", () => new color(0.0f, 0.0f, 0.0f, 0.0f));

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<color> InspectorColorOverride = new ModConfigurationKey<color>("Inspector Color Override", "When alpha is greater than zero, use this color on the inspector instead of your cloud color", () => new color(0.0f, 0.0f, 0.0f, 0.0f));    
        public override void OnEngineInit()
        {
            Config = GetConfiguration();
            Config!.Save(true);
            Harmony harmony = new Harmony("net.Cyro.Inspectrum");
            harmony.PatchAll();
        }


        [HarmonyPatch(typeof(InspectorPanel), "Setup")]
        class InspectrumPatcher
        {
            static void Postfix(InspectorPanel __instance, NeosPanel __result)
            {
                Slot VisualConfigs = __instance.Slot.AddSlot("VisualConfigs");
                VisualConfigs.OrderOffset = 1024;

                var NeosUIStyle = VisualConfigs.AttachComponent<NeosUIStyle>();
                if (Config!.GetValue(CustomGrabColor).a == 0.0f)
                {
                    var GradientDriver = VisualConfigs.AttachComponent<ValueGradientDriver<color>>();
                    GradientDriver.Progress.Value = 0.5f;
                    GradientDriver.AddPoint(0f, color.Black);
                    GradientDriver.AddPoint(1f, color.Gray);
                    GradientDriver.Target.Target = NeosUIStyle.UserParentedColor;
                    GradientDriver.Points[0].Value.DriveInvertedFrom(NeosUIStyle.Color);
                }
                else
                {
                    NeosUIStyle.UserParentedColor.Value = Config!.GetValue(CustomGrabColor);
                }

                if (Config!.GetValue(InspectorColorOverride).a == 0.0f)
                {
                    var cloudDriver = VisualConfigs.AttachComponent<CloudValueVariableDriver<color>>();
                    cloudDriver.WriteBack.Value = false;
                    cloudDriver.FallbackValue.Value = RandomX.RGB;
                    cloudDriver.Path.Value = "G-Neos.CustomUserColor";
                    cloudDriver.Target.Target = NeosUIStyle.Color;
                    cloudDriver.OverrideOwner.Value = __instance.World.LocalUser.UserID;
                }
                else
                {
                    NeosUIStyle.Color.Value = Config!.GetValue(InspectorColorOverride);
                }


                __result.Style = NeosUIStyle;
                __result.Color = __result.Color.SetA(0f);
                //Thanks to badhaloninja for pointing this out
                __result.RunInUpdates(3, () => {
                    __result.MarkChangeDirty();
                    __result.Title = $"{__instance.LocalUser.UserName}'s Inspector"; // Title gets set after setup finishes so this needs to run after it gets set
                });
            }
        }
    }
}
