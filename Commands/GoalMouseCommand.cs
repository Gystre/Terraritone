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

            PathMap.instance.goal = mouseCoords.ToVector2();
            PathMap.instance.Path.Clear();
            Main.NewText("New goal: " + PathMap.instance.goal);
        }
    }
}
