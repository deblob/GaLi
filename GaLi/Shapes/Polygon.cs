using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using GaLi.Collections;
using OpenTK.Graphics.OpenGL;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using FarseerPhysics;

namespace GaLi.Shapes
{
    public class Polygon : BaseShape
    {
        private List<PointF> _points = new List<PointF>();
        public List<PointF> Points
        {
            get
            {
                _cachedVerts = null;
                return _points;
            }
            set
            {
                _cachedVerts = null;
                _points = value;
            }
        }

        public override PointF Center
        {
            get
            {
                float minX = _points.Min(p => p.X) + X;
                float maxX = _points.Max(p => p.X) + X;
                float minY = _points.Min(p => p.Y) + Y;
                float maxY = _points.Max(p => p.Y) + Y;

                return new PointF((minX + maxX) / 2, (minY + maxY) / 2);
            }
        }

        internal override Body GenerateBody(World world, bool forSimulation = true)
        {
            Vertices verts = new Vertices();
            verts.AddRange(Points.Select(p => new Vector2(ConvertUnits.ToSimUnits(p.X + 0), ConvertUnits.ToSimUnits(p.Y + 0))));

            Body result = BodyFactory.CreatePolygon(world, verts, 1);
            if (forSimulation)
            {
                result.Restitution = Physics.Restitution;
                result.BodyType = Physics.Dynamic ? BodyType.Dynamic : BodyType.Static;
                result.IsStatic = !Physics.Dynamic;
            }

            result.Rotation = -(float)(Rotation * (Math.PI * 2));
            result.FixtureList.First().UserData = this;

            return result;
        }

        internal override VertexCollection GetVertices(PointF origin)
        {
            if (_cachedVerts != null)
                return _cachedVerts;

            VertexCollection result = new VertexCollection() { Mode = BeginMode.Polygon };
            result.AddRange(_points.Select(p => p = new PointF(p.X + X, p.Y + Y)));
            result = base.RotateVerts(result);

            _cachedVerts = result;
            return result;
        }
    }
}
