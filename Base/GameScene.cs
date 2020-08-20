using ITW.Exts;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace lnbase.Base {
	public class GameScene {

		public string ID { get; private set; }
		public SceneHandler.Flag FLAG { get; private set; }

		public SceneBehaviour TYPE { get; private set; }

		public SceneBackground BCKG { get; private set; }
		public SceneBars BARS { get; private set; }
		public SpriteFont FONT { get; private set; }

		/// <summary>
		/// Values of given scene
		/// </summary>
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
			VALUES = new SceneValues(new InputStates( ), name, text);
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
			VALUES.BefInput = bef;
		}

		/// <summary>
		/// Class to handle direct changes to Scene without direct access to Scene object
		/// </summary>
		public class SceneValues {

			/// <summary>
			/// Name on the scene
			/// </summary>
			public string NAME { get; private set; }
			/// <summary>
			/// Text visible in the text window
			/// </summary>
			public string VISIBLE { get; private set; }
			/// <summary>
			/// Full text inside text window
			/// </summary>
			private string FULL_TEXT { get; set; }

			public InputStates BefInput;

			/// <summary>
			/// Length of entire text
			/// </summary>
			public int FullLength { get => FULL_TEXT.Length + 1; }

			/// <summary>
			/// Miscancellous objects that can be passed to upt function
			/// </summary>
			public List<object> MiscValues;

			public SceneValues(InputStates i, string name, string text) {
				FULL_TEXT = text;
				NAME = name;
				VISIBLE = "";
				MiscValues = new List<object>( );
				BefInput = i;
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

			/// <summary>
			/// Main Clock of a scene
			/// </summary>
			public Timer MainClock { get; private set; }

			/// <summary>
			/// Action that happens before first Update (and Draw) of a scene
			/// </summary>
			public Action Before { get; private set; }

			/// <summary>
			/// ActiveUpdate of a scene
			/// </summary>
			public Func<int, SceneValues, bool> Update { get; private set; }

			/// <summary>
			/// Action that happens after last Update (when exiting given scene).
			/// </summary>
			public Action After { get; private set; }

			/// <summary>
			/// Condition to go to next scene
			/// </summary>
			public Func<InputStates, SceneValues, bool> Condition { get; private set; }

			public int Period { get; private set; }

			public SceneHandler Parent { get; private set; }

			private bool ConditionWait { get; set; }

			private int UC_Count { get; set; }

			private InputStates input;

			/// <summary>
			/// Start of whole system
			/// </summary>
			/// <param name="sv"></param>
			public void Init(GameScene sv) {
				Before( );
				input = new InputStates( );
				if( Period > 0 ) {
					MainClock = new Timer(UpdateClock, sv, this.Period, this.Period);
				} else
					UpdateClick(sv);
			}

			/// <summary>
			/// Set to defualt behaviour of a scene (show at once)
			/// </summary>
			/// <param name="sh">SceneHandler to access other scenes</param>
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

			/// <summary>
			/// Set behaviour of given scene
			/// </summary>
			/// <param name="sh">SceneHandler to be able toaccess other scenes</param>
			/// <param name="updatepace">Spped in ms of update function</param>
			/// <param name="upt">Update function. If returns true, scene ends</param>
			/// <param name="aft">Function after </param>
			/// <param name="bef"></param>
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
			public void ConditionClock(InputStates b, GameScene gs) {
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
				input.Update( );
			}
		}

	}

	public static class GameSceneConditions {

		// Click condition function
		public static bool Click(InputStates i, GameScene.SceneValues sv) {
			InputStates ni = new InputStates( );
			if( ni.MouseReleased(i).Button == MouseButton.LEFT )
				return true;
			return false;
		}

		public static bool Enter(InputStates i, GameScene.SceneValues sv) {
			InputStates ni = new InputStates( );
			if( ni.KeyUp(i, Keys.Enter) )
				return true;
			return false;
		}

		public static bool ClickOrEnter(InputStates i, GameScene.SceneValues sv) {
			if( Click(i, sv) )
				return true;
			else if( Enter(i, sv) )
				return true;
			return false;
		}
	}

}
