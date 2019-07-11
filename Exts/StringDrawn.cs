using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using System;
using System.Linq;

namespace ITW.Exts {

	/// <summary>
	/// It's cass that provides XNA support for working with typed strings one letter at the time.
	/// </summary>
	public class StringDrawn {

		private string full;
		private string visible;
		private DrawType type;

		public string Text { get => full; set { if( !First ) full = value; } }

		private int walkerPeriod;
		private Timer walker;

		/// <summary>
		/// A class, that defines how <see cref="StringDrawn"/> will be drawn
		/// </summary>
		public class DrawType {

			/// <summary>
			/// Struct that holds a <see cref="string"/> and an <see cref="int"/>
			/// </summary>
			public struct Stint {
				/// <summary>
				/// Converts <see cref="Stint" /> into <see cref="string"/> formatted $d,$s
				/// </summary>
				/// <returns><seealso cref="string"/> formatted $d,$s </returns>
				public override string ToString() => $"{Int},{String}";

				/// <summary>
				/// <see cref="Stint"/>'s <see cref="string"/>
				/// </summary>
				public string String;

				/// <summary>
				/// <see cref="Stint"/>'s <see cref="int"/>
				/// </summary>
				public int Int;

				/// <summary>
				/// <see cref="Stint"/>'s constructor
				/// </summary>
				/// <param name="s"><see cref="string"/> to hold</param>
				/// <param name="i"><see cref="int"/> to hold</param>
				public Stint(string s, int i) { String = s; Int = i; }
			}

			/// <summary>
			/// Type of draw:<para></para>
			/// 0 - all at once<para></para>
			/// 1 - every character each 'pace' ms<para></para>
			/// 2 - Point array where X indicates index where to start section (first X in array **IS** always 0) and Y indicates pace
			/// <para></para></summary>
			private readonly byte type;

			/// <summary>
			/// A speed, when next letter will be drawn
			/// </summary>
			private int pace;

			/// <summary>
			/// Number of letters already drawn
			/// </summary>
			private int step = 0;

			/// <summary>
			/// <para>Array of <see cref="Point"/>s that stores information about changes in speed</para>
			/// Each <see cref="Point"/>'s:
			/// <para>X is considered a step, when to change pace</para>
			/// <para>Y is considered a new pace set, when given step is reached</para>
			/// </summary>
			private Point[] steps;

			/// <summary>
			/// Default constructor. It creates type1 DrawType, which draws whole string at once.
			/// </summary>
			public DrawType() {
				type = 0;
				step = 0;
			}

			/// <summary>
			/// Pace constructor. It creates type2 DrawType, which draws whole string with constant speed.
			/// </summary>
			/// <param name="pc"></param>
			public DrawType(int pc) {
				type = 1;
				pace = pc;
			}

			/// <summary>
			/// Pace constructor. It creates type3 DrawType, which draws whole string with ability to change speeds.
			/// </summary>
			/// <param name="pace"></param>
			public DrawType(Point[] pace) {
				type = 2;
				steps = pace;
				this.pace = steps[0].Y;
				for( int i = 1; i < steps.Length; i++ ) // Shift given step indexes by one to ensure same results for each string
					steps[i].X--;
				steps = steps.OrderBy((o) => o.X).ToArray( );   // Sort changes
				steps = steps.GroupBy(p => p.X).Select(g => g.First( )).ToArray( ); // Prevent X duplicates
			}


			/// <summary>
			/// Getter for first step ("empty step")
			/// </summary>
			/// <param name="s">Full string to get first step of</param>
			/// <returns>A first step where <see cref="Stint.String"/> is starting point and <see cref="Stint.Int"/> is starting pace</returns>
			public Stint GetFirstStep(string s) {
				if( type != 0 )
					return new Stint("", pace);
				return new Stint(s, -1);
			}

			/// <summary>
			/// Returns <see cref="Point"/> which corresponds to speed of drawing on n-step speed of drawing.
			/// </summary>
			/// <param name="n">step to get <see cref="Point"/> from</param>
			/// <returns><see cref="Point"/> which corresponds to speed of drawing on n-step speed of drawing.</returns>
			private Point GetPointFromStep(int n) {
				if( this.steps == null || this.steps.Length == 0 )
					throw new NullReferenceException("Points are null! Set at least one!");
				int r = 0;
				for( int i = 0; i < this.steps.Length - 1; i++ ) {  // foreach step
																	//if( this.steps[i].X == this.steps[i + 1].X )	// if step duplicate
																	//continue;
																	// if on i+1 or between i and i+
					if( n == this.steps[i + 1].X || ( n < steps[i + 1].X && n > steps[i].X ) )
						r = i;
					// if on i
					else if( n == steps[i].X )
						r = i - 1;
					// if bigger than last step
					else if( i == steps.Length - 2 && n > steps[i].X )
						r = i + 1;
				}
				return this.steps[r < 0 ? 0 : r];
			}

			/// <summary>
			/// <para>Getter for next step's <see cref="Stint"/></para>
			/// Returns:
			/// <para><see cref="Stint"/>: <see cref="Stint.String"/> = what should be vivisble at this step, <see cref="Stint.Int"/> = delay to next step</para>
			/// </summary>
			/// <param name="s">Full <see cref="string"/></param>
			/// <returns><see cref="Stint"/>: <see cref="Stint.String"/> = what should be visble at this step, <see cref="Stint.Int"/> = delay to next step</returns>
			public Stint GetNextStep(string s) {
				if( type == 2 ) {
					if( steps.Length <= 0 )     // no steps found!
						throw new NullReferenceException("Points are null! Set at least one!");
					pace = GetPointFromStep(++step).Y;  // Change pace to one next step's
					return new Stint(s.Substring(0, step), pace);
				}
				if( type == 1 )
					return new Stint(s.Substring(0, ++step), pace);
				return new Stint(s, -1);
			}

		}

		/// <summary>
		/// Timer's function to invoke every time new letter should be drawn
		/// </summary>
		/// <param name="o">Timer <see langword="null"/> object</param>
		public void ShowNext(object o) {
			// Get next step data
			var nextStep = type.GetNextStep(full);
			if( nextStep.Int != walkerPeriod ) {    // If speed changed

				// Change Timer's speed to new one
				walkerPeriod = nextStep.Int;                // Change compare variable to new one
				walker.Change(walkerPeriod, walkerPeriod);  // Change Timer's delay to new one
			}
			visible = nextStep.String;      // start showing new string
			if( nextStep.String == full ) { // If whole string has been drawn

				// Destroy timer and wait Show():aft ammount of time before executing endFunc
				walker.Dispose( );          // Stop timer
				walker = null;              // Destroy timer
				walker = new Timer(         // Make new timer
					(object ox) => {        // ox always null 
						walker.Dispose( );  // Stop timer
						walker = null;      // Destroy timer
						endFunc?.Invoke( ); // If endFunc specified, invoke it
					}
					, null, afterWait, 1    // invoke after Show():aft ammount of time
				);
			}
		}

		/// <summary>
		/// Position of <see cref="string"/> on screen
		/// </summary>
		public Vector2 Position { get; private set; }

		/// <summary>
		/// last font this string was drawn with
		/// </summary>
		public SpriteFont lastFont;

		/// <summary>
		/// Border of entire string as it HAS BEEN drawn previously/will be drawn now
		/// </summary>
		public Rectangle? Border => lastFont == null ? null :
		new Rectangle?(new Rectangle(
			(int) Position.X, (int) Position.Y,
			(int) lastFont.MeasureString(this.full).X, (int) lastFont.MeasureString(this.full).Y
		));

		/// <summary>
		/// Moves <see cref="string"/> to <paramref name="p"/> position
		/// </summary>
		/// <param name="p">New position, if null nothing moves</param>
		public void MoveTo(Vector2? p) => Position = p ?? Position;

		/// <summary>
		/// Color of string
		/// </summary>
		public Color Color { get; private set; }

		/// <summary>
		/// Recolors whole string
		/// </summary>
		/// <param name="c">New color, if null color is set to <see cref="Color.White"/></param>
		public void Recolor(Color? c = null) => Color = c ?? Color.White;

		/// <summary>
		/// This class is a class that 'types' a string as defined in <paramref name="dt"/>.
		/// </summary>
		/// <param name="s">A string to show</param>
		/// <param name="p">Position where to draw (left-top corner)</param>
		/// <param name="dt">A <code>DrawType</code> (how to type string)</param>
		/// <param name="c">Color of string</param>
		public StringDrawn(string s, Vector2? p = null, StringDrawn.DrawType dt = null, Color? c = null) {
			this.Text = s ?? "undefined";
			this.MoveTo(p);
			this.type = dt ?? new DrawType( );
			this.Recolor(c);
		}

		/// <summary>
		/// Function invoked after drawing has ended
		/// </summary>
		private Action endFunc;

		/// <summary>
		/// Setter for endFunc - function invoked after drawing has ended.
		/// </summary>
		/// <param name="f"></param>
		public void End(Action f) {
			this.endFunc = f;
		}

		/// <summary>
		/// Flag, set when <see cref="Show(int, int)"/> is invokedfor the first time
		/// </summary>
		private bool First { get; set; }


		/// <summary>
		/// How much time to wait before invoking <see cref="endFunc"/> after drawing changed
		/// </summary>
		private int afterWait;

		/// <summary>
		/// <see cref="Show(int, int)"/> returning <code>this</code> to chain with <see cref="Draw(SpriteBatch, SpriteFont)"/>
		/// </summary>
		/// <param name="rt">complementary param to work distinguish from <see cref="Show(int, int)"/></param>
		/// <param name="bef"><see cref="Show(int, int)"/>:bef</param>
		/// <param name="aft"><see cref="Show(int, int)"/>:aft</param>
		/// <returns>this</returns>
		public StringDrawn Show(bool rt, int bef = 0, int aft = 0){
			Show(bef, aft);
			return this;
		}

		/// <summary>
		/// Shows to screen string in its starting position which is either empty or full<para></para>
		/// and starts adding new characters after delay, also sets EndDelay
		/// <para>This should be called at least once before showing to the screen </para>
		/// BEFORE first <see cref="Draw(SpriteBatch, SpriteFont)"/>
		/// </summary>
		/// <param name="bef">First delay: before string starts to draw</param>
		/// <param name="aft">Second(End) delay: after string ends to draw, before invoking <see cref="End(Action)"/></param>
		public void Show(int bef = 0, int aft = 0) {
			if( First ) // if already started
				return; // ABORT!

			// First start
			First = true;       // Setting 'already-started' flag
			afterWait = aft;    // Setting EndDelay
			this.walkerPeriod = type.GetFirstStep(full).Int;    // Getting first step's pace
			if( walkerPeriod != -1 )    // If we type
				this.walker = new Timer(ShowNext, null, bef, walkerPeriod);
			else                        // If we show all at once
				this.walker = new Timer((object x) => { // after bef:
														// If shown all: stop, delete timer and invoke endFunc
					if( visible == full ) {
						if( walker != null ) { endFunc?.Invoke( ); return; }
						walker.Dispose( );
						walker = null;
						endFunc?.Invoke( );
						return;
					}
					visible = full;             // show all
					if( walker == null )
						return;
					walker.Change(aft, 1);      // EndDelay 
				}, null, bef, 1);
		}

		/// <summary>
		/// Resets string to its starting values.
		/// <para>Invoking <see cref="Show(int, int)"/> is necesarry for second draw</para>
		/// Setting parameters will change them after reset
		/// </summary>
		/// <param name="s">Text</param>
		/// <param name="p">Position</param>
		/// <param name="dt">DrawType</param>
		/// <param name="c">Color</param>
		public void Reset(string s = null, Vector2? p = null, StringDrawn.DrawType dt = null, Color? c = null) {
			walker?.Dispose( );  // stop timer
			walker = null;      // delete timer
			afterWait = 0;      // set EndDelay to 0
			First = false;      // set 'already-started' flag to 0

			// Reset values
			this.Text = s ?? this.Text;
			this.MoveTo(p ?? this.Position);
			this.type = dt ?? this.type;
			this.Recolor(c ?? this.Color);
		}

		/// <summary>
		/// Draws visible <see cref="string"/> on <see cref="SpriteBatch"/> using
		/// <para><see cref="SpriteBatch.DrawString(SpriteFont, string, Vector2, Color)"/> </para>
		/// </summary>
		/// <param name="sb"><see cref="SpriteBatch"/> to draw to</param>
		/// <param name="font"><see cref="SpriteFont"/> to draw with</param>
		public void Draw(SpriteBatch sb, SpriteFont font) {
			if( font != lastFont )	// if font has changed / first time drawing
				lastFont = font;	// set current font to new one
			if( First ) // if shown at least once
				sb.DrawString(font, visible ?? " ", Position, this.Color);  // draw
		}

	}
}