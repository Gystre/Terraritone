using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace Terraritone
{
    class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Jump 1")]
        [Increment(1)]
        [Range(1, 15)]
        [DefaultValue(3)]
        [Slider]
        public int Jump1;

        [Label("Jump 2")]
        [Increment(1)]
        [Range(1, 15)]
        [DefaultValue(4)]
        [Slider]
        public int Jump2;

        [Label("Jump 3")]
        [Increment(1)]
        [Range(1, 15)]
        [DefaultValue(5)]
        [Slider]
        public int Jump3;

        [Label("Jump 4")]
        [Increment(1)]
        [Range(1, 15)]
        [DefaultValue(10)]
        [Slider]
        public int Jump4;

        [Label("Jump 5")]
        [Increment(1)]
        [Range(1, 15)]
        [DefaultValue(13)]
        [Slider]
        public int Jump5;

        [Label("Jump 6")]
        [Increment(1)]
        [Range(1, 15)]
        [DefaultValue(15)]
        [Slider]
        public int Jump6;
    }
}
