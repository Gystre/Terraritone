using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using Terraritone.UI;

namespace Terraritone
{
	public class Terraritone : Mod
	{
		private MenuUI MenuUI;
		private UserInterface MyInterface;

		public Terraritone()
		{

		}

		public override void Load()
		{
			if (!Main.dedServ) //won't initialize on server
			{
				MenuUI = new MenuUI();
				MenuUI.Activate();

				MyInterface = new UserInterface();
				MyInterface?.SetState(MenuUI);
			}
		}

		public override void Unload()
		{
			PathMap.instance = null;
		}

		public override void PreSaveAndQuit()
		{
			PathMap.instance = null; //memory leak deleted.
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if(MenuUI.Visible)
			{
				MyInterface.Update(gameTime);
			}
		}

		//create custom layer to vanilla layer to activate draw call
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"Terraritone: MyInterface",
					delegate
					{
						if (MenuUI.Visible)
						{
							MyInterface.Draw(Main.spriteBatch, new GameTime());
						}
						return true;
					},
					   InterfaceScaleType.UI));
			}
		}
	}
}