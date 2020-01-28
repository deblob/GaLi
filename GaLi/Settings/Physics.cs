using GaLi.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLi.Settings
{
    public class Physics
    {
        internal event Action<PointF, PointF?> OnPush;

        public bool Enabled { get; set; } = true;
        public bool Dynamic { get; set; }
        //public float Weight { get; set; }
        public float Restitution { get; set; }
        public float Density { get; set; }
        public bool IgnoreGravity { get; set; }
        public float GravityScale { get; set; } = 1;

        public PointF LinearVelocity { get; internal set; }
        public float AngularVelocity { get; internal set; }

        public Physics(bool dynamic, float weight, float restitution, float density)
        {
            Dynamic = dynamic;
            //Weight = weight;
            Restitution = restitution;
            Density = density;
        }

        public Physics()
            : this(true, 10, 0, 1)
        { }

        public void Push(PointF direction, PointF? source) => OnPush?.Invoke(direction, source);
        public void Push(PointF direction) => Push(direction, null);

        public event Action<CollisionInformation> OnCollision;
        internal void TriggerCollide(CollisionInformation info) => OnCollision?.Invoke(info);
    }
}
