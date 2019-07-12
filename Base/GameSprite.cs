using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace lnbase.Base {
	public class GameSprite {

		public Texture2D Texture { get; private set; }
		public Rectangle Source { get; private set; }

		public GameSprite(Texture2D txt, Rectangle? src = null) {
			Texture = txt;
			Source = src ?? txt.Bounds;
		}

	}
}
