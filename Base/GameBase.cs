using ITW.Exts;
using Microsoft.Xna.Framework.Graphics;

namespace lnbase.Base {
	public class GameBase {

		public SceneHandler Scenes;

		public SceneBackground DefaultBCKG { get; private set; }
		public SceneBars DefaultBARS { get; private set; }
		public SpriteFont DefaultFONT { get; private set; }

		private bool Loaded { get; set; }

		public enum TerminateState { ON }
		public const TerminateState Terminate = TerminateState.ON;


		public GameBase() {
			Loaded = false;
		}

		public GameScene GenerateScene(
			string id, string text, string name,
			SceneType t, SceneBackground bg, SceneBars br, SpriteFont f,
			SceneHandler.Flag flag = SceneHandler.Flag.MIDDLE
		){
			return new GameScene(id,text,name,t,bg,br,f,flag);
		}

		public void FirstScene(string text = "", string name = "",
			SceneType type = null, SceneBackground bckg = null, SceneBars bars = null,
			SpriteFont font = null
		) {
			Scenes.Add(GenerateScene(
				id: "first", text: text, name: name,
				t: type ? new SceneType( ), bg: bckg ? DefaultBCKG, br: bars ? DefaultBARS, f: font ? DefaultFONT,
				flag: SceneHandler.Flag.FIRST
			));
		}

		public void EndScene(string text = "", string name = "",
			SceneType type = null, SceneBackground bckg = null, SceneBars bars = null,
			SpriteFont font = null
		) {
			Scenes.Add(GenerateScene(
				id: "end", text: text, name: name,
				t: type ? new SceneType( ), bg: bckg ? DefaultBCKG, br: bars ? DefaultBARS, f: font ? DefaultFONT,
				flag: SceneHandler.Flag.END
			));
		}

		public void ErrorScene(string text = "", string name = "",
			SceneType type = null, SceneBackground bckg = null, SceneBars bars = null,
			SpriteFont font = null
		){
			Scenes.Add(GenerateScene(
				id: "end", text: text, name: name,
				t: type ? new SceneType( ), bg: bckg ? DefaultBCKG, br: bars ? DefaultBARS, f: font ? DefaultFONT,
				flag: SceneHandler.Flag.ERROR
			));
		}

		public void NewScene(
			string id, string text = "", string name = "",
			SceneType type = null, SceneBackground bckg = null, SceneBars bars = null, SpriteFont font = null
		){
			Scenes.Add(GenerateScene(
				id: id, text: text, name: name,
				t: type ? new SceneType( ), bg: bckg ? DefaultBCKG, br: bars ? DefaultBARS, f: font ? DefaultFONT,
				flag: SceneHandler.Flag.MIDDLE
			));
		}

		public void LoadContent(SceneBackground defBCKG, SceneBars defBARS, SpriteFont defFONT) {
			if( defBCKG != null || DefaultBARS != null || defFONT != null )
				return;
			DefaultBCKG = defBCKG;
			DefaultBARS = defBARS;
			DefaultFONT = defFONT;
			Loaded = true;

		}

		public void Start(){
			Scenes.Start( );
		}

		public void Draw(SpriteBatch sb){
			Scenes.Current.Draw( );
		}

		public InputStates inputStates { get; private set; }

		public void Update(InputStates bef){
			inputStates = bef;
		}

	}
}
