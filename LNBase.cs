using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using lnbase.Base;
using System;
using ITW.Exts;
using System.Collections.Generic;

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
			this.HEIGHT = (int) ( this.WIDTH / RevertFactor(res) );
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
				name: "",
				text: "",
				behav: new GameScene.SceneBehaviour(GameBase.Scenes, 1)   // type each char at 150 speed
			);

			GameBase.NewScene(
				"first",
				name: "You",
				text: "Why am I here?",
				behav: new GameScene.SceneBehaviour(GameBase.Scenes, 15)
			);

			GameBase.NewScene(
				"second",
				name: "Girl A",
				text: "I-I... I don't know",
				behav: new GameScene.SceneBehaviour(GameBase.Scenes,
					updatepace: 15,
					upt: (int cycle, GameScene.SceneValues sv) => {     // each 50ms
						if( sv.VISIBLE.Length == sv.FullLength - 1 ) {
							return true;
						} else if( sv.VISIBLE.Length < 6 ) {
							if( (int) sv.MiscValues[0] % 5 == 0 )
								sv.SetVisible(sv.VISIBLE.Length + 1);
						} else {
							sv.SetVisible(sv.VISIBLE.Length + 1);
						}
						sv.MiscValues[0] = (int) sv.MiscValues[0] + 1;
						return false;
					}
				),
				generationFunc: (GameScene gs) => { gs.VALUES.MiscValues.Insert(0, 0); }
			// bckg: null // default
			);

			GameBase.NewScene(
				"third",
				name: "***",
				text: "1 2 3...",
				behav: new GameScene.SceneBehaviour(GameBase.Scenes,
					updatepace: 50,
					upt: (int cycle, GameScene.SceneValues sv) => {
						InputStates i = new InputStates( );
						if( cycle < 1 )
							sv.SetVisible(1);
						if( i.KeyUp(sv.BefInput, Keys.Right) )
							sv.SetVisible(sv.VISIBLE.Length + 1);
						sv.MiscValues[1] = (int) sv.MiscValues[1] + 1;
						if( (int) sv.MiscValues[1] % 10  == 0 )
							sv.SetVisible(sv.VISIBLE.Length + 1);
						if( sv.VISIBLE.Length == sv.FullLength - 1 )
							return true;
						return false;
					}
				),
				generationFunc: (GameScene gs) => { gs.VALUES.MiscValues.InsertRange(0, new List<object>( ) { 8, 0 }); }
			);

			GameBase.EndScene(
				name: "",
				text: "Its the end!",
				behav: new GameScene.SceneBehaviour(GameBase.Scenes)
			);

			GameBase.Scenes.First.Next(GameBase.Scenes["first"]);
			GameBase.Scenes["first"].Next(GameBase.Scenes["second"]);
			GameBase.Scenes["second"].Next(GameBase.Scenes["third"]);
			GameBase.Scenes["third"].Next(GameBase.Scenes.Last);

			GameBase.Scenes.First.Condition(GameSceneConditions.ClickOrEnter);
			GameBase.Scenes["first"].Condition(GameSceneConditions.ClickOrEnter);
			GameBase.Scenes["second"].Condition(GameSceneConditions.ClickOrEnter);
			GameBase.Scenes["third"].Condition(GameSceneConditions.ClickOrEnter);
			GameBase.Scenes.Last.Condition(GameSceneConditions.ClickOrEnter);

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
