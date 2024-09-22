using Mantodea.Content.Rooms;
using Mantodea.Content.Components;
using Microsoft.Xna.Framework;

namespace Mantodea.Content.Players
{
    public class Player : Entity
    {
        public override void SetDefaults()
        {
            Timer = new Timer();

            CurrentPath = new();

            Direction = -1;

            Rotation = 0;

            base.SetDefaults();
        }

        public void GoToRoom(Room room, Vector2 pos)
        {
            CurrentRoom = room;

            room.LocalPlayer = this;

            room.RegisterEntity(this, pos);

            SetPos(pos);
        }

        public Timer Timer;
    }
}
