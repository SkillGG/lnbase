using ITW.Exts;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace lnbase.Base {
	public class GameBase {

		public SceneHandler Scenes;

		public SceneBackground DefaultBCKG { get; private set; }
		public SceneBars DefaultBARS { get; private set; }
		public SpriteFont DefaultFONT { get; private set; }

		private bool Loaded { get; set; }

		public enum TerminateState { ON }
		public const TerminateState Terminate = TerminateState.ON;

		public InputStates InputStates { get; private set; }

		private LNBase Parent;

		public GameBase(LNBase p) {
			Loaded = false;
			Parent = p;
			Scenes = new SceneHandler(this);
		}

		public GameScene GenerateScene(
			string id, string text, string name,
			GameScene.SceneType t, SceneBackground bg, SceneBars br, SpriteFont f,
			SceneHandler.Flag flag = SceneHandler.Flag.MIDDLE
		) {
			return new GameScene(id, text, name, t,
				bg ?? t.Parent.DefaultBCKG,
				br ?? t.Parent.DefaultBARS,
				f ?? t.Parent.DefaultFONT,
				flag
			);
		}

		public void FirstScene(string text = "", string name = "",
			GameScene.SceneType type = null, SceneBackground bckg = null, SceneBars bars = null,
			SpriteFont font = null
		) {
			Scenes.Add(GenerateScene(
				id: "first", text: text, name: name,
				t: type ?? new GameScene.SceneType(Scenes),
				bg: bckg, br: bars, f: font,
				flag: SceneHandler.Flag.FIRST
			));
		}

		public void EndScene(string text = "", string name = "",
			GameScene.SceneType type = null, SceneBackground bckg = null, SceneBars bars = null,
			SpriteFont font = null
		) {
			Scenes.Add(GenerateScene(
				id: "end", text: text, name: name,
				t: type ?? new GameScene.SceneType(Scenes),
				bg: bckg, br: bars, f: font,
				flag: SceneHandler.Flag.END
			));
		}

		public void ErrorScene(string text = "", string name = "",
			GameScene.SceneType type = null, SceneBackground bckg = null, SceneBars bars = null,
			SpriteFont font = null
		) {
			Scenes.Add(GenerateScene(
				id: "end", text: text, name: name,
				t: type ?? new GameScene.SceneType(Scenes),
				bg: bckg, br: bars, f: font,
				flag: SceneHandler.Flag.ERROR
			));
		}

		public void NewScene(
			string id, string text = "", string name = "",
			GameScene.SceneType type = null, SceneBackground bckg = null, SceneBars bars = null, SpriteFont font = null
		) {
			Scenes.Add(GenerateScene(
				id: id, text: text, name: name,
				t: type ?? new GameScene.SceneType(Scenes),
				bg: bckg, br: bars, f: font,
				flag: SceneHandler.Flag.MIDDLE
			));
		}

		public void LoadContent(SceneBackground defBCKG, SceneBars defBARS, SpriteFont defFONT) {
			DefaultBCKG = defBCKG;
			DefaultBARS = defBARS;
			DefaultFONT = defFONT;
			Loaded = true;
			Scenes.SetSceneDefaults(defBCKG, defBARS, defFONT);
		}

		public void Start() {
			Scenes.Start( );
		}

		public void Quit() {
			Parent.Exit( );
		}

		public void Draw(SpriteBatch sb) {
			Scenes.Current?.Draw(sb);
		}

		public void Update(InputStates bef) {
			InputStates = bef;
			Scenes.Current?.Update(bef);
		}

	}
}
