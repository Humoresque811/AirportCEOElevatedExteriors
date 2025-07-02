using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportCEOElevatedExteriors;

internal static class AirportCEOElevatedExteriorsConfig
{
    internal static ConfigEntry<bool> MakeChanges { get; private set; }

    internal static void SetUpConfig()
    {
        MakeChanges = AirportCEOElevatedExteriors.ConfigReference.Bind("General", "Make Placement Changes", true, "Make changes to allow structure placement above floor 0, including " +
            "updates to texture rendering to prevent visual bugs.");
    }
}
