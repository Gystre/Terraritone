using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

public static class Helper
{
    public static int GetPlayerWidth()
    {
        return (int)Math.Ceiling(Main.LocalPlayer.width / 16f);
    }

    public static int GetPlayerHeight()
    {
        return (int)Math.Ceiling(Main.LocalPlayer.height / 16f);
    }

    //tile coords
    public static Point GetBottomLeftPoint()
    {
        Point pos = Main.LocalPlayer.position.ToTileCoordinates();
        pos.Y += GetPlayerHeight() - 1;
        return pos;
    }
}
