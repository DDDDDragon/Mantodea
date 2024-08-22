using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mantodea.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mantodea.Content.Components
{
    public class ListView : ColumnContainer
    {
        public ListView(int childTabLength = 0)
        {
            ChildTabLength = childTabLength;
        }

        public ListItem CreateListGroup(string name)
        {
            if(SelectChildById(name) != null) return SelectChildById(name) as ListItem;
            var g = new ListItem(name);
            g.Parent = this;
            g.RelativePosition.X = ChildTabLength;
            Children.Add(g);
            return g;
        }

        public int ChildTabLength; 
        
        public override void RegisterChild(Component component)
        {
            if (component is not ListItem)
                throw new Exception("This component can only have ListItem as child.");
            if (SelectChildById(component.id) != null && component.id != "")
                return;
            if (component.id == "")
                throw new Exception("This component's child must have id.");
            component.Parent = this;
            Children.Add(component);
        }

        public override void RegisterChildAt(int index, Component component, bool behind = false)
        {
            if (component is not ListItem)
                throw new Exception("This component can only have ListItem as child.");
            if (SelectChildById(component.id) != null)
                return;
            if (component.id == "")
                throw new Exception("This component's child must have id.");
            component.Parent = this;
            if (behind)
                Children.Insert(Children.Count - index, component);
            else
                Children.Insert(index, component);
        }
    }

    public class ListItem : ColumnContainer
    {
        public ListItem(string title, int childTabLength = 20)
        {
            ChildTabLength = childTabLength;
            id = title;
            Title = new UIText("MiSans-Regular", text: title, fontSize: 25, fontColor: Color.Black);
            Title.Parent = this;
            Title.RelativePosition.X = childTabLength;
            Children.Add(Title);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Clicked)
            {
                IsExpanded = !IsExpanded;
            }

            foreach (var child in Children)
                if (child is ListItem) child.Visible = IsExpanded;

            if (Parent is not ListView && Parent is not ListItem)
                throw new Exception("This component can only be used as children of ListItem or ListView.");
        }

        public ListItem CreateListGroup(string name)
        {
            if (SelectChildById(name) != null) return SelectChildById(name) as ListItem;
            var g = new ListItem(name);
            g.Parent = this;
            g.RelativePosition.X = ChildTabLength;
            Children.Add(g);
            return g;
        }

        public bool IsExpanded = false;

        public bool HasChild => Children.Count > 0;

        public int ChildTabLength;

        public UIText Title;

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!_init || !Visible) return;
            BackgroundColor = Color.Green;
            if (BackgroundColor != default && _isHovering)
                spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, Width, Height), BackgroundColor * _alpha);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = Rectangle.X + Rectangle.Width / 2 - _font.MeasureString(Text).X / 2;
                var y = Rectangle.Y + Rectangle.Height / 2 - _font.MeasureString(Text).Y / 2;

                spriteBatch.DrawString(_font, Text, new(x, y), BackgroundColor * _alpha);
            }

            Title.Draw(spriteBatch, gameTime);

            if (IsExpanded)
            {
                if(HasChild)
                    foreach (var item in Children)
                        if(item is ListItem) item.Draw(spriteBatch, gameTime);
            }
        }

        public override void RegisterChild(Component component)
        {
            if (component is not ListItem)
                throw new Exception("This component can only have ListItem as child.");
            if (SelectChildById(component.id) != null && component.id != "")
                return;
            if (component.id == "")
                throw new Exception("This component's child must have id.");
            component.Parent = this;
            Children.Add(component);
        }

        public override void RegisterChildAt(int index, Component component, bool behind = false)
        {
            if (component is not ListItem)
                throw new Exception("This component can only have ListItem as child.");
            if (SelectChildById(component.id) != null)
                return; 
            if (component.id == "")
                throw new Exception("This component's child must have id.");
            component.Parent = this;
            if (behind)
                Children.Insert(Children.Count - index, component);
            else
                Children.Insert(index, component);
        }
    }
}
