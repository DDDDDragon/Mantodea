using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mantodea.Extensions;
using System;

namespace Mantodea.Content.Components
{
    public class ScrollableContainer : SizeContainer
    {
        public ScrollableContainer() { }

        public ScrollableContainer(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public ScrollableContainer(Vector2 size)
        {
            _width = (int)size.X;
            _height = (int)size.Y;
        }

        public Vector2 InnerPos = new();

        public int MaxHeight = 0;

        public int MaxWidth = 0;

        public bool ShowVerticalScrollBar = false;

        public bool ShowHorizontalScrollBar = false;

        public override void Draw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!_init || !Visible) return;
            if (BackgroundColor != default)
                spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, Width, Height), BackgroundColor * _alpha);

            spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, BorderWidth.X, _height), BorderColor);
            spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, _width, BorderWidth.Y), BorderColor);
            spriteBatch.DrawRectangle(new(_width - BorderWidth.Z + (int)Position.X, (int)Position.Y, BorderWidth.Z, Height), BorderColor);
            spriteBatch.DrawRectangle(new((int)Position.X, _height - BorderWidth.W + (int)Position.Y, _width, BorderWidth.W), BorderColor);

            spriteBatch.Rebegin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone);
            spriteBatch.EnableScissor();
            spriteBatch.GraphicsDevice.ScissorRectangle = Rectangle;

            foreach (var component in Children)
                component.Draw(spriteBatch, gameTime);

            spriteBatch.Rebegin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone);
            spriteBatch.GraphicsDevice.ScissorRectangle = new(Point.Zero, Main.GameSize.ToPoint());
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ShowVerticalScrollBar = false;
            ShowHorizontalScrollBar = false;

            var deltaWheel = UserInput.GetDeltaWheelValue();

            MaxHeight = Height;

            MaxWidth = Width;

            foreach(var child in Children)
            {
                MaxHeight = Math.Max((int)(child.RelativePosition.Y + child.Height), MaxHeight);
                MaxWidth = Math.Max((int)(child.RelativePosition.X + child.Width), MaxWidth);
            }

            if(_isHovering)
            {
                if (InnerPos.Y + Height < MaxHeight)
                {
                    ShowVerticalScrollBar = true;
                    if (deltaWheel < 0)
                        InnerPos.Y += MaxHeight / 20;
                }

                if (InnerPos.Y > 0)
                {
                    ShowHorizontalScrollBar = true;
                    if (deltaWheel > 0)
                        InnerPos.Y -= MaxHeight / 20;
                }
            }

            if (InnerPos.Y + Height > MaxHeight)
                InnerPos.Y = MaxHeight - Height;
            if (InnerPos.Y < 0)
                InnerPos.Y = 0;

            foreach (var child in Children)
            {
                child.Position = child.RelativePosition - InnerPos + Position;
                child.Update(gameTime);
            }

            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].shouldCollect)
                {
                    Children.RemoveAt(i);
                    i--;
                }
            }

            var mouseRect = UserInput.GetMouseRectangle();

            _isHovering = false;

            if (mouseRect.Intersects(Rectangle))
            {
                _isHovering = true;
            }

            if (!_init) _init = true;

            if (HorizontalMiddle) RelativePosition.X = (Parent.Width - Width) / 2;
            if (VerticalMiddle) RelativePosition.Y = (Parent.Height - Height) / 2;

            switch (Anchor)
            {
                case Anchor.Left:
                    RelativePosition.X = 0;
                    break;
                case Anchor.Right:
                    RelativePosition.X = Parent.Width - Width;
                    break;
                case Anchor.Top:
                    RelativePosition.Y = 0;
                    break;
                case Anchor.Bottom:
                    RelativePosition.Y = Parent.Height - Height;
                    break;
                case Anchor.None:
                    break;
            }

            DrawOffset = new(0, 0);

            if (!_init) _init = true;
        }
    }
}
