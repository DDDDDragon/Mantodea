using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mantodea.Managers;
using System;
using Mantodea.Content.Scenes;
using Mantodea.Content.Players;
using Mantodea.Content.Components;
using Mantodea.Content;
using Mantodea.Mods;
using System.IO;

namespace Mantodea
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static int GameHeight => Instance._graphics.PreferredBackBufferHeight;
        public static int GameWidth => Instance._graphics.PreferredBackBufferWidth;

        public static Vector2 GameSize => new(GameWidth, GameHeight);

        public static string GamePath => Environment.CurrentDirectory;

        public static string ModPath => Path.Combine(GamePath, "Mod");

        public static string ModSourcePath => Path.Combine(GamePath, "ModSource");

        public static int TileSize = 52;

        public static string CurrentLanguage;

        public static string CursorType;

        public static Main Instance;

        public static Random Random;

        public static GraphicsDeviceManager Graphics;

        public static SpriteBatch SpriteBatch;

        public static TextureManager TextureManager;

        public static FontManager FontManager;

        public static LocalizationManager LocalizationManager;

        public static ModLoader ModLoader;

        public static Matrix? CurrentProjection = null;

        public static Matrix? CurrentView = null;

        public static Player LocalPlayer;

        public static Scene CurrentScene;

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

            Random = new Random();

            TextureManager = new TextureManager();
            FontManager = new FontManager();
            LocalizationManager = new LocalizationManager();

            CurrentLanguage = "en_US";
            CursorType = "";
            LocalPlayer = null;
        }

        protected override void Initialize()
        {
            TextureManager.Load();
            FontManager.Load();
            LocalizationManager.Load();
            UserInput.Initialize();
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SpriteBatch = _spriteBatch;
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Reset();

            UserInput.Update();

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

        public static string GetText(string key, string language = "")
        {
            return language == ""
                ? LocalizationManager[CurrentLanguage, key]
                : LocalizationManager[language, key];
        }

        public static void SetCursor(string texID)
        {
            var cursor = MouseCursor.FromTexture2D(TextureManager[TexType.UI, texID], 0, 0);
            Mouse.SetCursor(cursor);
            CursorType = texID;
        }

        public virtual void Reset()
        {
            Entity.HoverEntity = null;
        }
    }
}
