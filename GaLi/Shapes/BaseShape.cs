using FarseerPhysics.Common;
using GaLi.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GaLi.Settings;
using FarseerPhysics.Dynamics;
using GaLi.Internal;
using GaLi.Contracts;

namespace GaLi.Shapes
{
    public abstract class BaseShape
    {
        protected internal event Action Changed;

        private PointF _position;
        public PointF Position
        {
            get { return _position; }
            set
            {
                _cachedVerts = null;
                _position = value;
                Changed?.Invoke();
            }
        }
        internal void SetPosition(PointF value)
        {
            _cachedVerts = null;
            _position = value;
        }

        public float X { get { return Position.X; } }
        public float Y { get { return Position.Y; } }

        private float _width;
        public float Width
        {
            get { return _width; }
            set
            {
                _cachedVerts = null;
                _width = value;
                Changed?.Invoke();
            }
        }
        internal void SetWidth(float value)
        {
            _cachedVerts = null;
            _width = value;
        }

        private float _height;
        public float Height
        {
            get { return _height; }
            set
            {
                _cachedVerts = null;
                _height = value;
                Changed?.Invoke();
            }
        }
        internal void SetHeight(float value)
        {
            _cachedVerts = null;
            _height = value;
        }

        private float _rotation;
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _cachedVerts = null;
                _rotation = value;
                Changed?.Invoke();
            }
        }
        internal void SetRotation(float value)
        {
            _cachedVerts = null;
            _rotation = value;
        }

        public bool Visible { get; set; } = true;
        public object Tag { get; set; }

        public Physics Physics { get; set; }

        public float LineWidth { get; set; } = 1;
        public Color FillColor { get; set; } = Color.White;

        internal VertexCollection _cachedVerts;
        protected internal void ClearVertexCache() => _cachedVerts = null;
        
        public abstract PointF Center { get; }

        public bool Contains(PointF point)
        {
            Body body = GenerateBody(PhysicsSimulation.World, false);
            bool result = PhysicsSimulation.CheckPoint(body, point);
            PhysicsSimulation.RemoveBody(body);

            return result;
        }

        public void RotateAround(PointF center, float rotation)
        {

        }

        private const float PI2 = (float)(2 * Math.PI);
        internal VertexCollection RotateVerts(VertexCollection verts)
        {
            VertexCollection result = new VertexCollection();
            result.Mode = verts.Mode;

            foreach (PointF vert in verts)
            {
                float distanceX = vert.X - Center.X;
                float distanceY = vert.Y - Center.Y;

                float x = distanceX * (float)Math.Cos(-Rotation * PI2) - distanceY * (float)Math.Sin(-Rotation * PI2);
                float y = distanceX * (float)Math.Sin(-Rotation * PI2) + distanceY * (float)Math.Cos(-Rotation * PI2);

                result.Add(Center.X + x, Center.Y + y);
            }

            return result;
        }

        internal abstract VertexCollection GetVertices(PointF origin);
        internal abstract Body GenerateBody(World world, bool forSimulation = true);
    }
}
