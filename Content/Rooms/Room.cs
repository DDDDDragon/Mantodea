using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mantodea.Content.Components;
using Mantodea.Content.NPCs;
using Mantodea.Content.Players;
using Mantodea.Content.Tiles;
using Mantodea.Extensions;
using System.Collections.Generic;
using Mantodea.Managers;
using System.Linq;

namespace Mantodea.Content.Rooms
{
    public class Room : GameContent
    {
        public List<Entity> Entities;

        public List<NPC> NPCs;

        public Rectangle Rectangle => new((int)Position.X, (int)Position.Y, (int)RealSize.X, (int)RealSize.Y);

        public Rectangle TileRectangle => Rectangle.Divide(Main.TileSize);

        public Vector2 TileMapSize => Background.GetSize() / Main.TileSize;

        public Vector2 RealSize => Background.GetSize();

        public Vector2 TilePostion => Position / Main.TileSize;

        public Vector2 Position;

        public Player LocalPlayer;

        public bool inHouse;

        public Timer Timer;

        public Room LastRoom;

        public Texture2D Background;

        public string BackgroundPath;

        public string Mod;

        public int[,] TileMap;

        public Matrix View;

        public GameContent Handler;

        public static Room Empty => new EmptyRoom();

        public override string Name => GetType().Name;

        public override void SetDefaults()
        {
            Entities = new();

            NPCs = new();

            Timer = new();

            base.SetDefaults();
        }

        public virtual void DrawBack(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Background, Position, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1);
        }

        public virtual void DrawAlphaBlend(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach(var entity in Entities)
            {
                if (entity is Tile && !entity.UseAdditive)
                {
                    entity.PreDraw(spriteBatch, gameTime);
                    entity.Draw(spriteBatch, gameTime);
                    entity.PostDraw(spriteBatch, gameTime);
                }
            }
        }

        public virtual void DrawAdditive(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var entity in Entities)
            {
                if (entity is Tile && entity.UseAdditive)
                {
                    entity.PreDraw(spriteBatch, gameTime);
                    entity.Draw(spriteBatch, gameTime);
                    entity.PostDraw(spriteBatch, gameTime);
                }
            }
        }

        public virtual void DrawPlayer(SpriteBatch spriteBatch, GameTime gameTime)
        {
            LocalPlayer?.Draw(spriteBatch, gameTime);
        }

        public virtual void EditorDrawBack(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Background, Position, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1);
        }

        public virtual void EditorDrawAlphaBlend(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var entity in Entities)
            {
                if (entity is Tile && !entity.UseAdditive)
                {
                    entity.EditorPreDraw(spriteBatch, gameTime);
                    entity.EditorDraw(spriteBatch, gameTime);
                    entity.EditorPostDraw(spriteBatch, gameTime);
                }
            }
        }

        public virtual void EditorDrawAdditive(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var entity in Entities)
            {
                if (entity is Tile && entity.UseAdditive)
                {
                    entity.EditorPreDraw(spriteBatch, gameTime);
                    entity.EditorDraw(spriteBatch, gameTime);
                    entity.EditorPostDraw(spriteBatch, gameTime);
                }
            }
        }

        public virtual void EditorDrawPlayer(SpriteBatch spriteBatch, GameTime gameTime)
        {
            LocalPlayer?.EditorDraw(spriteBatch, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawBack(spriteBatch, gameTime);

            DrawAlphaBlend(spriteBatch, gameTime);

            DrawPlayer(spriteBatch, gameTime);

            DrawAdditive(spriteBatch, gameTime);
        }

        public override void EditorDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            EditorDrawBack(spriteBatch, gameTime);

            EditorDrawAlphaBlend(spriteBatch, gameTime);

            EditorDrawPlayer(spriteBatch, gameTime);

            EditorDrawAdditive(spriteBatch, gameTime);
        }

        public override void EditorUpdate(GameTime gameTime)
        {
            foreach (var entity in Entities)
                if (entity is not Player) entity.EditorUpdate(gameTime);
            if (Entity.HoverEntity != null) Entity.HoverEntity.IsHovering = true;
        }

        public virtual void UpdatePlayer(GameTime gameTime)
        {

        }

        public int this[int x, int y]
        {
            get => TileMap[y, x];
            set => TileMap[y, x] = value;
        }

        public void RegisterEntity(Entity entity, Vector2 position = default)
        {
            entity.CurrentRoom = this;

            entity.SetPos(position);

            Entities.Add(entity);
        }

        public void RemoveEntity(Entity entity) => Entities.Remove(entity);

        public virtual RoomData ToData()
        {
            return new RoomData();
        }

        public virtual void LoadData(RoomData data)
        {
            
        }

        public virtual void Enter(Room lastRoom)
        {

        }
    }

    public class EmptyRoom : Room 
    {
        public override void Update(GameTime gameTime) { }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }
    }

    public class RoomData
    {
        public RoomData() { }

        public RoomData(int width, int height)
        {
            Entities = new();
            Width = width;
            Height = height;
            Reachable = new int[height, width];
        }

        public RoomData(Vector2 roomSize) : this((int)roomSize.X, (int)roomSize.Y) { }

        public int Width;

        public int Height;

        public string Name;

        public string BackgroundPath;

        public string Mod;

        public int[,] Reachable;

        public List<EntityData> Entities;
    }
}
