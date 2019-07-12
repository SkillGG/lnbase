using Microsoft.Xna.Framework.Graphics;

namespace lnbase.Base {
	public class GameScene {

		public string ID { get; private set; }
		public SceneHandler.Flag FLAG{ get; private set; }

		public GameScene(string id, string text, string name,
			SceneType t, SceneBackground bg, SceneBars br, SpriteFont f,
			SceneHandler.Flag flag) {
			ID = id;
		}

	}
}
