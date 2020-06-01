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
            if (PathMap.instance != null)
            {
                PathMap.instance.UpdateMovementBot();
            }
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
