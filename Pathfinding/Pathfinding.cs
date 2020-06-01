using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Terraria;

/*
 * TODO:
 * potential improvements here,
 *  1. remove tile type array (saves ~100mb of ram)
 *  2. use Main.tile and calculate grid at runtime so can manipulate world without having to recalculate path
 */

public enum HeuristicFormula
{
    Manhattan = 1,
    MaxDXDY = 2,
    DiagonalShortCut = 3,
    Euclidean = 4,
    EuclideanNoSQR = 5,
    Custom1 = 6
}

public struct Location
{
    public Location(int xy, int z)
    {
        this.xy = xy;
        this.z = z;
    }

    public int xy;
    public int z;
}

public class Pathfinding
{
    internal class ComparePFNodeMatrix : IComparer<Location>
    {
        List<Node>[] Matrix;

        public ComparePFNodeMatrix(List<Node>[] matrix)
        {
            Matrix = matrix;
        }

        public int Compare(Location a, Location b)
        {
            if (Matrix[a.xy][a.z].F > Matrix[b.xy][b.z].F)
                return 1;
            else if (Matrix[a.xy][a.z].F < Matrix[b.xy][b.z].F)
                return -1;
            return 0;
        }
    }

    private List<Node>[] nodes; //3d array containing x, y, and jump values of every node
    private Stack<int> TouchedLocations; //container to keep track of which nodes were touched to clear them later;

    public PathMap Map;

    // Heap variables are initializated to default
    private byte[,] Grid = null;
    private PriorityQueueB<Location> Open = null;
    private List<Point> Close = null;
    private bool Stop = false;
    private byte OpenNodeValue = 1;
    private byte CloseNodeValue = 2;
    //vars that can be changed with get
    private bool mStopped = true;
    private HeuristicFormula mFormula = HeuristicFormula.Manhattan;
    private bool mDiagonals = true;
    private int mHEstimate = 2;
    private bool mPunishChangeDirection = false;
    private bool mTieBreaker = false;
    private bool mHeavyDiagonals = false;
    private int mSearchLimit = 2000;
    private double mCompletedTime = 0;
    private bool mDebugProgress = false;
    private bool mDebugFoundPath = false;

    //Promoted local variables to member variables to avoid recreation between calls
    private int H = 0;
    private Location Location;
    private int NewLocation = 0;
    private Node Node;
    private ushort LocationX = 0;
    private ushort LocationY = 0;
    private ushort NewLocationX = 0;
    private ushort NewLocationY = 0;
    private int CloseNodeCounter = 0;
    private ushort GridX = 0;
    private ushort GridY = 0;
    private ushort GridXMinus1 = 0;
    private ushort GridXLog2 = 0;
    private bool Found = false;
    private int EndLocation = 0;
    private int NewG = 0;
    private sbyte[,] mDirection = new sbyte[8, 2] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };

    public Pathfinding(byte[,] grid, PathMap map)
    {
        if (map == null)
            throw new Exception("Map is null");
        if (grid == null)
            throw new Exception("Grid is null");

        Map = map;
        Grid = grid;
        GridX = (ushort)(Grid.GetUpperBound(0) + 1);
        GridY = (ushort)(Grid.GetUpperBound(1) + 1);
        GridXMinus1 = (ushort)(GridX - 1);
        GridXLog2 = (ushort)Math.Log(GridX, 2);

        if (Math.Log(GridX, 2) != (int)Math.Log(GridX, 2) ||
                Math.Log(GridY, 2) != (int)Math.Log(GridY, 2))
            throw new Exception("Invalid Grid, size in X and Y must be power of 2");

        if (nodes == null || nodes.Length != (GridX * GridY))
        {
            nodes = new List<Node>[GridX * GridY];
            TouchedLocations = new Stack<int>(GridX * GridY);
            Close = new List<Point>(GridX * GridY);
        }

        for (var i = 0; i < nodes.Length; ++i)
        {
            nodes[i] = new List<Node>(1);
        }

        Open = new PriorityQueueB<Location>(new ComparePFNodeMatrix(nodes));
    }

    #region Getter and setter functions
    public bool Stopped
    {
        get { return Stopped; }
    }

    public HeuristicFormula Formula
    {
        get { return mFormula; }
        set { mFormula = value; }
    }

    public bool Diagonals
    {
        get { return mDiagonals; }
        set
        {
            mDiagonals = value;
            if (mDiagonals)
                mDirection = new sbyte[8, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 } };
            else
                mDirection = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
        }
    }

    public bool HeavyDiagonals
    {
        get { return mHeavyDiagonals; }
        set { mHeavyDiagonals = value; }
    }

    public int HeuristicEstimate
    {
        get { return mHEstimate; }
        set { mHEstimate = value; }
    }

    public bool PunishChangeDirection
    {
        get { return mPunishChangeDirection; }
        set { mPunishChangeDirection = value; }
    }

    public bool TieBreaker
    {
        get { return mTieBreaker; }
        set { mTieBreaker = value; }
    }

    public int SearchLimit
    {
        get { return mSearchLimit; }
        set { mSearchLimit = value; }
    }

    public double CompletedTime
    {
        get { return mCompletedTime; }
        set { mCompletedTime = value; }
    }

    public bool DebugProgress
    {
        get { return mDebugProgress; }
        set { mDebugProgress = value; }
    }

    public bool DebugFoundPath
    {
        get { return mDebugFoundPath; }
        set { mDebugFoundPath = value; }
    }
    #endregion


    public List<Point> CalculatePath(Point start, Point end, int characterWidth, int characterHeight, short maxJumpHeight)
    {
        lock (this) //locked to prevent multiple algorithms from running at the same time
        {
            //clear the lists at the previously touched locations
            while (TouchedLocations.Count > 0)
                nodes[TouchedLocations.Pop()].Clear();

            //check if the bottom right of the character will be able to fit in the goal
            bool inSolidTile = false;

            for(var i = 0; i < characterWidth; ++i)
            {
                inSolidTile = false;
                //check characterWidth number of nodes to the right of the goal
                for(var w = 0; w < characterWidth; ++w)
                {
                    if(Grid[end.X + w, end.Y] == 0 || Grid[end.X + w, end.Y - characterHeight + 1] == 0)
                    {
                        inSolidTile = true;
                        break;
                    }
                }

                if(inSolidTile == false)
                {
                    //check characterHeight number of blocks 
                    for(var h = 0; h < characterHeight; ++h)
                    {
                        if(Grid[end.X, end.Y - h] == 0 || Grid[end.X + characterWidth - 1, end.Y - h] == 0)
                        {
                            inSolidTile = true;
                            break;
                        }
                    }
                }

                if (inSolidTile)
                    end.X -= characterWidth - 1;
                else
                    break;
            }

            if(inSolidTile == true)
            {
                Main.NewText("Character cannot fit in end location, exiting...");
                return null;
            }

            Found = false;
            Stop = false;
            mStopped = false;
            CloseNodeCounter = 0;
            OpenNodeValue += 2;
            CloseNodeValue += 2;
            Open.Clear();

            Location.xy = (start.Y << GridXLog2) + start.X; //find starting node location in grid
            Location.z = 0;
            EndLocation = (end.Y << GridXLog2) + end.X; //do the same for the end location

            Node firstNode = new Node(); //this is the start node
            firstNode.G = 0;
            firstNode.F = mHEstimate;
            firstNode.PX = (ushort)start.X;
            firstNode.PY = (ushort)start.Y;
            firstNode.PZ = 0;
            firstNode.Status = OpenNodeValue;

            //check all nodes beneath the character to see if they are on the ground
            bool startsOnGround = false;
            for(int x = start.X; x < start.X + characterWidth; ++x)
            {
                if(Map.IsGround(x, start.Y + 1))
                {
                    startsOnGround = true;
                    break;
                }
            }

            if (startsOnGround)
                firstNode.JumpLength = 0;
            else
                firstNode.JumpLength = (short)(maxJumpHeight * 2);

            nodes[Location.xy].Add(firstNode);
            TouchedLocations.Push(Location.xy);

            Open.Push(Location); //push the starting node into the open set

            //the actual algorithm starts here
            while (Open.Count > 0 && !Stop)
            {
                Location = Open.Pop();

                //is it in closed list? means this node was already processed and skip this iteration
                if (nodes[Location.xy][Location.z].Status == CloseNodeValue)
                    continue;

                //calculate node we are evaulating
                LocationX = (ushort)(Location.xy & GridXMinus1);
                LocationY = (ushort)(Location.xy >> GridXLog2);

                //current node is the end location
                if (Location.xy == EndLocation)
                {
                    nodes[Location.xy][Location.z] = nodes[Location.xy][Location.z].UpdateStatus(CloseNodeValue);
                    Found = true;
                    break;
                }

                //closed nodes has reach threshold, either no path possible or just too many darn nodes to sift through
                if (CloseNodeCounter > mSearchLimit)
                {
                    Main.NewText("Hit maximum search limit threshold, exiting...");
                    mStopped = true;
                    return null;
                }

                //calculate the nodes around the current one
                for (var i = 0; i < (mDiagonals ? 8 : 4); i++)
                {
                    NewLocationX = (ushort)(LocationX + mDirection[i, 0]);
                    NewLocationY = (ushort)(LocationY + mDirection[i, 1]);
                    NewLocation = (NewLocationY << GridXLog2) + NewLocationX;

                    var onGround = false;
                    var atCeiling = false;

                    for(var w = 0; w < characterWidth; ++w)
                    {
                        //check if top most and bottom most blocks of the character to see if they are a solid block
                        if (Grid[NewLocationX + w, NewLocationY] == 0 || Grid[NewLocationX + w, NewLocationY - characterHeight + 1] == 0)
                            goto CHILDREN_LOOP_END;

                        //any of the bottom nodes right above the ground (air block between them) then it is onGround
                        if(Map.IsGround(NewLocationX + w, NewLocationY + 1))
                        {
                            onGround = true;
                        }else if(Grid[NewLocationX + w, NewLocationY - characterHeight] == 0) //any tiles above the character are solid then character would be at ceiling
                        {
                            atCeiling = true;
                        }
                    }

                    //check the left and right cells of the character, skip if they are blocks b/c character won't fit in that position
                    for(var h = 1; h < characterHeight - 1; ++h)
                    {
                        if (Grid[NewLocationX, NewLocationY - h] == 0 || Grid[NewLocationX + characterHeight - 1, NewLocationY - h] == 0)
                            goto CHILDREN_LOOP_END;
                    }

                    //calculate jumplength value for neighbor node
                    var jumpLength = nodes[Location.xy][Location.z].JumpLength;
                    short newJumpLength = jumpLength;

                    if (atCeiling)
                    {
                        if (NewLocationX != LocationX)
                            //character needs to drop straight down
                            //we are falling and our next move needs to be done vertically
                            newJumpLength = (short)Math.Max(maxJumpHeight * 2 + 1, jumpLength + 1);
                        else
                            //character can still move one cell to either side
                            //since value is even, the neighbor node will still be able to move either left or right
                            newJumpLength = (short)Math.Max(maxJumpHeight * 2, jumpLength + 2);
                    }
                    else if (onGround)
                    {
                        newJumpLength = 0;
                    }
                    //calcualting jump value mid-air
                    else if (NewLocationY < LocationY) // neighbor node is above parent node
                    {
                        if (jumpLength < 2) //first jump is always two blocks up instead of one up and optionally one to either right or left
                            newJumpLength = 3;
                        else if (jumpLength % 2 == 0)
                            //jump length is even, increment by 2 otherwise increment by 1
                            //this is to handle jumps that are straight up
                            newJumpLength = (short)(jumpLength + 2);
                        else
                            newJumpLength = (short)(jumpLength + 1);
                    }
                    else if (NewLocationY > LocationY) //neighbor node is below the parent
                    {
                        //same calculation for vertical jumps, just downwards
                        if (jumpLength % 2 == 0)
                            newJumpLength = (short)Math.Max(maxJumpHeight * 2, jumpLength + 2);
                        else
                            newJumpLength = (short)Math.Max(maxJumpHeight * 2, jumpLength + 1);
                    }
                    else if (!onGround && NewLocationX != LocationX) //node is to the left or right of parent node, same y-axis
                    {
                        newJumpLength = (short)(jumpLength + 1);
                    }

                    //dismiss this node if it's jump value is odd and the parent is either to the left/right
                    //character went to side already and needs to move up/down
                    if (jumpLength >= 0 && jumpLength % 2 != 0 && LocationX != NewLocationX)
                        continue;

                    //if we're falling and neighbor node is higher, skip impossible jump
                    if (jumpLength >= maxJumpHeight * 2 && NewLocationY < LocationY)
                        continue;

                    //prevent giving incorrect values when the character is falling really fast
                    //without this, character would be moving 1 block to side and 2 or more blocks down instead of 1 block to side and 1 block down
                    if (newJumpLength >= maxJumpHeight * 2 + 6 && NewLocationX != LocationX && (newJumpLength - (maxJumpHeight * 2 + 6)) % 8 != 3)
                        continue;

                    //cost higher if jump value higher
                    //divide by 4 to make character stick to ground more often and not get 2 jumpy
                    NewG = nodes[Location.xy][Location.z].G + Grid[NewLocationX, NewLocationY] + newJumpLength / 4;

                    //revisit nodes with different jump values
                    if (nodes[NewLocation].Count > 0)
                    {
                        int lowestJump = short.MaxValue;
                        bool couldMoveSideways = false;

                        for (int j = 0; j < nodes[NewLocation].Count; ++j)
                        {
                            if (nodes[NewLocation][j].JumpLength < lowestJump)
                                lowestJump = nodes[NewLocation][j].JumpLength;

                            if (nodes[NewLocation][j].JumpLength % 2 == 0 && nodes[NewLocation][j].JumpLength < maxJumpHeight * 2 + 6)
                                couldMoveSideways = true;
                        }

                        //skip node if jump value isn't lower than any of the other nodes at the same x, y (doesn't promise higher jump)
                        //and the currently processed node's jump value is even and all the others aren't (node allows for sideways movement while others only allow up or down)
                        if (lowestJump <= newJumpLength && (newJumpLength % 2 != 0 || newJumpLength >= maxJumpHeight * 2 + 6 || couldMoveSideways))
                            continue;
                    }

                    //calculate H (heuristic) cost
                    switch (mFormula)
                    {
                        default:
                        case HeuristicFormula.Manhattan:
                            H = mHEstimate * (Math.Abs(NewLocationX - end.X) + Math.Abs(NewLocationY - end.Y));
                            break;
                        case HeuristicFormula.MaxDXDY:
                            H = mHEstimate * (Math.Max(Math.Abs(NewLocationX - end.X), Math.Abs(NewLocationY - end.Y)));
                            break;
                        case HeuristicFormula.DiagonalShortCut:
                            var h_diagonal = Math.Min(Math.Abs(NewLocationX - end.X), Math.Abs(NewLocationY - end.Y));
                            var h_straight = (Math.Abs(NewLocationX - end.X) + Math.Abs(NewLocationY - end.Y));
                            H = (mHEstimate * 2) * h_diagonal + mHEstimate * (h_straight - 2 * h_diagonal);
                            break;
                        case HeuristicFormula.Euclidean:
                            H = (int)(mHEstimate * Math.Sqrt(Math.Pow(NewLocationY - end.X, 2) + Math.Pow(NewLocationY - end.Y, 2)));
                            break;
                        case HeuristicFormula.EuclideanNoSQR:
                            H = (int)(mHEstimate * (Math.Pow(NewLocationY - end.X, 2) + Math.Pow(NewLocationY - end.Y, 2)));
                            break;
                        case HeuristicFormula.Custom1:
                            var dxy = new Point(Math.Abs(end.X - NewLocationX), Math.Abs(end.Y - NewLocationY));
                            var orthogonal = Math.Abs(dxy.X - dxy.Y);
                            var diagonal = Math.Abs((dxy.X + dxy.Y - orthogonal) / 2);
                            H = mHEstimate * (diagonal + orthogonal + dxy.X + dxy.Y);
                            break;
                    }

                    //add node to the node list after checks and calculations, phew...
                    Node newNode = new Node();
                    newNode.JumpLength = newJumpLength;
                    newNode.PX = LocationX;
                    newNode.PY = LocationY;
                    newNode.PZ = (byte)Location.z;
                    newNode.G = NewG;
                    newNode.F = NewG + H;
                    newNode.Status = OpenNodeValue;

                    if (nodes[NewLocation].Count == 0)
                        TouchedLocations.Push(NewLocation);

                    nodes[NewLocation].Add(newNode);
                    Open.Push(new Location(NewLocation, nodes[NewLocation].Count - 1));

                    //removes the need to break loop and continue to next node
                    CHILDREN_LOOP_END:
                    continue;
                }

                //set status of parent node to "closed"
                nodes[Location.xy][Location.z] = nodes[Location.xy][Location.z].UpdateStatus(CloseNodeValue);
                CloseNodeCounter++;
            }

            //filter only necessary nodes
            if (Found)
            {
                //start at the end
                Close.Clear();
                int posX = end.X;
                int posY = end.Y;

                Node fPrevNodeTmp = new Node();
                Node fNodeTmp = nodes[EndLocation][0];

                Point fNode = end;
                Point fPrevNode = end;

                //points to the next node to be evaluated
                var loc = (fNodeTmp.PY << GridXLog2) + fNodeTmp.PX;

                //keep going until parent's position == node's position (we hit the start node)
                while (fNode.X != fNodeTmp.PX || fNode.Y != fNodeTmp.PY)
                {
                    Node fNextNodeTmp = nodes[loc][fNodeTmp.PZ];
                    if (Close.Count == 0 //add end node
                        || Map.IsOneWayPlatform(fNode.X, fNode.Y + 1)
                        || (Grid[fNode.X, fNode.Y + 1] == 0 && Map.IsOneWayPlatform(fPrevNode.X, fPrevNode.Y + 1))
                        || fNodeTmp.JumpLength == 3 //add first jump up or first in air direction change
                        || (fNextNodeTmp.JumpLength != 0 && fNodeTmp.JumpLength == 0) //add landing node (node that has non-zero jump value becomes 0)
                        || (fNodeTmp.JumpLength == 0 && fPrevNodeTmp.JumpLength != 0) //next node is on ground while next one isn't (landing node)
                        //node y-coordinate is higher than previous and next node in closed list (highest point of jump)
                        || (fNode.Y > Close[Close.Count - 1].Y && fNode.Y > fNodeTmp.PY)
                        || (fNode.Y < Close[Close.Count - 1].Y && fNode.Y < fNodeTmp.PY)
                        //next to an obstacle and previous node isn't aligned with current one either horizontally or vertically (went around an obstacle)
                        || ((Map.IsGround(fNode.X - 1, fNode.Y) || Map.IsGround(fNode.X + 1, fNode.Y))
                            && fNode.Y != Close[Close.Count - 1].Y && fNode.X != Close[Close.Count - 1].X))
                        Close.Add(fNode);

                    fPrevNode = fNode;
                    posX = fNodeTmp.PX;
                    posY = fNodeTmp.PY;
                    fPrevNodeTmp = fNodeTmp;
                    fNodeTmp = fNextNodeTmp;
                    loc = (fNodeTmp.PY << GridXLog2) + fNodeTmp.PX;
                    fNode = new Point(posX, posY);
                }

                Close.Add(fNode); //at start of list which means fNode = start node
                mStopped = true;
                return Close;
            }
            Main.NewText("No path found, exiting...");
            mStopped = true;
            return null;
        }
    }
}