using FarseerPhysics.Dynamics;
using GaLi.Entities;
using GaLi.Shapes;
using GaLi.Structure;
using GaLi.Extensions;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Collision.Shapes;
//using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;

namespace GaLi.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleTests tests = new SimpleTests();

            #region old
            //Line line = new Line() { Position = new PointF(100, 420), End = new PointF(320, 400), FillColor = Color.Red };
            //Shapes.Rectangle rectangle = new Shapes.Rectangle()
            //{
            //    Position = new PointF(400, 350),
            //    Width = 300,
            //    Height = 240,
            //    FillColor = Color.AliceBlue
            //};

            //Entity entity = new Entity() { Position = new PointF(1024 / 2, 768 / 2) };
            //entity.Shapes.Add(
            //    new Shapes.Rectangle() { Position = new PointF(-60, -20), Width = 50, Height = 10, FillColor = Color.Orange });
            //entity.Shapes.Add(
            //    new Shapes.Rectangle() { Position = new PointF(30, 30), Width = 60, Height = 20, FillColor = Color.Green });

            //bool mouseDownLeft = false, mouseDownRight = false;

            //float deltaTime = 1;
            //float pmX = -1;
            //float pmY = -1;
            //game.MouseMove += e =>
            //{
            //    float x = e.X;
            //    float y = e.Y;

            //    float dX = x - pmX;
            //    float dY = pmY - y;

            //    pmX = x;
            //    pmY = y;

            //    Console.WriteLine($"Delta: {dX}, {dY}");

            //    if (mouseDownLeft)
            //    {
            //        line.End = new PointF(line.End.X, line.End.Y + dY);
            //        line.Position = new PointF(line.Position.X + dX, line.Position.Y);
            //    }
            //    if (mouseDownRight)
            //    {
            //        rectangle.Position = new PointF(rectangle.Position.X + dX, rectangle.Position.Y);
            //        rectangle.Height += dY;
            //    }
            //};
            //game.MouseDown += e =>
            //{
            //    Console.WriteLine($"Mouse down: {e.X}, {game.Height - e.Y}");

            //    if (e.Button == MouseButton.Left)
            //        mouseDownLeft = true;
            //    if (e.Button == MouseButton.Right)
            //        mouseDownRight = true;
            //};
            //game.MouseUp += e =>
            //{
            //    Console.WriteLine($"Mouse up: {e.X}, {game.Height - e.Y}");

            //    if (e.Button == MouseButton.Left)
            //        mouseDownLeft = false;
            //    if (e.Button == MouseButton.Right)
            //        mouseDownRight = false;
            //};
            //game.KeyDown += e =>
            //{
            //    Console.WriteLine($"Key down: {e.Key.ToString()}");
            //};
            //game.KeyUp += e =>
            //{
            //    Console.WriteLine($"Key up: {e.Key.ToString()}");
            //};
            //game.KeyPress += e =>
            //{
            //    Console.WriteLine($"Key press: {e.KeyChar}");
            //    if (e.KeyChar == 'r')
            //    {
            //        rectangle.FillColor = Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256));
            //    }
            //    if (e.KeyChar == 'd')
            //        entity.Position = new PointF(entity.Position.X + 80 * deltaTime, entity.Position.Y);
            //    if (e.KeyChar == 'a')
            //        entity.Position = new PointF(entity.Position.X - 80 * deltaTime, entity.Position.Y);
            //    if (e.KeyChar == 'h')
            //        entity.Visible = !entity.Visible;
            //};

            //Scene testScene = new Scene();
            //testScene.Initialize += () =>
            //{
            //    Console.WriteLine("Scene initialized.");
            //};
            //testScene.Update += e =>
            //{
            //    deltaTime = (float)e.Time;
            //};

            //entBlock testBlock = new entBlock(120, 120, 300, 300) { Dynamic = true };
            //entBlock testBlock2 = new entBlock(450, 120, 210, 240) { MaxHeight = 240, MinHeight = 60, GrowSpeed = 140, ShrinkSpeed = 100, Color = Color.CadetBlue, Dynamic = true };

            ////testScene.Shapes.Add(line);
            ////testScene.Shapes.Add(rectangle);
            //testScene.Entities.Add(entity);
            //testScene.Entities.Add(testBlock);
            //testScene.Entities.Add(testBlock2);

            //game.ActiveScene = testScene;
            #endregion

            tests.ImplementedPhysicsTest();
            //tests.StressTest();
            //tests.MarchingCubesTest();
            //tests.PolygonTest();
            //tests.BezierTest();
        }
    }
}
