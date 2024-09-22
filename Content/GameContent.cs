using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Mantodea.Content
{
    public abstract class GameContent
    {
        public virtual string Name { get; set; }

        public virtual void PreDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }

        public virtual void PostDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }

        public virtual void EditorPreDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }

        public virtual void EditorDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }

        public virtual void EditorPostDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void EditorUpdate(GameTime gameTime)
        {

        }

        public virtual void SetDefaults()
        {

        }

        public virtual void SetStaticDefaults()
        {

        }
    }
}
