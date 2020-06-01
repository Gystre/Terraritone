using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraritone;

class Bot
{
    public enum BotState
    {
        None = 0,
        MoveTo,
    }

    public Bot(Pathfinding pathfinder)
    {
        Pathfinder = pathfinder;
    }

    Pathfinding Pathfinder;
    List<Point> Path = new List<Point>();
    public BotState CurrentAction = BotState.None;

    Vector2 Position = new Vector2(); //current position in world coordinates
    Vector2 OldPosition = new Vector2();

    private int CurrentNodeId = -1;
    private int FramesOfJumping = 0;
    private int StuckFrames = 0;

    private const int MaxStuckFrames = 20; //maximum amount of frames the character can be stuck for before recalculating path
    private const float BotMaxPositionError = 1.0f; //allows for some inaccuracy with the path, maximum of 1 block off

    //called whenever the user clicks on somewhere on the screen
    public void TappedOnTile(List<Point> Path, Point mapPos)
    {
        //make sure point is on the ground
        while (!PathMap.instance.IsGround(mapPos.X, mapPos.Y))
            --mapPos.Y;

        MoveTo(Path, new Point(mapPos.X, mapPos.Y));
    }


    private void MoveTo(List<Point> path, Point destination)
    {
        this.Path = path;
        StuckFrames = 0;

        int width = Helper.GetPlayerWidth();
        int height = Helper.GetPlayerHeight();
        Point startTile = Helper.GetBottomLeftPoint();

        if (PlayerIsOnGround() && !IsOnGroundAndFitsPos(destination))
        {
            if (IsOnGroundAndFitsPos(new Point(startTile.X + 1, startTile.Y)))
                startTile.X += 1;
            else
                startTile.X -= 1;
        }

        CurrentNodeId = 1;
        ChangeState(BotState.MoveTo);
        FramesOfJumping = GetJumpFramesForNode(0);

    }

    public void StopMoving()
    {
        CurrentNodeId = -1;
        CurrentAction = BotState.None;
    }

    //need to jump X amount of blocks, return Y amt of frames jump needs to be held
    private int GetJumpFrameCount(int deltaY)
    {
        if (deltaY <= 0)
            return 0;
        else
        {
            switch (deltaY)
            {
                //pls if anybody knows more specific values dm me kale#0104
                case 1:
                    return ModContent.GetInstance<Config>().Jump1;
                case 2:
                    return ModContent.GetInstance<Config>().Jump2;
                case 3:
                    return ModContent.GetInstance<Config>().Jump3;
                case 4:
                    return ModContent.GetInstance<Config>().Jump4;
                case 5:
                    return ModContent.GetInstance<Config>().Jump5;
                case 6:
                    return ModContent.GetInstance<Config>().Jump6;
                default:
                    return ModContent.GetInstance<Config>().Jump6;
            }
        }
    }

    //calculate the how long player needs to hold jump button for
    private int GetJumpFramesForNode(int prevNodeId)
    {
        int currentNodeId = prevNodeId + 1;

        //only calculate if new node is higher than the current one and character is on the ground
        if(Path[prevNodeId].Y - Path[currentNodeId].Y> 0 && PlayerIsOnGround())
        {
            int jumpHeight = 1; //how high (in blocks) the character must jump to reach the node
            for(int i = currentNodeId; i < Path.Count; ++i)
            {
                if (Path[prevNodeId].Y - Path[i].Y >= jumpHeight)
                    jumpHeight = Path[prevNodeId].Y - Path[i].Y;

                //new node is lower than the previous or it's on the ground, return number of frames of jump needed for the found height
                if(Path[prevNodeId].Y - Path[i].Y < jumpHeight || PathMap.instance.IsGround(Path[i].X, Path[i].Y + 1))
                {
                    Main.NewText(jumpHeight);
                    return GetJumpFrameCount(jumpHeight);
                }
            }
        }

        //no need to jump
        return 0;
    }

    bool PlayerIsOnGround()
    {
        Point pos = Helper.GetBottomLeftPoint();

        for (int x = pos.X; x < pos.X + Helper.GetPlayerWidth(); ++x)
        {
            if (PathMap.instance.IsGround(x, pos.Y + 1))
                return true;
        }

        return false;
    }

    bool PlayerIsOnOneWayPlatform()
    {
        Point pos = Helper.GetBottomLeftPoint();

        for (int x = pos.X; x < pos.X + Helper.GetPlayerWidth(); ++x)
        {
            if (PathMap.instance.IsOneWayPlatform(x, pos.Y + 1))
                return true;
        }

        return false;
    }

    bool IsOnGroundAndFitsPos(Point pos)
    {
        int width = Helper.GetPlayerWidth();
        int height = Helper.GetPlayerHeight();

        //check if player fits the spot
        for (int y = 0; y < pos.Y + height; ++y)
        {
            for(int x = pos.X; x < pos.X + width; ++x)
            {
                if (PathMap.instance.IsObstacle(x, y))
                    return false;
            }
        }

        //check if tiles below are ground tiles
        for(int x = pos.X; x < pos.X + width; ++x)
        {
            if (PathMap.instance.IsGround(x, pos.Y + 1))
                return true;
        }

        return false;
    }

    public void ChangeState(BotState newState)
    {
        CurrentAction = newState;
    }

    private void GetContext(out Vector2 prevDest, out Vector2 currentDest, out Vector2 nextDest, out bool destOnGround, out bool reachedX, out bool reachedY)
    {
        prevDest = Path[CurrentNodeId - 1].ToWorldCoordinates();
        currentDest = Path[CurrentNodeId].ToWorldCoordinates();
        nextDest = currentDest;

        //see if there are any more nodes to follow after the current one, if so that's our nextDest
        if(Path.Count > CurrentNodeId + 1)
        {
            nextDest = Path[CurrentNodeId + 1].ToWorldCoordinates();
        }

        int width = Helper.GetPlayerWidth();
        destOnGround = false;
        for(int x = Path[CurrentNodeId].X; x < Path[CurrentNodeId].X + width; ++x)
        {
            if(PathMap.instance.IsGround(x, Path[CurrentNodeId].Y + 1))
            {
                destOnGround = true;
                break;
            }
        }

        //check if player has reached goal on x and y axis
        Vector2 pathPosition = Helper.GetBottomLeftPoint().ToWorldCoordinates();

        reachedX = ReachedNodeOnXAxis(pathPosition, prevDest, currentDest);
        reachedY = ReachedNodeOnYAxis(pathPosition, prevDest, currentDest);

        //snap the character if it reached the goal but overshot it by more than BotMaxPositionError
        if (reachedX && Math.Abs(pathPosition.X - currentDest.X) > BotMaxPositionError
            && Math.Abs(pathPosition.X - currentDest.X) < BotMaxPositionError * 3.0f
            && !PlayerInput.Triggers.Old.Right && !PlayerInput.Triggers.Old.Left)
        {
            pathPosition.X = currentDest.X;
            Position.X = pathPosition.X;
        }

        //destination on the ground but the player isn't, goal on Y-axis not reached
        if (destOnGround && !PlayerIsOnGround())
            reachedY = false;
    }


    private bool ReachedNodeOnXAxis(Vector2 pathPosition, Vector2 prevDest, Vector2 currentDest)
    {
        return (prevDest.X <= currentDest.X && pathPosition.X >= currentDest.X) //character is moving right and has an x-pos >= destination
            || (prevDest.X >= currentDest.X && pathPosition.X <= currentDest.X) //previous destination was tot he right of the current one and the character's x-pos is <= goal
            || (Math.Abs(pathPosition.X - currentDest.X) <= BotMaxPositionError);
    }

    private bool ReachedNodeOnYAxis(Vector2 pathPosition, Vector2 prevDest, Vector2 currentDest)
    {
        return (prevDest.Y >= currentDest.Y && pathPosition.Y <= currentDest.Y) //same applies for y-axis
            || (prevDest.Y <= currentDest.Y && pathPosition.Y >= currentDest.Y)
            || (Math.Abs(pathPosition.Y - currentDest.Y) <= BotMaxPositionError);
    }

    //call this every frame
    public void BotUpdate()
    {
        switch (CurrentAction)
        {
            case BotState.None:
                if(FramesOfJumping > 0)
                {
                    FramesOfJumping -= 1;
                    PlayerInput.Triggers.Current.Jump = true;
                }

                break;

            case BotState.MoveTo:

                Vector2 prevDest, currentDest, nextDest;
                bool destOnGround, reachedY, reachedX;
                GetContext(out prevDest, out currentDest, out nextDest, out destOnGround, out reachedX, out reachedY);

                Vector2 pathPosition = Helper.GetBottomLeftPoint().ToWorldCoordinates();

                PlayerInput.Triggers.Current.Right = false;
                PlayerInput.Triggers.Current.Left = false;
                PlayerInput.Triggers.Current.Jump = false;
                PlayerInput.Triggers.Current.Down = false;

                //if current destination is lower than the position of the character AND the character is standing on a one way platform, press down
                if(currentDest.Y - pathPosition.Y > BotMaxPositionError && PlayerIsOnOneWayPlatform())
                {
                    PlayerInput.Triggers.Current.Down = true;
                }

                //reached destination node
                if(reachedX && reachedY)
                {
                    int prevNodeId = CurrentNodeId;
                    CurrentNodeId++;

                    if(CurrentNodeId >= Path.Count)
                    {
                        //no more nodes to follow, at goal
                        CurrentNodeId = -1;
                        ChangeState(BotState.None);
                        break;
                    }

                    if (PlayerIsOnGround())
                    {
                        FramesOfJumping = GetJumpFramesForNode(prevNodeId);
                    }

                    
                    //updated goal, now calculate next movement frame
                    goto case BotState.MoveTo;
                }else if (!reachedX) //haven't reached destination on x-axis
                {
                    //destination to right, go right, if left, go left
                    //only move if difference between positions is 1 block
                    if (currentDest.X - pathPosition.X > BotMaxPositionError)
                        PlayerInput.Triggers.Current.Right = true;
                    else if (pathPosition.X - currentDest.X > BotMaxPositionError)
                        PlayerInput.Triggers.Current.Left = true;
                }else if(!reachedY && Path.Count > CurrentNodeId + 1 && !destOnGround) //might be a node we need to fall through but not move to left or right
                {
                    int checkedX = 0;
                    Point tilePos = pathPosition.ToTileCoordinates();

                    if (Path[CurrentNodeId + 1].X != Path[CurrentNodeId].X)
                    {
                        if (Path[CurrentNodeId + 1].X > Path[CurrentNodeId].X)
                            checkedX = tilePos.X + Helper.GetPlayerWidth();
                        else
                            checkedX = tilePos.X - 1;
                    }

                    //check if no solid blocks between the two nodes
                    //if so player can continue to move to the left or right while falling
                    if(checkedX != 0 && !PathMap.instance.AnySolidBlockInStripe(checkedX, tilePos.Y, Path[CurrentNodeId + 1].Y))
                    {
                        if (nextDest.X - pathPosition.X > BotMaxPositionError)
                            PlayerInput.Triggers.Current.Right = true;
                        else if (pathPosition.X - nextDest.X > BotMaxPositionError)
                            PlayerInput.Triggers.Current.Left = true;

                        //in case the player moves through a node too quickly for it to be calculated
                        //if it's reached the next node's destination, skip the current one and recalcalculate
                        if(ReachedNodeOnXAxis(pathPosition, currentDest, nextDest) && ReachedNodeOnYAxis(pathPosition, currentDest, nextDest))
                        {
                            CurrentNodeId += 1;
                            goto case BotState.MoveTo;
                        }
                    }
                }

                //press jump button if at a ledge and needs to jump, player has reached dest on x-axis and destination node is not on the ground (a start of jump node)
                //or player is on ground, and dest is on ground, needs to jump up one block up to reach
                if(FramesOfJumping > 0 
                    && (!PlayerIsOnGround() || (reachedX && !destOnGround) || (PlayerIsOnGround() && destOnGround)))
                {
                    //activate the jump for FramesOfJumping number of frames
                    PlayerInput.Triggers.Current.Jump = true;
                    if (!PlayerIsOnGround()) //avoid accidentally decrememnting jump length before starting the jump
                        --FramesOfJumping; 
                }

                //still stuck in the same position for 20 frames, recalculate path and try again
                /*
                 * TODO: 
                 *  1. calculate grid at runtime so bot can adapt to new blocks placed
                 *  2. get old position so that this check works
                */
                if(Position == OldPosition)
                {
                    ++StuckFrames;
                    if (StuckFrames > MaxStuckFrames)
                        MoveTo(Path, Path[Path.Count - 1]);
                }
                else
                {
                    StuckFrames = 0;
                }

                break;
        }
    }
}