using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using fpsoccer.Interfaces;

namespace fpsoccer.Input
{
    public class MouseWrapper: ICanUpdate
    {
        public MouseState State { get; private set; }
        public InvertOptions InvertOptions { get; set; }

        public MouseWrapper()
        {
            // TODO: expose
            InvertOptions = new InvertOptions();
        }

        public void Update(GameTime time)
        {
            State = Mouse.GetState();
        }
    }
}
