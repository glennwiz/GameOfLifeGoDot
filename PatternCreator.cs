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
        public static bool[,] GosperGun => gosperCells;
        public static bool f = false;
        // create a Gosper's glider gun at mouse position
        private static readonly bool[,] gosperCells =  {
            {f 	  , f   , f , f	, f	, f	, f	, f , f , f , f   , f   , f   , f   , f   , f   , f   , f    , f , f , f   , f   , f   , f , true, f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
            {f 	  , f   , f , f	, f	, f	, f	, f , f , f , f   , f   , f   , f   , f   , f   , f   , f    , f , f , f   , f   , true, f , true, f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
            {f 	  , f   , f , f	, f	, f	, f	, f , f , f , f   , f   , true, true, f   , f   , f   , f    , f , f , true, true, f   , f , f   , f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
            {f 	  , f   , f , f	, f	, f	, f	, f , f , f , f   , true, f   , f   , f   , true, f   , f    , f , f , true, true, f   , f , f   , f  , f	, f	, f	, f	, f	, f , f , f , true, true },
            {true , true, f , f	, f	, f	, f	, f , f , f , true, f   , f   , f   , f   , f   , true, f    , f , f , true, true, f   , f , f   , f  , f	, f	, f	, f	, f	, f , f , f , true, true },
            {true , true, f , f	, f	, f	, f	, f , f , f , true, f   , f   , f   , true, f   , true, true , f , f , f   , f   , true, f , true, f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
            {f    , f 	, f , f	, f	, f	, f	, f , f , f , true, f   , f   , f   , f   , f   , true, f    , f , f , f   , f   , f   , f , true, f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
            {f    , f 	, f , f	, f	, f	, f	, f , f , f , f   , true, f   , f   , f   , true, f   , f    , f , f , f   , f   , f   , f , f   , f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
            {f    , f 	, f , f	, f	, f	, f	, f , f , f , f   , f   , true, true, f   , f   , f   , f    , f , f , f   , f   , f   , f , f   , f  , f	, f	, f	, f	, f	, f , f , f , f   , f    }
        };
        
        public static bool[,] Star => star;
        private static readonly bool[,] star =
        {
            {true}, {true}, {true}, {f}, {f}, {f}, {f}, {f}, {f}, {true}, {true}, {true},
            {true}, {   f}, {   f}, {f}, {f}, {f}, {f}, {f}, {f}, {   f}, {   f}, {true},
            {true}, {true}, {true}, {f}, {f}, {f}, {f}, {f}, {f}, {true}, {true}, {true}
        };
    }
    
    public static Cell[,] CreatePattern(bool[,] star, Cell[,] initialState)
    {
        var pattern = new Pattern(star.GetLength(0), star.GetLength(1), star);

        int patternWidth = pattern.Width;
        int patternHeight = pattern.Height;

        int initialWidth = initialState.GetLength(0);
        int initialHeight = initialState.GetLength(1);

        int midWidth = initialWidth / 2;
        int midHeight = initialHeight / 2;

        for (int i = 0; i < patternWidth; i++)
        {
            for (int j = 0; j < patternHeight; j++)
            {
                // Wrap pattern onto initialState from the middle of the initialState
                int x = (midWidth - patternWidth / 2 + i + initialWidth) % initialWidth;
                int y = (midHeight - patternHeight / 2 + j + initialHeight) % initialHeight;

                // Modify initialState according to pattern
                initialState[x, y] = pattern.Cells[i, j] ? new Cell() : initialState[x, y];
            }
        }

        return initialState;
    }
    
}