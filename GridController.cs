using Godot;

namespace GameOfLife;

public partial class GridController : Node2D
{
    private readonly Grid _grid;

    public GridController(Grid grid)
    {
        _grid = grid;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public void GridProcess(double delta)
    {
        if (_grid.IsPaused) return;
		
        _grid.TimeElapsed += delta;
        if (_grid.TimeElapsed >= _grid.UpdateTickRate)
        {
            _grid.GridCells[_grid.CurrentStateIndex] = ApplyConwaysRules(_grid.GridCells[_grid.CurrentStateIndex]);
            _grid.SaveState();
            _grid.TimeElapsed = 0.0;
        }
    }
    
    private Cell[,] ApplyConwaysRules(Cell[,] currentGrid)
    { 
        var newGrid = new Cell[_grid.GridWidth, _grid.GridHeight];

        for (var x = 0; x < _grid.GridWidth; x++)
        {
            for (var y = 0; y < _grid.GridHeight; y++)
            {
                var liveNeighbors = _grid.CountLiveNeighbors(currentGrid, x, y);
                var cellColor = _grid.GetCellColor(liveNeighbors);

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
}