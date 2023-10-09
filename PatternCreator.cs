using Godot;

namespace GameOfLife;

public partial class PatternCreator : Node2D
{
    private readonly Grid _grid;

    public PatternCreator(Grid grid)
    {
        _grid = grid;
    }

    public class Pattern
    {
        public int Width { get; } // Width of the pattern grid
        public int Height { get; } // Height of the pattern grid
        public bool[,] Cells { get; } // 2D array representing the pattern's initial state

        public Pattern(int width, int height, bool[,] cells)
        {
            Width = width;
            Height = height;
            Cells = cells;
        }
    }
}