using ITW.Exts;
using lnbase.Game;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace lnbase.Scene {
	public class GameScene {

		public GameText Text { get; private set; }
		public GameSprite Bckg { get; private set; }
		public GameSprite Bar { get; private set; }

		private bool Loaded;

		public GameScene() {
		}

		public void GenereateScene(GameText text, GameSprite background, GameSprite bar, Action ea) {

			if( text.Loaded && background.Loaded && bar.Loaded )
				Loaded = true;
			else
				Loaded = false;

			if( Loaded ) {
				EndAction = ea;
				// TODO: Loaded. Generate scene
				text.SetValues(new Microsoft.Xna.Framework.Vector2( ));
				background.Show( );
				bar.SetValues( );
				bar.Show( );
			}

		}

		private Action EndAction = null;

		public bool FirstDraw { get; private set; }

		public void Draw(SpriteBatch sb) {

			if( FirstDraw ) {
				Text.Init(EndAction);
				FirstDraw = false;
			}

			Bckg.Draw(sb);
			Bar.Draw(sb);
			Text.Draw(sb);
		}

		public void Update(InputStates bef) {
			// Listen to Inputs
		}

	}
}
