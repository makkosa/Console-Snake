using System;
using System.Collections.Generic;
using System.IO;

namespace Snake
{
    class Game
    {
        private static bool quit = false;
        private static bool gameEnded = false;

        private const string NAME = "Snake";
        private const int SCREEN_WIDTH = 74;
        private const int SCREEN_HEIGHT = 40;
        private const string SNAKE_SYMBOL = "x";
        private const string FOOD_SYMBOL = "Q";
        private const int PERIOD = 100;

        private const ConsoleColor SNAKE_COLOR = ConsoleColor.Cyan;
        private const ConsoleColor FOOD_COLOR = ConsoleColor.Red;
        private const ConsoleColor SCORE_COLOR = ConsoleColor.Gray;
        private const ConsoleColor MENU_COLOR = ConsoleColor.Gray;

        private static List<Position> snakeBody;
        private static Direction snakeDirection;
        private static int snakeLength;
        private static int score;

        private static Position foodPosition;
        private static Position scorePosition;
        private static Position menuPosition;

        private static Random random = new Random();
        private static string[] menu;

        static void Main(string[] args)
        {
            Initialize();

            while (!quit)
            {
                GameLoop();
                ShowMenu(menuPosition, menu, gameEnded);

                if (gameEnded)
                {
                    Initialize();
                    gameEnded = false;
                }
            }
        }

        static void Initialize()
        {
            Console.Title = NAME;
            Console.CursorVisible = false;
            Console.SetWindowSize(SCREEN_WIDTH, SCREEN_HEIGHT);
            Console.SetBufferSize(SCREEN_WIDTH, SCREEN_HEIGHT);

            snakeLength = 4;
            score = 0;

            snakeBody = new List<Position>();

            for (int i = snakeLength - 1; i >= 0; i--)
            {
                snakeBody.Add(new Position() { X = (SCREEN_WIDTH / 2) - i, Y = SCREEN_HEIGHT / 2 });
            }
            snakeDirection = Direction.Right;

            foodPosition = new Position() { X = random.Next(SCREEN_WIDTH - 1), Y = random.Next(SCREEN_HEIGHT - 1) };
            scorePosition = new Position() { X = 2, Y = 1 };

            menu = File.ReadAllLines("Menu.txt");
            int menuWidth = menu[0].Length;
            int menuHeight = menu.Length;
            menuPosition = new Position() { X = (SCREEN_WIDTH - menuWidth) / 2, Y = (SCREEN_HEIGHT - menuHeight) / 2 };
        }

        static void GameLoop() 
        {
            while (true)
            {
                Console.Clear();
                 
                // Проверяем выходит ли координата головы змейки за пределы окна
                if (snakeBody[snakeLength - 1].X < 0 || snakeBody[snakeLength - 1].X >= SCREEN_WIDTH ||
                    snakeBody[snakeLength - 1].Y < 0 || snakeBody[snakeLength - 1].Y >= SCREEN_HEIGHT)
                {
                    gameEnded = true;
                    break;
                }
                // Проверяем содержит ли список копию позиции головы
                else if (snakeBody.IndexOf(snakeBody[snakeLength - 1], 0, snakeLength - 1) >= 0)
                {
                    gameEnded = true;
                    break;
                }

                Draw(scorePosition, "Счет: " + score, SCORE_COLOR);

                for (int i = 0; i < snakeLength; i++)
                {
                    Draw(snakeBody[i], SNAKE_SYMBOL, SNAKE_COLOR);
                }

                Draw(foodPosition, FOOD_SYMBOL, FOOD_COLOR);

                System.Threading.Thread.Sleep(PERIOD);

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape) break;
                    snakeDirection = GetDirection(snakeDirection, key);
                }
                
                // Добавляем новый элемент змейки в направлении движения (новая голова змейки с изменненной координатой)
                switch (snakeDirection)
                {
                    case Direction.Up:
                        snakeBody.Add(new Position() { X = snakeBody[snakeLength - 1].X, Y = snakeBody[snakeLength - 1].Y - 1 });
                        break;
                    case Direction.Down:
                        snakeBody.Add(new Position() { X = snakeBody[snakeLength - 1].X, Y = snakeBody[snakeLength - 1].Y + 1 });
                        break;
                    case Direction.Left:
                        snakeBody.Add(new Position() { X = snakeBody[snakeLength - 1].X - 1, Y = snakeBody[snakeLength - 1].Y });
                        break;
                    case Direction.Right:
                        snakeBody.Add(new Position() { X = snakeBody[snakeLength - 1].X + 1, Y = snakeBody[snakeLength - 1].Y });
                        break;
                    default:
                        break;
                }

                if (snakeBody[snakeLength - 1].X == foodPosition.X &&
                    snakeBody[snakeLength - 1].Y == foodPosition.Y)
                {
                    snakeLength++;
                    score++;
                    foodPosition = new Position() { X = random.Next(SCREEN_WIDTH - 1), Y = random.Next(SCREEN_HEIGHT - 1) };
                }
                // Если координаты головы и еды не совпали, то удаляем 0 элемент списка (хвост змейки), длина змеи остается прежней
                else
                {
                    snakeBody.RemoveAt(0);
                }
            }
        }

        static void Draw(Position position, string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write(text);
        }

        static void ShowMenu(Position position, string[] menu, bool newGame)
        {
            for (int i = 0; i < menu.Length; i++)
            {
                string line = menu[i];
                if (newGame)
                {
                    line = line.Replace("1. ПРОДОЛЖИТЬ ИГРУ", "   1. НОВАЯ ИГРА  ");
                }

                switch (score.ToString().Length)
                {
                    default:
                    case 1:
                        line = line.Replace("0", score.ToString());
                        break;
                    case 2:
                        line = line.Replace("0 ", score.ToString());
                        break;
                    case 3:
                        line = line.Replace("0  ", score.ToString());
                        break;
                }
                Draw(new Position() { X = position.X, Y = position.Y + i }, line, MENU_COLOR);
            }

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.D1) break;
                    else if (key.Key == ConsoleKey.D2) 
                    {
                        quit = true;
                        break;
                    }
                }
            }
        }

        static Direction GetDirection(Direction currentDirection, ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.UpArrow && currentDirection != Direction.Down) return Direction.Up;
            else if (key.Key == ConsoleKey.DownArrow && currentDirection != Direction.Up) return Direction.Down;
            else if (key.Key == ConsoleKey.LeftArrow && currentDirection != Direction.Right) return Direction.Left;
            else if (key.Key == ConsoleKey.RightArrow && currentDirection != Direction.Left) return Direction.Right;
            else return currentDirection; // Защита от нажатия левых клавиш
        }

        struct Position
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
