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
using Microsoft.Xna.Framework;
using FarseerPhysics;

namespace GaLi.Shapes
{
    public class Ellipse : BaseShape
    {
        public int Sides { get; set; } = 100;

        public override PointF Center
        {
            get
            {
                return Position;
            }
        }

        internal override Body GenerateBody(World world, bool forSimulation = true)
        {
            Body result = BodyFactory.CreateEllipse(world,
                ConvertUnits.ToSimUnits(Width),
                ConvertUnits.ToSimUnits(Height),
                Sides,
                1f,
                new Vector2(ConvertUnits.ToSimUnits(X), ConvertUnits.ToSimUnits(Y)));
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

        private float PI2 = (float)(Math.PI * 2);
        internal override VertexCollection GetVertices(PointF origin)
        {
            if (_cachedVerts != null)
                return _cachedVerts;

            VertexCollection result = new VertexCollection() { Mode = BeginMode.Polygon };
            for (int i = 0; i < Sides; i++)
            {
                float x = (float)Math.Cos(((float)i / (float)Sides) * PI2) * Width;
                float y = (float)Math.Sin(((float)i / (float)Sides) * PI2) * Height;

                result.Add(new PointF(x + X, y + Y));
            }

            result = base.RotateVerts(result);

            _cachedVerts = result;
            return result;
        }
    }
}
