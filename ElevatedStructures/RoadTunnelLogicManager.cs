using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AirportCEOElevatedExteriors.ElevatedStructures;

internal static class RoadTunnelLogicManager
{
    private const float rotationForTunnels = -0.262576240474f;

    private static Sprite tunnelTextureEnd => ElevatedStructureChangeManager.tunnelSpriteEnd;
    private static Sprite tunnelTextureFull => ElevatedStructureChangeManager.tunnelSpriteFull;

    private static Texture2D shadowTex;
    private static Material shadowMaterial;

    internal static List<CustomRoadTunnelTextureHandler> roadTunnelsWithShadows = new List<CustomRoadTunnelTextureHandler>();

    internal static void Awake()
    {
        shadowTex = new Texture2D(1, 1);
        shadowTex.SetPixels([new Color(1, 1, 1)]);

        shadowMaterial = new Material(SingletonNonDestroy<DataPlaceholderMaterials>.Instance.nonLitMateral);
        shadowMaterial.SetTexture("_MainTex", shadowTex);
        shadowMaterial.SetColor("_Color", new Color(0, 0, 0, 0.2f));
        shadowMaterial.renderQueue = 3000;        
    }

    internal static void UpdateTextureOnRoadTunnel(RoadTunnel tunnel)
    {
        if (((tunnel.EscalatorFloor == 1 && tunnel.Floor == 0) || (tunnel.EscalatorFloor == 0 && tunnel.Floor == 1)))
        {
            tunnel.tunnel.drawMode = SpriteDrawMode.Simple;
            tunnel.tunnel.size = new Vector2(1, 1);
            tunnel.tunnel.transform.localScale = new Vector3(1, 1, 1);
            tunnel.tunnel.sprite = tunnelTextureEnd;

            CustomRoadTunnelTextureHandler textureHandler = AddShadow(tunnel);
            AdjustPositionRotationOfTextures(tunnel, textureHandler, true);
        }
        else if (tunnel.EscalatorFloor > 0 && tunnel.Floor > 0)
        {
            tunnel.tunnel.drawMode = SpriteDrawMode.Simple;
            tunnel.tunnel.size = new Vector2(1, 1);
            tunnel.tunnel.transform.localScale = new Vector3(1, 1, 1);
            tunnel.tunnel.sprite = tunnelTextureFull;

            CustomRoadTunnelTextureHandler textureHandler = AddShadow(tunnel);
            AdjustPositionRotationOfTextures(tunnel, textureHandler, false);
        }
    }

    private static void AdjustPositionRotationOfTextures(RoadTunnel roadTunnel, CustomRoadTunnelTextureHandler textureHandler, bool needsRotation)
    {
        if (textureHandler == null || textureHandler.parentTunnel == null)
        {
            AirportCEOElevatedExteriors.EELogger.LogError("I hate you 2");
        }

        float zOffset = textureHandler.roadTunnelBottomFloor == 0 ? -0.055f : FloorManager.TERMINAL_FLOOR_SHIFT;
        roadTunnel.tunnel.transform.parent.transform.position = new Vector3(roadTunnel.transform.position.x, roadTunnel.transform.position.y, zOffset);
        roadTunnel.tunnel.transform.localPosition = new Vector3(roadTunnel.tunnel.transform.localPosition.x, roadTunnel.tunnel.transform.localPosition.y, -0.0001f);

        Transform overlay = roadTunnel.transform.Find("Overlay");
        if (overlay == null)
        {
            AirportCEOElevatedExteriors.EELogger.LogError($"Failed to find overlay of a road tunnel (just skipped that part of the code)");
        }
        else
        {
            overlay.position = new Vector3(overlay.position.x, overlay.position.y, FloorManager.TERMINAL_FLOOR_SHIFT);
        }

        if (!needsRotation)
        {
            return;
        }

        if (roadTunnel.escalatorDirection == Enums.EscalatorDirection.Up)
        {
            if (roadTunnel.transform.rotation.eulerAngles.z == 90)
            {
                roadTunnel.tunnel.transform.parent.eulerAngles = new Vector3(-rotationForTunnels, roadTunnel.transform.eulerAngles.y, roadTunnel.transform.eulerAngles.z);
            }
            else if (roadTunnel.transform.rotation.eulerAngles.z == 180)
            {
                roadTunnel.tunnel.transform.parent.eulerAngles = new Vector3(roadTunnel.transform.eulerAngles.y, -rotationForTunnels, roadTunnel.transform.eulerAngles.z);
            }
            else if (roadTunnel.transform.rotation.eulerAngles.z == 270)
            {
                roadTunnel.tunnel.transform.parent.eulerAngles = new Vector3(rotationForTunnels, roadTunnel.transform.eulerAngles.y, roadTunnel.transform.eulerAngles.z);
            }
            else if (roadTunnel.transform.rotation.eulerAngles.z == 0)
            {
                roadTunnel.tunnel.transform.parent.eulerAngles = new Vector3(roadTunnel.transform.eulerAngles.y, rotationForTunnels, roadTunnel.transform.eulerAngles.z);
            }
        }
        if (roadTunnel.escalatorDirection == Enums.EscalatorDirection.Down)
        {
            if (roadTunnel.transform.rotation.eulerAngles.z == 90)
            {
                roadTunnel.tunnel.transform.parent.eulerAngles = new Vector3(rotationForTunnels, roadTunnel.transform.eulerAngles.y, roadTunnel.transform.eulerAngles.z);
            }
            else if (roadTunnel.transform.rotation.eulerAngles.z == 180)
            {
                roadTunnel.tunnel.transform.parent.eulerAngles = new Vector3(roadTunnel.transform.eulerAngles.y, rotationForTunnels, roadTunnel.transform.eulerAngles.z);
            }
            else if (roadTunnel.transform.rotation.eulerAngles.z == 270)
            {
                roadTunnel.tunnel.transform.parent.eulerAngles = new Vector3(-rotationForTunnels, roadTunnel.transform.eulerAngles.y, roadTunnel.transform.eulerAngles.z);
            }
            else if (roadTunnel.transform.rotation.eulerAngles.z == 0)
            {
                roadTunnel.tunnel.transform.parent.eulerAngles = new Vector3(roadTunnel.transform.eulerAngles.y, -rotationForTunnels, roadTunnel.transform.eulerAngles.z);
            }
        }
    }

    internal static CustomRoadTunnelTextureHandler AddShadow(RoadTunnel tunnel)
    {
        Transform shadowObject = tunnel.transform.Find("customShadow(Clone)");

        if (shadowObject != null)
        {
            return tunnel.GetComponentInChildren<CustomRoadTunnelTextureHandler>(); 
        }

        shadowObject = GameObject.Instantiate(new GameObject("customShadow").transform);
        shadowObject.parent = tunnel.transform;

        shadowObject.eulerAngles = new Vector3(0, 0, 0);

        shadowObject.position = tunnel.transform.position;
        Vector3[] customQuad = GenerateVerticies(tunnel, new Vector2(0, 0), new Vector2(0, 0));

        AirportCEOElevatedExteriors.EELogger.LogInfo($"A mesh is found at {tunnel.transform.position}");

        Vector2[] uvs = new Vector2[4] {
            new Vector2(0, 0),
            new Vector2(3, 0),
            new Vector2(3, 3),
            new Vector2(0, 3)
        };

        int[] triangles = new int[6] { 0, 1, 2, 0, 2, 3 };

        Mesh mesh = new Mesh();
        mesh.vertices = customQuad;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        MeshFilter mf = shadowObject.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = shadowObject.gameObject.AddComponent<MeshRenderer>();
        mf.mesh = mesh;

        mr.material = shadowMaterial;


        CustomRoadTunnelTextureHandler customShadow = shadowObject.gameObject.AddComponent<CustomRoadTunnelTextureHandler>();
        customShadow.Init(tunnel);
        roadTunnelsWithShadows.Add(customShadow);
        return customShadow;
    }

    internal static Vector3[] GenerateVerticies(RoadTunnel roadTunnel, Vector2 offsetVectorLow, Vector2 offsetVectorHigh)
    {
        Vector3[] customQuad = null;

        if (roadTunnel.escalatorDirection == Enums.EscalatorDirection.Down)
        {
            Vector2 oldOffsetLow = offsetVectorLow;
            offsetVectorLow = offsetVectorHigh;
            offsetVectorHigh = oldOffsetLow;
        }

        if (roadTunnel.transform.rotation.eulerAngles.z == 0)
        {
             customQuad = new Vector3[4] {
                        new Vector3(+12 + offsetVectorLow.x,  +4 + offsetVectorLow.y, 0),
                        new Vector3(-12 + offsetVectorHigh.x, +4 + offsetVectorHigh.y, 0),
                        new Vector3(-12 + offsetVectorHigh.x, -4 + offsetVectorHigh.y, 0),
                        new Vector3(+12 + offsetVectorLow.x,  -4 + offsetVectorLow.y, 0)
            };        
        }
        else if (roadTunnel.transform.rotation.eulerAngles.z == 90)
        {
             customQuad = new Vector3[4] {
                        new Vector3(+4 + offsetVectorLow.x,  +12 + offsetVectorLow.y, 0),
                        new Vector3(+4 + offsetVectorHigh.x, -12 + offsetVectorHigh.y, 0),
                        new Vector3(-4 + offsetVectorHigh.x, -12 + offsetVectorHigh.y, 0),
                        new Vector3(-4 + offsetVectorLow.x,  +12 + offsetVectorLow.y, 0)
            };        
        }
        else if (roadTunnel.transform.rotation.eulerAngles.z == 180)
        {
             customQuad = new Vector3[4] {
                        new Vector3(-12 + offsetVectorLow.x,  -4 + offsetVectorLow.y, 0),
                        new Vector3(+12 + offsetVectorHigh.x, -4 + offsetVectorHigh.y, 0),
                        new Vector3(+12 + offsetVectorHigh.x, +4 + offsetVectorHigh.y, 0),
                        new Vector3(-12 + offsetVectorLow.x,  +4 + offsetVectorLow.y, 0)
            };        
        }
        else if (roadTunnel.transform.rotation.eulerAngles.z == 270)
        {
             customQuad = new Vector3[4] {
                        new Vector3(-4 + offsetVectorLow.x,  -12 + offsetVectorLow.y, 0),
                        new Vector3(-4 + offsetVectorHigh.x, +12 + offsetVectorHigh.y, 0),
                        new Vector3(+4 + offsetVectorHigh.x, +12 + offsetVectorHigh.y, 0),
                        new Vector3(+4 + offsetVectorLow.x,  -12 + offsetVectorLow.y, 0)
            };        
        }

        return customQuad;
    }

    internal static void PreventFlippingTexture(RoadTunnel tunnel)
    {
        if (!tunnel.isPlaced)
        {
            return;
        }

        if (((tunnel.EscalatorFloor == 1 && tunnel.Floor == 0) || (tunnel.EscalatorFloor == 0 && tunnel.Floor == 1)) && FloorManager.currentFloor > 0)
        {
            tunnel.tunnel.flipX = !tunnel.tunnel.flipX;
        }
    }
}
