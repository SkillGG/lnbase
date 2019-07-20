using ITW.Exts;
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

		/// <summary>
		/// Draws Scene
		/// </summary>
		/// <param name="sb"></param>
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

		/// <summary>
		/// Locks NextScene to currently seleted
		/// <para>==== LOCK IS IRREMOVABLE ====</para>
		/// </summary>
		public void Lock() {
			Locked = true;
		}

		/// <summary>
		/// Changes next Scene (can be locked via .Lock)
		/// </summary>
		/// <param name="gs"></param>
		public void Next(GameScene gs) {
			if( !Locked )
				NextScene = gs;
		}

		/// <summary>
		/// Changes scene from this to next one in Queue
		/// </summary>
		public void ShowNext() {
			if( NextScene == null ) {
				TYPE.Parent.Terminate( );
			} else {
				TYPE.Parent.Next(NextScene);
			}
		}

		/// <summary>
		/// Changes next scenes condition
		/// <para>When condition returns true next scene will be started</para>
		/// </summary>
		/// <param name="condition"></param>
		public void Condition(Func<InputStates, SceneValues, bool> condition)
			=> TYPE.SetCondition(condition);

		/// <summary>
		/// GameScene initialization (fires Before() and starts UpdateTimer)
		/// </summary>
		public void Start()
			=> TYPE.Init(this);

		/// <summary>
		/// Stops running of whole Scene.
		/// </summary>
		public void Stop() {
			TYPE.Dispose( );
			TYPE = null;
		}

		/// <summary>
		/// ConditionUPDATE
		/// </summary>
		/// <param name="bef"></param>
		public void Update(InputStates bef) {
			TYPE.ConditionClock(bef, this);
		}

		/// <summary>
		/// Class to handle direct changes to Scene without direct access to Scene object
		/// </summary>
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

			/// <summary>
			/// Set how many letters from full text will be viisble in text-box
			/// </summary>
			/// <param name="ln"></param>
			public void SetVisible(int ln) {
				if( ln >= FULL_TEXT.Length )
					VISIBLE = FULL_TEXT;
				else {
					VISIBLE = FULL_TEXT.Substring(0, ln);
				}
			}

		}

		/// <summary>
		/// Class that control Scene behaviour.
		/// </summary>
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

			/// <summary>
			/// Start of whole system
			/// </summary>
			/// <param name="sv"></param>
			public void Init(GameScene sv) {
				Before( );
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

			/// <summary>
			/// Resets Condition to its first value
			/// </summary>
			private void Reset() {
				UC_Count = 0;
				ConditionWait = false;
			}

			/// <summary>
			/// Deletes clock.
			/// </summary>
			public void Dispose() {
				Reset( );
				MainClock?.Dispose( );
				MainClock = null;
			}

			/// <summary>
			/// Start waiting for Condition() to return true to show next scene
			/// </summary>
			public void ConditionInit()
				=> ConditionWait = true;

			/// <summary>
			/// Function that check results of Condition()
			/// </summary>
			/// <param name="b"></param>
			/// <param name="gs"></param>
			private void ConditionClock(InputStates b, GameScene gs) {
				if( ConditionWait ) {
					if( Condition(b, gs.VALUES) ) {
						gs.ShowNext( );
						Reset( );
					}
				}
			}

			public void SetCondition(Func<InputStates, SceneValues, bool> cond)
				=> Condition = cond;
			
			/// <summary>
			/// UpdateClock for ICW ( immidiate condition wait ) SceneBehaviour
			/// </summary>
			/// <param name="sv"></param>
			private void UpdateClick(GameScene sv) {
				Update(0, sv.VALUES);
				Dispose( );
				After( );
				ConditionInit( );
				return;
			}

			/// <summary>
			/// Funtion that each Period[ms] checks for result from Update()
			/// </summary>
			/// <param name="sv"></param>
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
