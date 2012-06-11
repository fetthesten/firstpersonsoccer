using Microsoft.Xna.Framework.Input;

namespace fpsoccer.InputWrappers
{
    public class KeyboardWrapper: IUpdatable
    {
        public KeyboardState State { get; private set; }
        public InvertOptions InvertOptions { get; set; }

        public KeyboardWrapper()
        {
            InvertOptions = new InvertOptions();
        }

        public void Update()
        {
            State = Keyboard.GetState();
        }
    }
}
