using System;
namespace RobotCleaner
{
    public class Map
    {
        private enum CellType { Empty, Dirt, Obstacle, Cleaned };
        private CellType[,] _grid;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            _grid = new CellType[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _grid[x, y] = CellType.Empty;
                }
            }
            
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < this.Width && y >= 0 && y < this.Height;
        }

        public bool IsDirt(int x, int y)
        {
            return IsInBounds(x, y) && _grid[x, y] == CellType.Dirt;
        }

        public bool IsObstacle(int x, int y)
        {
            return IsInBounds(x, y) && _grid[x, y] == CellType.Obstacle;
        }

        public void AddObstacle(int x, int y)
        {
            _grid[x, y] = CellType.Obstacle;
        }
        public void AddDirt(int x, int y)
        {
            _grid[x, y] = CellType.Dirt;
        }

        public void Clean(int x, int y)
        {
            if (IsInBounds(x, y))
            {
                _grid[x, y] = CellType.Cleaned;
            }
        }
        public void Display(int robotX, int robotY)
        {
            // display the 2d grid, it accepts the location of the robot in x and y
            Console.Clear();
            Console.WriteLine("Vacuum cleaner robot simulation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Legends: #=Obstacles, D=Dirt, .=Empty, R=Robot, C=Cleaned");

            //display the grid using loop
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    if (x == robotX && y == robotY)
                    {
                        Console.Write("R ");
                    }
                    else
                    {
                        switch (_grid[x, y])
                        {
                            case CellType.Empty: Console.Write(". "); break;
                            case CellType.Dirt: Console.Write("D "); break;
                            case CellType.Obstacle: Console.Write("# "); break;
                            case CellType.Cleaned: Console.Write("C "); break;
                        }
                    }
                }
                Console.WriteLine();
            }

            Thread.Sleep(200);
        }
    }
    public interface IStrategy
    {
        void Clean(Robot robot);
    }

    public class Robot
    {
        private readonly Map _map;
        private readonly IStrategy _strategy;

        public int X { get; set; }
        public int Y { get; set; }

        public Map Map { get { return _map; } }

        public Robot(Map map, IStrategy strategy)
        {
            _map = map;
            _strategy = strategy;
            X = 0;
            Y = 0;
        }

        public bool Move(int newX, int newY)
        {
            if (_map.IsInBounds(newX, newY))
            {
                // set the new location
                X = newX;
                Y = newY;
                // display the map with the robot in its location in the grid
                _map.Display(X, Y);
                return true;
            }
            // it cannot move
            return false;
        }// Move method

        public void CleanCurrentSpot()
        {
            if (_map.IsDirt(X, Y))
            {
                _map.Clean(X, Y);
                _map.Display(X, Y);
            }
        }

        public void StartCleaning()
        {
            _strategy.Clean(this);
        }
    }


    public class PerimeterHuggerStrategy : IStrategy
    {
        public void Clean(Robot robot)
        {
            Console.WriteLine("Start Perimeter Hugger...");
            while (robot.Move(robot.X + 1, robot.Y)) // Move Right
            {
                robot.CleanCurrentSpot();
            }
            while (robot.Move(robot.X, robot.Y + 1)) // Move Down
            {
                robot.CleanCurrentSpot();
            }
            while (robot.Move(robot.X - 1, robot.Y)) // Move Left
            {
                robot.CleanCurrentSpot();
            }
            while (robot.Move(robot.X, robot.Y - 1)) // Move Up
            {
                robot.CleanCurrentSpot();
            }
            Console.WriteLine("End Perimeter Hugger...");

        }
    }
    public class SpiralStrategy : IStrategy
    {
        public void Clean(Robot robot)
        {
            robot.CleanCurrentSpot();

            int direction = 0;
            int segmentLength = 1;
            int segmentsPassed = 0;
            for (int i = 0; i < robot.Map.Width + robot.Map.Height; i++)
            {
                for (int j = 0; j < segmentLength; j++)
                {
                    switch (direction)
                    {
                        case 0: // Right
                            robot.Move(robot.X + 1, robot.Y);
                            break;
                        case 1: // Down
                            robot.Move(robot.X, robot.Y + 1);
                            break;
                        case 2: // Left
                            robot.Move(robot.X - 1, robot.Y);
                            break;
                        case 3: // Up
                            robot.Move(robot.X, robot.Y - 1);
                            break;
                    }
                    robot.CleanCurrentSpot();
                }
                direction = (direction + 1) % 4;
                segmentsPassed++;

                if (segmentsPassed % 2 == 0)
                {
                    segmentLength++;
                }
            }
        }
    }

    public class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Initialize robot");

            IStrategy strategy = new PerimeterHuggerStrategy();
            IStrategy strategy1 = new SpiralStrategy();

            Map map = new Map(20, 10);
            map.AddDirt(5, 3);
            map.AddDirt(10, 8);
            map.AddDirt(3, 1);
            map.AddObstacle(2, 5);
            map.AddObstacle(12, 1);

            Robot robot1 = new Robot(map, strategy); // Robot using PerimeterHuggerStrategy
            robot1.StartCleaning();

           
            int middleMapPosX = map.Width / 2;
            int middleMapPosY = map.Height / 2;

            Robot robot = new Robot(map, strategy1); // Robot using SpiralStrategy
            robot.Move(middleMapPosX, middleMapPosY);


            robot.StartCleaning();
            Console.WriteLine("Done.");
        }
    }
}

