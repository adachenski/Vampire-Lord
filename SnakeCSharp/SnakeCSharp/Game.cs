
// TODO: Някъде да се сложат две try{}catch{} конструкции. Най-лесно май ще е в метода
// (несъществуващ все още) за писане в текстов файл на резултат при GameOver(ред 70 и нещо);
// Със сигурност може да се поправи, бая мазало стана кода, дано е що-годе разбираем.

namespace SnakeCSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO;

    class Game
    {
        const char snakeSymbol = '*';
        const char obstacleSymbol = 'O';
        const char foodSymbol = 'F';
        const char poisonFood = 'P';
        const ConsoleColor snakeColor = ConsoleColor.White;
        const ConsoleColor obstacleColor = ConsoleColor.Yellow;
        const ConsoleColor foodColor = ConsoleColor.Green;
        const ConsoleColor poisonColor = ConsoleColor.Red;

        static GameObject[] directions = new GameObject[]
            {
                new GameObject(1, 0), // right
                new GameObject(-1, 0), // left
                new GameObject(0, -1), // up
                new GameObject(0, 1), // down
            };
        enum Commands
        {
            right,
            left,
            up,
            down
        }

        static Queue<GameObject> snakeBody = new Queue<GameObject>();

        static Random randomNumberGenerator = new Random();

        static int level = 1;
        static int fullScore = 0;
        static int levelScore = 0;

        static void Main()
        {
            int timeSleep = 200;
            LoadingGame();
            // Някакво старт меню може да се направи преди да се нарисува полето, ако на някой му
            // се занимава - пак си е допълнителен метод :) Примерно - Press 1 to start game. 
            // Press 2 to see high scores. Press 3 to exit. Нещо такова.
            InitiateGameField();


            int command = (int)Commands.right;
            bool[,] obstacleCoordinates = new bool[Console.WindowHeight, Console.WindowWidth];
            GenerateObstacles(obstacleCoordinates);
            List<GameObject> obstacles = GetObstacles(obstacleCoordinates);
            PrintObstacles(obstacles);
            GameObject food = GenerateFood(obstacles);
            GameObject poison = GeneratePoisonFood(obstacles, food);
            food.Print(foodSymbol, foodColor);
            poison.Print(poisonFood, poisonColor);
            while (true)
            {

                if (Console.KeyAvailable)
                {
                    command = GetDirectionFromKeyboard(command);
                }
                GameObject currentSnakeHead = snakeBody.Last();
                if (currentSnakeHead.Equals(poison))
                {
                    levelScore -= 50;
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Scores = {0}", fullScore + levelScore);
                    FeedSnake(command);
                    poison = GenerateFood(obstacles);
                    poison.Print(poisonFood, poisonColor);

                }
                if (currentSnakeHead.Equals(food))
                {

                    levelScore += 50;
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Scores = {0}", fullScore + levelScore);
                    FeedSnake(command);
                    food = GenerateFood(obstacles);
                    food.Print(foodSymbol, foodColor);
                }

                bool gameOver = MoveSnake(command, obstacles) || fullScore < 0;
                if (gameOver)
                {

                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Game over".PadRight(Console.WindowWidth));
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("Scores: {0}".PadRight(Console.WindowWidth + 2), fullScore + levelScore);
                    Console.SetCursorPosition(0, 2);
                    Console.Write("Enter username:");
                    string user = Console.ReadLine();
                    Console.WriteLine();

                    // Writing of result(asking user for name) in text file result.txt.
                    // streamWriter.WriteLine() so it keeps previous results.)
                    // Нещо с DateTime, примерно колко време е играно, или пък с резултата да се 
                    // принтира и датата, колкото да има употреба на класа в проекта, другите 
                    // 2 .NET класа са ни Random и Thread.
                    return;
                }
                Thread.Sleep(timeSleep);
                if (levelScore == 100)// Тук може да си поиграе човек да измисли кога да
                //почва следващото ниво(сега е на 2 изядени "храни", колкото за тестване).
                // Също - да се прави някаква промяна на скоростта в зависимост от нивото. 
                {
                    level++;
                    fullScore += levelScore;
                    levelScore = 0;
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine(new string('-', Console.WindowWidth));
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Scores = {0}", fullScore + levelScore);
                    timeSleep -= level * 10;
                    int snakeLength = snakeBody.Count;
                    snakeBody.Clear();
                    InitializePrintNewSnake(snakeLength);
                    command = 0;
                    obstacleCoordinates = new bool[Console.WindowHeight, Console.WindowWidth];
                    GenerateObstacles(obstacleCoordinates);
                    obstacles = GetObstacles(obstacleCoordinates);
                    PrintObstacles(obstacles);
                    food = GenerateFood(obstacles);
                    food.Print(foodSymbol, foodColor);
                    poison = GenerateFood(obstacles);
                    poison.Print(poisonFood, poisonColor);
                    Thread.Sleep(1200);

                }
            }
        }
        static GameObject GeneratePoisonFood(List<GameObject> obstacles, GameObject food)
        {
            GameObject poison;
            while (true)
            {
                int vertical = randomNumberGenerator.Next(2, Console.WindowHeight);
                int horizontal = randomNumberGenerator.Next(0, Console.WindowWidth);
                poison = new GameObject(horizontal, vertical);
                if (!snakeBody.Contains(poison) && !obstacles.Contains(poison) && poison != food)
                {
                    return poison;
                }
            }
        }

        static bool DetectCollisions(List<GameObject> obstacles, GameObject newSnakeHead)
        {

            if (obstacles.Contains(newSnakeHead) || snakeBody.Contains(newSnakeHead))
            {
                return true;
            }
            return false;
        }

        static void FeedSnake(int command)
        {
            GameObject currentSnakeHead = snakeBody.Last();
            GameObject nextDirection = directions[command];
            GameObject newSnakeHead = new GameObject(currentSnakeHead.Horizontal +
              nextDirection.Horizontal, currentSnakeHead.Vertical + nextDirection.Vertical);
            CheckGameFieldBorders(newSnakeHead);

            currentSnakeHead.Print(snakeSymbol, snakeColor);
            snakeBody.Enqueue(newSnakeHead);
            newSnakeHead.Print(snakeSymbol, snakeColor);
        }

        static GameObject GenerateFood(List<GameObject> obstacles)
        {
            GameObject food;
            while (true)
            {
                int vertical = randomNumberGenerator.Next(2, Console.WindowHeight);
                int horizontal = randomNumberGenerator.Next(0, Console.WindowWidth);
                food = new GameObject(horizontal, vertical);
                if (!snakeBody.Contains(food) && !obstacles.Contains(food))
                {
                    return food;
                }
            }

        }

        private static List<GameObject> GetObstacles(bool[,] obstacleCoordinates)
        {
            List<GameObject> obstacles = new List<GameObject>();
            for (int row = 0; row < obstacleCoordinates.GetLength(0); row++)
            {
                for (int col = 0; col < obstacleCoordinates.GetLength(1); col++)
                {
                    if (obstacleCoordinates[row, col] == true)
                    {
                        obstacles.Add(new GameObject(col, row));
                    }
                }
            }
            return obstacles;
        }

        static void PrintObstacles(List<GameObject> obstacles)
        {
            foreach (var obstacle in obstacles)
            {
                for (int i = 0; i < level; i++)
                {
                    obstacle.Print(obstacleSymbol, obstacleColor);
                }
            }
        }

        static void GenerateObstacles(bool[,] obstacleCoordinates)
        {
            int obstacleCount = level * 4;
            for (int i = 0; i < obstacleCount; i++)
            {
                int row = randomNumberGenerator.Next(2, Console.WindowHeight);
                // Check starting row of snake, so there is no way obstacle lands on the snake body upon starting the game;
                if (row == 14)
                {
                    row++;
                }
                int col = randomNumberGenerator.Next(0, Console.WindowWidth);
                obstacleCoordinates[row, col] = true;
            }
        }

        static bool MoveSnake(int command, List<GameObject> obstacles)
        {
            GameObject currentSnakeHead = snakeBody.Last();
            GameObject nextDirection = directions[command];
            GameObject newSnakeHead = new GameObject(currentSnakeHead.Horizontal +
              nextDirection.Horizontal, currentSnakeHead.Vertical + nextDirection.Vertical);
            if (DetectCollisions(obstacles, newSnakeHead))
            {
                return true;
            }
            CheckGameFieldBorders(newSnakeHead);
            snakeBody.Enqueue(newSnakeHead);
            newSnakeHead.Print(snakeSymbol, snakeColor);

            GameObject last = snakeBody.Dequeue();
            last.Print(' ', snakeColor);
            return false;
        }

        static void CheckGameFieldBorders(GameObject newSnakeHead)
        {
            if (newSnakeHead.Horizontal < 0)
            {
                newSnakeHead.Horizontal = Console.WindowWidth - 1;
            }
            if (newSnakeHead.Vertical < 2)
            {
                newSnakeHead.Vertical = Console.WindowHeight - 1;
            }
            if (newSnakeHead.Vertical >= Console.WindowHeight)
            {
                newSnakeHead.Vertical = 2;
            }
            if (newSnakeHead.Horizontal >= Console.WindowWidth)
            {
                newSnakeHead.Horizontal = 0;
            }
        }

        static int GetDirectionFromKeyboard(int direction)
        {
            ConsoleKeyInfo pressedKey = Console.ReadKey(true);
            if (pressedKey.Key == ConsoleKey.LeftArrow)
            {
                if (direction != (int)Commands.right)
                {
                    direction = (int)Commands.left;
                }
            }
            if (pressedKey.Key == ConsoleKey.RightArrow)
            {
                if (direction != (int)Commands.left)
                {
                    direction = (int)Commands.right;
                }
            }
            if (pressedKey.Key == ConsoleKey.UpArrow)
            {
                if (direction != (int)Commands.down)
                {
                    direction = (int)Commands.up;
                }
            }
            if (pressedKey.Key == ConsoleKey.DownArrow)
            {
                if (direction != (int)Commands.up)
                {
                    direction = (int)Commands.down;
                }
            }
            return direction;
        }

        static void InitiateGameField()
        {
            // Set game field size, color and initial size and position of the snake:
            Console.SetWindowSize(50, 30);
            Console.BufferHeight = Console.WindowHeight + 1;
            Console.BufferWidth = Console.WindowWidth + 1;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, 1);
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Scores = {0}", fullScore + levelScore);
            Console.CursorVisible = false;
            Console.OutputEncoding = Encoding.Unicode;
            Console.Title = "Vampire Lord - Snake";

            for (int i = 0; i < 6; i++)
            {
                snakeBody.Enqueue(new GameObject(i, 14));
            }
            foreach (var element in snakeBody)
            {
                element.Print(snakeSymbol, snakeColor);
            }
        }

        static void InitializePrintNewSnake(int snakeLength)
        {
            for (int i = 0; i < snakeLength; i++)
            {
                snakeBody.Enqueue(new GameObject(i, 14));
            }
            foreach (var element in snakeBody)
            {
                element.Print(snakeSymbol, snakeColor);
            }
        }

        static void LoadingGame()
        {

            int start = 15, tempStart = start;
            int end = Console.WindowWidth - (start * 2) + 1;

            Console.SetCursorPosition(start * 2, 19);
            Console.WriteLine("LOADING :    %");
            for (int i = 0; i < end; i++)
            {
                Console.SetCursorPosition(start * 2 + 10, 19);
                Console.Write(i + i);
                Console.SetCursorPosition(tempStart, 20);
                Console.Write('.');
                Console.SetCursorPosition(tempStart, 21);
                Console.Write("|");
                Console.SetCursorPosition(tempStart, 22);
                Console.Write('\'');
                Thread.Sleep(100);
                if (i == end - 12)
                {
                    Thread.Sleep(1000);
                }
                ++tempStart;
            }
            Console.Clear();
        }
    }
}
