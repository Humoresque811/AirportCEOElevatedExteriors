using AirportCEOElevatedExteriors.ElevatedStructures;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace AirportCEOElevatedExteriors;

[BepInPlugin("org.airportceoelevatedexteriors.humoresque", "AirportCEO Elevated Exteriors", PluginInfo.PLUGIN_VERSION)]
[BepInDependency("org.airportceomodloader.humoresque")]
public class AirportCEOElevatedExteriors : BaseUnityPlugin
{
    public static AirportCEOElevatedExteriors Instance { get; private set; }
    internal static Harmony Harmony { get; private set; }
    internal static ManualLogSource EELogger { get; private set; }
    internal static ConfigFile ConfigReference {  get; private set; }


    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

        Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        Harmony.PatchAll(); 

        Instance = this;
        EELogger = Logger;
        ConfigReference = Config;

        // Config
        Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is setting up config.");
        AirportCEOElevatedExteriorsConfig.SetUpConfig();
        Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} finished setting up config.");

    }

    private void Start()
    {
        AirportCEOModLoader.WorkshopUtils.WorkshopUtils.Register("ElevatedExteriorSprites", ElevatedStructureChangeManager.AllowForTextureLoad);
        AirportCEOModLoader.WatermarkUtils.WatermarkUtils.Register(new AirportCEOModLoader.WatermarkUtils.WatermarkInfo("EE", "1.0", true));

        if (AirportCEOElevatedExteriorsConfig.AutomaticallyTurnModOn.Value)
        {
            ModManager.ActivateMod("14ad6366-bed7-4fdf-96fc-18e74bb068c7"); // We just quietly activate ourselves so that the textures load
        }


        RoadTunnelLogicManager.Awake();
    }
}
