using SFML.Graphics;
using SFML.Window;
using SFML.System;
using NESharpLib;
using NESharpLib.Modules.PPU;

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
        public Mut<Image> GameScreen = new Mut<Image>(new Image(new Vector2u(256, 240)));
        public ProgramState()
        {
            WindowTitle = "NESharp";
            ShouldClose = false;
            GameScreen.Access(out MutLock<Image> image);
            GameScreenTexture.Update(image.Item);
            image.Dispose();
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
            console.PPU.FrameReady += FrameReadyHandler;
        }
        public void FrameReadyHandler(object? sender, FrameReadyArgs e)
        {
            State.GameScreen.Access(out MutLock<Image> value);
            value.Item = e.Frame;
            value.Release();
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
        public void Tick()
        {
            if (eventLoop.HasActions())
                eventLoop.Process(ref State);
        }
        public void RenderWindow()
        {
            MainWindow.Clear();
            if (State.HasNewFrameData)
            {
                State.GameScreen.Access(out MutLock<Image> l);
                State.GameScreenTexture.Update(l.Item);
                l.Release();
                State.HasNewFrameData = false;
            }
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
            app.RenderWindow();
            while (!app.State.ShouldClose)
            {
                app.Tick();
                app.RenderWindow();
            }
        }
    }
}