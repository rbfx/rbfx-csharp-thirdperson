using System;
using Urho3DNet;

namespace RbfxTemplate
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Launcher.Run(_ => new UrhoApplication(_));
        }
    }
}
