using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace lnbase.Base {
	public class SceneHandler {

		public GameScene First { get => this[Flag.FIRST]; }
		public GameScene Last { get => this[Flag.END]; }
		public GameScene Error { get => this[Flag.ERROR]; }

		private string current = null;
		public string CRR { get => current; }
		public GameScene Current { get => this[current ?? ""]; }

		public enum Flag {
			FIRST, MIDDLE, END, ERROR
		}

		public List<GameScene> allScenes;

		public GameScene this[string s] {
			get {
				if( s == null || s == "" )
					return null;
				return allScenes.Find((GameScene gs) => gs.ID == s);
			}
		}

		public GameScene this[Flag f] {
			get {
				return allScenes.Find((GameScene s) => s.FLAG == f);
			}
		}

		public void Next(GameScene ngs) {
			if( allScenes.Contains(ngs) ) {
				current = ngs.ID;
			}
		}

		private GameBase Base;
		public void Terminate() {
			Base.Quit( );
		}

		public void Add(GameScene gs) {
			if( this[gs.ID] != null ) {
				// error
				return;
			}
			allScenes.Add(gs);
			if( gs.FLAG == Flag.FIRST )
				current = gs.ID;
		}

		public void Start() {
			Next(this[Flag.FIRST] ?? allScenes[0]);
			if( Current == null )
				Terminate( );
			Current.Start( );
		}

		public SceneHandler(GameBase gb) {
			allScenes = new List<GameScene>( );
			Base = gb;
		}

		public SceneBackground DefaultBCKG { get; private set; }
		public SceneBars DefaultBARS { get; private set; }
		public SpriteFont DefaultFONT { get; private set; }

		public void SetSceneDefaults(SceneBackground defBCKG, SceneBars defBARS, SpriteFont defFONT) {
			if( defBCKG != null || DefaultBARS != null || defFONT != null )
				return;
			DefaultBCKG = defBCKG;
			DefaultBARS = defBARS;
			DefaultFONT = defFONT;
		}

	}
}
