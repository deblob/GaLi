using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLi.Extensions
{
    public static class PointExtensions
    {
        public static PointF Add(this PointF p1, PointF p2) => new PointF(p1.X + p2.X, p1.Y + p2.Y);
        public static PointF Subtract(this PointF p1, PointF p2) => new PointF(p1.X - p2.X, p1.Y - p2.Y);

        public static PointF LerpTo(this PointF p1, PointF p2, float value)
        {
            float x = Microsoft.Xna.Framework.MathHelper.Lerp(p1.X, p2.X, value);
            float y = Microsoft.Xna.Framework.MathHelper.Lerp(p1.Y, p2.Y, value);

            //float x = (p1.X + p2.X) * value;
            //float y = (p1.Y + p2.Y) * value;

            return new PointF(x, y);
        }

        public static Microsoft.Xna.Framework.Vector2 ToVector2(this PointF p) => new Microsoft.Xna.Framework.Vector2(p.X, p.Y);
    }
}
