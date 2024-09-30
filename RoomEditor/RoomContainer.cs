using Mantodea.Content.Components;
using Mantodea.Content.Rooms;
using Mantodea.Extensions;
using Mantodea.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mantodea.Content.Components
{
    public class RoomContainer : SizeContainer
    {
        public Room Room;

        public RoomContainer(Room room, RoomData roomData)
        {
            Room = room;
            Room.SetDefaults();
            Room.LoadData(roomData);
        }

        public override int Width => (int)Room.RealSize.X;

        public override int Height => (int)Room.RealSize.Y;

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            spriteBatch.Change(sortMode: SpriteSortMode.BackToFront);
            Room.EditorDraw(spriteBatch, gameTime);
            spriteBatch.Change(sortMode: SpriteSortMode.Deferred);
            for (int i = 52; i < Room.RealSize.X; i += 52)
                spriteBatch.DrawLine(new Line(new Vector2(i, 0) + Position, new Vector2(i, Room.RealSize.Y) + Position), Color.Gray * 0.5f);
            for (int i = 52; i < Room.RealSize.Y; i += 52)
                spriteBatch.DrawLine(new Line(new Vector2(0, i) + Position, new Vector2(Room.RealSize.X, i) + Position), Color.Gray * 0.5f);
            for (int i = 0; i < (int)Room.TileMapSize.X; i++)
            {
                for (int j = 0; j < (int)Room.TileMapSize.Y; j++)
                {
                    if (!Reachable(i, j))
                        spriteBatch.Draw(Main.TextureManager[TexType.Tile, "collision"], new Vector2(i, j) * Main.TileSize + Position, Color.White * 0.3f);
                }
            }
            spriteBatch.Rebegin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Room.View = Parent.View;
            Room.Position = Position;
            Room.EditorUpdate(gameTime);
        }

        public void LoadData(RoomData roomData) => Room.LoadData(roomData);

        public bool Reachable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= (int)Room.TileMapSize.X || y >= (int)Room.TileMapSize.Y)
                return false;
            return Room[x, y] == 0;
        }
    }
}
