using System;
using System.Collections.Generic;
using Godot;

namespace GameOfLife;

public partial class Grid: Node2D
{
    public static bool _debugState = true;
    public bool _isMouseDown;	
 
    public bool _drawDeadCell = true;
    public Color _newAliveColor = Colors.Yellow;

    private readonly Random _random = new ();
    private int _boxSize = 10;
 

    public readonly List<Cell[,]> _gridCells = new ();
    
    public int _currentStateIndex;
    public MatrixManipulation _matrixManipulation = null;

    public int GridWidth { get; set; }
    public int GridHeight { get; set; }
    
    public bool IsPaused { get; set; }
    public double TimeElapsed { get; set; }
    public float UpdateTickRate { get; set; }
    public List<Cell[,]> GridCells { get; }
    public int BoxSize { get; set; }
    public bool DebugState { get; set; }

    // Default speed value
    private const float DefaultUpdateTickRate = 0.5f;
	
    public void ResetUpdateTickRate()
    {
        UpdateTickRate = DefaultUpdateTickRate;
    }

    public Grid()
    {
        _matrixManipulation = new MatrixManipulation();
    }
    
    private void InitGrid()
    {
        var viewportSize = GetViewportRect().Size;
        GridWidth = (int)viewportSize.X / _boxSize;
        GridHeight = (int)viewportSize.Y / _boxSize;
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
        _gridCells.Add(initialState);
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
        _currentStateIndex++;
		
        if (_currentStateIndex >= _gridCells.Count)
        {
            _currentStateIndex = _gridCells.Count - 1;
        }
        //trigger redraw
        QueueRedraw();
    }

    public void Rewind()
    {
        _currentStateIndex--;
		
        if (_currentStateIndex < 0)
        {
            _currentStateIndex = 0;
        }
        //trigger redraw
        QueueRedraw();
    }
    private Color RandomColor()
    {
        return new Color((float)_random.NextDouble(), (float)_random.NextDouble(), (float)_random.NextDouble());
    }

    public void DrawPattern(PatternCreator.Pattern pattern)
    {
        var currentGridState = _gridCells[_currentStateIndex];
        var patternWidth = pattern.Width;
        var patternHeight = pattern.Height;
        var patternCells = pattern.Cells;
        var mousePosition = GetGlobalMousePosition();
        var mousePositionX = (int) (mousePosition.X / _boxSize);
        var mousePositionY = (int) (mousePosition.Y / _boxSize);
		
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

        var currentGridState = _gridCells[_currentStateIndex];
		
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
        var currentGridState = _gridCells[_currentStateIndex];
        var newGridState = (Cell[,]) currentGridState.Clone();
        _gridCells.Add(newGridState);
        _currentStateIndex++;

        // Optionally, limit the history to the last 100 states
        if (_gridCells.Count > 100)
        {
            _gridCells.RemoveAt(0);
            _currentStateIndex--;
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