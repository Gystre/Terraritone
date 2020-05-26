using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Terraria;

public enum TileType
{
    Empty,
    Block,
    OneWay
}

public partial class PathMap
{
    internal static PathMap instance;

    public Vector2 goal;

    public bool debugGrid;

    //map's Position in world space. Bottom left corner
    public Vector2 Position;

    public Pathfinding Pathfinder;

    //two arrays for storing grid information:
    //Grid = array for use by pathfinder, will be larger than tiles for faster accesses
    //tiles = readable, enum based array, exact size of map, not used by pathfinder
    //nodes that are fed to the pathfinder (0 = block, 1 = empty)
    public byte[,] Grid;

    //map's tile data
    public TileType[,] tiles;

    //size of a tile (16 pixels wide and high)
    static public int Tilesize = 16;

    //width and height of map
    public int Width;
    public int Height;

    //and finally the path
    public List<Point> Path = new List<Point>();

    #region Get certain tile helper functions
    public TileType GetTile(int x, int y)
    {
        if (x < 0 || x >= Width
            || y < 0 || y >= Height)
            return TileType.Block;

        return tiles[x, y];
    }

    public bool IsOneWayPlatform(int x, int y)
    {
        if (x < 0 || x >= Width
            || y < 0 || y >= Height)
            return false;

        return (tiles[x, y] == TileType.OneWay);
    }

    public bool IsGround(int x, int y)
    {
        if (x < 0 || x >= Width
           || y < 0 || y >= Height)
            return false;

        return (tiles[x, y] == TileType.OneWay || tiles[x, y] == TileType.Block);
    }

    public bool IsObstacle(int x, int y)
    {
        if (x < 0 || x >= Width
            || y < 0 || y >= Height)
            return true;

        return (tiles[x, y] == TileType.Block);
    }

    public bool IsNotEmpty(int x, int y)
    {
        if (x < 0 || x >= Width
            || y < 0 || y >= Height)
            return true;

        return (tiles[x, y] != TileType.Empty);
    }
    #endregion

    public void InitPathFinder()
    {
        Pathfinder = new Pathfinding(Grid, this);

        Pathfinder.Formula = HeuristicFormula.Manhattan;
        //if false then diagonal movement will be prohibited
        Pathfinder.Diagonals = false;
        //if true then diagonal movement will have higher cost
        Pathfinder.HeavyDiagonals = false;
        //estimate of path length
        Pathfinder.HeuristicEstimate = 6;
        Pathfinder.PunishChangeDirection = false;
        Pathfinder.TieBreaker = false;
        Pathfinder.SearchLimit = 10000;
        Pathfinder.DebugProgress = false;
        Pathfinder.DebugFoundPath = false;
    }

    public void SetTile(int x, int y, TileType type)
    {
        if (x <= 1 || x >= Width - 2 || y <= 1 || y >= Height - 2)
            return;

        tiles[x, y] = type;

        if (type == TileType.Block)
        {
            Grid[x, y] = 0;
        }
        else if (type == TileType.OneWay)
        {
            Grid[x, y] = 1;
        }
        else
        {
            Grid[x, y] = 1; //empty
        }
    }

    //handles up to largest 32 bit number which is like 1.4 billion or smthn
    //neat right????
    int NextPowerOfTwo(int v)
    {
        v--;
        v |= v >> 1;
        v |= v >> 2;
        v |= v >> 4;
        v |= v >> 8;
        v |= v >> 16;
        v++;

        return v;
    }

    TileType GetTypeInTerrariaWorld(int x, int y)
    {
        Tile tile = Main.tile[x, y];

        //air
        if (tile.active() == false && Main.tileSolid[tile.type] && Main.tileSolidTop[tile.type] == false){
            return TileType.Empty;
        }
        //decor
        else if (tile.active() && Main.tileSolid[tile.type] == false && Main.tileSolidTop[tile.type] == false)
        {
            return TileType.Empty;
        }
        //platform
        else if (tile.active() && Main.tileSolid[tile.type] && Main.tileSolidTop[tile.type]) {
            return TileType.OneWay;
        }
        else
        {
            return TileType.Block;
        }

    }

    public void FindPath()
    {
        Path.Clear();

        //start at player's feet for now
        Point lowerPoint = Main.LocalPlayer.position.ToTileCoordinates();
        lowerPoint.Y += 2;

        Main.NewText(Main.LocalPlayer.height);
        Main.NewText(Main.LocalPlayer.width);


        List<Point> container = Pathfinder.CalculatePath(lowerPoint,
                                                goal.ToPoint(),
                                                2, 3, 6);

        if (container != null && container.Count > 1)
        {
            Main.NewText("Successfully found a path");
            for(var i = container.Count - 1; i >= 0; --i)
            {
                Path.Add(container[i]);
            }
        }
        else
        {
            Main.NewText("Couldn't find a path!");
        }
    }

    public PathMap()
    {
        goal = new Vector2(-1, -1);

        int width = Enumerable.Range(0, Main.tile.GetLength(0)).Count(i => Main.tile[i, 0] != null);
        int height = Enumerable.Range(0, Main.tile.GetLength(1)).Count(i => Main.tile[0, i] != null);

        Position = new Vector2(0, 0);
        Width = width;
        Height = height;

        tiles = new TileType[Width, Height];

        Grid = new byte[NextPowerOfTwo(Width), NextPowerOfTwo(Height)];

        InitPathFinder();

        for(int y = 0; y < Height; ++y)
        {
            for(int x = 0; x < Width; ++x)
            {
                SetTile(x, y, GetTypeInTerrariaWorld(x, y));
            }
        }

        //tiles at [x, 2] and [2, y] are blocks (2 block buffer up top and to left of array)
        for(int y = 0; y < Height; ++y)
        {
            tiles[1, y] = TileType.Block;
            tiles[Width - 2, y] = TileType.Block;
        }

        for(int x = 0; x < Width; ++x)
        {
            tiles[x, 1] = TileType.Block;
            tiles[x, Height - 2] = TileType.Block;
        }
    }
}
