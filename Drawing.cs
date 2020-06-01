using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Diagnostics;

namespace Terraritone
{
    class Drawing : ModWorld
    {
        //transforms tile coordinates -> screen coordinates
        Vector2 TileToScreen(Vector2 tileCoords, bool topLeft = true)
        {
            Vector2 upperLeftScreen = new Vector2();
            if (topLeft)
            {
                upperLeftScreen = tileCoords * 16f;
                upperLeftScreen -= Main.screenPosition;
            }
            else
            {
                //not perfect center, probably start with position and not tile coordinates
                upperLeftScreen = tileCoords * 16f;
                upperLeftScreen.X += 8;
                upperLeftScreen.Y += 8;
                upperLeftScreen -= Main.screenPosition;
            }

            return upperLeftScreen;
        }

        //make sure the tile coords are the center NOT the top left!
        private void DrawHighlightedTile(Vector2 tileCoords, float width, float height, Color color, bool drawBack = true)
        {
            Vector2 screenCoords = TileToScreen(tileCoords);

            Rectangle value = new Rectangle(0, 0, 1, 1);
            float scale = 0.6f;
            if (drawBack)
            {
                Main.spriteBatch.Draw(Main.magicPixel, screenCoords, new Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(width * 16f, height * 16f), SpriteEffects.None, 0f);
            }
            scale = 1f;
            Main.spriteBatch.Draw(Main.magicPixel, screenCoords + Vector2.UnitX * -2f, new Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * height), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.magicPixel, screenCoords + Vector2.UnitX * 16f * width, new Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * height), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.magicPixel, screenCoords + Vector2.UnitY * -2f, new Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * width, 2f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.magicPixel, screenCoords + Vector2.UnitY * 16f * height, new Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * width, 2f), SpriteEffects.None, 0f);
        }

        private void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);


            Main.spriteBatch.Draw(Main.magicPixel,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }

        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //need this sanity check b/c PostDraw() is called before OnEnterWorld() resulting in crash w/out
            if(PathMap.instance != null)
            {
                //debug grid
                if (PathMap.instance.debugGrid)
                {
                    Point playerPos = Main.LocalPlayer.position.ToTileCoordinates();
                    for (var x = 0; x < 40; x++)
                    {
                        for (var y = 0; y < 40; y++)
                        {
                            Point nodePos = new Point((playerPos.X - 1) + x - 15, (playerPos.Y - 2) + y - 15);
                            TileType node = PathMap.instance.tiles[nodePos.X, nodePos.Y];
                            if (node == TileType.Empty)
                            {
                                DrawHighlightedTile(nodePos.ToVector2(), 1, 1, Color.Green);
                            }
                            else if (node == TileType.Block)
                            {
                                DrawHighlightedTile(nodePos.ToVector2(), 1, 1, Color.Red);
                            }else if(node == TileType.OneWay)
                            {
                                DrawHighlightedTile(nodePos.ToVector2(), 1, 1, Color.Yellow);
                            }
                        }
                    }
                }

                //goal
                DrawHighlightedTile(PathMap.instance.goal, 1, 1, Color.Green);

                //path
                if (PathMap.instance.Path != null)
                {
                    List<Point> PathNodes = PathMap.instance.Path; //because i hate typing
                    foreach (Point point in PathNodes)
                    {
                        DrawHighlightedTile(point.ToVector2(), 1, 1, new Color(125, 125, 125, 1));
                    }

                    //draw lines between the nodes
                    for(int i = 0; i < PathNodes.Count()-1; i++)
                    {
                        DrawLine(TileToScreen(PathNodes.ElementAt(i).ToVector2(), false), TileToScreen(PathNodes.ElementAt(i + 1).ToVector2(), false), Color.Red);
                    }
                }

            }

            Main.spriteBatch.End();
        }
    }
}
