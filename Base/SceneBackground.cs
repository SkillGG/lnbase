using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace lnbase.Base {
	public class SceneBackground {

		public GameSprite Texture { get; private set; }

		public SceneBackground(GameSprite txt) {
			Texture = txt;
		}
	}
}
