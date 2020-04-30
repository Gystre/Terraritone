using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

public class Grid
{
    public Node[,] grid;
    public List<Node> path;

    public int gridSizeX, gridSizeY;

    public Grid()
    {
        //get width and height of world
        gridSizeX = Enumerable.Range(0, Main.tile.GetLength(0)).Count(i => Main.tile[i, 0] != null) + 1;
        gridSizeY = Enumerable.Range(0, Main.tile.GetLength(1)).Count(i => Main.tile[0, i] != null) + 2;

        CreateGrid();
        Main.NewText("Created grid " + gridSizeX + "x" + gridSizeY);
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        for(int x = 0; x < 2; x++)
        {
            for(int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = new Node(false, new Vector2(0, 0), x, y);
            }
        }

        for(int i = 0; i < grid.GetLength(0); i++)
        {
            grid[i, 0] = new Node(false, new Vector2(0, 0), i, 0);
        }

        for (int x = 0; x < gridSizeX - 1; x++)
        {
            for (int y = 0; y < gridSizeY - 2; y++)
            {
                Tile tile = Main.tile[x, y];
                Vector2 worldPoint = new Vector2(x * 16 + 8, y * 16 + 8); //center of the tile in world coordinates
                bool walkable = false;
                //if tile is air, platform, or decor then it's walkable
                if ((tile.active() == false && Main.tileSolid[tile.type] && Main.tileSolidTop[tile.type] == false) || //air
                    (tile.active() && Main.tileSolid[tile.type] == false && Main.tileSolidTop[tile.type] == false) || //decor
                    (tile.active() && Main.tileSolid[tile.type] && Main.tileSolidTop[tile.type])) //platform
                {
                    walkable = true;
                }

                //if this tile isn't walkable then the two tiles above and the one tile the left isn't walkable, make space for the player
                if (!walkable)
                {
                    grid[x+1, y].walkable = false;
                    grid[x + 1, y + 1].walkable = false;
                    grid[x, y+2].walkable = false;
                }

                grid[x+1, y+2] = new Node(walkable, worldPoint, x+1, y+2);
            }
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        //searching in a 3x3 block around the node
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue; //skips iteration

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                //is node inside of grid?
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    public static T Clamp<T>(T value, T max, T min)
     where T : System.IComparable<T>
    {
        T result = value;
        if (value.CompareTo(max) > 0)
            result = max;
        if (value.CompareTo(min) < 0)
            result = min;
        return result;
    }
}
