using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using FrooxEngine.LogiX;
using BaseX;
using System;
using System.Collections.Generic;
using System.Reflection;

//Idea by Gareth48

namespace YourNamespaceHere;
public class ModClass : NeosMod
{
    public override string Author => "Cyro";
    public override string Name => "Inspectrum";
    public override string Version => "1.0.1";

    public override void OnEngineInit()
    {
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
            
            var cloudDriver = VisualConfigs.AttachComponent<CloudValueVariableDriver<color>>();
            var NeosUIStyle = VisualConfigs.AttachComponent<NeosUIStyle>();
            var GradientDriver = VisualConfigs.AttachComponent<ValueGradientDriver<color>>();

            GradientDriver.Progress.Value = 0.5f;
            GradientDriver.AddPoint(0f, color.Black);
            GradientDriver.AddPoint(1f, color.Gray);
            GradientDriver.Target.Target = NeosUIStyle.UserParentedColor;
            GradientDriver.Points[0].Value.DriveInvertedFrom(NeosUIStyle.Color);

            cloudDriver.FallbackValue.Value = RandomX.RGB;
            cloudDriver.Path.Value = "G-Neos.CustomUserColor";
            cloudDriver.Target.Target = NeosUIStyle.Color;
            cloudDriver.OverrideOwner.Value = __instance.World.LocalUser.UserID;
            
            __result.Style = NeosUIStyle;
            __result.Color = __result.Color.SetA(0f);
            //Thanks to badhaloninja for pointing this out
            __result.RunInUpdates(3, () => __result.MarkChangeDirty());
        }
    }
}
