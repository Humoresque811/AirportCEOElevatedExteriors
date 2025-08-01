using AirportCEOModLoader.Core;
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
        }
        else
        {
            __instance.transform.position = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, -0.03f);
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

        if (__instance.Floor >= 1)
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


    [HarmonyPatch(typeof(PersonController), nameof(PersonController.ShouldShowOnFloor))]
    [HarmonyPostfix]
    internal static void PersonShouldShowOnFloor(PersonController __instance, int floor, ref bool __result)
    {
        try
        {
            if (!SaveLoadGameDataController.loadComplete)
            {
                return;
            }
            if (__result == false || FloorManager.currentFloor < 0 || __instance.isInside)
            {
                return;
            }

            __result = FloorManager.currentFloor >= __instance.Floor;
        }
        catch (Exception ex)
        {
            AirportCEOElevatedExteriors.EELogger.LogError($"Failed to update persons ShouldShow func. {ExceptionUtils.ProccessException(ex)}");
        }
    }


    [HarmonyPatch(typeof(AssetController), nameof(AssetController.ShouldShowOnFloor))]
    [HarmonyPostfix]
    internal static void BagShouldShowOnFloor(AssetController __instance, int floor, ref bool __result)
    {
        try
        {
            if (!SaveLoadGameDataController.loadComplete)
            {
                return;
            }
            if (__result == false || FloorManager.currentFloor < 0 || __instance.AssetModel.isInside)
            {
                return;
            }

            __result = FloorManager.currentFloor >= __instance.Floor;
        }
        catch (Exception ex)
        {
            AirportCEOElevatedExteriors.EELogger.LogError($"Failed to update assets ShouldShow func. {ExceptionUtils.ProccessException(ex)}");
        }
    }


    [HarmonyPatch(typeof(FloorManager), nameof(FloorManager.SetFloor))]
    [HarmonyPrefix]
    internal static bool CinematicSwitchTo3rdFloor(FloorManager __instance, int floor)
    {
        if (!AirportCEOElevatedExteriorsConfig.CinematicMode.Value || floor == 3)
        {
            return true;
        }

        FloorManager.SetFloor(3);
        return false;
    }

    [HarmonyPatch(typeof(PlaceableObject), nameof(PlaceableObject.CanClickOn))]
    [HarmonyPostfix]
    internal static void PreventLowerFloorClicks(PlaceableObject __instance, ref bool __result)
    {
        if (!__result || __instance.objectType != Enums.ObjectType.Structure)
        {
            return;
        }

        // its true, and this is a structure, so we might have to make a change
        if (FloorManager.currentFloor > 0 && !__instance.isInside && __instance.Floor < FloorManager.currentFloor)
        {
            // we are on an upper floor
            __result = false;
        }
    }
}
