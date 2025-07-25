using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Perfy;

[HarmonyPatch(typeof(RestUtility), nameof(RestUtility.CanUseBedNow))]
public static class Patch_RestUtility_CanUseBedNow
{
    private class CacheKey
    {
        public Thing bedThing;
        public Pawn sleeper;

        public override bool Equals(object obj)
        {
            return obj is CacheKey other && bedThing == other.bedThing && sleeper == other.sleeper;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(bedThing, sleeper);
        }
    }


    private class CacheEntry
    {
        public bool value;
        public int tick;
    }

    private static readonly Dictionary<CacheKey, CacheEntry> cache = new();

    [HarmonyPrefix]
    public static bool Prefix(
        Thing bedThing,
        Pawn sleeper,
        bool checkSocialProperness,
        bool allowMedBedEvenIfSetToNoCare,
        GuestStatus? guestStatusOverride,
        ref bool __result)
    {
        // Only cache when parameters are defaultable
        if (checkSocialProperness || allowMedBedEvenIfSetToNoCare || guestStatusOverride != null)
            return true;

        var key = new CacheKey { bedThing = bedThing, sleeper = sleeper };
        int currentTick = Find.TickManager.TicksGame;

        if (cache.TryGetValue(key, out var entry))
        {
            if (currentTick - entry.tick < 180)
            {
                __result = entry.value;
                return false;
            }
        }

        return true;
    }

    [HarmonyPostfix]
    public static void Postfix(
        Thing bedThing,
        Pawn sleeper,
        bool checkSocialProperness,
        bool allowMedBedEvenIfSetToNoCare,
        GuestStatus? guestStatusOverride,
        ref bool __result)
    {
        if (checkSocialProperness || allowMedBedEvenIfSetToNoCare || guestStatusOverride != null)
            return;

        var key = new CacheKey { bedThing = bedThing, sleeper = sleeper };
        cache[key] = new()
        {
            value = __result,
            tick = Find.TickManager.TicksGame
        };
    }
}