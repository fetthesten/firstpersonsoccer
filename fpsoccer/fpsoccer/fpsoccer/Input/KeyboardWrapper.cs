using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using fpsoccer.Interfaces;

namespace fpsoccer.Input
{
    public class KeyboardWrapper: ICanUpdate
    {
        public KeyboardState State { get; private set; }
        public InvertOptions InvertOptions { get; set; }

        public KeyboardWrapper()
        {
            // TODO: expose
            InvertOptions = new InvertOptions();
        }

        public void Update(GameTime time)
        {
            State = Keyboard.GetState();
        }
    }
}
