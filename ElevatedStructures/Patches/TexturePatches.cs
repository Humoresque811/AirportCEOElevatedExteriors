using AirportCEOModLoader;
using AirportCEOModLoader.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Enums;

namespace AirportCEOElevatedExteriors.ElevatedStructures.Patches;

[HarmonyPatch]
internal class TexturePatches
{
    [HarmonyPatch(typeof(PlaceableObject), nameof(PlaceableObject.SetZOnFloor))]
    [HarmonyPostfix]
    internal static void RoadPreserveLayerPatch(PlaceableObject __instance)
    {
        PlaceableItem pli = __instance as PlaceableItem;

        if (pli != null && !pli.isInside && pli.Floor > 0)
        {
            pli.overlay.transform.position = new Vector3(pli.overlay.transform.position.x, pli.overlay.transform.position.y, pli.Position.z);

            if (pli.itemType == ItemType.Hedge)
            {
                pli.transform.localPosition += new Vector3(0, 0, -0.0001f);
            }
        }

        if (__instance is PlaceableRoad && __instance.shadows != null && __instance.shadows.Length != 0)
        {
            // works
            __instance.shadows[0].ShadowSprite.sortingLayerName = "Default";
            __instance.shadows[0].ShadowSprite.sortingOrder = -1000;
        }
        if (__instance is PlaceableRoad && __instance.Floor > 0)
        {
            __instance.transform.GetChild(1).position += new Vector3(0, 0, -0.01f); // This is the one way road icon that we are changing here!
        }
    }

    [HarmonyPatch(typeof(RoadBuilder), nameof(RoadBuilder.SetBuilderPiece))]
    [HarmonyPostfix]
    internal static void RoadTextureShadowPatch(RoadBuilder __instance, Enums.BuilderPieceType pieceType)
    {
        IRoad road = __instance.nodeAttacher.plo as IRoad;
        if (road.RoadType != Enums.RoadType.PublicRoad || __instance.nodeAttacher.Floor <= 0)
        {
            return;
        }

        Sprite spriteToUse;
        if (road.Foundation == Enums.FoundationType.Asphalt)
        {
            spriteToUse = ElevatedStructureChangeManager.cutUpSpritesByTypeAsphalt[pieceType];
            __instance.spriteRenderer.sprite = spriteToUse;
        }
        else
        {
            spriteToUse = ElevatedStructureChangeManager.cutUpSpritesByTypeConcrete[pieceType];
            __instance.spriteRenderer.sprite = spriteToUse;
        }

        ShadowLogicManager.AddShadowToObjectIfNotAlready(__instance.nodeAttacher.plo, __instance.spriteRenderer.transform, spriteToUse, __instance.nodeAttacher.Floor);
    }

    [HarmonyPatch(typeof(PlaceableStructure), nameof(PlaceableStructure.ChangeToPlaced))]
    [HarmonyPostfix]
    internal static void StructureShadowPatch(PlaceableStructure __instance)
    {
        try
        {
            if (__instance.Floor <= 0 || !ElevatedStructureChangeManager.elevatableStructureTypes.Contains(__instance.structureType) || __instance is PlaceableRoad || __instance.structureType == StructureType.RoadTunnel)
            {
                return;
            }
            Sprite sprite = __instance.spriteTransform.Find("Foundation").GetComponent<SpriteRenderer>().sprite;
            if (sprite == null)
            {
                // nope not right now
                return;
            }
            ShadowLogicManager.AddShadowToObjectIfNotAlready(__instance, __instance.transform, sprite, __instance.Floor);
        }
        catch (Exception ex)
        {
            AirportCEOElevatedExteriors.EELogger.LogError($"Failed to add shadow to structure. {ExceptionUtils.ProccessException(ex)}");
        }
    }

    [HarmonyPatch(typeof(MergedTile), nameof(MergedTile.SetTile))]
    [HarmonyPostfix]
    internal static void SidewalkTexturePatch(MergedTile __instance, Vector2 size)
    {
        if (__instance.TileType != TileType.Sidewalk || __instance.Floor <= 0)
        {
            return;
        }

        __instance.transform.localPosition = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, FloorManager.TERMINAL_FLOOR_SHIFT);
        ShadowLogicManager.AddShadowToTileIfNotAlready(__instance.transform, SingletonNonDestroy<DataPlaceholderMaterials>.Instance.sideWalkTile, __instance.Floor, size);
    }

    [HarmonyPatch(typeof(RoadTunnel), nameof(RoadTunnel.ChangeToPlaced))]
    [HarmonyPostfix]
    internal static void RoadTunnelPatch(RoadTunnel __instance)
    {
        RoadTunnelLogicManager.UpdateTextureOnRoadTunnel(__instance);
    }

    [HarmonyPatch(typeof(RoadTunnel), nameof(RoadTunnel.ToggleSprite))]
    [HarmonyPostfix]
    internal static void RoadTunnelPatch3(RoadTunnel __instance)
    {
        RoadTunnelLogicManager.PreventFlippingTexture(__instance);
    }
}
