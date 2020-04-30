using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Terraritone.Commands
{
    class GoalCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "goal";

        public override string Usage => "/goal <x> <y>";

        public override string Description => "Set a goal in tile coordinates";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Main.NewText("Please use 2 coordinates");
                throw new NotImplementedException();
            }
            else
            {
                if(!Pathfinding.instance.grid.grid[int.Parse(args[0]), int.Parse(args[1])].walkable)
                {
                    Main.NewText("Tile is not walkable, set the goal to a walkable tile");
                }
                else
                {
                    Pathfinding.instance.goal = new Vector2(int.Parse(args[0]), int.Parse(args[1]));
                    Main.NewText("New goal: " + Pathfinding.instance.goal);
                }
            }
        }
    }
}
