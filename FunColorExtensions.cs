using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using FrooxEngine.LogiX;
using FrooxEngine.LogiX.Color;
using BaseX;
using System;
using System.Collections.Generic;
using System.Reflection;

public static class FunColorExtensions
{
    public static Dictionary<string, LogixNode>? DriveInvertedFrom(this Sync<color> target, Sync<color> source)
    {
        // Make sure the source and target are not the same
        if (target == source)
            return null;
        
        var nodes = new Dictionary<string, LogixNode>();
        
        Slot slot = target.FindNearestParent<Slot>();
        
        nodes["Inverter"] = slot.AttachComponent<InvertColor>();
        var inverter = nodes["Inverter"] as InvertColor;

        nodes["Driver"] = slot.AttachComponent<DriverNode<color>>();
        var driver = nodes["Driver"] as DriverNode<color>;

        inverter.Color.Target = source;
        driver.Source.Target = inverter;
        driver.DriveTarget.Target = target;

        return nodes;
    }
}