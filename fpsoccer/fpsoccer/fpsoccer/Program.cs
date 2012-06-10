using System;

namespace fpsoccer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (fpsGame game = new fpsGame())
            {
                game.Run();
            }
        }
    }
#endif
}

