using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GaLi.Collections;
using FarseerPhysics.Dynamics;
using OpenTK.Graphics.OpenGL;
using GaLi.Extensions;

namespace GaLi.Shapes
{
    public class CubicBezier : BaseShape
    {
        public int Resolution { get; set; } = 100;

        private PointF _p0 = new PointF(-50, -50);
        /// <summary>
        /// Start point (relative to <see cref="BaseShape.Position"/>)
        /// </summary>
        public PointF P0
        {
            get { return _p0; }
            set
            {
                _p0 = value;
                ClearVertexCache();
                // base.Changed triggern?
            }
        }

        private PointF _p1 = new PointF(-50, 50);
        /// <summary>
        /// First handle (relative to <see cref="BaseShape.Position"/>)
        /// </summary>
        public PointF P1
        {
            get { return _p1; }
            set
            {
                _p1 = value;
                ClearVertexCache();
            }
        }

        private PointF _p2 = new PointF(50, 50);
        /// <summary>
        /// Second handle (relative to <see cref="BaseShape.Position"/>)
        /// </summary>
        public PointF P2
        {
            get { return _p2; }
            set
            {
                _p2 = value;
                ClearVertexCache();
            }
        }

        private PointF _p3 = new PointF(50, -50);
        /// <summary>
        /// End point (relative to <see cref="BaseShape.Position"/>)
        /// </summary>
        public PointF P3
        {
            get { return _p3; }
            set
            {
                _p3 = value;
                ClearVertexCache();
            }
        }

        public override PointF Center
        {
            get { return Position; }
        }

        public new float Width
        {
            get { return base.Width; }
            set { throw new Exception("Cannot set the width of this shape"); }
        }

        public new float Height
        {
            get { return base.Height; }
            set { throw new Exception("Cannot set the height of this shape"); }
        }

        internal override Body GenerateBody(World world, bool forSimulation = true)
        {
            throw new NotImplementedException();
        }

        internal override VertexCollection GetVertices(PointF origin)
        {
            if (_cachedVerts != null)
                return _cachedVerts;

            VertexCollection result = new VertexCollection() { Mode = BeginMode.LineStrip };

            float deltaValue = 1f / (float)Resolution;
            for (int i = 0; i < Resolution; i++)
            {
                float value = (float)i * deltaValue;

                PointF interpolated01 = P0.LerpTo(P1, value);
                PointF interpolated12 = P1.LerpTo(P2, value);
                PointF interpolated23 = P2.LerpTo(P3, value);

                PointF interpolated01_12 = interpolated01.LerpTo(interpolated12, value);
                PointF interpolated12_23 = interpolated12.LerpTo(interpolated23, value);

                PointF interpolatedFinal = interpolated01_12.LerpTo(interpolated12_23, value);
                result.Add(interpolatedFinal.Add(Position));
            }

            result = base.RotateVerts(result);

            _cachedVerts = result;
            return _cachedVerts;
        }
    }
}
