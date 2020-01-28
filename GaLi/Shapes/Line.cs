using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GaLi.Collections;
using OpenTK.Graphics.OpenGL;
using GaLi.Extensions;
using FarseerPhysics.Dynamics;

namespace GaLi.Shapes
{
    public class Line : BaseShape
    {
        private PointF _end;
        public PointF End
        {
            get { return _end; }
            set
            {
                _cachedVerts = null;
                _end = value;
            }
        }

        public float X2 { get { return End.X; } }
        public float Y2 { get { return End.Y; } }

        //public override bool Contains(PointF point)
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool Intersects(BaseShape other)
        //{
        //    throw new NotImplementedException();
        //}

        public override PointF Center
        {
            get
            {
                float distanceX = X2 - X;
                float distanceY = Y2 - Y;

                return new PointF(X + distanceX / 2, Y + distanceY / 2);
            }
        }

        internal override Body GenerateBody(World world, bool forSimulation = true)
        {
            throw new NotImplementedException();
        }
        internal override VertexCollection GetVertices(PointF origin)
        {
            if (_cachedVerts != null)
                return _cachedVerts;

            VertexCollection result = new VertexCollection() { Mode = BeginMode.Lines };
            result.Add(origin.Add(Position));
            result.Add(origin.Add(End));

            result = base.RotateVerts(result);

            _cachedVerts = result;
            return result;
        }
    }
}
