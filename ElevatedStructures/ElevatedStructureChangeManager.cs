using AirportCEOElevatedExteriors.ElevatedStructures.Patches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
    internal static Dictionary<Enums.BuilderPieceType, Sprite> cutUpSpritesByTypeAsphalt = new Dictionary<Enums.BuilderPieceType, Sprite>();
    internal static Dictionary<Enums.BuilderPieceType, Sprite> cutUpSpritesByTypeConcrete = new Dictionary<Enums.BuilderPieceType, Sprite>();

    internal static void AllowForTextureLoad(string basepath)
    {
        DelayTextureLoadPatch.workshopInfoCache = basepath;

    }

    internal static void PrepareTextures(string basepath)
    {
        spriteSheet = LoadTexture(Path.Combine(basepath, "spriteSheet.png"));

        tunnelEnd = LoadTexture(Path.Combine(basepath, "tunnel.png"));
        tunnelSpriteEnd = Sprite.Create(tunnelEnd, new Rect(0f, 0f, tunnelEnd.width, tunnelEnd.height), Vector2.one / 2f, 256, 0u, SpriteMeshType.FullRect);
        tunnelFull = LoadTexture(Path.Combine(basepath, "tunnelFull.png"));
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

        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.Straight] =               cutUpSprites[2];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.Turn] =                   cutUpSprites[13];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.TTurn] =                  cutUpSprites[9];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.XTurn] =                  cutUpSprites[8];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.End] =                    cutUpSprites[15];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.DoubleEnd] =              cutUpSprites[7];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.Corner] =                 cutUpSprites[11];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.ThreeTurn] =              cutUpSprites[1];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.ThreeTurnFlipped] =       cutUpSprites[3];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.Center] =                 cutUpSprites[10];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.SingleToNormal] =         cutUpSprites[4];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.SingleToNormalFlipped] =  cutUpSprites[5];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.SingleTurn] =             cutUpSprites[0];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.SingleStraight] =         cutUpSprites[14];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.SingleTTurn] =            cutUpSprites[6];
        cutUpSpritesByTypeAsphalt[Enums.BuilderPieceType.SingleDiagonal] =         cutUpSprites[12];

        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.Straight] =               cutUpSprites[16 + 2];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.Turn] =                   cutUpSprites[16 + 13];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.TTurn] =                  cutUpSprites[16 + 9];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.XTurn] =                  cutUpSprites[16 + 8];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.End] =                    cutUpSprites[16 + 15];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.DoubleEnd] =              cutUpSprites[16 + 7];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.Corner] =                 cutUpSprites[16 + 11];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.ThreeTurn] =              cutUpSprites[16 + 1];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.ThreeTurnFlipped] =       cutUpSprites[16 + 3];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.Center] =                 cutUpSprites[16 + 10];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.SingleToNormal] =         cutUpSprites[16 + 4];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.SingleToNormalFlipped] =  cutUpSprites[16 + 5];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.SingleTurn] =             cutUpSprites[16 + 0];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.SingleStraight] =         cutUpSprites[16 + 14];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.SingleTTurn] =            cutUpSprites[16 + 6];
        cutUpSpritesByTypeConcrete[Enums.BuilderPieceType.SingleDiagonal] =         cutUpSprites[16 + 12];
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
