using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GaLi.Collections;
using GaLi.Extensions;
using OpenTK.Graphics.OpenGL;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using GaLi.Internal;

namespace GaLi.Shapes
{
    public class Rectangle : BaseShape
    {
        public override PointF Center
        {
            get
            {
                float dX = X + Width / 2;
                float dY = Y + Height / 2;

                return new PointF(dX, dY);
            }
        }

        internal override Body GenerateBody(World world, bool forSimulation = true)
        {
            Body result = BodyFactory.CreateRectangle(world,
                ConvertUnits.ToSimUnits(Width),
                ConvertUnits.ToSimUnits(Height),
                Physics.Density,
                new Vector2(ConvertUnits.ToSimUnits(X + Width / 2), ConvertUnits.ToSimUnits(Y + Height / 2)));
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

            VertexCollection result = new VertexCollection() { Mode = BeginMode.Quads };
            result.AddRange(new List<PointF>{
                origin.Add(Position),
                origin.Add(new PointF(Position.X, Position.Y + Height)),
                origin.Add(new PointF(Position.X + Width, Position.Y + Height)),
                origin.Add(new PointF(Position.X + Width, Position.Y))
            });

            result = base.RotateVerts(result);

            _cachedVerts = result;
            if (result == null)
                ;
            return result;
        }
    }
}
