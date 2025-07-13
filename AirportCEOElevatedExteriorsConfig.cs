using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportCEOElevatedExteriors;

internal static class AirportCEOElevatedExteriorsConfig
{
    internal static ConfigEntry<bool> CinematicMode { get; private set; }

    internal static void SetUpConfig()
    {
        CinematicMode = AirportCEOElevatedExteriors.ConfigReference.Bind("General", "Cinematic Mode", false, "Forces camera to be on floor 3 when following a vehicle, or when the floor " +
            "is changed with most methods besides PGUP and PGDOWN. Most useful for filming stuff");
    }
}
