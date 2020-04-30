using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terraritone.Commands
{
    class PathCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "path";

        public override string Description => "Start pathing to the set goal";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (Pathfinding.instance.goal.X == -1)
            {
                Main.NewText("Set a goal first");
            }
            else
            {
                Main.NewText("Starting path to tile " + Pathfinding.instance.goal);
                Pathfinding.instance.Start();
                Main.NewText("Done!");
            }
        }
    }
}
