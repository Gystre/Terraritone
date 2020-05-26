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

        }

        public override void OnEnterWorld(Player player)
        {
            PathMap.instance = new PathMap();
        }
    }
}
