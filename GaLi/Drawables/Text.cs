using GaLi.Contracts;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLi.Drawables
{
    public class Text : IDrawable
    {
        private bool _contentChanged = true;
        private Bitmap _canvas;
        int text_texture;

        public Text()
        {
            _canvas = new Bitmap(1024, 768); // match window size

            text_texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, text_texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _canvas.Width, _canvas.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero); // just allocate memory, so we can update efficiently using TexSubImage2D
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                _contentChanged = true;
            }
        }
        public Font Font { get; set; } = new Font("Arial", 10);
        public Color Color { get; set; } = Color.White;
        public PointF Position { get; set; }

        public void Draw(float width, float height)
        {
            if (_contentChanged)
            {
                _canvas = new Bitmap((int)width, (int)height);
                using (Graphics g = Graphics.FromImage(_canvas))
                {
                    g.Clear(Color.Transparent);
                    g.DrawString(Content, Font, new SolidBrush(Color), Position);
                }
                _canvas.Save(System.IO.Path.Combine(Environment.CurrentDirectory, "test.png"), ImageFormat.Png);

                BitmapData data = _canvas.LockBits(new Rectangle(0, 0, _canvas.Width, _canvas.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)width, (int)height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                _canvas.UnlockBits(data);

                _contentChanged = false;
            }

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, width, height, 0, -1, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0f, 1f); GL.Vertex2(0f, 0f);
            GL.TexCoord2(1f, 1f); GL.Vertex2(1f, 0f);
            GL.TexCoord2(1f, 0f); GL.Vertex2(1f, 1f);
            GL.TexCoord2(0f, 0f); GL.Vertex2(0f, 1f);
            GL.End();
        }
    }
}
