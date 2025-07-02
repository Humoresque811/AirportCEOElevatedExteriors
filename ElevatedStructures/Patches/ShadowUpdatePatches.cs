using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AirportCEOElevatedExteriors.ElevatedStructures.Patches;

[HarmonyPatch]
internal class ShadowUpdatePatches
{
    [HarmonyPatch(typeof(ShadowSystem), nameof(ShadowSystem.UpdateShadowsLoop), MethodType.Enumerator)]
    [HarmonyPostfix]
    internal static void RoadTunnelShadowRepeatedPatch()
    {
        ShadowLogicManager.RoadTunnelShadowRepeated();
    }
}
