using System.Collections.Generic;

namespace lnbase.Base {
	public class SceneHandler {

		public GameScene First { get => this[Flag.FIRST]; }
		public GameScene Last { get => this[Flag.END]; }
		public GameScene Error { get => this[Flag.ERROR]; }

		private string current = null;
		public GameScene Current{ get => this[current??""]; }

		public enum Flag {
			FIRST, MIDDLE, END, ERROR
		}

		public List<GameScene> allScenes;

		public GameScene this[string s] {
			get {
				if( s == null )
					return null;
				return allScenes.Find((GameScene gs) => gs.ID == s);
			}
		}

		public GameScene this[Flag f] {
			get {
				return allScenes.Find((GameScene s) => s.FLAG == f);
			}
		}

		public void Add(GameScene gs) {
			if( this[gs.ID] != null ) {
				// error
				return;
			}
			if( current == null )
				current = gs.ID;
			allScenes.Add(gs);
		}

		public SceneHandler() {
			allScenes = new List<GameScene>( );
		}

	}
}
