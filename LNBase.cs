using Microsoft.Xna.Framework;
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
		private bool fullScreen = true;
		private bool borderLess = true;
		public int WIDTH { get => this.useRes[0]; private set { this.useRes[0] = value; } }
		public int HEIGHT { get => this.useRes[1]; private set { this.useRes[1] = value; } }
		public bool FULLSCREEN { get => this.fullScreen; private set { this.fullScreen = value; } }
		public bool BORDERLESS { get => this.borderLess; private set { this.borderLess = value; } }

		public Color BACKGROUND { get; set; }

		private bool FirstUpdate { get; set; }

		public void ChangeGameResolution(int? w, int? h, bool? FS, bool? BL) {
			string pl = "Multris#ChangeGameResolution";
			this.WIDTH = w ?? this.WIDTH;
			this.HEIGHT = h ?? this.HEIGHT;
			this.FULLSCREEN = FS ?? this.FULLSCREEN;
			this.BORDERLESS = BL ?? this.BORDERLESS;
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

			GameBase = new GameBase( );

			GameBase.FirstScene(
				name: "me",
				text: "Why am I here?",
				type: new GameBase.SceneType(150)   // type each char at 150 speed

			// bckg: null // default
			);

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

			// TODO: use this.Content to load your game content here

			GameBase.LoadContent(
				defBCKG: new SceneBackground(new GameSprite(txt: Content.Load<Texture2D>("background"))),
				defBARS: new SceneBars(
					txt: new GameSprite(txt: Content.Load<Texture2D>("bars")),
					name: new Rectangle?( ),
					text: new Rectangle?( )
				),
				defFONT: Content.Load<SpriteFont>("Fira")
			);

			GameBase.NewScene(
				"second",
				name: "You",
				text: "I don't know",
				type: new GameBase.SceneType(
					updatepace: 50,
					bef: () => { }, // before starting showing
					upt: (int cycle, SceneValues sv) => {     // each 50ms

						// return true = end of every scene
						Console.WriteLine($"Cycle #{cycle}");
						if( cycle >= 5 ) {
							sv.TextVisible(sv.FullText);
							return true;
						} else
							return false;
					},
					aft: () => { }  // after upt returns true. Before Condition wait!
				)
			// bckg: null // default
			);

			bool Click(InputStates bef) =>
				bef.MouseReleased(new InputStates( )).Button == MouseButton.LEFT
					? true : false;

			GameBase.EndScene(
				id: "end",
				name: "",
				text: "Its the end!",
				type: new GameBase.SceneType( )
			);

			GameBase.Scenes["end"].Next(GameBase.Terminate);

			GameBase.Scenes["second"].Next(
				GameBase.Scenes["end"]
			);

			GameBase.Scenes.First.Next(
				GameBase.Scenes["second"]
			);

			GameBase.Scenes["second"].Condition(Click);
			GameBase.Scenes.First.Condition(Click);

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
				GameBase.Start( );
				FirstUpdate = true;
			}

			GameBase.Update(inputs);

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

			GameBase.Draw(spriteBatch);

			spriteBatch.End( );

			base.Draw(gameTime);
		}
	}
}
