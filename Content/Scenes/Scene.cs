using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Mantodea.Extensions;
using Mantodea.Content.Components;

namespace Mantodea.Content.Scenes
{
    public class Scene : GameContent
    {
        public Main Instace => Main.Instance;

        public GraphicsDevice Device => Instace.GraphicsDevice;

        public Texture2D Background = null;

        public Color BackgroundColor = default;

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Background != null)
                spriteBatch.Draw(Background, new Vector2(0, 0), Color.White);
            else if (BackgroundColor != default)
                spriteBatch.DrawRectangle(new(0, 0, Main.GameWidth, Main.GameHeight), BackgroundColor);
            foreach (var component in Components)
                component.Draw(spriteBatch, gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in Components)
                component.Update(gameTime);
        }

        public List<Component> Components { get; set; } = new List<Component>();
    }
}
