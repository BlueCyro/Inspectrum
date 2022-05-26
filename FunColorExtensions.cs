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
    public static DriverNode<color>? DriveInvertedFrom(this Sync<color> target, Sync<color> source)
    {
        // Make sure the source and target are not the same
        if (target == source)
            return null;
        
        Slot slot = target.FindNearestParent<Slot>();
        InvertColor inverter = slot.AttachComponent<InvertColor>();
        DriverNode<color> driver = slot.AttachComponent<DriverNode<color>>();

        inverter.Color.Target = source;
        driver.Source.Target = inverter;
        driver.DriveTarget.Target = target;

        return driver;
    }
}