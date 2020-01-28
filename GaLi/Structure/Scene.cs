using FarseerPhysics;
using FarseerPhysics.Dynamics;
using GaLi.Collections;
using GaLi.Entities;
using GaLi.Extensions;
using GaLi.Settings;
using GaLi.Shapes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLi.Structure
{
    public class Scene
    {
        private World _world;
        private Dictionary<BaseShape, Body> _bodies = new Dictionary<BaseShape, Body>();

        public event Action Initialize;
        public event Action<FrameEventArgs> Update;
        public event Action<CollisionInformation> Collision;

        public ShapeCollection Shapes { get; set; } = new ShapeCollection();
        public DrawableCollection Drawables { get; set; } = new DrawableCollection();
        public EntityCollection Entities { get; set; } = new EntityCollection();

        public Color BackgroundColor { get; set; } = Color.Black;

        public void Add(BaseShape shape)
        {
            shape.Changed += () =>
            {
                if (_bodies.ContainsKey(shape))
                {
                    Body body = _bodies[shape];
                    _world.RemoveBody(body);
                    _bodies.Remove(shape);
                }
            };

            Shapes.Add(shape);
        }
        public void Add(params BaseShape[] shapes) => shapes?.ToList().ForEach(s => Add(s));
        public void Add(Entity entity) => Entities.Add(entity);

        public void Remove(BaseShape shape) => Shapes.Remove(shape);

        internal void TriggerInitialize()
        {
            _world = new World(new Microsoft.Xna.Framework.Vector2(0, -9.82f));

            Initialize?.Invoke();
        }
        internal void TriggerUpdate(FrameEventArgs e)
        {
            foreach (BaseShape shape in Shapes.Where(n => (n.Physics != null) && (n.Physics?.Enabled ?? false)))
            {
                if (!_bodies.ContainsKey(shape))
                {
                    Body body = shape.GenerateBody(_world);
                    body.OnCollision += onCollision;
                    _bodies.Add(shape, body);
                    
                    shape.Physics.OnPush += (force, point) =>
                    {
                        if (point == null)
                            body.ApplyLinearImpulse(force.ToVector2());
                        else
                            body.ApplyForce(force.ToVector2(), point.Value.ToVector2());
                    };
                }

                Body bodyToUpdate = _bodies[shape];
                bodyToUpdate.IgnoreGravity = shape.Physics.IgnoreGravity;
                bodyToUpdate.GravityScale = shape.Physics.GravityScale;

                shape.Physics.LinearVelocity = new PointF(bodyToUpdate.LinearVelocity.X, bodyToUpdate.LinearVelocity.Y);
                shape.Physics.AngularVelocity = bodyToUpdate.AngularVelocity;
            }

            _world.Step((float)e.Time);

            List<BaseShape> toRemove = new List<BaseShape>();
            foreach (BaseShape shape in _bodies.Keys)
            {
                if (shape.Physics == null)
                {
                    toRemove.Add(shape);
                    continue;
                }

                Body body = _bodies[shape];
                body.Enabled = shape.Physics.Enabled;

                float xPos = -1, yPos = -1;
                if (shape is Shapes.Rectangle)
                {
                    xPos = ConvertUnits.ToDisplayUnits(body.Position.X) - shape.Width / 2;
                    yPos = ConvertUnits.ToDisplayUnits(body.Position.Y) - shape.Height / 2;
                }
                else if (shape is Shapes.Ellipse)
                {
                    xPos = ConvertUnits.ToDisplayUnits(body.Position.X);
                    yPos = ConvertUnits.ToDisplayUnits(body.Position.Y);
                }
                shape.SetPosition(new PointF(xPos, yPos));
                shape.SetRotation(-(float)(body.Rotation / (2 * Math.PI)));
            }

            foreach (BaseShape shape in toRemove)
            {
                _world.RemoveBody(_bodies[shape]);
                _bodies.Remove(shape);
            }

            Update?.Invoke(e);
        }

        private bool onCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            BaseShape shapeA = fixtureA.UserData as BaseShape;
            BaseShape shapeB = fixtureB.UserData as BaseShape;

            CollisionInformation info = new CollisionInformation()
            {
                ShapeA = shapeA,
                ShapeB = shapeB,
                Friction = contact.Friction,
                Restitution = contact.Restitution,
                TangentSpeed = contact.TangentSpeed
            };

            shapeA.Physics.TriggerCollide(info);
            shapeB.Physics.TriggerCollide(info);
            Collision?.Invoke(info);

            return true;
        }
    }
}
