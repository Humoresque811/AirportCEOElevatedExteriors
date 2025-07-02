using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AirportCEOElevatedExteriors.ElevatedStructures;

internal static class ShadowLogicManager
{
    // Preset Variables - No in line values allowed lol

    // Floor 2: -0.22
    private static readonly float Floor2ShadowHeight = 0.11f;
    private static readonly int Floor2ShadowDistance = 6;
    // Floor 1: -0.11
    private static readonly float Floor1ShadowHeight = 0.11f;
    private static readonly int Floor1ShadowDistance = 3;
    // Floor 0: 0

    internal static void AddShadowToTileIfNotAlready(Transform parentTransform, Sprite spriteReference, int floor, Vector2 size, bool useAlternateShadowHeight = false)
    {
        Transform shadowObject = parentTransform.Find("customShadow(Clone)");

        if (shadowObject != null)
        {
            return; 
        }

        shadowObject = GameObject.Instantiate(new GameObject("customShadow").transform);
        shadowObject.parent = parentTransform;
        SpriteRenderer renderer = shadowObject.gameObject.AddComponent<SpriteRenderer>();
        renderer.drawMode = SpriteDrawMode.Tiled;
		renderer.tileMode = SpriteTileMode.Adaptive;
        shadowObject.gameObject.SetActive(true);

        ShadowHandler shadowHandler = shadowObject.gameObject.AddComponent<ShadowHandler>();
        shadowHandler.fixedShadowRotation = false;
        shadowHandler.shadowSprite = renderer;
        shadowHandler.shadowSprite.sprite = spriteReference;
        shadowHandler.shadowDistance = GetShadowDistance(floor);
        shadowHandler.shouldUpdateShadow = true;
        shadowHandler.referenceTransform = parentTransform;
        shadowHandler.ActivateShadow(false, false);
        float localz = useAlternateShadowHeight ? 0.055f : GetShadowLocalPosZ(floor);
        shadowObject.localPosition = new Vector3(0, 0, localz);
        shadowHandler.SetShadowHeight(localz);
        renderer.size = size;
        AirportCEOElevatedExteriors.EELogger.LogInfo($"size once set is {renderer.size}" );
    }

    internal static void AddShadowToObjectIfNotAlready(PlaceableObject plo, Transform parentTransform, Sprite spriteReference, int floor)
    {
        Transform shadowObject = parentTransform.Find("customShadow(Clone)");

        if (shadowObject != null)
        {
            return; 
        }

        shadowObject = GameObject.Instantiate(new GameObject("customShadow").transform);
        shadowObject.parent = parentTransform;
        SpriteRenderer renderer = shadowObject.gameObject.AddComponent<SpriteRenderer>();
        shadowObject.gameObject.SetActive(true);

        ShadowHandler shadowHandler = shadowObject.gameObject.AddComponent<ShadowHandler>();
        shadowHandler.fixedShadowRotation = false;
        shadowHandler.shadowSprite = renderer;
        shadowHandler.shadowSprite.sprite = spriteReference;
        shadowHandler.shadowDistance = GetShadowDistance(floor);
        shadowHandler.shouldUpdateShadow = true;
        shadowHandler.referenceTransform = parentTransform;
        shadowHandler.ActivateShadow(false, false);
        plo.shadows = [shadowHandler];
        plo.EnableDisableShadows(true);
        shadowObject.localPosition = new Vector3(0, 0, GetShadowLocalPosZ(floor));
        shadowObject.rotation = plo.transform.rotation;

        if (plo is PlaceableStructure && (plo as PlaceableStructure).structureType == Enums.StructureType.PersonCarParkingLot)
        {
            shadowObject.eulerAngles = new Vector3(shadowObject.eulerAngles.x, shadowObject.eulerAngles.y, shadowObject.eulerAngles.z + 90);
        }

        shadowHandler.SetShadowHeight(GetShadowLocalPosZ(floor));
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = -1000;
    }

    internal static void UpdateCustomShadow(PlaceableStructure structure, int floorBeingViewed, int floorOfStructure)
    {
        Transform shadowObject = structure.transform.Find("customShadow(Clone)") ?? structure.transform.Find("Sprite")?.Find("customShadow(Clone)");

        if (shadowObject == null)
        {
            return;
        }

        // We have a custom shadow that *we* added

        if (floorBeingViewed < 0)
        {
            shadowObject.gameObject.SetActive(false);
        }
        else
        {
            shadowObject.gameObject.SetActive(true);
        }
    }
    internal static void UpdateCustomShadow(MergedTile structure, int floorBeingViewed, int floorOfStructure)
    {
        Transform shadowObject = structure.transform.Find("customShadow(Clone)") ?? structure.transform.Find("Sprite")?.Find("customShadow(Clone)");

        if (shadowObject == null)
        {
            return;
        }

        // We have a custom shadow that *we* added

        if (floorBeingViewed < 0)
        {
            shadowObject.gameObject.SetActive(false);
        }
        else
        {
            shadowObject.gameObject.SetActive(true);
        }
    }

    internal static void RoadTunnelShadowRepeated()
    {
        foreach (CustomRoadTunnelTextureHandler customShadowHandler in RoadTunnelLogicManager.roadTunnelsWithShadows)
        {
            if (customShadowHandler == null)
            {
                continue;
            }

            int shadowDistanceHigh = GetShadowDistance(customShadowHandler.roadTunnelTopFloor);
            int shadowDistanceLow = GetShadowDistance(customShadowHandler.roadTunnelBottomFloor);
            float shadowHeight = GetShadowLocalPosZ(customShadowHandler.roadTunnelTopFloor);

		    Vector2 offsetVectorHigh = new Vector2(EnvironmentController.cosAngle, EnvironmentController.sinAngle) * shadowDistanceHigh;
		    Vector2 offsetVectorLow = new Vector2(EnvironmentController.cosAngle, EnvironmentController.sinAngle) * shadowDistanceLow;

            Mesh mesh = customShadowHandler.mesh;
            customShadowHandler.meshRenderer.material.color =  Singleton<EnvironmentController>.Instance.GetCurrentOutsideShadowColor();
            mesh.vertices = RoadTunnelLogicManager.GenerateVerticies(customShadowHandler.parentTunnel, offsetVectorLow, offsetVectorHigh);
        }
    }

    private static int GetShadowDistance(int floor)
    {
        switch (floor)
        {
            case 2:
                return Floor2ShadowDistance;
            case 1:
                return Floor1ShadowDistance;
            default:
                return 0;
        }
    }
    private static float GetShadowLocalPosZ(int floor)
    {
        switch (floor)
        {
            case 2:
                return Floor2ShadowHeight;
            case 1:
                return Floor1ShadowHeight;
            default:
                return 0.1f;
        }
    }
}
