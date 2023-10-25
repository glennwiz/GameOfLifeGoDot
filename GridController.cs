using Godot;

namespace GameOfLife;

public partial class GridController : Node2D
{
    private readonly Grid _grid;

    public GridController(Grid grid)
    {
        _grid = grid;
    }

    public void GridProcess(double delta)
    {
        PerformGridProcess(delta);
    }
    
    private void PerformGridProcess(double delta)
    {
        if (_grid.IsPaused) return;
        _grid.TimeElapsed += delta;

        if (_grid.TimeElapsed >= _grid.UpdateTickRate)
        {
            PerformGridUpdate();
            _grid.TimeElapsed = 0.0;
        }
    }
    
    private void PerformGridUpdate()
    {
        _grid.GridCells[_grid.CurrentStateIndex] = ApplyConwaysRules(_grid.GridCells[_grid.CurrentStateIndex]);
        _grid.SaveState();
    }

    private Cell[,] ApplyConwaysRules(Cell[,] currentGrid)
    { 
        var newGrid = new Cell[_grid.GridWidth, _grid.GridHeight];
        
        ProcessGridRules(currentGrid, newGrid);
        return newGrid;
    }

    private void ProcessGridRules(Cell[,] currentGrid, Cell[,] newGrid)
    {
        for (var x = 0; x < _grid.GridWidth; x++)
        {
            for (var y = 0; y < _grid.GridHeight; y++)
            {
                newGrid[x, y] = GetNewCell(currentGrid, x, y);
            }
        }
    }

    private Cell GetNewCell(Cell[,] currentGrid, int x, int y)
    {
        var liveNeighbors = _grid.CountLiveNeighbors(currentGrid, x, y);
        var newCell = InitializeCell(liveNeighbors, currentGrid[x, y]?.IsAlive ?? false, x, y);
        return newCell;
    }
    
    private Cell InitializeCell(int liveNeighbors, bool isAlive, int x, int y)
    {
        var cellColor = _grid.GetCellColor(liveNeighbors);
        
        return new Cell
        {
           LiveNeighbors = liveNeighbors,
           Color = cellColor,
           IsAlive = isAlive ? liveNeighbors is 2 or 3 : (liveNeighbors == 3),
           Position = new Vector2(x, y)
        };
    }
}