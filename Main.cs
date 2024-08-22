using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mantodea.Managers;
using System;
using Mantodea.Content.Scenes;

namespace Mantodea
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static int GameHeight => Instance._graphics.PreferredBackBufferHeight;
        public static int GameWidth => Instance._graphics.PreferredBackBufferWidth;

        public static Vector2 GameSize => new(GameWidth, GameHeight);

        internal static string GamePath => Environment.CurrentDirectory; 
        
        public static int TileSize = 52;

        public static Main Instance;

        public static GraphicsDeviceManager Graphics;

        public static TextureManager TextureManager;
        public static FontManager FontManager;
        public static LocalizationManager LocalizationManager;

        public static Matrix? CurrentProjection = null;

        public static Matrix? CurrentView = null;

        public Scene CurrentScene;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1200,
            };
            Graphics = _graphics;
            Content.RootDirectory = "Content";
            IsMouseVisible = true; 
            Instance = this;

            TextureManager = new TextureManager();
            FontManager = new FontManager();
            LocalizationManager = new LocalizationManager();
        }

        protected override void Initialize()
        {
            TextureManager.Load();
            FontManager.Load();
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            CurrentScene?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone);

            CurrentScene?.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
