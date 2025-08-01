using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportCEOElevatedExteriors.ElevatedStructures.Patches;

[HarmonyPatch]
internal class DelayTextureLoadPatch
{
    internal static string? workshopInfoCache = null;

    [HarmonyPatch(typeof(TemplateController), nameof(TemplateController.LoadQueuedObjects), MethodType.Enumerator)]
    [HarmonyPostfix]
    internal static void OnEndOfModLoad(TemplateController __instance, bool __result)
    {
        if (__result || workshopInfoCache == null)
        {
            return;
        }

        ElevatedStructureChangeManager.PrepareTextures(workshopInfoCache);
    }
}
