using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Terraritone.Commands
{
    class CoordsCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "coords";

        public override string Usage => "coords";

        public override string Description => "Display ur coordsinates in chat";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.NewText("World pos: " + Main.LocalPlayer.position.X + " " + Main.LocalPlayer.position.Y);
            Point tilePos = Main.LocalPlayer.position.ToTileCoordinates().ToVector2().ToPoint();
            Main.NewText("Tile pos: " + tilePos.X + "x " + tilePos.Y + "y");
        }

    }
}
