using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terraritone.Commands
{
    class DebugGridCommand : ModCommand 
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "debuggrid";

        public override string Usage => "debuggrid";

        public override string Description => "Displays a little square around the player to show if a tile is walkable or not";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            PathMap.instance.debugGrid = !PathMap.instance.debugGrid;
        }
    }
}
