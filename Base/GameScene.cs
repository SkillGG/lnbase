﻿using ITW.Exts;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Threading;

namespace lnbase.Base {
	public class GameScene {

		public string ID { get; private set; }
		public SceneHandler.Flag FLAG { get; private set; }

		public SceneBehaviour TYPE { get; private set; }

		public SceneBackground BCKG { get; private set; }
		public SceneBars BARS { get; private set; }
		public SpriteFont FONT { get; private set; }

		public SceneValues VALUES { get; private set; }

		private bool Locked { get; set; }

		public void Draw(SpriteBatch sb) {
			( BCKG ?? TYPE.Parent.DefaultBCKG )?.Draw(sb);
			( BARS ?? TYPE.Parent.DefaultBARS )?.Draw(sb, FONT ?? TYPE.Parent.DefaultFONT,
				new Rectangle(100, LNBase.CH - 200, 1166, 200),
				VALUES.NAME, VALUES.VISIBLE
			);
		}

		public GameScene(string id, string text, string name,
			SceneBehaviour t, SceneBackground bg, SceneBars br, SpriteFont f,
			SceneHandler.Flag flag) {
			Locked = false;
			ID = id;
			VALUES = new SceneValues(name, text);
			TYPE = t;
			BCKG = bg ?? t.Parent.DefaultBCKG;
			BARS = br ?? t.Parent.DefaultBARS;
			FONT = f ?? t.Parent.DefaultFONT;
			FLAG = flag;
		}

		public GameScene NextScene { get; private set; }

		public void Lock() {
			Locked = true;
		}

		public void Next(GameScene gs) {
			if( !Locked )
				NextScene = gs;
		}

		public void Next(GameBase.TerminateState t) {
			if( !Locked )
				NextScene = null;
		}

		public void ShowNext() {
			if( NextScene == null ) {
				TYPE.Parent.Terminate( );
			} else {
				TYPE.Parent.Next(NextScene);
			}
		}

		public void Condition(Func<InputStates, SceneValues, bool> condition) {
			TYPE.SetCondition(condition);
		}

		public void Start() {
			TYPE.Init(this);
		}

		public void Stop() {
			TYPE.Dispose( );
			TYPE = null;
		}

		public void Update(InputStates bef) {
			TYPE.ConditionClock(bef, this);
		}

		public class SceneValues {

			public string NAME { get; private set; }
			public string VISIBLE { get; private set; }
			private string FULL_TEXT { get; set; }

			public int FullLength { get => FULL_TEXT.Length + 1; }

			public SceneValues(string name, string text) {
				FULL_TEXT = text;
				NAME = name;
				VISIBLE = "";
			}

			public void SetVisible(int ln) {
				if( ln >= FULL_TEXT.Length )
					VISIBLE = FULL_TEXT;
				else {
					VISIBLE = FULL_TEXT.Substring(0, ln);
				}
			}

		}

		public class SceneBehaviour {

			public Timer MainClock { get; private set; }

			public Action Before { get; private set; }
			public Func<int, SceneValues, bool> Update { get; private set; }
			public Action After { get; private set; }

			public Func<InputStates, SceneValues, bool> Condition { get; private set; }

			public int Period { get; private set; }

			public SceneHandler Parent { get; private set; }

			private bool ConditionWait { get; set; }

			private int UC_Count { get; set; }

			public void Init(GameScene sv) {
				if( Period > 0 ) {
					MainClock = new Timer(UpdateClock, sv, this.Period, this.Period);
				} else
					UpdateClick(sv);
			}

			public SceneBehaviour(SceneHandler sh) {
				Period = 0;
				Parent = sh;
				Before = () => { };
				After = () => { };
				Update = (int cycle, SceneValues sv) => {
					sv.SetVisible(sv.FullLength);
					return true;
				};
				Reset( );
			}

			public SceneBehaviour(SceneHandler sh,
				int updatepace = 100,
				Func<int, SceneValues, bool> upt = null,
				Action aft = null,
				Action bef = null
			) {
				Parent = sh;
				Period = updatepace;
				if( upt == null )
					upt = (int c, SceneValues sv) => {
						if( c < sv.FullLength ) {
							sv.SetVisible(c);
							return false;               // continue;
						}
						sv.SetVisible(sv.FullLength);
						return true;                    // break;
					};
				if( aft == null )
					aft = () => { };
				if( bef == null )
					bef = () => { };

				Before = bef;
				Update = upt;
				After = aft;

				Reset( );

			}

			private void Reset() {
				UC_Count = 0;
				ConditionWait = false;
			}

			public void Dispose() {
				Reset( );
				MainClock?.Dispose( );
				MainClock = null;
			}

			public void ConditionInit() {
				ConditionWait = true;
			}

			public void ConditionClock(InputStates b, GameScene gs) {
				if( ConditionWait ) {
					if( Condition(b, gs.VALUES) ) {
						gs.ShowNext( );
						Reset( );
					}
				}
			}

			public void SetCondition(Func<InputStates, SceneValues, bool> cond) { Condition = cond; }

			private void UpdateClick(GameScene sv) {
				Update(0, sv.VALUES);
				Dispose( );
				After( );
				ConditionInit( );
				return;
			}

			public void UpdateClock(object sv) {
				GameScene scene = (GameScene) sv;
				if( Update(UC_Count, scene.VALUES) ) {
					Dispose( );
					After( );
					ConditionInit( );
					return;
				}
				UC_Count++;
			}
		}

	}
}
