using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace DubsAnalyzer
{
<<<<<<< HEAD
    [PerformancePatch]
=======
    [HarmonyPatch(typeof(TickManager))]
    [HarmonyPatch(nameof(TickManager.TickRateMultiplier), MethodType.Getter)]
>>>>>>> master
    public static class Patch_TickRateMultiplier
    {
        public static float panNorm = 0.0f;
        public static float nMinusCameraScale = 0.0f;

<<<<<<< HEAD
        public static void PerformancePatch(Harmony harmony)
        {
            var tickPost = new HarmonyMethod(typeof(Patch_TickRateMultiplier), nameof(TickManager_Postfix));
            var driverDollyPost = new HarmonyMethod(typeof(Patch_TickRateMultiplier), nameof(CameraDriver_Dolly_Postfix));
            var driverOnGuiPost = new HarmonyMethod(typeof(Patch_TickRateMultiplier), nameof(CameraDriver_OnGUI_Postfix));

            harmony.Patch(AccessTools.PropertyGetter(typeof(TickManager), nameof(TickManager.TickRateMultiplier)), null, tickPost);
            harmony.Patch(AccessTools.Method(typeof(CameraDriver), nameof(CameraDriver.CalculateCurInputDollyVect)), null, driverDollyPost);
            harmony.Patch(AccessTools.Method(typeof(CameraDriver), nameof(CameraDriver.CameraDriverOnGUI)), null, driverOnGuiPost);
        }

        static void TickManager_Postfix(ref float __result)
        {

            if (!Analyzer.Settings.DynamicSpeedControl) return;

=======
        [HarmonyPriority(Priority.LowerThanNormal)]
        static void Postfix(ref float __result)
        {

            if (!Analyzer.Settings.DynamicSpeedControl)
            {
                return;
            }
>>>>>>> master

            if (Find.CameraDriver.CurrentViewRect.Area != nMinusCameraScale)
            {
                nMinusCameraScale = Find.CameraDriver.CurrentViewRect.Area;

<<<<<<< HEAD
                switch (Find.CameraDriver.CurrentZoom)
                {
                    case CameraZoomRange.Furthest: panNorm += 150f; break;
                    case CameraZoomRange.Far: panNorm += 80f; break;
                    case CameraZoomRange.Middle: panNorm += 40f; break;
                    case CameraZoomRange.Close: panNorm += 5f; break;
                }
            }

            if (panNorm <= 15f) return;

            switch(Find.TickManager.CurTimeSpeed)
            {
                case TimeSpeed.Fast: __result = Mathf.Max(2.0f - panNorm / 128f, 1.0f); break;
                case TimeSpeed.Superfast: __result = Mathf.Max(3.0f - panNorm / 96f, 1.5f); break;
                case TimeSpeed.Ultrafast: __result = Mathf.Max(4.0f - panNorm / 64f, 2.0f); break;
            }
        }        
        
        static void CameraDriver_Dolly_Postfix(ref Vector2 __result)
        {
            if (!Analyzer.Settings.DynamicSpeedControl) return;

            panNorm = __result.magnitude;
        }
        
        static void CameraDriver_OnGUI_Postfix(CameraDriver __instance)
        {
            if (!Analyzer.Settings.DynamicSpeedControl) return;

            if (Find.Camera == null || Find.TickManager.Paused || Find.TickManager.NotPlaying)
=======
                if (Find.CameraDriver.CurrentZoom == CameraZoomRange.Furthest)
                    panNorm += 150f;

                if (Find.CameraDriver.CurrentZoom == CameraZoomRange.Far)
                    panNorm += 80f;

                if (Find.CameraDriver.CurrentZoom == CameraZoomRange.Middle)
                    panNorm += 40f;

                if (Find.CameraDriver.CurrentZoom == CameraZoomRange.Close)
                    panNorm += 5f;
            }

            if (panNorm <= 15f)
                return;

            if (Find.TickManager.CurTimeSpeed == TimeSpeed.Fast)
            {
                __result = Mathf.Max(2.0f - panNorm / 128f, 1.0f);
            }
            else if (Find.TickManager.CurTimeSpeed == TimeSpeed.Superfast)
            {
                __result = Mathf.Max(3.0f - panNorm / 96f, 1.5f);
            }
            else if (Find.TickManager.CurTimeSpeed == TimeSpeed.Ultrafast)
            {
                __result = Mathf.Max(4.0f - panNorm / 64f, 2.0f);
            }
        }
    }

    [HarmonyPatch(typeof(CameraDriver))]
    [HarmonyPatch("CalculateCurInputDollyVect")]
    static class Patch_CalculateCurnputDollyVect
    {
        static void Postfix(ref Vector2 __result)
        {
            Patch_TickRateMultiplier.panNorm = __result.magnitude;
        }
    }

    [HarmonyPatch(typeof(CameraDriver))]
    [HarmonyPatch(nameof(CameraDriver.CameraDriverOnGUI))]
    public static class Patch_CameraPositionChanged
    {
        [HarmonyPriority(Priority.HigherThanNormal)]
        static void Postfix(CameraDriver __instance)
        {
            if (!Analyzer.Settings.DynamicSpeedControl)
            {
                return;
            }

            if (Find.Camera == null)
                return;

            if (Find.TickManager.Paused || Find.TickManager.NotPlaying)
>>>>>>> master
                return;

            var modifer = 0f;

<<<<<<< HEAD
            switch(Find.CameraDriver.CurrentZoom)
            {
                case CameraZoomRange.Furthest: modifer = 150f; break;
                case CameraZoomRange.Far: modifer = 80f; break;
                case CameraZoomRange.Middle: modifer = 40f; break;
                case CameraZoomRange.Close: modifer = 5f; break;
            }

            if (KeyBindingDefOf.MapDolly_Left.IsDown || KeyBindingDefOf.MapDolly_Up.IsDown || KeyBindingDefOf.MapDolly_Right.IsDown || KeyBindingDefOf.MapDolly_Down.IsDown)
            {
                panNorm = modifer;
            }
            else if (KeyBindingDefOf.MapZoom_Out.KeyDownEvent || KeyBindingDefOf.MapZoom_In.KeyDownEvent)
            {
                panNorm = modifer;
            }
        }
    }

=======
            if (Find.CameraDriver.CurrentZoom == CameraZoomRange.Furthest)
                modifer = 150f;

            if (Find.CameraDriver.CurrentZoom == CameraZoomRange.Far)
                modifer = 80f;

            if (Find.CameraDriver.CurrentZoom == CameraZoomRange.Middle)
                modifer = 40f;

            if (Find.CameraDriver.CurrentZoom == CameraZoomRange.Close)
                modifer = 5f;

            if (KeyBindingDefOf.MapDolly_Left.IsDown || KeyBindingDefOf.MapDolly_Up.IsDown || KeyBindingDefOf.MapDolly_Right.IsDown || KeyBindingDefOf.MapDolly_Down.IsDown)
            {
                Patch_TickRateMultiplier.panNorm = modifer;
            }

            if (KeyBindingDefOf.MapZoom_Out.KeyDownEvent || KeyBindingDefOf.MapZoom_In.KeyDownEvent)
            {
                Patch_TickRateMultiplier.panNorm = modifer;
            }
        }
    }
>>>>>>> master
}
