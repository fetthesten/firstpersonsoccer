using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using fpsoccer.Input;
using fpsoccer.Interfaces;

namespace fpsoccer.GameEntities
{
    public class Player: ICanUpdate, ICanExit
    {
        // input
        public KeyboardWrapper Keyboard { get; private set; }
        public MouseWrapper Mouse { get; private set; }
        private Weapon _weapon = new Weapon();

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

        public bool HasFiredShot()
        {
            return _hasFiredShot;
        }

        private void Shoot(GameTime time)
        {
            _hasFiredShot = _weapon.TryShoot(time);

        }
    }
}
