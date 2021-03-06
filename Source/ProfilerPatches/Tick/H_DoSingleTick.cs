﻿using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace DubsAnalyzer
{
    [ProfileMode("Single Tick", UpdateMode.Tick)]
    internal class H_DoSingleTick
    {
        public static bool Active = false;

        public static void Start(object __instance, MethodBase __originalMethod, ref Profiler __state)
        {
            if (!Active) return;

            var state = string.Empty;
            if (__instance != null)
            {
                state = $"{__instance.GetType().Name}.{__originalMethod.Name}";
            }
            else
            if (__originalMethod.ReflectedType != null)
            {
                state = $"{__originalMethod.ReflectedType.Name}.{__originalMethod.Name}";
            }
            __state = Analyzer.Start(state, null, null, null, null, __originalMethod);
        }

        public static void Stop(Profiler __state)
        {
            if (Active)
            {
                __state?.Stop();
            }
        }

        public static void ProfilePatch()
        {
            var go = new HarmonyMethod(typeof(H_DoSingleTick), nameof(Start));
            var biff = new HarmonyMethod(typeof(H_DoSingleTick), nameof(Stop));

            void slop(Type e, string s)
            {
                Analyzer.harmony.Patch(AccessTools.Method(e, s), go, biff);
            }
            slop(typeof(GameComponentUtility), nameof(GameComponentUtility.GameComponentTick));
            slop(typeof(ScreenshotTaker), nameof(ScreenshotTaker.QueueSilentScreenshot));
            slop(typeof(FilthMonitor), nameof(FilthMonitor.FilthMonitorTick));
            slop(typeof(Map), nameof(Map.MapPreTick));
            slop(typeof(Map), nameof(Map.MapPostTick));
            slop(typeof(DateNotifier), nameof(DateNotifier.DateNotifierTick));
            slop(typeof(Scenario), nameof(Scenario.TickScenario));
            slop(typeof(World), nameof(World.WorldTick));
            slop(typeof(StoryWatcher), nameof(StoryWatcher.StoryWatcherTick));
            slop(typeof(GameEnder), nameof(GameEnder.GameEndTick));
            slop(typeof(Storyteller), nameof(Storyteller.StorytellerTick));
            slop(typeof(TaleManager), nameof(TaleManager.TaleManagerTick));
            slop(typeof(World), nameof(World.WorldPostTick));
            slop(typeof(History), nameof(History.HistoryTick));
            slop(typeof(LetterStack), nameof(LetterStack.LetterStackTick));
            slop(typeof(Autosaver), nameof(Autosaver.AutosaverTick));
        }
    }
}