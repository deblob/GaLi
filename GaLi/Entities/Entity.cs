using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using GaLi.Collections;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLi.Entities
{
    public class Entity
    {
        public event Action Initialize;
        public event Action<FrameEventArgs> Update;

        private PointF _position;
        public PointF Position
        {
            get { return _position; }
            set
            {
                _position = value;
                Shapes.ForEach(s => s.ClearVertexCache());
            }
        }

        private bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                Shapes.ToList().ForEach(shape => shape.Visible = value);
                _visible = value;
            }
        }

        public bool Dynamic { get; set; }
        public Shape CollisionShape { get; set; }

        public ShapeCollection Shapes { get; set; } = new ShapeCollection();

        internal void TriggerInitialize() => Initialize?.Invoke();
        internal void TriggerUpdate(FrameEventArgs args) => Update?.Invoke(args);
    }
}
