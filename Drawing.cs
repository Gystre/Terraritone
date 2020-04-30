using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terraritone
{
    class Drawing : ModWorld
    {
        //make sure the tile coords are the center NOT the top left!
        private void DrawHighlightedTile(Vector2 tileCoords, float width, float height, float r, float g, float b, bool drawBack = true)
        {
            //tile -> screen
            Vector2 lowerRight = new Vector2();
            lowerRight.X = tileCoords.X + 1;
            lowerRight.Y = tileCoords.Y + 1;
            Vector2 upperLeftScreen = tileCoords * 16f;
            upperLeftScreen -= Main.screenPosition;

            Rectangle value = new Rectangle(0, 0, 1, 1);
            float scale = 0.6f;
            Color color = new Color(r, g, b, 1);
            if (drawBack)
            {
                Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen, new Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(width * 16f, height * 16f), SpriteEffects.None, 0f);
            }
            scale = 1f;
            color = new Color(r, g, b, 1);
            Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitX * -2f, new Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * height), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitX * 16f * width, new Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * height), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitY * -2f, new Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * width, 2f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitY * 16f * height, new Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * width, 2f), SpriteEffects.None, 0f);
        }

        //private void DrawLine( Vector2 start, Vector2 end)
        //{
        //    Vector2 edge = end - start;
        //    // calculate angle to rotate line
        //    float angle =
        //        (float)Math.Atan2(edge.Y, edge.X);


        //    Main.spriteBatch.Draw(Main.magicPixel,
        //        new Rectangle(// rectangle defines shape of line and position of start of line
        //            (int)start.X,
        //            (int)start.Y,
        //            (int)edge.Length(), //sb will strech the texture to fill this rectangle
        //            1), //width of line, change this to make thicker line
        //        null,
        //        Color.Red, //colour of line
        //        angle,     //angle of line (calulated above)
        //        new Vector2(0, 0), // point in line about which to rotate
        //        SpriteEffects.None,
        //        0);

        //}

        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //debug grid
            if (Pathfinding.instance.debugGrid && Pathfinding.instance.grid != null)
            {
                Point playerPos = Main.LocalPlayer.position.ToTileCoordinates();
                for (var x = 0; x < 40; x++)
                {
                    for (var y = 0; y < 40; y++)
                    {
                        Node node = Pathfinding.instance.grid.grid[(playerPos.X - 1) + x - 15, (playerPos.Y - 2) + y - 15];
                        if (node.walkable)
                        {
                            DrawHighlightedTile(node.worldPos.ToTileCoordinates().ToVector2(), 1, 1, 0, 255, 0);
                        }
                        else
                        {
                            DrawHighlightedTile(node.worldPos.ToTileCoordinates().ToVector2(), 1, 1, 255, 0, 0);
                        }
                    }
                }
            }

            //path
            if (Pathfinding.instance?.grid?.path != null)
            {
                foreach (Node node in Pathfinding.instance.grid.path)
                {
                    DrawHighlightedTile(node.worldPos.ToTileCoordinates().ToVector2(), 1, 1, 255, 255, 0);
                }
            }

            //goal
            DrawHighlightedTile(Pathfinding.instance.goal, 1, 1, 0, 255, 0);

            Main.spriteBatch.End();
        }

        public override void PostUpdate()
        {
            //for (int x = 0; x < Pathfinding.instance.grid.gridSizeX - 1; x++)
            //{
            //    for (int y = 0; y < Pathfinding.instance.grid.gridSizeY - 2; y++)
            //    {
            //        Tile tile = Main.tile[x, y];
            //        if ((tile.active() == false && Main.tileSolid[tile.type] && Main.tileSolidTop[tile.type] == false) || //air
            //            (tile.active() && Main.tileSolid[tile.type] == false && Main.tileSolidTop[tile.type] == false) || //decor
            //            (tile.active() && Main.tileSolid[tile.type] && Main.tileSolidTop[tile.type])) //platform
            //        {
            //            Pathfinding.instance.grid.grid[x + 1, y + 2].walkable = true;
            //        }
            //        else
            //        {
            //            Pathfinding.instance.grid.grid[x + 1, y + 2].walkable = false;

            //        }
            //    }
            //}
        }
    }
}
