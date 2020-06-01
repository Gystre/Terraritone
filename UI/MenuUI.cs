using System;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using System.Diagnostics;

namespace Terraritone.UI
{
    class MenuUI : UIState
    {
        public DraggablePanel panel;
        public static bool Visible = true;

        UIText debugMovements;

        public override void OnInitialize()
        {
            panel = new DraggablePanel();
            panel.SetPadding(0);
            panel.Left.Set(400f, 0f);
            panel.Top.Set(200, 0f);
            panel.Width.Set(170f, 0f);
            panel.Height.Set(200, 0f);
            panel.BackgroundColor = new Color(73, 94, 171);

            UIText text = new UIText("Terraritone");
            text.Left.Set(10, 0f);
            text.Top.Set(10, 0f);
            text.Width.Set(22, 0f);
            text.Height.Set(22, 0f);
            panel.Append(text);

            Texture2D buttonPlayTexture = ModContent.GetTexture("Terraria/UI/ButtonPlay");
            HoverImageButton pathButton = new HoverImageButton(buttonPlayTexture, "Path");
            pathButton.Left.Set(10, 0f);
            pathButton.Top.Set(30, 0f);
            pathButton.Width.Set(22, 0f);
            pathButton.Height.Set(22, 0f);
            pathButton.OnClick += new MouseEvent(PathButtonClicked);
            panel.Append(pathButton);

            Texture2D buttonStopTexture = ModContent.GetTexture("Terraria/UI/ButtonDelete");
            HoverImageButton stopButton = new HoverImageButton(buttonStopTexture, "Stop");
            stopButton.Left.Set(35, 0);
            stopButton.Top.Set(30, 0);
            stopButton.Width.Set(22, 0f);
            stopButton.Height.Set(22, 0f);
            stopButton.OnClick += new MouseEvent(StopButtonClicked);
            panel.Append(stopButton);

            Texture2D buttonDeleteTexture = ModContent.GetTexture("Terraritone/UI/closeButton");
            HoverImageButton closeButton = new HoverImageButton(buttonDeleteTexture, Language.GetTextValue("LegacyInterface.52")); // Localized text for "Close"
            closeButton.Left.Set(140, 0f);
            closeButton.Top.Set(10, 0f);
            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.OnClick += new MouseEvent(CloseButtonClicked);
            panel.Append(closeButton);

            debugMovements = new UIText("asdf");
            debugMovements.Left.Set(10, 0);
            debugMovements.Top.Set(50, 0);
            debugMovements.Width.Set(100, 0);
            debugMovements.Height.Set(500, 0);
            panel.Append(debugMovements);

            Append(panel);
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(SoundID.MenuClose);
            Visible = false;
        }

        private void PathButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if (PathMap.instance.goal.X == -1)
            {
                Main.NewText("Set a goal first");
            }
            else
            {
                Main.NewText("Starting path to tile " + PathMap.instance.goal + " from " + Main.LocalPlayer.position.ToTileCoordinates());
                var thread = new Thread(() =>
                {
                    PathMap.instance.FindPath();

                });
                thread.Start();
                thread = null;
            }
        }

        private void StopButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if(PathMap.instance != null)
            {
                PathMap.instance.Stop();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(PlayerInput.Triggers != null)
            {
                string container = "";

                container += "Left " + PlayerInput.Triggers.Current.Left + "\n";
                container += "Right " + PlayerInput.Triggers.Current.Right + "\n";
                container += "Down " + PlayerInput.Triggers.Current.Down + "\n";
                container += "Jump " + PlayerInput.Triggers.Current.Jump + "\n";

                debugMovements.SetText(container);
            }

            base.Draw(spriteBatch);

        }
    }
}
