using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ITW.Exts {

	/// <summary>
	/// Possible mouse clicks
	/// </summary>
	public enum MouseButton {
		RIGHT,
		LEFT,
		MIDDLE,
		NONE
	}

	/// <summary>
	/// A class that holds information about an instance of mouse click
	/// </summary>
	public class MouseClick {

		/// <summary>
		/// <see cref="MouseButton"/> which was pressed on mouse
		/// </summary>
		public MouseButton Button { get; private set; }

		/// <summary>
		/// Position of the mouse
		/// </summary>
		public Point Point { get; private set; }

		/// <summary>
		/// Rectangle at mouse's position to use <see cref="Rectangle.Intersects(Rectangle)"/>
		/// </summary>
		public Rectangle Rectangle { get => new Rectangle(Point, new Point(1, 1)); private set { } }

		private MouseClick() { }

		public MouseClick(Point p, MouseButton b) {
			this.Point = p;
			this.Button = b;
		}

		public MouseClick(int x, int y, MouseButton b) {
			this.Point = new Point(x, y);
			this.Button = b;
		}

	}

	/// <summary>
	/// Class that takes care of every kind of input.
	/// </summary>
	public class InputStates {

		/// <summary>
		/// MonoGame's <see cref="MouseState"/>.
		/// </summary>
		public MouseState mouse;
		/// <summary>
		/// MonoGame's <see cref="KeyboardState"/>.
		/// </summary>
		public KeyboardState keyboard;
		/// <summary>
		/// MonoGame's <see cref="GamePadState"/>. (Player 1)
		/// </summary>
		public GamePadState gamePad1;
		/// <summary>
		/// MonoGame's <see cref="GamePadState"/>. (Player 2)
		/// </summary>
		public GamePadState gamePad2;
		
		/// <summary>
		/// Mouse position as a rectangle.
		/// </summary>
		public Rectangle MouseRectangle {
			get {
				return new Rectangle(mouse.Position, new Point(1, 1));
			}
		}

		/// <summary>
		/// First initialization of InputState
		/// <para>Best used in: <see cref="ITW.Initialize()"/></para>
		/// </summary>
		public InputStates() {
			this.mouse = Mouse.GetState( );
			this.keyboard = Keyboard.GetState( );
			this.gamePad1 = GamePad.GetState(PlayerIndex.One);
			this.gamePad2 = GamePad.GetState(PlayerIndex.Two);
		}

		/// <summary>
		/// Refreshing all mouse states
		/// <para>Best used in: <see cref="ITW.Update(GameTime)"/> (AFTER <code>base.Update()</code>)</para>
		/// </summary>
		public void Update() {
			this.mouse = Mouse.GetState( );
			this.keyboard = Keyboard.GetState( );
			this.gamePad1 = GamePad.GetState(0);
			this.gamePad2 = GamePad.GetState(1);
		}


		// KEYS

		/// <summary>
		/// Check if given key is pressed (state check)
		/// </summary>
		/// <param name="k"></param>
		/// <returns></returns>
		public bool IsKeyDown(Keys k) { return this.keyboard.IsKeyDown(k); }
		/// <summary>
		/// Check if given key is released (state check)
		/// </summary>
		public bool IsKeyUp(Keys k) { return this.keyboard.IsKeyUp(k); }

		// KeyChange

		/// <summary>
		/// Check if given key's state has changed (fires every change).
		/// </summary>
		public bool KeyChange(InputStates b, Keys k) {
			this.Update( );
			if( b.keyboard.IsKeyDown(k) != IsKeyDown(k) )
				return true;
			return false;
		}

		// KeyUp
		/// <summary>
		/// Check if given key's state has been released (fires once).
		/// </summary>
		public bool KeyUp(InputStates b, Keys k) {
			if( KeyChange(b, k) )
				if( IsKeyUp(k) )
					return true;
			return false;
		}

		// KeyDown
		/// <summary>
		/// Check if given key's state has been pressed (fires once).
		/// </summary>
		public bool KeyDown(InputStates b, Keys k) {
			if( KeyChange(b, k) )
				if( IsKeyDown(k) )
					return true;
			return false;
		}


		// BUTTONS

		// IsButtonDown
		/// <summary>
		/// Check if pad button is pressed (state check)
		/// <para>Returns player's pad ID or 3 (if both players).</para>
		/// </summary>
		/// <param name="k">Button to check for</param>
		/// <returns>player's pad ID or 3 (if both players)</returns>
		public byte IsButtonDown(Buttons k) {
			this.Update( );
			if( gamePad1.IsButtonDown(k) ) {
				if( gamePad2.IsButtonDown(k) )
					return 3;
				return 1;
			} else if( gamePad2.IsButtonDown(k) )
				return 2;
			return 0;
		}

		/// <summary>
		/// Checks if button is pressed on any gamepad
		/// </summary>
		/// <param name = "k" >Button to check for</param>
		/// <returns></returns>
		public bool IsButtonDownAny(Buttons k) {
			return ( IsButtonDown(k) != 0 );
		}

		// IsButtonUp
		/// <summary>
		/// Check if pad button is released (state check)
		/// <para>Returns player's pad ID or 3 (if both players).</para>
		/// </summary>
		/// <param name="k">Button to check for</param>
		/// <returns>player's pad ID or 3 (if both players)</returns>
		public byte IsButtonUp(Buttons k) {
			this.Update( );
			if( gamePad1.IsButtonUp(k) ) {
				if( gamePad2.IsButtonUp(k) )
					return 3;
				return 1;
			} else if( gamePad2.IsButtonUp(k) )
				return 2;
			return 0;
		}

		/// <summary>
		/// Checks if button is released on any gamepad
		/// </summary>
		/// <param name = "k" >Button to check for</param>
		/// <returns></returns>
		public bool IsButtonUpAny(Buttons k) {
			return ( IsButtonUp(k) != 0 );
		}

		// Button Change
		/// <summary>
		/// Check if pad button's state changed (fires every change)
		/// <para>Returns player's pad ID or 3 (if both players).</para>
		/// </summary>
		/// <param name="k">Button to check for</param>
		/// <returns>player's pad ID or 3 (if both players)</returns>
		public byte ButtonChange(InputStates b, Buttons k) {
			this.Update( );
			if( b.gamePad1.IsButtonDown(k) != this.gamePad1.IsButtonDown(k) ) {
				if( b.gamePad2.IsButtonDown(k) != this.gamePad2.IsButtonDown(k) )
					return 3;
				return 1;
			}
			if( b.gamePad2.IsButtonDown(k) != this.gamePad2.IsButtonDown(k) )
				return 2;
			return 0;
		}

		/// <summary>
		/// Checks if button's state changed on any gamepad (fires every change)
		/// </summary>
		/// <param name = "k" >Button to check for</param>
		/// <returns></returns>
		public bool ButtonChangeAny(InputStates b, Buttons k) {
			return ( ButtonChange(b, k) != 0 );
		}

		// ButtonDown
		/// <summary>
		/// Check if pad button has been pressed (fires once)
		/// <para>Returns player's pad ID or 3 (if both players).</para>
		/// </summary>
		/// <param name="k">Button to check for</param>
		/// <returns>player's pad ID or 3 (if both players)</returns>
		public byte ButtonDown(InputStates b, Buttons k) {
			byte bc = ButtonChange(b, k);
			if( bc != 0 && bc == IsButtonDown(k) )
				return bc;
			return 0;
		}

		/// <summary>
		/// Checks if button has been pressed on any gamepad (fires once)
		/// </summary>
		/// <param name = "k" >Button to check for</param>
		/// <returns></returns>
		public bool ButtonDownAny(InputStates b, Buttons k) {
			if( ButtonChangeAny(b, k) )
				if( IsButtonDownAny(k) )
					return true;
			return false;
		}

		// ButtonUp
		/// <summary>
		/// Check if pad button has been released (fires once)
		/// <para>Returns player's pad ID or 3 (if both players).</para>
		/// </summary>
		/// <param name="k">Button to check for</param>
		/// <returns>player's pad ID or 3 (if both players)</returns>
		public byte ButtonUp(InputStates b, Buttons k) {
			byte bc = ButtonChange(b, k);
			if( bc != 0 && bc == IsButtonUp(k) )
				return bc;
			return 0;
		}

		/// <summary>
		/// Checks if button has been released on any gamepad (fires once)
		/// </summary>
		/// <param name = "k" >Button to check for</param>
		/// <returns></returns>
		public bool ButtonUpAny(InputStates b, Buttons k) {
			if( ButtonChangeAny(b, k) )
				if( IsButtonUpAny(k) )
					return true;
			return false;
		}

		/// <summary>
		/// A function that given previous InputState checks if either Right/Left/Middle mouse Button was clicked.
		/// </summary>
		/// <param name="b">Previous InputState (before .Update())</param>
		/// <returns>
		/// MouseClick object that contains every information about given click.
		/// </returns>
		public MouseClick MousePressed(InputStates b) {
			this.Update( );
			if( b.mouse.LeftButton == ButtonState.Released && this.mouse.LeftButton == ButtonState.Pressed )
				return new MouseClick(mouse.X, mouse.Y, MouseButton.LEFT);
			if( b.mouse.RightButton == ButtonState.Released && this.mouse.RightButton == ButtonState.Pressed )
				return new MouseClick(mouse.X, mouse.Y, MouseButton.RIGHT);
			if( b.mouse.MiddleButton == ButtonState.Released && this.mouse.MiddleButton == ButtonState.Pressed )
				return new MouseClick(mouse.X, mouse.Y, MouseButton.MIDDLE);
			return new MouseClick(0, 0, MouseButton.NONE); // No change in state appeared
		}

		/// <summary>
		/// A function that given previous InputState checks if either Right/Left/Middle mouse Button was released.
		/// </summary>
		/// <param name="b">Previous InputState (before .Update())</param>
		/// <returns>
		/// MouseClick object that contains every information about given click.
		/// </returns>
		public MouseClick MouseReleased(InputStates b) {
			this.Update( );
			if( b.mouse.LeftButton == ButtonState.Pressed && this.mouse.LeftButton == ButtonState.Released )
				return new MouseClick(mouse.X, mouse.Y, MouseButton.LEFT);
			if( b.mouse.RightButton == ButtonState.Pressed && this.mouse.RightButton == ButtonState.Released )
				return new MouseClick(mouse.X, mouse.Y, MouseButton.RIGHT);
			if( b.mouse.MiddleButton == ButtonState.Pressed && this.mouse.MiddleButton == ButtonState.Released )
				return new MouseClick(mouse.X, mouse.Y, MouseButton.MIDDLE);
			return new MouseClick(0, 0, MouseButton.NONE); // No change in state appeared
		}

	}

}
