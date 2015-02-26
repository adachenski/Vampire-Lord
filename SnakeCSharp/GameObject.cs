using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeCSharp
{
    public class GameObject : IEquatable<GameObject>
    {
        public GameObject(int horizontal, int vertical)
        {
            this.Horizontal = horizontal;
            this.Vertical = vertical;
        }

        public int Horizontal { get; set; }

        public int Vertical { get; set; }

        public ConsoleColor Color { get; set; }

        public bool Equals(GameObject other)
        {
            return this.Horizontal.Equals(other.Horizontal) &&
                this.Vertical.Equals(other.Vertical);
        }

        public void Print(char symbol)
        {
            Console.SetCursorPosition(this.Horizontal, this.Vertical);
            Console.Write(symbol);
        }

        public void Print(string str)
        {
            Console.SetCursorPosition(this.Horizontal, this.Vertical);
            Console.Write(str);
        }
    }
}
