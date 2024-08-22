using Microsoft.Xna.Framework;
using System;

namespace Mantodea.Content.Components
{
    public class ColumnContainer : Container
    {
        public ColumnContainer() { }

        public int ChildrenMargin = 0;

        public override void Update(GameTime gameTime)
        {
            _height = 0;

            foreach (var component in Children)
                if (component.Visible)
                    _width = Math.Max(component.Width + (int)component.RelativePosition.X, Width);
            foreach (var component in Children)
            {
                if (!component.Visible)
                    continue;
                component.RelativePosition.Y = _height;
                _height += component.Height + ChildrenMargin;
                component.Update(gameTime);
            }
            base.Update(gameTime);
            if (!_init) _init = true;
        }
    }
}
