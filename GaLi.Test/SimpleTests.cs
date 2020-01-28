using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using GaLi.Shapes;
using GaLi.Structure;
using GaLi.Extensions;
using Microsoft.Xna.Framework;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Factories;
using FarseerPhysics;
using static FarseerPhysics.ConvertUnits;
using GaLi.Settings;
using System.Threading;
using GaLi.Drawables;

namespace GaLi.Test
{
    public class SimpleTests
    {
        public void BezierTest()
        {
            Game game = new Game(1024, 768, false, "Bezier test", 60);

            Scene scene = new Scene();

            CubicBezier bez = new CubicBezier()
            {
                Position = new PointF(1024 / 2, 768 / 2),
                FillColor = Color.Red
            };

            int handleDiameter = 6;
            Color handleColor = Color.Orange;
            Color handleLineColor = Color.DimGray;

            Ellipse handle0 = new Ellipse()
            {
                Position = bez.P0.Add(bez.Position),
                Width = handleDiameter,
                Height = handleDiameter,
                FillColor = handleColor
            };
            handle0.Tag = new Action(() => bez.P0 = handle0.Position.Subtract(bez.Position));

            Ellipse handle1 = new Ellipse()
            {
                Position = bez.P1.Add(bez.Position),
                Width = handleDiameter,
                Height = handleDiameter,
                FillColor = handleColor
            };
            handle1.Tag = new Action(() => bez.P1 = handle1.Position.Subtract(bez.Position));

            Ellipse handle2 = new Ellipse()
            {
                Position = bez.P2.Add(bez.Position),
                Width = handleDiameter,
                Height = handleDiameter,
                FillColor = handleColor
            };
            handle2.Tag = new Action(() => bez.P2 = handle2.Position.Subtract(bez.Position));

            Ellipse handle3 = new Ellipse()
            {
                Position = bez.P3.Add(bez.Position),
                Width = handleDiameter,
                Height = handleDiameter,
                FillColor = handleColor
            };
            handle3.Tag = new Action(() => bez.P3 = handle3.Position.Subtract(bez.Position));

            Line line01 = new Line()
            {
                Position = handle0.Position,
                End = handle1.Position,
                FillColor = handleLineColor,
                Tag = Tuple.Create(handle0, handle1)
            };

            Line line12 = new Line()
            {
                Position = handle1.Position,
                End = handle2.Position,
                FillColor = handleLineColor,
                Tag = Tuple.Create(handle1, handle2)
            };

            Line line23 = new Line()
            {
                Position = handle2.Position,
                End = handle3.Position,
                FillColor = handleLineColor,
                Tag = Tuple.Create(handle2, handle3)
            };

            List<Ellipse> handles = new List<Ellipse>() { handle0, handle1, handle2, handle3 };
            List<Line> lines = new List<Line>() { line01, line12, line23 };

            scene.Add(line01, line12, line23);
            scene.Add(handle0, handle1, handle2, handle3);
            scene.Add(bez);

            Ellipse clickedOn = null;
            bool isLeftMouseDown = false;
            bool isRightMouseDown = false;
            game.MouseDown += args =>
            {
                PointF mousePos = new PointF(args.X, 768 - args.Y);

                if (args.Button.Equals(MouseButton.Left))
                {
                    isLeftMouseDown = true;
                    if (clickedOn == null)
                    {
                        foreach (Ellipse handle in handles)
                            if (handle.Contains(mousePos))
                            {
                                clickedOn = handle;
                                break;
                            }
                    }
                }

                if (args.Button.Equals(MouseButton.Right))
                    isRightMouseDown = true;
            };
            game.MouseUp += args =>
            {
                if (args.Button.Equals(MouseButton.Left))
                {
                    isLeftMouseDown = false;
                    clickedOn = null;
                }

                if (args.Button.Equals(MouseButton.Right))
                    isRightMouseDown = false;
            };

            PointF lastMousePos = PointF.Empty;
            PointF mouseDelta = PointF.Empty;
            game.MouseMove += args =>
            {
                PointF mousePos = new PointF(args.X, 768 - args.Y);
                mouseDelta = mousePos.Subtract(lastMousePos);

                if (isLeftMouseDown)
                {
                    if (clickedOn != null)
                    {
                        clickedOn.Position = clickedOn.Position.Add(mouseDelta);
                        ((Action)clickedOn.Tag)();
                    }
                }
                else if (isRightMouseDown)
                {
                    bez.Position = bez.Position.Add(mouseDelta);
                    handles.ForEach(h => h.Position = h.Position.Add(mouseDelta));
                }

                lastMousePos = mousePos;
            };

            scene.Update += args =>
            {
                foreach (Line line in lines)
                {
                    Tuple<Ellipse, Ellipse> points = (Tuple<Ellipse, Ellipse>)line.Tag;
                    line.Position = points.Item1.Position;
                    line.End = points.Item2.Position;
                }
            };

            game.ActiveScene = scene;
            game.Start();
        }

        public void RotationTest()
        {
            Game game = new Game(1024, 768, false, "Test Game", 60);

            Line line = new Line() { Position = new PointF(30, 300), End = new PointF(420, 500), FillColor = Color.Aqua, Rotation = 0 };
            Shapes.Rectangle rectangle = new Shapes.Rectangle() { Position = new PointF(200, 300), Width = 120, Height = 240, FillColor = Color.OrangeRed, Rotation = 1 };

            Scene scene = new Scene();
            scene.Add(line);
            scene.Add(rectangle);

            float deltaTime = 1;
            scene.Update += e =>
            {
                deltaTime = (float)e.Time;
            };

            bool mouseDownLeft = false, mouseDownRight = false;
            float pmX = -1, pmY = -1;

            game.MouseDown += e =>
            {
                if (e.Button.Equals(MouseButton.Left))
                    mouseDownLeft = true;
            };
            game.MouseUp += e =>
            {
                if (e.Button.Equals(MouseButton.Left))
                    mouseDownLeft = false;
            };

            game.MouseMove += e =>
            {
                float x = e.X;
                float y = e.Y;

                float dX = x - pmX;
                float dY = pmY - y;

                pmX = x;
                pmY = y;

                if (mouseDownLeft)
                {
                    line.Position = new PointF(line.Position.X + dX, line.Position.Y + dY);
                }
                if (mouseDownRight)
                {
                    line.End = new PointF(line.End.X + dX, line.End.Y + dY);
                }
            };

            game.KeyPress += e =>
            {
                if (e.KeyChar == 'e')
                    rectangle.Rotation += 1f * deltaTime;
                if (e.KeyChar == 'q')
                    rectangle.Rotation -= 1f * deltaTime;
            };

            game.ActiveScene = scene;
            game.Start();
        }

        public void NewPhysicsTest()
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(32);
            Random rng = new Random();
            float delta = .3333333f;
            Dictionary<BaseShape, Body> bodies = new Dictionary<BaseShape, Body>();

            float width = 1024, height = 768;
            Game game = new Game((int)width, (int)height, false, "New Physics Test", 60);
            Scene scene = new Scene() { BackgroundColor = Color.DarkGray };
            World world = new World(new Vector2(0, -9.82f));

            Shapes.Rectangle floor = new Shapes.Rectangle()
            {
                Position = new PointF(width / 2 - 200, height * .25f),
                FillColor = Color.LightGray,
                Width = 400,
                Height = 80
            };
            scene.Add(floor);

            Body bodyFloor = BodyFactory.CreateRectangle(world, ToSimUnits(floor.Width), ToSimUnits(floor.Height), 1,
                new Vector2(ToSimUnits(floor.X) + ToSimUnits(floor.Width / 2), ToSimUnits(floor.Y + floor.Height / 2)));
            bodyFloor.IsStatic = true;
            bodyFloor.BodyType = BodyType.Static;

            game.KeyPress += e =>
            {
                if (e.KeyChar.Equals('e'))
                {
                    floor.Rotation += .2f * delta;
                    bodyFloor.Rotation = -(float)(floor.Rotation * (Math.PI * 2));
                }
                if (e.KeyChar.Equals('q'))
                {
                    floor.Rotation -= .2f * delta;
                    bodyFloor.Rotation = -(float)(floor.Rotation * (Math.PI * 2));
                }
            };

            game.MouseDown += e =>
            {
                float rWidth = rng.Next(30, 120);
                float rHeight = rng.Next(30, 120);
                Shapes.Rectangle rect = new Shapes.Rectangle()
                {
                    Position = new PointF(e.X - rWidth / 2, height - e.Y - rHeight / 2),
                    Width = rWidth,
                    Height = rHeight,
                    FillColor = Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256))
                };
                scene.Add(rect);

                Body rectBody = BodyFactory.CreateRectangle(world, ToSimUnits(rect.Width), ToSimUnits(rect.Height), 1,
                    new Vector2(ToSimUnits(rect.X + rect.Width / 2), ToSimUnits(rect.Y + rect.Height / 2)));
                rectBody.BodyType = BodyType.Dynamic;
                rectBody.FixtureList.First().UserData = rect;

                bodies.Add(rect, rectBody);
            };

            scene.Update += e =>
            {
                delta = (float)e.Time;
                world.Step(delta);

                foreach (BaseShape shape in bodies.Keys)
                {
                    Body body = bodies[shape];

                    //shape.Position = new PointF(
                    //    ToDisplayUnits(body.Position.X) + shape.Width / 2, ToDisplayUnits(body.Position.Y) + shape.Height / 2);
                    shape.Position = new PointF(
                          ToDisplayUnits(body.Position.X) - shape.Width / 2, ToDisplayUnits(body.Position.Y) - shape.Height / 2);
                    shape.Rotation = -(float)(body.Rotation / (2 * Math.PI));
                }
            };

            game.ActiveScene = scene;
            game.Start();
        }

        public void ImplementedPhysicsTest()
        {
            Game game = new Game(1024, 768, false, "Physics Test", 60);
            Scene scene = new Scene();
            Random rng = new Random();

            List<BaseShape> shapes = new List<BaseShape>();

            Line aim = new Line()
            {
                FillColor = Color.Pink
            };
            Shapes.Rectangle rect1 = new Shapes.Rectangle()
            {
                Position = new PointF(1024f / 2f, 768f * .7f),
                Width = 200,
                Height = 200,
                FillColor = Color.Red,
                Physics = new Physics(true, 1, .15f, 1)
            };
            rect1.Physics.OnCollision += info =>
            {
                rect1.FillColor = Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256));
            };
            shapes.Add(rect1);
            Shapes.Rectangle rect2 = new Shapes.Rectangle()
            {
                Position = new PointF(1024f / 2f - 250, 768f * .75f),
                Width = 100,
                Height = 100,
                FillColor = Color.Yellow,
                Physics = new Physics(true, 1, .25f, 1)
            };
            rect2.Physics.OnCollision += info =>
            {
                rect2.FillColor = Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256));
            };
            shapes.Add(rect2);
            Shapes.Rectangle floor = new Shapes.Rectangle()
            {
                Position = new PointF(0, 0),
                Width = 1024,
                Height = 60,
                FillColor = Color.Gray,
                Physics = new Physics(false, 1, 0, 1)
            };
            floor.Physics.OnCollision += info =>
            {
                Console.WriteLine($"Something just hit the floor with a tangent speed of {info.TangentSpeed}.");
            };
            //shapes.Add(floor);
            Shapes.Rectangle wall1 = new Shapes.Rectangle()
            {
                Position = new PointF(0, 0),
                Width = 60,
                Height = 90001,
                FillColor = Color.Gray,
                Physics = new Physics(false, 1, 0, 1)
            };
            //shapes.Add(wall1);
            Shapes.Rectangle wall2 = new Shapes.Rectangle()
            {
                Position = new PointF(1024 - 60, 0),
                Width = 60,
                Height = 90001,
                FillColor = Color.Gray,
                Physics = new Physics(false, 1, 0, 1)
            };
            Shapes.Rectangle ceiling = new Shapes.Rectangle()
            {
                Position = new PointF(0, 768 - 60),
                Width = 1024,
                Height = 60,
                FillColor = Color.Gray,
                Physics = new Physics(false, 1, 0, 1)
            };
            //shapes.Add(wall2);
            Shapes.Ellipse ell = new Ellipse()
            {
                Position = new PointF(320, 550),
                Sides = 100,
                Width = 160,
                Height = 80,
                FillColor = Color.Green,
                Physics = new Physics(true, 1, 0, 1)
            };
            shapes.Add(ell);

            BaseShape pushTarget = null;
            bool mouseDown = false;
            PointF originalClick = PointF.Empty;
            game.MouseDown += e =>
            {
                if (e.Button == MouseButton.Left)
                {
                    if (!mouseDown)
                    {
                        pushTarget = shapes.FirstOrDefault(s => s.Contains(new PointF(e.X, 768 - e.Y)));
                        if (pushTarget != null)
                        {
                            mouseDown = true;
                            aim.Position = new PointF(e.X, 768 - e.Y);
                            aim.End = new PointF(e.X, 768 - e.Y);
                            aim.Visible = true;
                            originalClick = new PointF(e.X, 768 - e.Y);
                        }
                    }
                }
                else if (e.Button == MouseButton.Right)
                {
                    float width = rng.Next(40, 150);
                    float height = rng.Next(40, 150);
                    Shapes.Rectangle newRect = new Shapes.Rectangle()
                    {
                        Position = new PointF(e.X - width / 2, 768 - e.Y - height / 2),
                        Width = width,
                        Height = height,
                        FillColor = Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256)),
                        Physics = new Physics(true, 1, 0, 1)
                    };
                    newRect.Physics.OnCollision += info =>
                    {
                        newRect.FillColor = Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256));
                    };
                    shapes.Add(newRect);

                    scene.Shapes.Insert(0, newRect);
                }
                else if (e.Button == MouseButton.Middle)
                {
                    BaseShape shape = shapes.Where(s => s.Physics != null).FirstOrDefault(n => n.Contains(new PointF(e.X, 768 - e.Y)));
                    if (shape != null)
                    {
                        if (shape.Physics.GravityScale == 1)
                            shape.Physics.GravityScale = 0;
                        else
                            shape.Physics.GravityScale = 1;
                    }
                }
            };
            game.MouseUp += e =>
            {
                if (e.Button == MouseButton.Left)
                {
                    if (mouseDown)
                    {
                        if (pushTarget != null)
                        {
                            mouseDown = false;
                            aim.Visible = false;

                            PointF force = new PointF(
                                (aim.X2 - aim.X),
                                (aim.Y2 - aim.Y));

                            pushTarget.Physics.Push(force, new PointF(ToSimUnits(originalClick.X), ToSimUnits(originalClick.Y)));
                        }
                    }
                }
            };

            bool leftShiftHeld = false;
            bool rightShiftHeld = false;
            bool leftControlHeld = false;
            bool rightControlHeld = false;
            game.KeyDown += e =>
            {
                switch (e.Key)
                {
                    case Key.LShift:
                        leftShiftHeld = true;
                        break;
                    case Key.RShift:
                        rightShiftHeld = true;
                        break;
                    case Key.LControl:
                        leftControlHeld = true;
                        break;
                    case Key.RControl:
                        rightControlHeld = true;
                        break;
                    case Key.A:
                        if (leftShiftHeld)
                            shapes.Where(n => n.Physics != null).Select(n => n.Physics).ToList().ForEach(p => p.Push(new PointF(-35, 0)));
                        break;
                    case Key.D:
                        if (leftShiftHeld)
                            shapes.Where(n => n.Physics != null).Select(n => n.Physics).ToList().ForEach(p => p.Push(new PointF(35, 0)));
                        break;
                    case Key.W:
                        if (leftShiftHeld)
                            shapes.Where(n => n.Physics != null).Select(n => n.Physics).ToList().ForEach(p => p.Push(new PointF(0, 35)));
                        break;
                    case Key.S:
                        if (leftShiftHeld)
                            shapes.Where(n => n.Physics != null).Select(n => n.Physics).ToList().ForEach(p => p.Push(new PointF(0, -35)));
                        break;
                    case Key.G:
                        shapes.Where(n => n.Physics != null).Select(n => n.Physics).ToList().ForEach(p =>
                        {
                            if (p.GravityScale == 0)
                                p.GravityScale = 1;
                            else
                                p.GravityScale = 0;
                        });
                        break;
                    case Key.T:
                        ceiling.Visible = !ceiling.Visible;
                        ceiling.Physics.Enabled = !ceiling.Physics.Enabled;
                        break;
                }
            };

            game.KeyUp += e =>
            {
                switch (e.Key)
                {
                    case Key.LShift:
                        leftShiftHeld = false;
                        break;
                    case Key.RShift:
                        rightShiftHeld = false;
                        break;
                    case Key.LControl:
                        leftControlHeld = false;
                        break;
                    case Key.RControl:
                        rightControlHeld = false;
                        break;
                }
            };

            PointF mousePoint = Point.Empty;
            game.MouseMove += e =>
            {
                mousePoint = new PointF(e.X, 768 - e.Y);

                if (mouseDown)
                    aim.End = new PointF(e.X, 768 - e.Y);
            };
            Timer spawnTimer = null;
            game.KeyPress += e =>
            {
                switch (e.KeyChar)
                {
                    case 'f':
                        shapes.ForEach(s =>
                        {
                            if (s.Physics != null)
                                s.Physics.Enabled = !s.Physics.Enabled;
                        });
                        break;
                    case 's':
                        if (spawnTimer != null)
                        {
                            spawnTimer.Dispose();
                            spawnTimer = null;
                        }
                        else
                        {
                            spawnTimer = new Timer(state =>
                            {
                                float width = rng.Next(10, 35);
                                float height = rng.Next(10, 35);
                                Shapes.Ellipse newRect = new Shapes.Ellipse()
                                {
                                    Position = new PointF(mousePoint.X, mousePoint.Y),
                                    Width = width,
                                    Height = width,
                                    FillColor = Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256)),
                                    Physics = new Physics(true, 1, 0, 1)
                                };
                                newRect.Physics.OnCollision += info =>
                                {
                                    newRect.FillColor = Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256));
                                };
                                shapes.Add(newRect);

                                scene.Shapes.Insert(0, newRect);
                            }, new object(), 0, 200);
                        }
                        break;
                    case 'a':
                        if (Math.Abs(rect1.Physics.LinearVelocity.X) < 6)
                            rect1.Physics.Push(new PointF(-3, 0));
                        break;
                    case 'd':
                        if (Math.Abs(rect1.Physics.LinearVelocity.X) < 6)
                            rect1.Physics.Push(new PointF(3, 0));
                        break;
                    case ' ':
                        rect1.Physics.Push(new PointF(0, 20));
                        break;
                    case '+':
                        BaseShape shape = shapes.FirstOrDefault(n => n.Contains(mousePoint));
                        if (shape != null)
                        {
                            if (!leftShiftHeld)
                                shape.Width += 10;
                            else
                                shape.Height += 10;
                        }
                        break;
                    case '-':
                        BaseShape shapeM = shapes.FirstOrDefault(n => n.Contains(mousePoint));
                        if (shapeM != null)
                        {
                            if (!leftShiftHeld)
                                shapeM.Width -= 10;
                            else
                                shapeM.Height -= 10;
                        }
                        break;
                }
            };
            scene.Update += e =>
            {
                // ...
            };

            shapes.ForEach(s => scene.Add(s));
            scene.Add(floor);
            scene.Add(wall1);
            scene.Add(wall2);
            scene.Add(aim);
            scene.Add(ceiling);

            game.ActiveScene = scene;
            game.Start();
        }

        public void StressTest()
        {
            Random rng = new Random();

            Game game = new Game(1024, 768, false, "Stress Test", 60);
            Scene scene = new Scene();

            float rectsX = 20, rectsY = 20;
            float rectsWidth = 1024 / rectsX;
            float rectsHeight = 1024 / rectsY;

            for (int u = 0; u < rectsX; u++)
            {
                for (int v = 0; v < rectsY; v++)
                {
                    Shapes.Rectangle rect = new Shapes.Rectangle()
                    {
                        Position = new PointF(u * rectsWidth, v * rectsHeight),
                        Width = rectsWidth,
                        Height = rectsHeight,
                        FillColor = rng.NextColor()
                    };

                    scene.Add(rect);
                }
            }

            Shapes.Rectangle floor = new Shapes.Rectangle()
            {
                Position = new PointF(0, -100),
                Width = 1024,
                Height = 60,
                Physics = new Physics(false, 10, 0, 1f),
                Tag = "FLOOR"
            };
            scene.Add(floor);

            double delta = 0;
            scene.Update += e =>
            {
                delta = e.Time;
                Console.WriteLine(1 / delta);
            };

            int tEllapsed = 0;
            Timer t = new Timer(state =>
            {
                if (tEllapsed >= 5200)
                    scene.Shapes.ForEach(s => s.Physics = new Physics(true, 1, .5f, 1));
                else
                {
                    scene.Shapes.Where(s => s.Tag == null).ToList().ForEach(s => s.Rotation += 1f * (float)delta);
                }
                tEllapsed += 100;
            }, null, 1000, 100);

            game.ActiveScene = scene;
            game.Start();
        }

        public void MarchingSquaresTest()
        {
            Game game = new Game(1024, 768, false, "Marching Cubes", 60);
            Scene scene = new Scene() { BackgroundColor = Color.LightGray };

            Polygon pol = new Polygon() { Position = new PointF(1024 / 2, 768 / 2) };
            pol.Points.Add(new PointF(-120, -100));
            pol.Points.Add(new PointF(100, -100));
            pol.Points.Add(new PointF(100, 80));
            pol.Points.Add(new PointF(50, 20));
            pol.Points.Add(new PointF(-100, 100));

            //scene.Add(pol);

            Ellipse eC = new Ellipse()
            {
                Position = new PointF(pol.Center.X, pol.Center.Y),
                Width = 8,
                Height = 8,
                FillColor = Color.Pink
            };
            //scene.Add(eC);

            foreach (PointF p in pol.Points)
            {
                Ellipse e = new Ellipse()
                {
                    Position = new PointF(p.X, p.Y),
                    Width = 8,
                    Height = 8,
                    FillColor = Color.Pink
                };

                //scene.Add(e);
            }

            Random rng = new Random();

            int cellsX = 30;
            int cellsY = 30;

            Cell[,] cells = new Cell[cellsX, cellsY];
            UberCell[,] ubercells = new UberCell[cellsX / 2, cellsY / 2];
            for (int u = 0; u < cellsX; u++)
            {
                for (int v = 0; v < cellsY; v++)
                {
                    Cell c = new Cell()
                    {
                        PosX = 320 + u * 10,
                        PosY = 250 + v * 10
                    };
                    // if (u > 5 && u < 25 && v > 5 && v < 25)
                    c.Value = 0;
                    //else
                    //  c.Value = 0;

                    cells[u, v] = c;
                }
            }

            int rU = 0;
            int rV = 0;
            for (int u = 0; u < cellsX - 1; u += 2)
            {
                rV = 0;
                for (int v = 0; v < cellsY - 1; v += 2)
                {
                    ubercells[rU, rV] = new UberCell()
                    {
                        TopLeft = cells[u, v],
                        TopRight = cells[u + 1, v],
                        BottomLeft = cells[u, v + 1],
                        BottomRight = cells[u + 1, v + 1],
                        PosX = cells[u, v].PosX,
                        PosY = cells[u, v].PosY
                    };

                    rV++;
                }
                rU++;
            }

            List<Shapes.Rectangle> cellRects = new List<Shapes.Rectangle>();
            foreach (Cell cell in cells)
            {
                Shapes.Rectangle r = new Shapes.Rectangle()
                {
                    Position = new PointF(cell.PosY, cell.PosX),
                    Width = 9,
                    Height = 9,
                    FillColor = Color.FromArgb(cell.Value, 20, 20),
                    Tag = cell
                };

                cellRects.Add(r);
                scene.Shapes.Add(r);
            }

            List<Shapes.Rectangle> ubercellRects = new List<Shapes.Rectangle>();

            foreach (UberCell uCell in ubercells)
            {
                Shapes.Rectangle rect = new Shapes.Rectangle()
                {
                    Position = new PointF(uCell.PosX, uCell.PosY),
                    Width = 19,
                    Height = 19,
                    Visible = false
                };

                ubercellRects.Add(rect);
                scene.Shapes.Add(rect);
            }

            List<PointF> isoPoints = new List<PointF>();
            foreach (UberCell uCell in ubercells)
            {
                bool tl = uCell.TopLeft.Value > 100;
                bool tr = uCell.TopRight.Value > 100;
                bool bl = uCell.BottomLeft.Value > 100;
                bool br = uCell.BottomRight.Value > 100;

                if (!tl && !tr && !bl && !br)
                {
                    // ...
                }
                else if (tl && tr && bl && br)
                {
                    // ...
                }
                else if ((!tl && !tr && bl && !br) || (tl && tr && !bl && br))
                {
                    isoPoints.Add(new PointF(uCell.PosX, uCell.HalfY));
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.FullY));
                }
                else if ((tl && !tr && !bl && !br) || (!tl && tr && bl && br))
                {
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.PosY));
                    isoPoints.Add(new PointF(uCell.PosX, uCell.HalfY));
                }
                else if ((!tl && tr && !bl && !br) || (tl && !tr && bl && br))
                {
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.PosY));
                    isoPoints.Add(new PointF(uCell.FullX, uCell.HalfY));
                }
                else if ((!tl && !tr && !bl && br) || (tl && tr && bl && !br))
                {
                    isoPoints.Add(new PointF(uCell.FullX, uCell.HalfY));
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.FullY));
                }
                else if ((tl && tr && !bl && !br) || (!tl && !tr && bl && br))
                {
                    isoPoints.Add(new PointF(uCell.PosX, uCell.HalfY));
                    isoPoints.Add(new PointF(uCell.FullX, uCell.HalfY));
                }
                else if ((tl && !tr && bl && !br) || (!tl && tr && !bl && br))
                {
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.PosY));
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.FullY));
                }
                else
                {
                    // ...
                }
            }

            List<Ellipse> isoCircles = new List<Ellipse>();
            foreach (PointF p in isoPoints)
            {
                Ellipse e = new Ellipse()
                {
                    Position = p,
                    Width = 4,
                    Height = 4,
                    FillColor = Color.Pink
                };

                isoCircles.Add(e);
                scene.Shapes.Add(e);
            }

            Ellipse crosshair = new Ellipse()
            {
                Width = 30,
                Height = 30,
                FillColor = Color.Cyan
            };
            scene.Add(crosshair);

            bool keyRight = false;
            bool keyLeft = false;
            bool keyDown = false;
            bool keyUp = false;
            bool keyE = false;
            bool keyQ = false;
            game.KeyDown += args =>
            {
                switch (args.Key)
                {
                    case Key.Right:
                        keyRight = true;
                        break;
                    case Key.Left:
                        keyLeft = true;
                        break;
                    case Key.Up:
                        keyUp = true;
                        break;
                    case Key.Down:
                        keyDown = true;
                        break;
                    case Key.E:
                        keyE = true;
                        break;
                    case Key.Q:
                        keyQ = true;
                        break;
                    case Key.H:
                        ubercellRects.ForEach(r => r.Visible = !r.Visible);
                        break;
                }
            };
            game.KeyUp += args =>
            {
                switch (args.Key)
                {
                    case Key.Right:
                        keyRight = false;
                        break;
                    case Key.Left:
                        keyLeft = false;
                        break;
                    case Key.Up:
                        keyUp = false;
                        break;
                    case Key.Down:
                        keyDown = false;
                        break;
                    case Key.E:
                        keyE = false;
                        break;
                    case Key.Q:
                        keyQ = false;
                        break;
                }
            };

            game.MouseMove += args =>
            {
                crosshair.Position = new PointF(args.X, 768 - args.Y);
            };

            Polygon isoshape = new Polygon();
            scene.Add(isoshape);

            game.MouseDown += args =>
            {
                foreach (Shapes.Rectangle r in cellRects)
                {
                    if (crosshair.Contains(r.Center))
                    {
                        if (args.Button == MouseButton.Left)
                            (r.Tag as Cell).Value -= 255;
                        else
                            (r.Tag as Cell).Value += 255;
                    }
                }

                redrawCells(cellRects, ref scene);
                isoCircles.ForEach(c => scene.Shapes.Remove(c));
                //isoCircles = marchingSquares(ubercells).Select(n => new Ellipse()
                //{
                //    Position = new PointF(n.X, n.Y),
                //    Width = 8,
                //    Height = 8,
                //    FillColor = Color.Yellow
                //}).ToList();
                isoshape.Points = new List<PointF>();
                marchingSquares(ubercells).ForEach(p => isoshape.Points.Add(p));
                //scene.Shapes.AddRange(isoCircles);
            };

            float movementSpeed = 2f;
            scene.Update += e =>
            {
                float delta = (float)e.Time;

                if (keyRight)
                    pol.Position = pol.Position.Translate(movementSpeed, 0);
                if (keyLeft)
                    pol.Position = pol.Position.Translate(-movementSpeed, 0);
                if (keyUp)
                    pol.Position = pol.Position.Translate(0, movementSpeed);
                if (keyDown)
                    pol.Position = pol.Position.Translate(0, -movementSpeed);
                if (keyE)
                    pol.Rotation += .1f * delta;
                if (keyQ)
                    pol.Rotation -= .1f * delta;
            };

            game.ActiveScene = scene;
            game.Start();
        }

        public void PolygonTest()
        {
            Game game = new Game(1024, 768, false, "Polygons", 60);
            Scene scene = new Scene();

            Random rng = new Random();

            bool placing = false;
            List<PointF> pointsPlaced = new List<PointF>();

            game.KeyDown += args =>
            {
                if (args.Key.Equals(Key.Enter))
                {
                    if (!placing)
                    {
                        placing = true;
                        pointsPlaced = new List<PointF>();
                        scene.BackgroundColor = Color.BlueViolet;
                    }
                    else
                    {
                        placing = false;
                        if (pointsPlaced.Count > 1)
                        {
                            Polygon p = new Polygon();
                            p.Points.AddRange(pointsPlaced);
                            scene.Shapes.Add(p);
                        }
                        scene.BackgroundColor = Color.Black;
                    }
                }
            };

            bool mouseLeftDown = false;
            game.MouseDown += args =>
            {
                if (args.Button == MouseButton.Left)
                    mouseLeftDown = true;

                if (placing)
                {
                    pointsPlaced.Add(new PointF(args.X, 768 - args.Y));
                }
            };
            game.MouseUp += args =>
            {
                if (args.Button == MouseButton.Left)
                    mouseLeftDown = false;
            };

            float mouseDX = 0;
            float mouseDY = 0;
            PointF previousMousePos = new PointF(0, 0);

            List<Ellipse> pointEllipses = new List<Ellipse>();
            Color previousColor = Color.Black;
            BaseShape previousShape = null;
            Ellipse selectedEllipse = null;
            game.MouseMove += args =>
            {
                PointF mousePos = new PointF(args.X, 768 - args.Y);
                mouseDX = mousePos.X - previousMousePos.X;
                mouseDY = mousePos.Y - previousMousePos.Y;
                previousMousePos = mousePos;

                Console.WriteLine(mouseDX + ", " + mouseDY);

                if (selectedEllipse != null && mouseLeftDown)
                {
                    selectedEllipse.Position = new PointF(selectedEllipse.X + mouseDX, selectedEllipse.Y + mouseDY);
                    Polygon p = (selectedEllipse.Tag as Tuple<Polygon, int>).Item1;
                    int i = (selectedEllipse.Tag as Tuple<Polygon, int>).Item2;

                    p.Points[i] = selectedEllipse.Position;
                }

                BaseShape hit = scene.Shapes.FirstOrDefault(s => s.Contains(mousePos));
                if (hit == null)
                {
                    if (previousShape != null)
                    {
                        previousShape.FillColor = previousColor;
                        previousShape = null;
                        foreach (Ellipse e in pointEllipses)
                            scene.Shapes.Remove(e);
                    }
                }

                if (hit is Ellipse)
                    selectedEllipse = hit as Ellipse;
                else
                {
                    selectedEllipse = null;

                    if (hit != previousShape)
                    {
                        if (previousShape == null)
                        {
                            previousShape = hit;
                            previousColor = hit.FillColor;
                            previousShape.FillColor = Color.Gold;

                            pointEllipses = new List<Ellipse>();
                            foreach (PointF p in (hit as Polygon).Points)
                            {
                                Ellipse e = new Ellipse()
                                {
                                    Position = p,
                                    Width = 12,
                                    Height = 12,
                                    FillColor = Color.Green,
                                    Tag = Tuple.Create((hit as Polygon), (hit as Polygon).Points.IndexOf(p))
                                };

                                pointEllipses.Add(e);
                            }

                            scene.Shapes.AddRange(pointEllipses);
                        }
                    }
                }
            };

            game.MouseWheelChanged += args =>
            {
                PointF mousePos = new PointF(args.X, 768 - args.Y);
                BaseShape hit = scene.Shapes.FirstOrDefault(s => s.Contains(mousePos));
                if (hit == null)
                    return;

                hit.FillColor = rng.NextColor();
                previousColor = hit.FillColor;
            };

            game.ActiveScene = scene;
            game.Start();
        }

        private List<PointF> marchingSquares(UberCell[,] ubercells)
        {
            List<PointF> isoPoints = new List<PointF>();
            foreach (UberCell uCell in ubercells)
            {
                bool tl = !(uCell.TopLeft.Value > 100);
                bool tr = !(uCell.TopRight.Value > 100);
                bool bl = !(uCell.BottomLeft.Value > 100);
                bool br = !(uCell.BottomRight.Value > 100);

                if (!tl && !tr && !bl && !br)
                {
                    // ...
                }
                else if (tl && tr && bl && br)
                {
                    // ...
                }
                else if ((!tl && !tr && bl && !br) || (tl && tr && !bl && br))
                {
                    isoPoints.Add(new PointF(uCell.PosX, uCell.HalfY));
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.FullY));
                }
                else if ((tl && !tr && !bl && !br) || (!tl && tr && bl && br))
                {
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.PosY));
                    isoPoints.Add(new PointF(uCell.PosX, uCell.HalfY));
                }
                else if ((!tl && tr && !bl && !br) || (tl && !tr && bl && br))
                {
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.PosY));
                    isoPoints.Add(new PointF(uCell.FullX, uCell.HalfY));
                }
                else if ((!tl && !tr && !bl && br) || (tl && tr && bl && !br))
                {
                    isoPoints.Add(new PointF(uCell.FullX, uCell.HalfY));
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.FullY));
                }
                else if ((tl && tr && !bl && !br) || (!tl && !tr && bl && br))
                {
                    isoPoints.Add(new PointF(uCell.PosX, uCell.HalfY));
                    isoPoints.Add(new PointF(uCell.FullX, uCell.HalfY));
                }
                else if ((tl && !tr && bl && !br) || (!tl && tr && !bl && br))
                {
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.PosY));
                    isoPoints.Add(new PointF(uCell.HalfX, uCell.FullY));
                }
                else
                {
                    // ...
                }
            }

            return isoPoints;
        }

        private void redrawCells(List<Shapes.Rectangle> rects, ref Scene scene)
        {
            foreach (var r in rects)
                r.FillColor = Color.FromArgb(
                    Math.Min(Math.Max((r.Tag as Cell).Value, 0), 255),
                    20, 20);
        }

        private class Cell
        {
            public int Value { get; set; }
            public float PosX { get; set; }
            public float PosY { get; set; }
        }

        private class UberCell
        {
            public Cell TopLeft { get; set; }
            public Cell TopRight { get; set; }
            public Cell BottomLeft { get; set; }
            public Cell BottomRight { get; set; }

            public float PosX { get; set; }
            public float PosY { get; set; }

            public float HalfX
            {
                get { return PosX + 10; }
            }

            public float FullX
            {
                get { return PosX + 20; }
            }

            public float HalfY
            {
                get { return PosY + 10; }
            }

            public float FullY
            {
                get { return PosY + 20; }
            }
        }
    }

    public static class Extensions
    {
        public static Color NextColor(this Random rng) => Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256));
        public static Color NextGreyscale(this Random rng)
        {
            int value = rng.Next(256);
            return Color.FromArgb(value, value, value);
        }
        public static PointF Translate(this PointF point, float dX, float dY) => new PointF(point.X + dX, point.Y + dY);
    }
}
