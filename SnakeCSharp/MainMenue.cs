
namespace SnakeCSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Media;
    using System.IO;

    public class MainMenue
    {
        public static void LoadingGame()
        {

            Console.CursorVisible = false;
            int start = 15, tempStart = start;
            int end = Console.WindowWidth - (start * 2) + 1;
            Console.SetCursorPosition(start * 2, 19);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("LOADING :    %");
            Console.ForegroundColor = ConsoleColor.Red;
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
            //SplashScreen();


        }
        public static void SplashScreen()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.Clear();

            char[,] telerikLogo = new char[,] {
            {'X','\0','\0','\0','\0','\0','\0','\0','X','\0','X','\0','\0','\0','\0','\0','\0','\0','\0','\0'},
            {'X','\0','\0','\0','\0','\0','\0','\0','X','\0','X','\0','\0','\0','\0','\0','\0','\0','\0','\0'},
            {'\0','X','\0','\0','\0','\0','\0','X','\0','\0','X','\0','\0','\0','\0','\0','\0','\0','\0','\0'},
            {'\0','X','\0','\0','\0','\0','\0','X','\0','\0','X','\0','\0','\0','\0','\0','\0','\0','\0','\0'},
            {'\0','\0','X','\0','\0','\0','X','\0','\0','\0','X','\0','\0','\0','\0','\0','\0','\0','\0','\0'},
            {'\0','\0','X','\0','\0','\0','X','\0','\0','\0','X','\0','\0','\0','\0','\0','\0','\0','\0','\0'},
            {'\0','\0','\0','X','\0','X','\0','\0','\0','\0','X','\0','\0','\0','\0','\0','\0','\0','\0','\0'},
            {'\0','\0','\0','X','\0','X','\0','\0','\0','\0','X','\0','\0','\0','\0','\0','\0','\0','\0','\0'},
            {'\0','\0','\0','X','X','X','\0','\0','\0','\0' ,'X','X','X','X','X','X','X','\0','\0','\0' },
            
            };
            Console.WriteLine();
            for (int i = 0; i < telerikLogo.GetLength(0); i++)
            {
                Console.Write("      ");
                for (int j = 0; j < telerikLogo.GetLength(1); j++)
                {
                    if (telerikLogo[i, j] == '\0')
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write("X");
                    }
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("\n\n     ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("Vampire Lord Team \n");
            Console.Write("\n    ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Green;

            Console.Write("Telerik Corporation\n\n");
            Console.ReadKey();
            // Console.BackgroundColor = ConsoleColor.Black;
            // Console.Clear();
            StartMenueOptions();
        }

        public static void StartMenueOptions()
        {
            SoundPlayer bgrMusic = new SoundPlayer("..\\..\\backgroundMusic.wav");
            bgrMusic.PlayLooping();
            bool quitRequested = false;
            while (!quitRequested)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.CursorVisible = false;
                Console.Clear();
                int selectedOptionIndex = 0;

                Console.WriteLine("\n\n\n\n\n");
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.CursorLeft = 6;
                Console.Write(" The Snake \n\n");
                string[] mainMenuOptions = { "Play Game", "Top score", "Developers", "Quit Game" };
                int menuTopPosition = Console.CursorTop;

                PrintMenuOptions(mainMenuOptions, 0, menuTopPosition);
                bool reloadTheWholeMenu = false;
                while (!reloadTheWholeMenu && !quitRequested)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                        case ConsoleKey.Spacebar:
                            switch (selectedOptionIndex)
                            {
                                case 0:

                                    reloadTheWholeMenu = true;
                                    Console.Clear();
                                    Game.GamePlay();
                                    return;
                                case 1:
                                    using (var reader = new StreamReader(@"..\..\result.txt"))
                                    {
                                        string text = reader.ReadToEnd().ToString();                                    
                                        Console.Clear();
                                        Console.WriteLine(text);
                                        Console.ReadLine();
                                    }
                                    reloadTheWholeMenu = true;
                                    break;
                                case 2:
                                    Developers();
                                    reloadTheWholeMenu = true;
                                    break;
                                case 3:
                                    Game.Exit();
                                    quitRequested = true;

                                    return;
                            }
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.RightArrow:
                            selectedOptionIndex++;
                            if (selectedOptionIndex == mainMenuOptions.Length)
                                selectedOptionIndex = 0;
                            PrintMenuOptions(mainMenuOptions, selectedOptionIndex, menuTopPosition);
                            break;
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.LeftArrow:
                            selectedOptionIndex--;
                            if (selectedOptionIndex == -1)
                                selectedOptionIndex = mainMenuOptions.Length - 1;
                            PrintMenuOptions(mainMenuOptions, selectedOptionIndex, menuTopPosition);
                            break;
                        case ConsoleKey.Escape:
                            quitRequested = true;
                            break;
                    }
                }
            }
        }
        static void PrintMenuOptions(string[] options, int selectedOption, int top)
        {
            Console.CursorVisible = false;
            Console.CursorTop = top;
            Console.CursorLeft = 0;
            for (int option = 0; option < options.Length; option++)
            {
                if (option == selectedOption)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.WriteLine("{0,6}{1}", "", options[option]);
            }
        }
        static void Developers()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("                       ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Tsvetan Razsolkov \nGeorgi Malkovski \nEsa Vehmanen "
                + "\nAtanas Dachenski \nKonstantina Krysteva "
                + "\nQnko Stoqnov \nSevdalina Nenkova");
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("                       ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ReadKey();
            // SplashScreen();
        }
    }
}

