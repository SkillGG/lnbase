using ITW.Exts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace lnbase.GameResources {
	public class GameText {

		public SpriteFont Font { get; private set; }
		public StringDrawn Text { get; private set; }

		public bool Loaded { get; private set; }

		public GameText() {
			Loaded = false;
		}

		public void Init(Action ea){
			if(Loaded){
				Text.End(ea);
			}
		}

		public void Load(StringDrawn text, SpriteFont f) {
			if( f != null && text != null ) {
				Loaded = true;
				Font = f;
				Text = text;
			}
		}

		public void SetValues(Vector2 tp) {
			if( Loaded ) {
				Text.MoveTo(tp);
			}
		}

		public void Draw(SpriteBatch sb) {
			if(Loaded){
				Text.Draw(sb, Font);
			}
		}

	}
}
