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

		public InputStates InputStates { get; private set; }

		public bool Started { get; private set; }

		private LNBase Parent;

		private bool AUTH { get; set; }

		public GameBase(LNBase p) {
			Loaded = false;
			Parent = p;
			Scenes = new SceneHandler(this);
			Started = false;
			AUTH = false;
		}

		/// <summary>
		/// Generates new GameScene from only given parameters
		/// </summary>
		/// <param name="id">ID of a Scene</param>
		/// <param name="text">Text shown in text-box</param>
		/// <param name="name">Text shown as a Name</param>
		/// <param name="t">Scene Behaviour</param>
		/// <param name="bg">Scene Background image</param>
		/// <param name="br">Scene Bar image and Name/Text-Box positions</param>
		/// <param name="f">Text font</param>
		/// <param name="flag">Scene flag (only works via .FirstScene/.EndScene/.ErrorScene)</param>
		/// <returns>Newly created GameScene</returns>
		public GameScene GenerateScene(
			string id, string text, string name,
			GameScene.SceneBehaviour t, SceneBackground bg, SceneBars br, SpriteFont f,
			SceneHandler.Flag flag = SceneHandler.Flag.MIDDLE
		) {
			return new GameScene(id, text, name, t,
				bg ?? t.Parent.DefaultBCKG,
				br ?? t.Parent.DefaultBARS,
				f ?? t.Parent.DefaultFONT,
				AUTH ? flag : SceneHandler.Flag.MIDDLE
			);
		}

		/// <summary>
		/// Adds new Scene as a FIRST.
		/// </summary>
		/// <param name="text">Text shown in text-box</param>
		/// <param name="name">Text shown as a Name</param>
		/// <param name="behav">Scene Behaviour</param>
		/// <param name="bckg">Scene Background image</param>
		/// <param name="bars">Scene Bar image and Name/Text-Box positions</param>
		/// <param name="font">Text font</param>
		public void FirstScene(string text = "", string name = "",
			GameScene.SceneBehaviour behav = null, SceneBackground bckg = null, SceneBars bars = null,
			SpriteFont font = null
		) {
			AUTH = true;
			Scenes.Add(GenerateScene(
				id: "first", text: text, name: name,
				t: behav ?? new GameScene.SceneBehaviour(Scenes),
				bg: bckg, br: bars, f: font,
				flag: SceneHandler.Flag.FIRST
			));
			AUTH = false;
		}

		/// <summary>
		/// Adds new Scene as a END (which has locked .Next as Terminate)
		/// </summary>
		/// <param name="text">Text shown in text-box</param>
		/// <param name="name">Text shown as a Name</param>
		/// <param name="behav">Scene Behaviour</param>
		/// <param name="bckg">Scene Background image</param>
		/// <param name="bars">Scene Bar image and Name/Text-Box positions</param>
		/// <param name="font">Text font</param>
		public void EndScene(string text = "", string name = "",
			GameScene.SceneBehaviour behav = null, SceneBackground bckg = null, SceneBars bars = null,
			SpriteFont font = null
		) {
			if( Scenes.Last != null )
				return;
			AUTH = true;
			Scenes.Add(GenerateScene(
				id: "end", text: text, name: name,
				t: behav ?? new GameScene.SceneBehaviour(Scenes),
				bg: bckg, br: bars, f: font,
				flag: SceneHandler.Flag.END
			));
			AUTH = false;
			Scenes.Last.Next(null);
			Scenes.Last.Lock( );
		}

		/// <summary>
		/// Adds new Scene as a ERROR
		/// </summary>
		/// <param name="text">Text shown in text-box</param>
		/// <param name="name">Text shown as a Name</param>
		/// <param name="behav">Scene Behaviour</param>
		/// <param name="bckg">Scene Background image</param>
		/// <param name="bars">Scene Bar image and Name/Text-Box positions</param>
		/// <param name="font">Text font</param>
		public void ErrorScene(string text = "", string name = "",
			GameScene.SceneBehaviour behav = null, SceneBackground bckg = null, SceneBars bars = null,
			SpriteFont font = null
		) {
			AUTH = true;
			Scenes.Add(GenerateScene(
				id: "err", text: text, name: name,
				t: behav ?? new GameScene.SceneBehaviour(Scenes),
				bg: bckg, br: bars, f: font,
				flag: SceneHandler.Flag.ERROR
			));
			AUTH = false;
		}

		/// <summary>
		/// Adds new Scene
		/// </summary>
		/// <param name="id">Scene ID</param>
		/// <param name="text">Text shown in text-box</param>
		/// <param name="name">Text shown as a Name</param>
		/// <param name="behav">Scene Behaviour</param>
		/// <param name="bckg">Scene Background image</param>
		/// <param name="bars">Scene Bar image and Name/Text-Box positions</param>
		/// <param name="font">Text font</param>
		public void NewScene(
			string id, string text = "", string name = "",
			GameScene.SceneBehaviour behav = null, SceneBackground bckg = null, SceneBars bars = null,
			SpriteFont font = null
		) {
			Scenes.Add(GenerateScene(
				id: id, text: text, name: name,
				t: behav ?? new GameScene.SceneBehaviour(Scenes),
				bg: bckg, br: bars, f: font,
				flag: SceneHandler.Flag.MIDDLE
			));
		}

		/// <summary>
		/// Loads defaut values for SceneBackground, SceneBars and SpriteFont
		/// </summary>
		/// <param name="defBCKG"></param>
		/// <param name="defBARS"></param>
		/// <param name="defFONT"></param>
		public void LoadContent(SceneBackground defBCKG, SceneBars defBARS, SpriteFont defFONT) {
			DefaultBCKG = defBCKG;
			DefaultBARS = defBARS;
			DefaultFONT = defFONT;
			Loaded = true;
			Scenes.SetSceneDefaults(defBCKG, defBARS, defFONT);
		}

		/// <summary>
		/// Starts showing from first scene
		/// </summary>
		public void Start() {
			Scenes.Start( );
			Started = true;
		}

		/// <summary>
		/// Terminates whole game
		/// </summary>
		public void Quit() {
			Parent.Exit( );
		}

		/// <summary>
		/// Draws Current Scene
		/// </summary>
		/// <param name="sb"></param>
		public void Draw(SpriteBatch sb) {
			Scenes.Current?.Draw(sb);
		}

		/// <summary>
		/// Updates current Scene (ConditionUpdate) 
		/// <para></para>
		/// ==== NOT BehaviourUPDATE ====
		/// </summary>
		/// <param name="bef">InputStates from previous Update to determine keyboard/mouse/gamepad inputs</param>
		public void Update(InputStates bef) {
			InputStates = bef;
			Scenes.Current?.Update(bef);
		}

	}
}
