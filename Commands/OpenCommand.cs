using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraritone.UI;

namespace Terraritone
{
    class OpenCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "t";

        public override string Usage => "/t <open|close>";

        public override string Description => "open the menu";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if(args[0] == "open")
            {
                MenuUI.Visible = true;
            }else if(args[0] == "close")
            {
                MenuUI.Visible = false;
            }
            else
            {
                throw new NotImplementedException();
            }

        }
    }
}
