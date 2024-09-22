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

        public ListItem CreateListGroup(string name, bool bold = false)
        {
            if (SelectChildById(name) != null) return SelectChildById(name) as ListItem;
            var g = new ListItem(name, bold: bold);
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
        public ListItem(string title, int childTabLength = 15, bool bold = false)
        {
            ChildTabLength = childTabLength;
            id = title;

            TitleBar = new RowContainer();
            TitleBar.Parent = this;

            ExpandButton = new UIText("JetBrainsMono-Regular", text: "▶", fontSize: 25, fontColor: Color.Black);

            ExpandButton.OnUpdate += delegate (object sender, EventArgs e)
            {
                if (!IsExpanded)
                {
                    if (ExpandButton._isHovering)
                        ExpandButton.Text = "▷";
                    else ExpandButton.Text = "▶";
                }
                else
                {
                    if (ExpandButton._isHovering)
                        ExpandButton.Text = "▽";
                    else ExpandButton.Text = "▼";
                }
                if (IsEmpty())
                    ExpandButton.Text = " ";
            };
            ExpandButton.OnClick += delegate (object sender, EventArgs e)
            {
                if (Visible) IsExpanded = !IsExpanded;
            };
            TitleBar.RegisterChild(ExpandButton);

            var separate = new SizeContainer(childTabLength, 1);
            TitleBar.RegisterChild(separate);

            if (!bold) Title = new UIText("JetBrainsMono-Regular", text: title, fontSize: 25, fontColor: Color.Black);
            else Title = new UIText("JetBrainsMono-Bold", text: title, fontSize: 25, fontColor: Color.Black);

            TitleBar.RegisterChild(Title);

            Children.Add(TitleBar);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var child in Children)
                if (child is ListItem) child.Visible = IsExpanded;

            base.Update(gameTime);

            if (Parent is not ListView && Parent is not ListItem)
                throw new Exception("This component can only be used as children of ListItem or ListView.");
        }

        public ListItem CreateListGroup(string name, bool bold = false)
        {
            if (SelectChildById(name) != null) return SelectChildById(name) as ListItem;
            var g = new ListItem(name, bold: bold);
            g.Parent = this;
            g.RelativePosition.X = ChildTabLength;
            Children.Add(g);
            return g;
        }

        public bool IsExpanded = false;

        public bool Selected = false;

        public bool HasChild => Children.Count > 0;

        public int ChildTabLength;

        public RowContainer TitleBar;

        public UIText Title;

        public UIText ExpandButton;

        public static ListItem SelectItem = null;

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!_init || !Visible) return;

            if (!string.IsNullOrEmpty(Text))
            {
                var x = Rectangle.X + Rectangle.Width / 2 - _font.MeasureString(Text).X / 2;
                var y = Rectangle.Y + Rectangle.Height / 2 - _font.MeasureString(Text).Y / 2;

                spriteBatch.DrawString(_font, Text, new(x, y), BackgroundColor * _alpha);
            }

            Title.Draw(spriteBatch, gameTime);
            ExpandButton.Draw(spriteBatch, gameTime);

            if (IsExpanded)
            {
                if (HasChild)
                    foreach (var item in Children)
                        if (item is ListItem) item.Draw(spriteBatch, gameTime);
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

        public bool IsEmpty()
        {
            return Children.Where(t => t is ListItem).Count() == 0;
        }
    }
}