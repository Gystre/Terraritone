using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Terraritone.Commands
{
    class StopCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "stop";

        public override string Description => "Cancel current movement along path";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            PathMap.instance.Stop();
        }
    }
}
