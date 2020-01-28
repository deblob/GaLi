using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLi.Internal
{
    internal class PhysicsSimulation
    {
        internal static World World = new World(new Vector2(0, -9.82f));

        internal static bool CheckPoint(Body body, PointF point)
        {
            Vector2 vPoint = new Vector2(ConvertUnits.ToSimUnits(point.X), ConvertUnits.ToSimUnits(point.Y));
            return body.FixtureList.First().TestPoint(ref vPoint);
        }

        internal static void RemoveBody(Body body) => World.RemoveBody(body);
    }
}
