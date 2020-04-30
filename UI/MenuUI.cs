using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;

namespace Terraritone.UI
{
    class MenuUI : UIState
    {
        public DraggablePanel panel;
        public HoverImageButton button;
        public static bool Visible = true;

        public override void OnInitialize()
        {
            panel = new DraggablePanel();
            panel.SetPadding(0);
            panel.Left.Set(400f, 0f); //width of gui
            panel.Top.Set(200, 0f); //height of gui
            panel.Width.Set(170f, 0f);
            panel.Height.Set(70f, 0f);
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

            Texture2D buttonDeleteTexture = ModContent.GetTexture("Terraria/UI/ButtonDelete");
            HoverImageButton closeButton = new HoverImageButton(buttonDeleteTexture, Language.GetTextValue("LegacyInterface.52")); // Localized text for "Close"
            closeButton.Left.Set(140, 0f);
            closeButton.Top.Set(10, 0f);
            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.OnClick += new MouseEvent(CloseButtonClicked);
            panel.Append(closeButton);

            Append(panel);
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(SoundID.MenuClose);
            Visible = false;
        }

        private void PathButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if(Pathfinding.instance.goal.X == -1)
            {
                Main.NewText("Set a goal first");
            }
            else
            {
                Main.NewText("Starting path to tile " + Pathfinding.instance.goal);
                Pathfinding.instance.Start();
                Main.NewText("Done!");
            }
        }

    }
}
