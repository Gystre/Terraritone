using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terraritone.Commands
{
    class TpCommand : ModCommand
    {
        public override CommandType Type => CommandType.World;

        public override string Command => "tp";

        public override string Usage => "tp <world|tile> <x> <y>";

        public override string Description => "teleport urself";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if(args[0] == "world")
            {
                Main.LocalPlayer.position = new Microsoft.Xna.Framework.Vector2(float.Parse(args[1]), float.Parse(args[2]));
            }
            else if(args[0] == "tile")
            {
                Main.LocalPlayer.position = new Microsoft.Xna.Framework.Vector2(float.Parse(args[1]), float.Parse(args[2])).ToWorldCoordinates();
            }
        }
    }
}
