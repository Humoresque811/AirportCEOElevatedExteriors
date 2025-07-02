using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AirportCEOElevatedExteriors.ElevatedStructures;

internal class CustomRoadTunnelTextureHandler : MonoBehaviour
{
    internal RoadTunnel parentTunnel;
    internal MeshFilter meshFilter;
    internal MeshRenderer meshRenderer;

    internal int roadTunnelTopFloor => Mathf.Max(parentTunnel.Floor, parentTunnel.EscalatorFloor);
    internal int roadTunnelBottomFloor => Mathf.Min(parentTunnel.Floor, parentTunnel.EscalatorFloor);
    internal Mesh mesh => meshFilter.mesh;

    internal Vector3 CachedTransformPosition { get; private set; }

    internal void Init(RoadTunnel roadTunnel)
    { 
        parentTunnel = roadTunnel;
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        CachedTransformPosition = parentTunnel.transform.position;
    }
}
