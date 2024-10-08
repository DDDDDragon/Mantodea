﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mantodea.Managers;
using Mantodea.Extensions;
using System.Linq;

namespace Mantodea.Content.Tiles
{
    public abstract class Tile : Entity
    {
        public virtual Texture2D Texture { get; set; }

        public Vector2 TileSize;

        public int SubID 
        { 
            get {
                return _subID;
            }
            set {
                _subID = value;
                SetSubTexture(value);
            } 
        }

        public int _subID;

        public override int Width => Texture.Width;

        public override int Height => Texture.Height;

        public override float ZIndex => 1 - TilePosition.Y / 1000;

        public override Vector2 Position 
        { 
            get => (TilePosition + CurrentRoom.TilePostion) * Main.TileSize; 
        }

        public override Rectangle TextureRectangle
        {
            get
            {
                var t = Position + DrawOffset + TextureOffset;
                return new Rectangle(new Vector2(t.X, t.Y).ToPoint(), Texture.GetSize().ToPoint());
            }
        }

        public Vector2 TextureOffset;

        public virtual void SetSubTexture(int subID)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Position + DrawOffset + TextureOffset, null, Color.White * Alpha, 0, Vector2.Zero, 1f, SpriteEffects.None, ZIndex);
        }

        public override void EditorPreDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsHovering || Equals(SelectEntity))
            {
                spriteBatch.Draw(SpriteBatchExt.pixel, TextureRectangle, null, Color.Green * 0.4f, 0, Vector2.Zero, SpriteEffects.None, ZIndex + 0.0002f);
            }
            if (IsHovering)
                DrawBorder(spriteBatch, Color.Red * Alpha);
            if (Equals(SelectEntity))
                DrawBorder(spriteBatch, Color.Red * Alpha * 0.4f);
        }

        public override void EditorDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Position + DrawOffset + TextureOffset, null, Color.White * Alpha, 0, Vector2.Zero, 1f, SpriteEffects.None, ZIndex);
        }

        public override void EditorPostDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsHovering || Equals(SelectEntity))
            {
                spriteBatch.Draw(SpriteBatchExt.pixel, new(Position.ToPoint(), (TileSize * 52).ToPoint()), null, Color.Yellow * 0.4f, 0, Vector2.Zero, SpriteEffects.None, ZIndex - 0.0001f);
            }
        }

        public virtual void DrawBorder(SpriteBatch spriteBatch, Color? borderColor = null)
        {
            Color[] data = new Color[Texture.Width * Texture.Height];
            Texture.GetData(data);
            data = (from c in data select c.A == 0 ? c : Color.White).ToArray();
            Texture2D t = new Texture2D(spriteBatch.GraphicsDevice, Texture.Width, Texture.Height);
            t.SetData(data);

            Color borderC = borderColor == null ? Color.White : (Color)borderColor;

            spriteBatch.Draw(t, Position + DrawOffset + TextureOffset - new Vector2(0, 2), null, borderC, 0, Vector2.Zero, 1f, SpriteEffects.None, ZIndex + 0.0001f);
            spriteBatch.Draw(t, Position + DrawOffset + TextureOffset - new Vector2(2, 0), null, borderC, 0, Vector2.Zero, 1f, SpriteEffects.None, ZIndex + 0.0001f);
            spriteBatch.Draw(t, Position + DrawOffset + TextureOffset + new Vector2(0, 2), null, borderC, 0, Vector2.Zero, 1f, SpriteEffects.None, ZIndex + 0.0001f);
            spriteBatch.Draw(t, Position + DrawOffset + TextureOffset + new Vector2(2, 0), null, borderC, 0, Vector2.Zero, 1f, SpriteEffects.None, ZIndex + 0.0001f);
        }
    }
}
