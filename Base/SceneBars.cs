using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace lnbase.Base {
	public class SceneBars {

		public GameSprite Texture { get; private set; }
		public Rectangle Name { get; private set; }
		public Rectangle Text { get; private set; }

		public SceneBars(GameSprite txt, Rectangle? name = null, Rectangle? text = null) {
			Texture = txt;
			Name = name ?? Texture.Texture.Bounds;
			Text = text ?? Texture.Texture.Bounds;
		}

	}
}
