
namespace SnakeCSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO;
    using System.Media;

    class Game
    {
        const char snakeSymbol = '*';
        const char obstacleSymbol = '\u2588';
        const char foodSymbol = '\u2665';
        const char poisonFood = '\u2660';
        const ConsoleColor snakeColor = ConsoleColor.White;
        const ConsoleColor obstacleColor = ConsoleColor.Yellow;
        const ConsoleColor foodColor = ConsoleColor.Green;
        const ConsoleColor poisonColor = ConsoleColor.Red;
        public const string fileName = @"..\..\highscores.txt";

        static SoundPlayer bgrMusic = new SoundPlayer("..\\..\\Sounds\\backgroundMusic.wav");

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

        static DateTime showFood = DateTime.Now;
        public static bool exit = false;

        static void Main()
        {

            bgrMusic.Play();
            MainMenue.LoadingGame();
            MainMenue.SplashScreen();
            Console.Clear();
            Exit();
            if (exit == true)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                return;
            }
        }

        public static void Exit()
        {
            using (var reader = new StreamReader(@"..\..\Logos\GameOver.txt"))
            {
                Console.Title = "Vampire Lord - Snake";
                Console.SetWindowSize(77, 25);
                Console.SetBufferSize(77, 25);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetWindowSize(77, 25);
                string text = reader.ReadToEnd().ToString();
                Console.Clear();
                Console.WriteLine(text);
                Console.ReadLine();
            }
            exit = true;
        }

        public static void GamePlay()
        {
            bgrMusic.Stop();
            InitiateGameField();
            fullScore = 0;
            levelScore = 0;
            level = 1;
            int timeSleep = 100;
            int command = (int)Commands.right;
            bool[,] obstacleCoordinates = new bool[Console.WindowHeight, Console.WindowWidth];
            GenerateObstacles(obstacleCoordinates);
            List<GameObject> obstacles = GetObstacles(obstacleCoordinates);
            PrintObstacles(obstacles);
            GameObject food = GenerateFood(obstacles);
            food.Print(foodSymbol, foodColor);
            showFood = DateTime.Now;
            GameObject poison = GeneratePoisonFood(obstacles, food);
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
                    SoundPlayer poisonSound = new System.Media.SoundPlayer(@"..\\..\\Sounds\\poisonSound.wav");
                    poisonSound.Play();
                    levelScore -= 50;
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Scores = {0}", fullScore + levelScore);
                    Console.SetCursorPosition(Console.WindowWidth - 11, 0);
                    Console.WriteLine("Level = {0}", level);
                    FeedSnake(command);
                    poison = GenerateFood(obstacles);
                    poison.Print(poisonFood, poisonColor);

                }
                if (currentSnakeHead.Equals(food))
                {
                    SoundPlayer eatingSound = new System.Media.SoundPlayer(@"..\\..\\Sounds\\bitingSound.wav");
                    eatingSound.Play();
                    levelScore += 50;
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Scores = {0}", fullScore + levelScore);
                    Console.SetCursorPosition(Console.WindowWidth - 11, 0);
                    Console.WriteLine("Level = {0}", level);
                    FeedSnake(command);
                    food = GenerateFood(obstacles);
                    food.Print(foodSymbol, foodColor);
                    showFood = DateTime.Now;
                }

                bool gameOver = MoveSnake(command, obstacles) || fullScore + levelScore < 0;
                if (gameOver)
                {
                    if (fullScore + levelScore < 0)
                    {
                        fullScore = 0;
                    }
                    else
                    {
                        fullScore += levelScore;
                    }
                    SoundPlayer gameOverSound = new System.Media.SoundPlayer(@"..\\..\\Sounds\\gameOverSound.wav");
                    gameOverSound.Play();
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Game over".PadRight(Console.WindowWidth));
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("Scores: {0}".PadRight(Console.WindowWidth + 2), fullScore);
                    Console.SetCursorPosition(0, 2);
                    Console.Write("Enter username:");
                    string user = Console.ReadLine();
                    user = user.Replace(" ", "_");
                    Console.WriteLine();
                    WriteToFile(fullScore, user);
                    Console.Clear();
                    MainMenue.WriteTopScores();
                    gameOverSound.Stop();
                    snakeBody.Clear();
                    MainMenue.StartMenueOptions();
                    bgrMusic.PlayLooping();
                    return;
                }
                Thread.Sleep(timeSleep);

                bool TooOldFood = DeleteFoodAfterTime(showFood, food, obstacles);
                if (TooOldFood)
                {
                    food = GenerateFood(obstacles);
                    food.Print(foodSymbol, foodColor);
                    TooOldFood = false;
                    showFood = DateTime.Now;
                }

                if (levelScore == 100)// Not very challenging for demonstration purposes.
                {
                    SoundPlayer changeLevelSound = new SoundPlayer(@"..\\..\\Sounds\\nextLevelSound.wav");
                    changeLevelSound.Play();
                    level++;
                    fullScore += levelScore;
                    levelScore = 0;
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine(new string('-', Console.WindowWidth));
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Scores = {0}", fullScore + levelScore);
                    Console.SetCursorPosition(Console.WindowWidth - 11, 0);
                    Console.WriteLine("Level = {0}", level);
                    timeSleep -= 5;
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
                    showFood = DateTime.Now;
                }
            }
        }

        private static bool DeleteFoodAfterTime(DateTime showFood, GameObject food, List<GameObject> obstacles)
        {
            DateTime hideFood = DateTime.Now;
            int h = food.Horizontal;
            int w = food.Vertical;
            TimeSpan timer = new TimeSpan(0, 0, 8);

            if (hideFood - showFood >= timer)
            {
                obstacles.Remove(food);
                food.Print(' ', foodColor);
                return true;
            }
            else
            {
                return false;
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
                obstacle.Print(obstacleSymbol, obstacleColor);
            }
        }

        static void GenerateObstacles(bool[,] obstacleCoordinates)
        {
            int obstacleCount = level * 4;
            for (int i = 0; i < obstacleCount; i++)
            {
                int row = randomNumberGenerator.Next(2, Console.WindowHeight);
                
                if (row == Console.WindowHeight / 2)
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
            try
            {
                ConsoleKeyInfo pressedKey = Console.ReadKey(true);
                if (pressedKey.Key == ConsoleKey.LeftArrow)
                {
                    if (direction != (int)Commands.right)
                    {
                        direction = (int)Commands.left;
                    }
                }
                else if (pressedKey.Key == ConsoleKey.RightArrow)
                {
                    if (direction != (int)Commands.left)
                    {
                        direction = (int)Commands.right;
                    }
                }
                else if (pressedKey.Key == ConsoleKey.UpArrow)
                {
                    if (direction != (int)Commands.down)
                    {
                        direction = (int)Commands.up;
                    }
                }
                else if (pressedKey.Key == ConsoleKey.DownArrow)
                {
                    if (direction != (int)Commands.up)
                    {
                        direction = (int)Commands.down;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Invalid key. To play - use only the arrow keys. Press any key to go to the main menu.");
                }
            }
            catch(InvalidOperationException exc)
            {
                Console.Clear();
                Console.WriteLine(exc.Message);
                Console.ReadKey();
                snakeBody.Clear();
                MainMenue.StartMenueOptions();
            }

            return direction;
        }

        public static void InitiateGameField()
        {
            // Set game field size, color and initial size and position of the snake:
            Console.SetWindowSize(60, 25);
            Console.BufferHeight = Console.WindowHeight + 1;
            Console.BufferWidth = Console.WindowWidth + 1;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, 1);
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.SetCursorPosition(0, 0);
            Console.Write("Scores = {0}", 0);
            Console.SetCursorPosition(Console.WindowWidth - 11, 0);
            Console.WriteLine("Level = {0}", 1);
            Console.CursorVisible = false;
            Console.OutputEncoding = Encoding.Unicode;
            Console.Title = "Vampire Lord - Snake";

            for (int i = 0; i < 6; i++)
            {
                snakeBody.Enqueue(new GameObject(i, Console.WindowHeight / 2));
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
                snakeBody.Enqueue(new GameObject(i, Console.WindowHeight / 2));
            }
            foreach (var element in snakeBody)
            {
                element.Print(snakeSymbol, snakeColor);
            }
        }

        public static Dictionary<string, int> ReadFromFile(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            var highScores = new Dictionary<string, int>();
            string[] splittedLine;

            using (reader)
            {               
                string line = reader.ReadToEnd();
                if (line == string.Empty)
                {
                    return highScores;
                }
                else
                {
                    string[] lines = line.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in lines)
                    {
                        splittedLine = item.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                        highScores.Add(splittedLine[1], int.Parse(splittedLine[0]));
                    }
                }
            }
            return highScores;
        }

        static void WriteToFile(int highScore, string userName)
        {
            DateTime now = DateTime.Now;
            try
            {
                var highScores = ReadFromFile(fileName);

                string template = "{0}/{1:dd.MM.yyyy}";
                string newResult = String.Format(template, userName, now.Date);
                                
                var sortedScores = highScores.OrderByDescending(x => x.Value);

                if (sortedScores.Count() > 4)
                {
                    highScores.Remove(sortedScores.Last().Key);
                }

                highScores.Add(newResult, highScore);
                sortedScores = highScores.OrderByDescending(x => x.Value);

                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    foreach (var score in sortedScores)
                    {
                        writer.WriteLine(score.Value + " - " + score.Key);
                    }
                }
            }
            catch (FileNotFoundException fne)
            {
                Console.WriteLine(fne.Message);
                Console.ReadKey();
            }
            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(ioe.Message);
                Console.ReadKey();
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}
