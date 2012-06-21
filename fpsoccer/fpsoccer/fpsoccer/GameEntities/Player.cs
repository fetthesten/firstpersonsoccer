using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using fpsoccer.Events;
using fpsoccer.Input;
using fpsoccer.Interfaces;

namespace fpsoccer.GameEntities
{
    public class Player: ICanUpdate, ICanExit
    {
        // events
        public PlayerEvents.PlayerEvent OnFiredShot;

        // input
        public KeyboardWrapper Keyboard { get; private set; }
        public MouseWrapper Mouse { get; private set; }
        private readonly Weapon _weapon = new Weapon();

        private bool _requestedExit = false;
        private bool _hasFiredShot = false;

        public Player()
        {
            Keyboard = new KeyboardWrapper();
            Mouse = new MouseWrapper();
        }

        public void Update(GameTime time)
        {
            Keyboard.Update(time);

            if (Keyboard.State.IsKeyDown(Keys.Escape))
                _requestedExit = true;

            Mouse.Update(time);

            if (Mouse.State.LeftButton == ButtonState.Pressed)
                Shoot(time);
        }

        public bool HasRequestedExit()
        {
            return _requestedExit;
        }

        private void Shoot(GameTime time)
        {
            if (_weapon.TryShoot(time))
                OnFiredShot(this);
        }
    }
}
