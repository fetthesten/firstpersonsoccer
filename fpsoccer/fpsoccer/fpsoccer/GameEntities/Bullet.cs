using Microsoft.Xna.Framework;
using fpsoccer.Interfaces;

namespace fpsoccer.GameEntities
{
    public class Bullet: ICanUpdate
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public float Speed { get; set; }

        public void Update(GameTime time)
        {
            // TODO: update position
        }
    }
}
