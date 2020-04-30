using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Microsoft.Xna.Framework;

namespace Terraritone
{
    class TestPlayer : ModPlayer
    {
        public override void PreUpdate()
        {
            //PlayerInput.Triggers.Current.Left = true;
            //PlayerInput.Triggers.Current.Left = false;

            //PlayerInput.Triggers.Current.Right = true;
            //PlayerInput.Triggers.Current.Right = false;
        }

        public override void PostUpdate()
        {
            //string type = "don't know";
            //if (tile.active() && Main.tileSolid[tile.type] && Main.tileSolidTop[tile.type] == false)
            //{
            //    type = "block";
            //}
            //else if (tile.active() == false && Main.tileSolid[tile.type] && Main.tileSolidTop[tile.type] == false)
            //{
            //    type = "air";
            //}
            //else if (tile.active() && Main.tileSolid[tile.type] == false && Main.tileSolidTop[tile.type] == false)
            //{
            //    type = "decor";
            //}else if(tile.active() && Main.tileSolid[tile.type] && Main.tileSolidTop[tile.type])
            //{
            //    type = "platform";
            //}

            //Main.NewText(tile.active() + " tile solid: " + Main.tileSolid[tile.type] + " tileSolidTop: " + Main.tileSolidTop[tile.type]);
            //Terraria.GameInput.PlayerInput.Triggers.Current.Left = true;
        }

        public override void OnEnterWorld(Player player)
        {
            Pathfinding.instance.grid = new Grid();
        }
    }
}
