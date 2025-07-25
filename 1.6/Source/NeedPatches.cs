using HarmonyLib;
using RimWorld;
using System.Collections.Generic;

namespace Perfy;

[HarmonyPatch(typeof(Need_Mood), nameof(Need_Mood.CurInstantLevel), MethodType.Getter)]
public static class Patch_NeedMood_CurInstantLevel
{
    private class CacheData
    {
        public float cachedValue;
        public int callsSinceLastUpdate = 9999;
    }

    private static readonly Dictionary<Need_Mood, CacheData> cache = new();

    [HarmonyPrefix]
    public static bool Prefix(Need_Mood __instance, ref float __result)
    {
        if (!cache.TryGetValue(__instance, out var data))
        {
            data = new();
            cache[__instance] = data;
        }

        if (data.callsSinceLastUpdate < 10)
        {
            data.callsSinceLastUpdate++;
            __result = data.cachedValue;
            return false;
        }

        data.callsSinceLastUpdate = 0;
        return true;
    }

    [HarmonyPostfix]
    public static void Postfix(Need_Mood __instance, ref float __result)
    {
        var data = cache[__instance];
        data.cachedValue = __result;
    }
}

[HarmonyPatch(typeof(SituationalThoughtHandler), nameof(SituationalThoughtHandler.SituationalThoughtInterval))]
public static class Patch_SituationalThoughtHandler_SituationalThoughtInterval
{
    private static readonly Dictionary<SituationalThoughtHandler, int> callCounts = new();

    [HarmonyPrefix]
    public static bool Prefix(SituationalThoughtHandler __instance)
    {
        var count = callCounts.GetValueOrDefault(__instance, 0);

        if (count < 10)
        {
            callCounts[__instance] = count + 1;
            return false;
        }

        callCounts[__instance] = 0;
        return true;
    }
}

[HarmonyPatch(typeof(Need_Beauty), nameof(Need_Beauty.CurInstantLevel), MethodType.Getter)]
public static class Patch_NeedBeauty_CurInstantLevel
{
    private class CacheData
    {
        public float cachedValue;
        public int callsSinceLastUpdate = 9999;
    }

    private static readonly Dictionary<Need_Beauty, CacheData> cache = new();

    [HarmonyPrefix]
    public static bool Prefix(Need_Beauty __instance, ref float __result)
    {
        if (!cache.TryGetValue(__instance, out var data))
        {
            data = new CacheData();
            cache[__instance] = data;
        }

        if (data.callsSinceLastUpdate < 10)
        {
            data.callsSinceLastUpdate++;
            __result = data.cachedValue;
            return false;
        }

        data.callsSinceLastUpdate = 0;
        return true;
    }

    [HarmonyPostfix]
    public static void Postfix(Need_Beauty __instance, ref float __result)
    {
        var data = cache[__instance];
        data.cachedValue = __result;
    }
}