using System;
using System.Collections.Generic;
using Godot;

namespace GameOfLife;

public class Grid: Node2D
{
    private int _boxSize = 10;
    private int _gridWidth;
    private int _gridHeight;
    
    private readonly List<Cell[,]> _gridCells = new ();
    
    private int _currentStateIndex; 
    private MatrixManipulation _matrixManipulation = null;
    
    private void InitGrid()
    {
        var viewportSize = GetViewportRect().Size;
        _gridWidth = (int)viewportSize.X / _boxSize;
        _gridHeight = (int)viewportSize.Y / _boxSize;
        var initialState = new Cell[_gridWidth, _gridHeight];
		
        for (var i = 0; i < _gridWidth; i++)
        {
            for (var j = 0; j < _gridHeight; j++)
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
    
    private Cell[,] ApplyConwaysRules(Cell[,] currentGrid)
    {
        var newGrid = new Cell[_gridWidth, _gridHeight];

        for (var x = 0; x < _gridWidth; x++)
        {
            for (var y = 0; y < _gridHeight; y++)
            {
                var liveNeighbors = CountLiveNeighbors(currentGrid, x, y);
                var cellColor = GetCellColor(liveNeighbors);

                if (currentGrid[x, y]?.IsAlive ?? false)
                {
                    newGrid[x, y] = new Cell
                    {
                        LiveNeighbors = liveNeighbors,
                        Color = cellColor,
                        IsAlive = liveNeighbors == 2 || liveNeighbors == 3,
                        Position = new Vector2(x, y)
                    };
                }
                else
                {
                    newGrid[x, y] = new Cell
                    {
                        LiveNeighbors = liveNeighbors,
                        Color = cellColor,
                        IsAlive = liveNeighbors == 3,
                        Position = new Vector2(x, y)
                    };
                }
            }
        }

        return newGrid;
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
    
    private void StepForward()
    {
        _currentStateIndex++;
		
        if (_currentStateIndex >= _gridCells.Count)
        {
            _currentStateIndex = _gridCells.Count - 1;
        }
        //trigger redraw
        QueueRedraw();
    }

    private void Rewind()
    {
        _currentStateIndex--;
		
        if (_currentStateIndex < 0)
        {
            _currentStateIndex = 0;
        }
        //trigger redraw
        QueueRedraw();
    }
    
    private void DrawPattern(GridManager.Pattern pattern)
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
    


    private void ToggleCell(Vector2 cellCoords)
    {
        var x = (int) cellCoords.X;
        var y = (int) cellCoords.Y;

        if (x < 0 || x >= _gridWidth || y < 0 || y >= _gridHeight)
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
    
    private void SaveState()
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
    
    private static Color GetCellColor(int liveNeighbors)
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