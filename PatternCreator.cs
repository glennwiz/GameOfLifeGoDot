using System;
using Godot;

namespace GameOfLife;

public partial class PatternCreator : Node2D
{
    static Random _random = new Random();
    
    public class Pattern
    {
        public int Width { get; } 
        public int Height { get; } 
        public bool[,] Cells { get; }
        
        
        public Pattern(bool[,] cells)
        {
            Width = cells.GetLength(0);
            Height = cells.GetLength(1);
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
        
        public static bool[,] Pulsar => pulsar;
        private static readonly bool[,] pulsar = {
            {f, f, f, f, true, true, true, f, f, f, f, f, f},
            {f, f, f, true, f, f, f, true, f, f, f, f, f},
            {f, f, f, f, f, f, f, f, f, f, f, f, f},
            {f, f, f, true, f, f, f, true, f, f, f, f, f},
            {f, f, f, f, true, true, true, f, f, f, f, f, f},
        };
        
        public static bool[,] Random1 => random1;
        private static readonly bool[,] random1 = {
            {f, f, true, true, true, f, f, f, true, true, true, f, f},
            {f, true, f, f, f, true, f, true, f, f, f, true, f},
            {f, true, f, f, f, true, f, true, f, f, f, true, f},
            {f, true, f, f, f, true, f, true, f, f, f, true, f},
            {f, f, true, true, true, f, f, f, true, true, true, f, f},
            {f, f, f, true, f, f, f, f, f, true, f, f, f},
            {f, f, f, f, f, f, f, f, f, f, f, f, f},
            {f, f, f, true, f, f, f, f, f, true, f, f, f},
            {f, f, true, true, true, f, f, f, true, true, true, f, f},
            {f, true, f, f, f, true, f, true, f, f, f, true, f},
            {f, true, f, f, f, true, f, true, f, f, f, true, f},
            {f, true, f, f, f, true, f, true, f, f, f, true, f},
            {f, f, true, true, true, f, f, f, true, true, true, f, f}
        };
        
        public static bool[,] CustomPattern => customPattern;
        private static readonly bool[,] customPattern = {
            {f, f, f, f, f, f, true, true, true, true, true, f, f, f, f, f},
            {f, f, f, f, f, true, f, f, f, f, true, true, f, f, f, f},
            {f, f, f, f, f, true, f, f, f, f, true, true, f, f, f, f},
            {f, f, f, f, f, true, f, f, f, f, true, true, f, f, f, f},
            {f, f, f, f, f, f, true, true, true, true, true, f, f, f, f, f},
            {f, f, f, f, true, true, f, f, f, f, true, true, true, f, f, f},
            {f, f, f, f, true, true, f, f, f, f, true, true, true, f, f, f},
            {f, f, f, f, f, f, f, f, f, f, f, f, f, f, f, f},
            {f, f, f, f, true, true, f, f, f, f, true, true, true, f, f, f},
            {f, f, f, f, true, true, f, f, f, f, true, true, true, f, f, f},
            {f, f, f, f, f, f, true, true, true, true, true, f, f, f, f, f},
            {f, f, f, f, f, true, f, f, f, f, true, true, f, f, f, f},
            {f, f, f, f, f, true, f, f, f, f, true, true, f, f, f, f},
            {f, f, f, f, f, true, f, f, f, f, true, true, f, f, f, f},
            {f, f, f, f, f, f, true, true, true, true, true, f, f, f, f, f}
        };
    }
    
    public static Cell[,] CreatePattern(bool[,] star, Cell[,] initialState)
    {
        var pattern = new Pattern(star);

        var patternWidth = pattern.Width;
        var patternHeight = pattern.Height;

        var initialWidth = initialState.GetLength(0);
        var initialHeight = initialState.GetLength(1);

        var midWidth = initialWidth / 2;
        var midHeight = initialHeight / 2;

        for (var i = 0; i < patternWidth; i++)
        {
            for (var j = 0; j < patternHeight; j++)
            {
                // Wrap pattern onto initialState from the middle of the initialState
                var x = (midWidth - patternWidth / 2 + i + initialWidth) % initialWidth;
                var y = (midHeight - patternHeight / 2 + j + initialHeight) % initialHeight;

                // Modify initialState according to pattern
                initialState[x, y] = pattern.Cells[i, j] ? new Cell() : initialState[x, y];
            }
        }

        return initialState;
    }

    public static Pattern CreateRandomPattern(int x, int y, float bias)
    {
        var patternWidth = x;
        var patternHeight = y;
        var patternCells = new bool[patternWidth, patternHeight];
        
        for (var i = 0; i < patternWidth; i++)
        {
            for (var j = 0; j < patternHeight; j++)
            {
                patternCells[i, j] = _random.NextDouble() < bias;
            }
        }

        return new Pattern(patternCells);
    }

    public static Pattern CopyPattern()
    {
        GD.Print("!CopyPattern!");
        return null;
    }
}