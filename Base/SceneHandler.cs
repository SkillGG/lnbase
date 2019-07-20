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

		private GameBase Base;

		/// <summary>
		/// Returns scene with ID as given string
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public GameScene this[string s] {
			get {
				if( s == null || s == "" )
					return null;
				return allScenes.Find((GameScene gs) => gs.ID == s);
			}
		}

		/// <summary>
		/// Returns first occurence of scene with given Flag
		/// <para>Used to find FIRST, END and ERROR scenes</para>
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public GameScene this[Flag f] {
			get {
				return allScenes.Find((GameScene s) => s.FLAG == f);
			}
		}

		/// <summary>
		/// Switches to given GameScene
		/// </summary>
		/// <param name="ngs"></param>
		public void Next(GameScene ngs) {
			if( allScenes.Contains(ngs) ) {
				current = ngs.ID;
				Current.Start( );
			}
		}

		/// <summary>
		/// Terminates whole game
		/// </summary>
		public void Terminate() {
			Base.Quit( );
		}

		/// <summary>
		/// Adds new GameScene to list
		/// </summary>
		/// <param name="gs"></param>
		public void Add(GameScene gs) {
			if( this[gs.ID] != null ) {
				// error
				return;
			}
			allScenes.Add(gs);
			if( gs.FLAG == Flag.FIRST )
				current = gs.ID;
		}

		/// <summary>
		/// Starts showing FIRST scene ( Terminate() if no FIRST Scene present )
		/// </summary>
		public void Start() {
			Next(this[Flag.FIRST] ?? allScenes[0]);
			if( Current == null )
				Terminate( );
		}

		public SceneHandler(GameBase gb) {
			allScenes = new List<GameScene>( );
			Base = gb;
		}

		public SceneBackground DefaultBCKG { get; private set; }
		public SceneBars DefaultBARS { get; private set; }
		public SpriteFont DefaultFONT { get; private set; }

		/// <summary>
		/// Sets defaults for Background, Bars and Font
		/// </summary>
		/// <param name="defBCKG"></param>
		/// <param name="defBARS"></param>
		/// <param name="defFONT"></param>
		public void SetSceneDefaults(SceneBackground defBCKG, SceneBars defBARS, SpriteFont defFONT) {
			DefaultBCKG = defBCKG;
			DefaultBARS = defBARS;
			DefaultFONT = defFONT;
		}

	}
}
