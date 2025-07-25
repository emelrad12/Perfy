using HarmonyLib;
using RimWorld;

namespace Perfy;

[HarmonyPatch(typeof(Building_HoldingPlatform), "DrawChains")]
public static class Patch_Building_HoldingPlatform_DrawChains
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        return false;
    }
}