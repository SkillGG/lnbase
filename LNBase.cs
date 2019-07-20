﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using lnbase.Base;
using System;
using ITW.Exts;

namespace lnbase {
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class LNBase : Game {

		// SCREEN
		private const int DEFRES = 1366;
		private readonly int[] useRes = new int[2] { DEFRES, DEFRES / 16 * 9 };
		private bool fullScreen = false;
		private bool borderLess = true;
		public int WIDTH { get => this.useRes[0]; private set { this.useRes[0] = value; } }
		public int HEIGHT { get => this.useRes[1]; private set { this.useRes[1] = value; } }
		public bool FULLSCREEN { get => this.fullScreen; private set { this.fullScreen = value; } }
		public bool BORDERLESS { get => this.borderLess; private set { this.borderLess = value; } }

		// Static resolution (from default resolution)
		public static int CW = DEFRES;
		public static int CH = DEFRES / 16 * 9;

		public Color BACKGROUND { get; set; }

		private bool FirstUpdate { get; set; }

		public void ChangeGameResolution(int? w = null, int? h = null, bool? FS = null, bool? BL = null) {
			this.WIDTH = w ?? this.WIDTH;
			this.HEIGHT = h ?? this.HEIGHT;
			this.FULLSCREEN = FS ?? this.FULLSCREEN;
			this.BORDERLESS = BL ?? this.BORDERLESS;
			CW = WIDTH;
			CH = HEIGHT;
			// Save game res
			this.graphics.PreferredBackBufferHeight = HEIGHT;
			this.graphics.PreferredBackBufferWidth = WIDTH;
			this.graphics.IsFullScreen = FULLSCREEN;
			Window.IsBorderless = BORDERLESS;
			this.graphics.ApplyChanges( );
		}

		public double RevertFactor(double? d) {
			double r = d ?? 0.75;
			int count = BitConverter.GetBytes(decimal.GetBits((decimal) r)[3])[2];
			double b = Math.Pow(10, count);
			double a = r * b;
			return b / a;
		}

		public void ChangeGameResolution(int? w = null, double? res = null, bool? FS = null, bool? BL = null) {
			this.WIDTH = w ?? this.WIDTH;
			this.HEIGHT = (int) ( w / RevertFactor(res) );
			this.FULLSCREEN = FS ?? this.FULLSCREEN;
			this.BORDERLESS = BL ?? this.BORDERLESS;
			CW = WIDTH;
			CH = HEIGHT;
			// Save game res
			this.graphics.PreferredBackBufferHeight = HEIGHT;
			this.graphics.PreferredBackBufferWidth = WIDTH;
			this.graphics.IsFullScreen = FULLSCREEN;
			Window.IsBorderless = BORDERLESS;
			this.graphics.ApplyChanges( );
		}

		// SCREEN

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public InputStates inputs;

		public GameBase GameBase { get; private set; }

		public LNBase() {

			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			// show the mouse
			this.IsMouseVisible = true;

			// Initialize (set for first time) all window-related stuff
			this.graphics.PreferredBackBufferWidth = WIDTH;
			this.graphics.PreferredBackBufferHeight = HEIGHT;
			this.graphics.IsFullScreen = FULLSCREEN;
			Window.IsBorderless = BORDERLESS;
			BACKGROUND = Color.Black;
			// screen center
			Window.Position = new Point(( GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - WIDTH ) / 2, 0);

			// Set FPS
			this.IsFixedTimeStep = true;
			this.graphics.SynchronizeWithVerticalRetrace = true;
			this.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 33); // ~30.3FPS

			// setup flags
			FirstUpdate = false;

			GameBase = new GameBase(this);

		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize() {
			// TODO: Add your initialization logic here

			inputs = new InputStates( );

			base.Initialize( );
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			SceneBackground sb = new SceneBackground(new GameSprite(Content.Load<Texture2D>("background")));
			SceneBars sr = new SceneBars(
				txt: new GameSprite(Content.Load<Texture2D>("bars")),
				name: new Rectangle(30, 0, 210, 50),
				text: new Rectangle(20, 60, 1126, 100)
			);
			SpriteFont sf = Content.Load<SpriteFont>("Fira");

			GameBase.LoadContent(sb, sr, sf);

			GameBase.FirstScene(
				name: "me",
				text: "Why am I here?",
				behav: new GameScene.SceneBehaviour(GameBase.Scenes, 150)   // type each char at 150 speed
			);

			GameBase.NewScene(
				"second",
				name: "You",
				text: "I don't know",
				behav: new GameScene.SceneBehaviour(GameBase.Scenes,
					updatepace: 50,
					upt: (int cycle, GameScene.SceneValues sv) => {     // each 50ms

						// return true = end of every scene
						if( cycle >= 5 ) {
							sv.SetVisible(sv.FullLength);
							return true;
						} else
							return false;
					}
				)
			// bckg: null // default
			);

			GameBase.EndScene(
				name: "",
				text: "Its the end!",
				behav: new GameScene.SceneBehaviour(GameBase.Scenes)
			);

			GameBase.Scenes.First.Next(GameBase.Scenes["second"]);
			GameBase.Scenes["second"].Next(GameBase.Scenes.Last);

			// Click condition function
			bool Click(InputStates i, GameScene.SceneValues sv) {
				InputStates ni = new InputStates( );
				if( ni.MouseReleased(i).Button == MouseButton.LEFT )
					return true;
				return false;
			}

			bool Enter(InputStates i, GameScene.SceneValues sv) {
				InputStates ni = new InputStates( );
				if( ni.KeyUp(i, Keys.Enter) )
					return true;
				return false;
			}

			GameBase.Scenes.First.Condition(Enter);
			GameBase.Scenes["second"].Condition(Enter);
			GameBase.Scenes.Last.Condition(Click);

		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent() {
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
			if( GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState( ).IsKeyDown(Keys.Escape) )
				Exit( );

			// TODO: Add your update logic here

			if( FirstUpdate ) {
				FirstUpdate = false;
			}

			if( gameTime.TotalGameTime.Ticks > 200 && !activated )
				FirstClick( );

			if( !FirstUpdate ) {
				GameBase.Update(inputs);
			}

			base.Update(gameTime);

			inputs.Update( );
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(BACKGROUND);

			// TODO: Add your drawing code here

			spriteBatch.Begin( );

			if( GameBase.Started )
				GameBase.Draw(spriteBatch);

			spriteBatch.End( );

			base.Draw(gameTime);
		}

		private bool activated = false;

		private void FirstClick() {
			activated = true;
			GameBase.Start( );
		}

	}
}
