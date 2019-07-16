using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace lnbase.Base {
	public class SceneBackground {

		public GameSprite Texture { get; private set; }

		public SceneBackground(GameSprite txt) {
			Texture = txt;
		}

		public void Draw(SpriteBatch sb, Rectangle? dest = null) {
			sb.Draw(Texture.Texture, dest ?? new Rectangle(0, 0, LNBase.CW, LNBase.CH), Texture.Source, Color.White);
		}

	}
}
