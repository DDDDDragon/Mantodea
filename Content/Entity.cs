using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Mantodea.Content.Rooms;
using Mantodea.Content.Components;

namespace Mantodea.Content
{
    public abstract class Entity : GameContent
    {
        public List<Vector2> CurrentPath;

        public virtual Vector2 Position { get; set; }

        public virtual Vector2 TilePosition { get; set; }

        public virtual Vector2 LastTilePos { get; set; }
        
        public virtual Vector2 TargetTilePos { get; set; }

        public virtual float ZIndex { get; set; }

        public Vector2 DrawOffset;

        public virtual Room CurrentRoom { get; set; }

        /// <summary>
        /// 一定要改啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊
        /// </summary>
        public ListItem listItem { get; set; }

        public float Rotation;

        public float Alpha = 1;

        public bool IsHovering;

        public string Mod = "";

        public string TexturePath = "";

        public bool CanClick = true;

        public bool IsMove = false;

        public bool Clicked { get; internal set; }

        public bool UseAdditive = false;

        internal int _width;

        internal int _height;

        public int Direction;

        public virtual int Width => _width;

        public virtual int Height => _height;

        public virtual string ID { get; }

        public Animations.Animation CurrentAnimation = Animations.Animation.Empty;

        internal MouseState _currentMouse;

        internal MouseState _previousMouse; 

        public static Entity SelectEntity = null;

        public static Entity HoverEntity = null;

        public virtual Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        public virtual Rectangle TextureRectangle => Rectangle;

        public event EventHandler OnClick;

        public Entity()
        {

        }

        public Entity(Room room)
        {
            CurrentRoom = room;
        }

        public void SetPos(Vector2 pos)
        {
            LastTilePos = pos;

            TilePosition = pos;

            TargetTilePos = pos;
        }

        public void SetPos(int x, int y) => SetPos(new Vector2(x, y));

        public override void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRect = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            if (CurrentAnimation != null && CurrentAnimation.MaxTime == 0) CurrentAnimation = Animations.Animation.Empty;
            CurrentAnimation?.Update(gameTime);

            IsHovering = false;

            if (mouseRect.Intersects(Rectangle))
            {
                IsHovering = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed && CanClick)
                {
                    Clicked = true;
                    OnClick?.Invoke(this, new EventArgs());
                }
            }
        }

        public override void EditorUpdate(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRect = new Rectangle(_currentMouse.Position, new(1, 1));

            IsHovering = false;

            var realpos = TextureRectangle.Location.ToVector2() * CurrentRoom.View.M11 + new Vector2(CurrentRoom.View.M41, CurrentRoom.View.M42);

            var realRect = new Rectangle(realpos.ToPoint(), (TextureRectangle.Size.ToVector2() * CurrentRoom.View.M11).ToPoint());

            if (realRect.Intersects(mouseRect))
            {
                if (HoverEntity == null)
                    HoverEntity = this;
                else if (HoverEntity.TilePosition.Y < TilePosition.Y)
                    HoverEntity = this;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    SelectEntity = this;
                    ListItem.SelectItem = listItem;

                    (listItem.Parent as ListItem).IsExpanded = true;
                }
            }

            base.Update(gameTime);
        }

        public bool PlayAnimation(Animations.Animation animation)
        {
            if (CurrentAnimation.MaxTime != 0)
                return false;

            CurrentAnimation = animation;
            CurrentAnimation.Target = this;
            return true;
        }
    }

    public class EntityData
    {
        public string Name;

        public string Mod;

        public string Type;

        public string TexturePath;

        public Vector2 Position;

        public Vector2 DrawOffset;
    }
}
