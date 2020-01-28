using GaLi.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLi.Settings
{
    public class CollisionInformation
    {
        public BaseShape ShapeA { get; internal set; }
        public BaseShape ShapeB { get; internal set; }
        public float Friction { get; internal set; }
        public float Restitution { get; internal set; }
        public float TangentSpeed { get; internal set; }
    }
}
