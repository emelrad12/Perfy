using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Perfy;

[HarmonyPatch(typeof(StatWorker_VacuumResistance), nameof(StatWorker_VacuumResistance.GetValueUnfinalized))]
public static class Patch_StatWorker_VacuumResistance_GetValueUnfinalized
{
    private class CacheData
    {
        public float cachedValue;
        public int tickCached;
    }

    private static readonly Dictionary<Thing, CacheData> cache = new();

    [HarmonyPrefix]
    public static bool Prefix(StatWorker_VacuumResistance __instance, StatRequest req, bool applyPostProcess, ref float __result)
    {
        if (req.Thing == null)
            return true;

        int currentTick = Find.TickManager.TicksGame;

        if (cache.TryGetValue(req.Thing, out var data))
        {
            if (currentTick - data.tickCached < 1000)
            {
                __result = data.cachedValue;
                return false;
            }
        }

        return true;
    }

    [HarmonyPostfix]
    public static void Postfix(StatRequest req, ref float __result)
    {
        if (req.Thing == null)
            return;

        cache[req.Thing] = new CacheData
        {
            cachedValue = __result,
            tickCached = Find.TickManager.TicksGame
        };
    }
}