using SFML.Graphics;
using SFML.Window;
using SFML.System;
using NESharpLib;
using NESharpLib.Modules.PPU;
using System.Timers;
using System.Diagnostics;

namespace NESharp
{
    record struct ProgramState
    {
        public float GameScale = 3.0f;
        public string WindowTitle { get; set; }
        public bool ShouldClose { get; set; }
        public bool HasNewFrameData = false;
        public readonly Texture GameScreenTexture = new Texture(new Vector2u(256, 240));
        public Sprite GameScreenSprite { get; }
        public Image GameScreen = new Image(new Vector2u(256, 240));
        public ProgramState()
        {
            WindowTitle = "NESharp";
            ShouldClose = false;
            GameScreenTexture.Update(GameScreen);
            GameScreenSprite = new Sprite(GameScreenTexture)
            {
                Scale = new Vector2f(GameScale, GameScale)
            };
        }
    }
    class Application
    {
        const uint RENDER_WIDTH = 256;
        const uint RENDER_HEIGHT = 240;
        public ProgramState State = new ProgramState();
        RenderWindow MainWindow;
        string[] _args;
        public EventLoop eventLoop = new EventLoop();
        public NES console;
        public Thread ConsoleThread;
        public Application(string[] args)
        {
            _args = args;
            if (args.Length == 0) throw new Exception("No path provided.");
            MainWindow = new RenderWindow(new VideoMode(new Vector2u(RENDER_WIDTH * (uint)State.GameScale, RENDER_HEIGHT * (uint)State.GameScale)), State.WindowTitle, Styles.Close, SFML.Window.State.Windowed);
            MainWindow.Closed += MainWindowClose;
            MainWindow.Resized += MainWindowResize;
            MainWindow.SetVerticalSyncEnabled(true);
            Joystick.Update();
            console = new NES(_args[0]);
            console.PPU.OnFrameReady += FrameReadyHandler;
            ConsoleThread = new Thread(() =>
            {
                int fp = 1000 / 60;
                Stopwatch s0 = new Stopwatch();
                while (!State.ShouldClose)
                {
                    s0.Restart();
                    console.PPU.RenderFrame();
                    s0.Stop();
                    int fp2 = Math.Max((int)s0.ElapsedMilliseconds - fp, 0);
                    Thread.Sleep(fp2);
                }
            });
        }
        public void FrameReadyHandler(object? sender, Image frame)
        {
            State.GameScreenTexture.Update(frame);
        }
        public void MainWindowClose(object? sender, EventArgs e)
        {
            State.ShouldClose = true;
            MainWindow.Close();
        }
        public void MainWindowResize(object? sender, EventArgs e)
        {
            RenderWindow();
        }
        public void RenderWindow()
        {
            MainWindow.Clear();
            MainWindow.Draw(State.GameScreenSprite);
            MainWindow.DispatchEvents();
            MainWindow.Display();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Application app = new Application(args);
            app.ConsoleThread.Start();
            app.RenderWindow();
            while (!app.State.ShouldClose)
            {
                //app.Tick();
                app.RenderWindow();
            }
        }
    }
}