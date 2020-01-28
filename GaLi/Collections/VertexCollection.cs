using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLi.Collections
{
    internal class VertexCollection : List<PointF>
    {
        public BeginMode Mode { get; set; }

        public void Add(float x, float y) => this.Add(new PointF(x, y));
    }
}
