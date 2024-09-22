using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mantodea.Content.Components
{
    public class Timer : Component
    {
        public Timer(EventHandler<GameTime> onUpdate = null)
        {
            timer = new float[10];
            OnUpdate = onUpdate;
        }

        public float[] timer;

        public override void Update(GameTime gameTime)
        {
            OnUpdate?.Invoke(this, gameTime);
        }

        public override void LeftClick(object sender, int pressTime, Vector2 mouseStart)
        {
            
        }

        public override void RightClick(object sender, int pressTime, Vector2 mouseStart)
        {

        }

        public EventHandler<GameTime> OnUpdate;

        public float this[int index]
        {
            get => timer[index];
            set => timer[index] = value;
        }
    }
}
