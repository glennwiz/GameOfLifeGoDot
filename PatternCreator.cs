using Godot;

namespace GameOfLife;

public partial class PatternCreator : Node2D
{
    public class Pattern
    {
        public int Width { get; } 
        public int Height { get; } 
        public bool[,] Cells { get; }

        public Pattern(int width, int height, bool[,] cells)
        {
            Width = width;
            Height = height;
            Cells = cells;
        }
    }
}