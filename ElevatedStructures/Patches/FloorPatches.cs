using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AirportCEOElevatedExteriors.ElevatedStructures.Patches;

[HarmonyPatch]
internal class FloorPatches
{
    [HarmonyPatch(typeof(PlaceableStructure), nameof(PlaceableStructure.CanBuildOnFloor))]
    [HarmonyPostfix]
    internal static void StructureFloorValidityPatch(PlaceableStructure __instance, int floor, ref bool __result)
    {
        if (floor == 0)
        {
            __result = true;
            return;
        }
        if (floor < 0 && __instance.canBuildBelowGround)
        {
            __result = true;
            return;
        }
        if (ElevatedStructureChangeManager.elevatableStructureTypes.Contains(__instance.structureType) && floor > 0)
        {
            if (__instance is PlaceableRoad && (__instance as PlaceableRoad).RoadType != Enums.RoadType.PublicRoad)
            {
                __result = false;
                return;
            }

            __result = true;
            return;
        }

        __result = false;
    }


    [HarmonyPatch(typeof(PlaceableStructure), nameof(PlaceableStructure.ShouldShowOnFloor))]
    [HarmonyPostfix]
    internal static void StructureFloorShowPatch(PlaceableStructure __instance, int floor, ref bool __result)
    {
        ShadowLogicManager.UpdateCustomShadow(__instance, floor, __instance.Floor);
        if (floor == __instance.Floor)
        {
            __result = true;
            return;
        }
        if (floor >= __instance.Floor && __instance.Floor >= 0)
        {
            __result = true;
            return;
        }

        __result = false;
        return;
    }


    [HarmonyPatch(typeof(RoadTunnel), nameof(RoadTunnel.ShouldShowOnFloor))]
    [HarmonyPostfix]
    internal static void RoadTunnelFloorShowPatch(RoadTunnel __instance, int floor, ref bool __result)
    {
        ShadowLogicManager.UpdateCustomShadow(__instance, floor, __instance.Floor);
    }


    [HarmonyPatch(typeof(RoadTunnel), nameof(RoadTunnel.CanBuildOnFloor))]
    [HarmonyPostfix]
    internal static void RoadTunnelFloorValidityPatch(RoadTunnel __instance, int floor, ref bool __result)
    {
        if (__instance.roadType != Enums.RoadType.PublicRoad)
        {
            return;
        }

	    if (__instance.escalatorDirection == Enums.EscalatorDirection.Up)
	    {
		    __result = floor < FloorManager.MAX_FLOOR;
            return;
	    }
	    else
	    {
            __result = floor > FloorManager.MIN_FLOOR;
		    return;
	    }
    }


    [HarmonyPatch(typeof(ObjectPlacementController), nameof(ObjectPlacementController.SetObject))]
    [HarmonyPrefix]
    internal static void PreventJumpToFloor0(ObjectPlacementController __instance, GameObject obj)
    {
        PlaceableObject plo = obj.GetComponent<PlaceableObject>();

        if (plo == null)
        {
            return;
        }
        PlaceableStructure placeableStructure = plo as PlaceableStructure;
        if (placeableStructure == null || !ElevatedStructureChangeManager.elevatableStructureTypes.Contains(placeableStructure.structureType))
        {
            return;
        }

        placeableStructure.cannotBePlacedOnUpperFloors = false;
    }


    [HarmonyPatch(typeof(VehicleController), nameof(VehicleController.ShouldShowOnFloor))]
    [HarmonyPostfix]
    internal static void VehicleFloorShowPatch(VehicleController __instance, int floor, ref bool __result)
    {
		if (__instance.isInside || floor < 0)
        {
			__result = __instance.Floor == floor;
            return;
        }

        __result = floor >= __instance.Floor;
		return;
    }


    [HarmonyPatch(typeof(VehicleController), nameof(VehicleController.UpdateAttachedTrailerPositioning))]
    [HarmonyPostfix]
    internal static void VehicleFloorHeightCheck(VehicleController __instance)
    {
        if (__instance.Floor >= 1)
        {
            __instance.transform.position = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, FloorManager.TERMINAL_FLOOR_SHIFT);
            //__instance.transform.GetChild(0).position = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, FloorManager.TERMINAL_FLOOR_SHIFT);
            //__instance.transform.GetChild(1).position = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, FloorManager.TERMINAL_FLOOR_SHIFT);
            //__instance.transform.GetChild(2).position = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, FloorManager.TERMINAL_FLOOR_SHIFT);
        }
        else
        {
            __instance.transform.position = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, -0.03f);
            //__instance.transform.GetChild(0).position = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, -0.01f);
            //__instance.transform.GetChild(1).position = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, -0.01f);
            //__instance.transform.GetChild(2).position = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, -0.01f);
        }
    }


    [HarmonyPatch(typeof(PlaceableItem), nameof(PlaceableItem.ShouldShowOnFloor))]
    [HarmonyPostfix]
    internal static void ItemFloorShowPatch(PlaceableItem __instance, int floor, ref bool __result)
    {
		if (__instance.isInside || floor < 0)
        {
			__result = __instance.Floor == floor;
            return;
        }

        if (floor >= 1)
        {
            __instance.transform.position = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, FloorManager.TERMINAL_FLOOR_SHIFT);
        }
        else
        {
            __instance.transform.position = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, 0);
        }

        __result = floor >= __instance.Floor;
		return;
    }


    [HarmonyPatch(typeof(MergedTile), nameof(MergedTile.ShouldShowOnFloor))]
    [HarmonyPostfix]
    internal static void SidewalkFloorShowPatch(MergedTile __instance, int floor, ref bool __result)
    {
        ShadowLogicManager.UpdateCustomShadow(__instance, floor, __instance.Floor);
        if (floor < 0)
        {
            return;
        }

        __result = floor >= __instance.Floor;
		return;
    }

    [HarmonyPatch(typeof(FloorManager), nameof(FloorManager.SetFloor))]
    [HarmonyPrefix]
    internal static bool Tester(FloorManager __instance, int floor)
    {
        if (floor == 3)
        {
            return true;
        }

        FloorManager.SetFloor(3);
        return false;
    }
}
