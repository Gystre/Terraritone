using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

public class Pathfinding
{
    internal static Pathfinding instance;
    public bool debugGrid;
    public Grid grid;
    public Vector2 goal;
    List<Move> MoveList;

    public Pathfinding()
    {
        debugGrid = false;
        goal = new Vector2(-1, -1);
        MoveList = new List<Move>();
    }

    public void Start()
    {
        FindPath();
        GenerateMovements();
    }

    private void FindPath()
    {
        Point playerTileCoords = Main.LocalPlayer.position.ToTileCoordinates();
        Node startNode = grid.grid[playerTileCoords.X + 1, playerTileCoords.Y + 2];
        Node targetNode = grid.grid[(int)goal.X + 1, (int)goal.Y + 2];

        //this is a heap instead of a List b/c it is way less comparisons = more efficient
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize); //nodes that need to be evaluated
        HashSet<Node> closedSet = new HashSet<Node>(); //nodes that have already been evaulated

        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            //find the node with the lowest fCost and set it to currentNode
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                //check if neighboring nodes are traversable or in closed set
                if (!neighbor.walkable || closedSet.Contains(neighbor)) //weird nullpointerexception here, has to do with setting a goal to unwalkable tile (ropes or seomthing)
                {
                    continue;
                }

                //the actual 'pathfinding algorithm' thinking here
                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    //set the g and h costs of the neighboring nodes
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    //add the neighbor node to the open set
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbor); //value already in the openset and need to update it
                    }
                }
            }
        }
    }

    private void GenerateMovements()
    {
        if(grid.path == null)
        {
            throw new Exception();
        }

        for(int i = 0; i < grid.path.Count - 1; i++)
        {
            Node src = grid.path.ElementAt(i);
            Node dst = grid.path.ElementAt(i + 1);
            //could probably speed things up here by multiplying moves by 16 so don't have to do division here
            //i just want something that works... :(
            Vector2 direction = new Vector2((dst.worldPos.X - src.worldPos.X) / 16, (dst.worldPos.Y - src.worldPos.Y) / 16); 
            foreach (Move move in Moves.List)
            {
                if(direction == move.cost)
                {
                    Main.NewText(move.name);
                }
            }
        }
    }

    void RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    int GetDistance(Node a, Node b)
    {
        int dstX = Math.Abs(a.gridX - b.gridX);
        int dstY = Math.Abs(a.gridY - b.gridY);

        if (dstX > dstY)
        {
            //14 is cost for diagonal
            //10 is cost for straight move
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }
}
