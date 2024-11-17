using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static void Main()
    {
        // Set up the game
        Console.CursorVisible = false;
        int screenWidth = 30;
        int screenHeight = 20;

        Console.SetWindowSize(screenWidth, screenHeight + 2); // +2 für Score-Anzeige
        Console.SetBufferSize(screenWidth, screenHeight + 2);
        Console.CursorVisible = false;

        int score = 0;
        Snake snake = new Snake(screenWidth, screenHeight);
        Food food = new Food(screenWidth, screenHeight);

        // Game loop
        while (true)
        {
            // Draw the game elements
            Console.Clear();
            snake.Draw();
            food.Draw();
            Console.SetCursorPosition(0, screenHeight + 1);
            Console.Write($"Score: {score}");

            // Update the snake's position
            snake.Move();

            // Check for collisions
            if (snake.HasCollided())
            {
                Console.SetCursorPosition(0, screenHeight + 2);
                Console.WriteLine("Game Over! Press any key to exit...");
                break;
            }

            // Check if the snake ate the food
            if (snake.HeadX == food.X && snake.HeadY == food.Y)
            {
                snake.Grow();
                food.Respawn();
                score++;
            }

            // Control the game speed
            Thread.Sleep(100);
        }

        Console.ReadKey();
    }
}

// Snake class
class Snake
{
    private List<(int X, int Y)> body = new List<(int X, int Y)>();
    private int directionX = 1; // 1 = right, -1 = left
    private int directionY = 0; // 1 = down, -1 = up
    private int screenWidth;
    private int screenHeight;

    public int HeadX => body[0].X;
    public int HeadY => body[0].Y;

    public Snake(int screenWidth, int screenHeight)
    {
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        body.Add((screenWidth / 2, screenHeight / 2));
    }

    public void Move()
    {
        // Update direction based on user input
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.UpArrow && directionY == 0) { directionX = 0; directionY = -1; }
            if (key == ConsoleKey.DownArrow && directionY == 0) { directionX = 0; directionY = 1; }
            if (key == ConsoleKey.LeftArrow && directionX == 0) { directionX = -1; directionY = 0; }
            if (key == ConsoleKey.RightArrow && directionX == 0) { directionX = 1; directionY = 0; }
        }

        // Calculate new head position
        int newX = (HeadX + directionX + screenWidth) % screenWidth;
        int newY = (HeadY + directionY + screenHeight) % screenHeight;

        // Check for collision with itself
        if (body.Contains((newX, newY)))
        {
            throw new Exception("Game Over");
        }

        // Add new head and remove the tail
        body.Insert(0, (newX, newY));
        body.RemoveAt(body.Count - 1);
    }

    public void Grow()
    {
        body.Add(body[body.Count - 1]);
    }

    public void Draw()
    {
        foreach (var segment in body)
        {
            Console.SetCursorPosition(segment.X, segment.Y);
            Console.Write("O");
        }
    }

    public bool HasCollided()
    {
        // Check if the snake collides with itself
        return body.Count > 1 && body.GetRange(1, body.Count - 1).Contains((HeadX, HeadY));
    }
}

// Food class
class Food
{
    private Random random = new Random();
    private int screenWidth;
    private int screenHeight;

    public int X { get; private set; }
    public int Y { get; private set; }

    public Food(int screenWidth, int screenHeight)
    {
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        Respawn();
    }

    public void Respawn()
    {
        // Sicherstellen, dass X und Y innerhalb der Grenzen der Konsole liegen
        X = random.Next(0, screenWidth);
        Y = random.Next(0, screenHeight - 1); // Höchste Y-Position darf nicht überschritten werden
    }

    public void Draw()
    {
        // Validierung der Position, um Fehler zu vermeiden
        if (X >= 0 && X < screenWidth && Y >= 0 && Y < screenHeight)
        {
            Console.SetCursorPosition(X, Y);
            Console.Write("@");
        }
    }
}