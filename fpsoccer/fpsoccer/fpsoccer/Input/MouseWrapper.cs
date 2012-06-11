using Microsoft.Xna.Framework.Input;

namespace fpsoccer.InputWrappers
{
    public class MouseWrapper: IUpdatable
    {
        public MouseState State { get; private set; }
        public InvertOptions InvertOptions { get; set; }

        public MouseWrapper()
        {
            InvertOptions = new InvertOptions();
        }

        public void Update()
        {
            State = Mouse.GetState();
        }
    }
}
