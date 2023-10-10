using System;
using System.Collections.Generic;
using Godot;

namespace GameOfLife;

public partial class Grid: Node2D
{
    public static bool _debugState = true;
    public bool IsMouseDown { get; set; }	
 
    public bool DrawDeadCell  { get;set;}
    private Color NewAliveColor { get; set; }

    private readonly Random _random = new ();
    private Vector2 _gridSize;

    public int CurrentStateIndex { get; set; } = 0;
    public MatrixManipulation MatrixManipulation { get; set;}
    public int GridWidth { get; set; }
    public int GridHeight { get; set; } 
    public bool IsPaused { get; set; } = false;
    public double TimeElapsed { get; set; }
    public float UpdateTickRate { get; set; } = 0.5f;
    public List<Cell[,]> GridCells { get; set; } = new List<Cell[,]>(); 
    public int BoxSize { get; set; } = 10;
    public bool DebugState { get; set; } = true;

    // Default speed value
    private const float DefaultUpdateTickRate = 0.5f;
	
    public void ResetUpdateTickRate()
    {
        UpdateTickRate = DefaultUpdateTickRate;
    }

    public Grid(MatrixManipulation matrixManipulation, Vector2 gridSize)
    {
        _gridSize = gridSize;
        MatrixManipulation = matrixManipulation;
        InitGrid();
    }

    private void InitGrid()
    {
        var viewportSize = _gridSize;;
        GridWidth = (int)viewportSize.X / BoxSize;
        GridHeight = (int)viewportSize.Y / BoxSize;
        var initialState = new Cell[GridWidth, GridHeight];
		
        for (var i = 0; i < GridWidth; i++)
        {
            for (var j = 0; j < GridHeight; j++)
            {
                var cellColor = Colors.Black;
					
                initialState[i, j] = new Cell
                {
                    Color = cellColor,
                    IsAlive = _random.Next(0, 2) == 1,
                    Position = new Vector2(i, j)
                };
            }
        }
        GridCells.Add(initialState);
    }
    
    public int CountLiveNeighbors(Cell[,] grid, int x, int y)
    {
        var count = 0;
        var width = grid.GetLength(0);
        var height = grid.GetLength(1);

        // Define offsets for all 8 neighbors
        int[] xOffset = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] yOffset = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (var i = 0; i < 8; i++)
        {
            var nx = x + xOffset[i];
            var ny = y + yOffset[i];

            // Wrap around edges
            if (nx < 0)
                nx = width - 1;
            else if (nx >= width)
                nx = 0;

            if (ny < 0)
                ny = height - 1;
            else if (ny >= height)
                ny = 0;

            if (grid[nx, ny].IsAlive)
                count++;
        }

        return count;
    }

    public void StepForward()
    {
        CurrentStateIndex++;
		
        if (CurrentStateIndex >= GridCells.Count)
        {
            CurrentStateIndex = GridCells.Count - 1;
        }

    }

    public void Rewind()
    {
        CurrentStateIndex--;
		
        if (CurrentStateIndex < 0)
        {
            CurrentStateIndex = 0;
        }

    }
    private Color RandomColor()
    {
        return new Color((float)_random.NextDouble(), (float)_random.NextDouble(), (float)_random.NextDouble());
    }

    public void DrawPattern(PatternCreator.Pattern pattern)
    {
        var currentGridState = GridCells[CurrentStateIndex];
        var patternWidth = pattern.Width;
        var patternHeight = pattern.Height;
        var patternCells = pattern.Cells;
        var mousePosition = GetGlobalMousePosition();
        var mousePositionX = (int) (mousePosition.X / BoxSize);
        var mousePositionY = (int) (mousePosition.Y / BoxSize);
		
        for (var i = 0; i < patternWidth; i++)
        {
            for (var j = 0; j < patternHeight; j++)
            {
                //index out of bounds check, print and return
                try
                {
                    var cellColor = Colors.Black;
                    if (patternCells[i, j])
                    {
                        cellColor = Colors.White;
                    }
				
                    currentGridState[mousePositionX + i, mousePositionY + j] = new Cell
                    {
                        Color = cellColor,
                        IsAlive = patternCells[i, j],
                        Position = new Vector2(mousePositionX + i, mousePositionY + j)
                    };
                }
                catch (IndexOutOfRangeException e)
                {
                    GD.Print("i = " + i + " | " + "j = " + j);
                    Console.WriteLine(e);
                }
            }
        }
    }

    public void ToggleCell(Vector2 cellCoords)
    {
        var x = (int) cellCoords.X;
        var y = (int) cellCoords.Y;

        if (x < 0 || x >= GridWidth || y < 0 || y >= GridHeight)
        {
            GD.PrintErr("Coordinates out of bounds");
            return;
        }

        var currentGridState = GridCells[CurrentStateIndex];
		
        if(currentGridState[x, y] == null)
        {
            currentGridState[x, y] = new Cell()
            {
                IsAlive = true , 
                Position = new Vector2(x, y),
                Color = Colors.Yellow,
            };
            return;
        }
		
        currentGridState[x, y].IsAlive = !currentGridState[x, y].IsAlive;
    }

    public void SaveState()
    {
        var currentGridState = GridCells[CurrentStateIndex];
        var newGridState = (Cell[,]) currentGridState.Clone();
        GridCells.Add(newGridState);
        CurrentStateIndex++;

        // Optionally, limit the history to the last 100 states
        if (GridCells.Count > 100)
        {
            GridCells.RemoveAt(0);
            CurrentStateIndex--;
        }
    }

    public Color GetCellColor(int liveNeighbors)
    {
        Dictionary<int, Color> colorMap = new()
        {
            {0, Colors.Black},
            {1, Colors.Red},
            {2, Colors.Yellow},
            {3, Colors.Green},
            {4, Colors.Cyan},
            {5, Colors.Blue},
            {6, Colors.Magenta},
            {7, Colors.White},
        };

        var cellColor = colorMap.ContainsKey(liveNeighbors) ? colorMap[liveNeighbors] : Colors.Gray;
        return cellColor;
    }
}