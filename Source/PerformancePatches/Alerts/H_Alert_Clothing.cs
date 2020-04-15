﻿using System;
using System.Collections.Generic;
using HarmonyLib;
using Verse;
using RimWorld;

namespace DubsAnalyzer
{
    class AlertInfo
    {
        public bool dirty = true;
        public bool changed = true;
        public AlertReport report = new AlertReport();

        public bool Dirty()
        {
            if (dirty)
            {
                this.dirty = false;
                this.changed = true;
                return true;
            }
            return false;
        }
        public void Update(AlertReport report)
        {
            this.report = report;
            this.changed = false;
        }
    }

    [PerformancePatch]
    class H_Alert_Clothing
    {

        public static AlertInfo tattered = new AlertInfo();
        public static AlertInfo nudist = new AlertInfo(); 
        public static void PerformancePatch()
        {
            var firstFlag = AccessTools.Method(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.TakeWearoutDamageForDay)); // pawns apparel has taken wearout damage
            var secondFlag = AccessTools.Method(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Wear), new Type[] { typeof(Apparel), typeof(bool), typeof(bool) }); // pawn has equiped new clothes
            var thirdFlag = AccessTools.Method(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.TryDrop), new Type[] { typeof(Apparel), typeof(Apparel).MakeByRefType(), typeof(IntVec3), typeof(bool) }); // pawn has removed a piece of clothing

            var reaction = new HarmonyMethod(typeof(H_Alert_Clothing), nameof(UpdateFlag));
            Analyzer.harmony.Patch(firstFlag, null, reaction);
            Analyzer.harmony.Patch(secondFlag, null, reaction);
            Analyzer.harmony.Patch(thirdFlag, null, reaction);

            var skiff2 = AccessTools.Method(typeof(Alert_Thought), nameof(Alert_Thought.GetReport));
            var pre = new HarmonyMethod(typeof(H_Alert_Clothing), nameof(AlertCheck));
            var post = new HarmonyMethod(typeof(H_Alert_Clothing), nameof(PostCheck));


            Analyzer.harmony.Patch(skiff2, pre, post);
        }

        public static void UpdateFlag()  
        {
            tattered.dirty = true;
            nudist.dirty = true;
        }

        public static bool AlertCheck(Alert __instance)
        {
            if (__instance is Alert_TatteredApparel)
                if (tattered.Dirty()) return true;

            if (__instance is Alert_UnhappyNudity)
                if (nudist.Dirty()) return true;

            return false;
        }

        public static void PostCheck(Alert __instance, ref AlertReport __result)
        {
            if (__instance is Alert_TatteredApparel)
            {
                if (!tattered.changed)
                    __result = tattered.report;
                else
                    tattered.Update(__result);

                return;
            }

            if (__instance is Alert_UnhappyNudity)
            {
                if (!nudist.changed)
                    __result = nudist.report;
                else
                    nudist.Update(__result);

                return;
            }
        }

    }
}