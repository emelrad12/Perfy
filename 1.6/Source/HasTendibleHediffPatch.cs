using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace Perfy;

[HarmonyPatch(typeof(HediffSet), nameof(HediffSet.HasTendableHediff))]
public static class Patch_HediffSet_HasTendableHediff
{
    private class CacheData
    {
        public bool cachedValue;
        public int tickCached;
    }

    // Separate cache per value of `forAlert`
    private static readonly Dictionary<(HediffSet, bool), CacheData> cache = new();

    [HarmonyPrefix]
    public static bool Prefix(HediffSet __instance, bool forAlert, ref bool __result)
    {
        int currentTick = Find.TickManager.TicksGame;
        var key = (__instance, forAlert);

        if (cache.TryGetValue(key, out var data))
        {
            if (currentTick - data.tickCached < 180)
            {
                __result = data.cachedValue;
                return false;
            }
        }

        return true;
    }

    [HarmonyPostfix]
    public static void Postfix(HediffSet __instance, bool forAlert, ref bool __result)
    {
        var key = (__instance, forAlert);

        cache[key] = new CacheData
        {
            cachedValue = __result,
            tickCached = Find.TickManager.TicksGame
        };
    }
}