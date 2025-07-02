using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;
using static UnityEngine.UIElements.UIRAtlasAllocator;

namespace AirportCEOElevatedExteriors.ElevatedStructures;

internal class ElevatedStructureChangeManager
{
    internal static readonly List<Enums.StructureType> elevatableStructureTypes = new List<Enums.StructureType>() { Enums.StructureType.Road, Enums.StructureType.BusStop, 
        Enums.StructureType.SideWalk, Enums.StructureType.RoadTunnel, Enums.StructureType.Crosswalk, Enums.StructureType.CarStop, Enums.StructureType.StreetLight, 
        Enums.StructureType.FloodLight, Enums.StructureType.TaxiStop, Enums.StructureType.PersonCarParkingLot};

    static Texture2D spriteSheet;

    static Texture2D tunnelFull;
    internal static Sprite tunnelSpriteFull;
    static Texture2D tunnelEnd;
    internal static Sprite tunnelSpriteEnd;

    static Sprite[] cutUpSprites;
    internal static Dictionary<Enums.BuilderPieceType, Sprite> cutUpSpritesByType = new Dictionary<Enums.BuilderPieceType, Sprite>();
    internal static void PrepareTextures(SaveLoadGameDataController _)
    {
        spriteSheet = LoadTexture("C:/My Stuff/ACEO Texture Work/Airport CEO Textures/ReRoad/Elevated Exteriors/spriteSheet.png");

        tunnelEnd = LoadTexture("C:/My Stuff/ACEO Texture Work/Airport CEO Textures/ReRoad/Elevated Exteriors/tunnel.png");
        tunnelSpriteEnd = Sprite.Create(tunnelEnd, new Rect(0f, 0f, tunnelEnd.width, tunnelEnd.height), Vector2.one / 2f, 256, 0u, SpriteMeshType.FullRect);
        tunnelFull = LoadTexture("C:/My Stuff/ACEO Texture Work/Airport CEO Textures/ReRoad/Elevated Exteriors/tunnelFull.png");
        tunnelSpriteFull = Sprite.Create(tunnelFull, new Rect(0f, 0f, tunnelFull.width, tunnelFull.height), Vector2.one / 2f, 256, 0u, SpriteMeshType.FullRect);

        CutSprites(spriteSheet);
    }
    private static void CutSprites(Texture2D sheet)
    {
        int rows = sheet.height / 1024;
        float rowHalf = (rows - 1) / 2f;
        int columns = sheet.width / 1024;

        cutUpSprites = new Sprite[rows * columns];

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                cutUpSprites[row * columns + column] = Sprite.Create(sheet, new Rect(new Vector2(column * 1024, (sheet.height - 1024 - row * 1024)), new Vector2(1024, 1024)), Vector2.one / 2f, 256, 0u, SpriteMeshType.FullRect);
            }
        }

        cutUpSpritesByType[Enums.BuilderPieceType.Straight] =               cutUpSprites[2];
        cutUpSpritesByType[Enums.BuilderPieceType.Turn] =                   cutUpSprites[13];
        cutUpSpritesByType[Enums.BuilderPieceType.TTurn] =                  cutUpSprites[9];
        cutUpSpritesByType[Enums.BuilderPieceType.XTurn] =                  cutUpSprites[8];
        cutUpSpritesByType[Enums.BuilderPieceType.End] =                    cutUpSprites[15];
        cutUpSpritesByType[Enums.BuilderPieceType.DoubleEnd] =              cutUpSprites[7];
        cutUpSpritesByType[Enums.BuilderPieceType.Corner] =                 cutUpSprites[11];
        cutUpSpritesByType[Enums.BuilderPieceType.ThreeTurn] =              cutUpSprites[1];
        cutUpSpritesByType[Enums.BuilderPieceType.ThreeTurnFlipped] =       cutUpSprites[3];
        cutUpSpritesByType[Enums.BuilderPieceType.Center] =                 cutUpSprites[10];
        cutUpSpritesByType[Enums.BuilderPieceType.SingleToNormal] =         cutUpSprites[4];
        cutUpSpritesByType[Enums.BuilderPieceType.SingleToNormalFlipped] =  cutUpSprites[5];
        cutUpSpritesByType[Enums.BuilderPieceType.SingleTurn] =             cutUpSprites[0];
        cutUpSpritesByType[Enums.BuilderPieceType.SingleStraight] =         cutUpSprites[14];
        cutUpSpritesByType[Enums.BuilderPieceType.SingleTTurn] =            cutUpSprites[6];
        cutUpSpritesByType[Enums.BuilderPieceType.SingleDiagonal] =         cutUpSprites[12];
    }

    private static Texture2D LoadTexture(string filePath)
    {
        Texture2D result = null;
	    if (File.Exists(filePath))
	    {
		    byte[] data = File.ReadAllBytes(filePath);
		    Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, true)
		    {
			    filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                loadAllMips = true,
		    };
		    texture2D.LoadImage(data);
		    if (GameSettingManager.CompressImages)
		    {
			    texture2D.Compress(highQuality: true);
		    }
            result = texture2D;
	    }
        else
        {
            AirportCEOElevatedExteriors.EELogger.LogError("File not found!");
        }
	    return result;
    }
}
