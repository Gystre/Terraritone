using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
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
            if (PathMap.instance.goal.X == -1)
            {
                Main.NewText("Set a goal first");
            }
            else
            {
                Main.NewText("Starting path to tile " + PathMap.instance.goal + " from " + Main.LocalPlayer.position.ToTileCoordinates());
                var thread = new Thread(() =>
                {
                    PathMap.instance.FindPath();

                });
                thread.Start();
                thread = null;
            }
        }
    }
}
