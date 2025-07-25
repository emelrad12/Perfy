using HarmonyLib;
using Verse;

namespace Perfy;

[StaticConstructorOnStartup]
public static class Main
{
    static Main()
    {
        var harmony = new Harmony("perfy.performance");
        harmony.PatchAll();
    }
}