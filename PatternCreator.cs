using System;
using Godot;

namespace GameOfLife;

public partial class PatternCreator : Node2D
{
    static CellPool pool = GameGridHandler.GetCellPool();
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
        
        public static bool[,] Rabit01 => rabit01;
        private static readonly bool[,] rabit01 = {
            {f, f, f, f,    true, f, f, f, f, f, f, f, f},
            {f, f, f, true, true, f, true, f, f, f, f, f, f},
            {f, f, f, f,    f,    f,    f, true,true, f, f, f, f},
            {f, f, f, true, f,    f, true, f, f, f, f, f, f},
            {f, f, f, true, f,    f, f, f, f, f, f, f, f},
            {f, f, f, true, f,    f, f, f, f, f, f, f, f},
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
    
    public static Cell[,] GetInitialGrid(int GridWidth, int GridHeight)
    {
        var emptyCellGrid = new Cell[GridWidth, GridHeight];

        for (var i = 0; i < GridWidth; i++)
        {
            for (var j = 0; j < GridHeight; j++)
            {
                var cellColor = Colors.Black;
                var isAlive = false;

                // START PATTERNS
                if ((i is >= 40 and <= 42 && j == 40)
                    || (i == 40 && j is >= 40 and <= 42)
                    || (i == 42 && j is >= 40 and <= 42))
                {
                    cellColor = Colors.White;
                    isAlive = true;
                }

                if ((i == 75 && j is 37 or 38 or 39 or 51 or 52 or 53)
                    || (i == 78 && j is 36 or 50 or 54)
                    || (i == 79 && j is 36 or 50 or 54)
                    || (i == 80 && j is 37 or 38 or 39 or 51 or 52 or 53)
                    || (i == 81 && j is 36 or 50 or 54)
                    || (i == 82 && j is 36 or 50 or 54)
                    || (i == 85 && j is 37 or 38 or 39 or 51 or 52 or 53))
                {
                    cellColor = Colors.White;
                    isAlive = true;
                }

                if ((i == 50 && j is 57 or 58 or 59 or 61 or 62 or 63)
                    || (i == 53 && j is 56 or 60 or 64)
                    || (i == 54 && j is 56 or 60 or 64)
                    || (i == 55 && j is 57 or 58 or 59 or 61 or 62 or 63)
                    || (i == 56 && j is 56 or 60 or 64)
                    || (i == 57 && j is 56 or 60 or 64)
                    || (i == 60 && j is 57 or 58 or 59 or 61 or 62 or 63))
                {
                    cellColor = Colors.White;
                    isAlive = true;
                }

                var cell = pool.GetCell();
                cell.Color = cellColor;
                cell.IsAlive = isAlive;
                cell.Position = new Vector2(i, j);
                cell.State = "Alive";
                
                emptyCellGrid[i, j] = cell;
            }
        }

        return emptyCellGrid;
    }
}