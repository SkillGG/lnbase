using ITW.Exts;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Threading;

namespace lnbase.Base {
	public class GameScene {

		public string ID { get; private set; }
		public SceneHandler.Flag FLAG { get; private set; }

		public SceneType TYPE { get; private set; }

		public SceneBackground BCKG { get; private set; }
		public SceneBars BARS { get; private set; }
		public SpriteFont FONT { get; private set; }

		public SceneValues VALUES { get; private set; }

		public void Draw(SpriteBatch sb) {
			( BCKG ?? TYPE.Parent.DefaultBCKG )?.Draw(sb);
			( BARS ?? TYPE.Parent.DefaultBARS )?.Draw(sb, FONT ?? TYPE.Parent.DefaultFONT,
				new Rectangle(100, LNBase.CH - 200, 1166, 200),
				VALUES.NAME, VALUES.VISIBLE
			);
		}

		public GameScene(string id, string text, string name,
			SceneType t, SceneBackground bg, SceneBars br, SpriteFont f,
			SceneHandler.Flag flag) {

			ID = id;
			VALUES = new SceneValues(name, text);
			TYPE = t;
			BCKG = bg;
			BARS = br;
			FONT = f;
			FLAG = flag;
		}

		public GameScene NextScene { get; private set; }

		public void Next(GameScene gs) {
			NextScene = gs;
		}

		public void Next(GameBase.TerminateState t) {
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

		public class SceneType {

			public Timer MainClock { get; private set; }

			public Action Before { get; private set; }
			public Func<int, SceneValues, bool> Update { get; private set; }
			public Action After { get; private set; }

			public Func<InputStates, SceneValues, bool> Condition { get; private set; }

			public int Period { get; private set; }

			public SceneHandler Parent { get; private set; }

			public void Init(GameScene sv) {
				Before( );
				if( Period > 0 ) {
					MainClock = new Timer(UpdateClock, sv, this.Period, this.Period);
				} else
					UpdateClock(null);
			}

			public SceneType(SceneHandler sh) {
				Period = 0;
				Parent = sh;
				Update = (int cycle, SceneValues sv) => {
					sv.SetVisible(sv.FullLength);
					return true;
				};
				Reset( );
			}

			public SceneType(SceneHandler sh,
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

			private bool ConditionWait { get; set; }

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

			private int UC_Count { get; set; }

			public void UpdateClock(object sv) {
				GameScene scene = (GameScene) sv;
				if( Update(UC_Count, scene.VALUES) ) {
					MainClock.Dispose( );
					MainClock = null;
					After( );
					ConditionInit( );
				}
				UC_Count++;
			}
		}

	}
}
