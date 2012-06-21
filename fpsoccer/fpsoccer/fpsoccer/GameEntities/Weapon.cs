using System;
using Microsoft.Xna.Framework;
using fpsoccer.Interfaces;

namespace fpsoccer.GameEntities
{
    public class Weapon
    {
        public float ShotsPerSecond { get; set; }

        private TimeSpan _timeLastShotFired = new TimeSpan();

        public Weapon()
        {
            ShotsPerSecond = 3.0f;
        }

        public bool TryShoot(GameTime gameTime)
        {
            var canShoot = gameTime.TotalGameTime - _timeLastShotFired > TimePerShot();
            if (canShoot)
            {
                _timeLastShotFired = gameTime.TotalGameTime;
                return true;
            }

            return false;
        }

        private TimeSpan TimePerShot()
        {
            var ticksPerShot = new TimeSpan(0, 0, 0, 1).Ticks / ShotsPerSecond;
            return new TimeSpan((long)ticksPerShot);
        }
    }
}
