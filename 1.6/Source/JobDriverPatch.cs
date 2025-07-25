using HarmonyLib;
using Verse.AI;

namespace Perfy;

[HarmonyPatch(typeof(JobDriver), nameof(JobDriver.DriverTick))]
public static class Patch_JobDriver_DriverTick
{
    [HarmonyPrefix]
    public static bool Prefix(JobDriver __instance)
    {
        if (__instance.ticksLeftThisToil > 0 && __instance.ticksLeftThisToil % 100 != 0)
        {
            __instance.ticksLeftThisToil--;
            return false;
        }

        return true;
    }
}