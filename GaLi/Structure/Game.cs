using GaLi.Collections;
using GaLi.Contracts;
using GaLi.Entities;
using GaLi.Shapes;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLi.Structure
{
    public class Game
    {
        private GameWindow _game;

        public event Action<MouseEventArgs> MouseMove;
        public event Action<MouseButtonEventArgs> MouseDown;
        public event Action<MouseButtonEventArgs> MouseUp;
        public event Action<MouseWheelEventArgs> MouseWheelChanged;
        public event Action<KeyboardKeyEventArgs> KeyDown;
        public event Action<KeyboardKeyEventArgs> KeyUp;
        public event Action<KeyPressEventArgs> KeyPress;

        public int Width
        {
            get { return _game.Width; }
            set { _game.Width = value; }
        }
        public int Height
        {
            get { return _game.Height; }
            set { _game.Height = value; }
        }
        public bool Fullscreen
        {
            get { return _game.WindowState.Equals(WindowState.Fullscreen); }
            set { _game.WindowState = value ? WindowState.Fullscreen : WindowState.Normal; }
        }
        public string Title
        {
            get { return _game.Title; }
            set { _game.Title = value; }
        }
        public double UpdateRate
        {
            get { return _game.TargetUpdateFrequency; }
            set { _game.TargetUpdateFrequency = value; }
        }

        private Scene _activeScene;
        public Scene ActiveScene
        {
            get { return _activeScene; }
            set
            {
                _activeScene = value;
                _activeScene?.TriggerInitialize();
            }
        }

        public Game(int width, int height, bool fullscreen, string title, double updateRate)
        {
            _game = new GameWindow();

            Width = width;
            Height = height;
            Fullscreen = fullscreen;
            Title = title;
            UpdateRate = updateRate;

            _game.Resize += (sender, e) => GL.Viewport(0, 0, _game.Width, _game.Height);
            _game.Load += onInitialize;
            _game.UpdateFrame += onUpdate;
            _game.RenderFrame += onRender;

            _game.Mouse.Move += (sender, e) => MouseMove?.Invoke(e);
            _game.Mouse.ButtonDown += onMouseDown;
            _game.Mouse.ButtonUp += onMouseUp;
            _game.Mouse.WheelChanged += onMouseWheelChanged;
            _game.Keyboard.KeyDown += onKeyDown;
            _game.Keyboard.KeyUp += onKeyUp;
            _game.KeyPress += onKeyPress;
        }

        private void onMouseWheelChanged(object sender, MouseWheelEventArgs e)
        {
            MouseWheelChanged?.Invoke(e);
        }

        public void Start() => _game.Run(UpdateRate);

        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseDown?.Invoke(e);

            // Shapes in ActiveScene überprüfen
        }

        private void onMouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseUp?.Invoke(e);

            // Shapes in ActiveScene überprüfen
        }

        private void onKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            KeyDown?.Invoke(e);
        }

        private void onKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            KeyUp?.Invoke(e);
        }

        private void onKeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(e);
        }

        private void onInitialize(object sender, EventArgs e)
        {
            // ...
        }

        private void onUpdate(object sender, FrameEventArgs e)
        {
            ActiveScene?.TriggerUpdate(e);
            ActiveScene?.Entities?.ForEach(ent => ent.TriggerUpdate(e));
        }

        private PointF _zeroOrigin = new PointF(0, 0);
        private void onRender(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(ActiveScene.BackgroundColor);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Width, 0, Height, 0.0, 4.0);

            if (ActiveScene != null)
            {
                foreach (Entity ent in ActiveScene.Entities)
                    foreach (BaseShape shape in ent.Shapes)
                        drawShape(shape, ent.Position);

                // Nötig, da in ActiveScene.Shapes während der Schleife neue Einträge hinzugefügt werden könnten
                List<BaseShape> currentShapes = new List<BaseShape>(ActiveScene.Shapes);
                foreach (BaseShape shape in currentShapes)
                    drawShape(shape, _zeroOrigin);

                List<IDrawable> currentDrawables = new List<IDrawable>(ActiveScene.Drawables);
                foreach (IDrawable drawable in currentDrawables)
                    drawable.Draw(Width, Height);
            }
            
            _game.SwapBuffers();
        }

        private void drawShape(BaseShape shape, PointF origin)
        {
            if (!shape.Visible)
                return;

            VertexCollection verts = shape.GetVertices(origin);

            GL.Color3(shape.FillColor);

            GL.Begin(verts.Mode);
            foreach (PointF vert in verts)
                GL.Vertex2(vert.X, vert.Y);
            GL.End();
        }
    }
}
