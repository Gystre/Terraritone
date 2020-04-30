using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Terraritone.Commands
{
    class GoalMouseCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "goalmouse";

        public override string Usage => "goalmouse";

        public override string Description => "Sets the goal to ur mouse position";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Point mouseCoords = Main.MouseWorld.ToTileCoordinates();

            if (!Pathfinding.instance.grid.grid[mouseCoords.X+1, mouseCoords.Y+2].walkable)
            {
                Main.NewText("Tile is not walkable, set the goal to a walkable tile");
            }
            else
            {
                Pathfinding.instance.goal = mouseCoords.ToVector2();
                Main.NewText("New goal: " + Pathfinding.instance.goal);
            }
        }
    }
}
