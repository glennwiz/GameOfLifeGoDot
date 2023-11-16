using Godot;

namespace GameOfLife;

public partial class GridController : Node2D
{
    private CellPool pool = GameGridHandler.GetCellPool();
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
        if (!(_grid.TimeElapsed >= _grid.UpdateTickRate))
        {
            //GD.Print("TimeElapsed: " + _grid.TimeElapsed);
            return;
        }
        
        PerformGridUpdate();
        _grid.TimeElapsed = 0.0;
    }
    
    private void PerformGridUpdate()
    {
        GD.Print("||||||||||GridUpdate|||||||||||||||||||||---------------------------------------");
        _grid.ListOfCellArrayStates[_grid.CurrentStateIndex] = ApplyConwaysRules(_grid.ListOfCellArrayStates[_grid.CurrentStateIndex]);
        _grid.SaveState();
    }

    private Cell[,] ApplyConwaysRules(Cell[,] currentGrid)
    { 
        var newGrid = new Cell[_grid.GridWidth, _grid.GridHeight];
        GD.Print("||||||||||GridUpdate|||||||||||||||||||||---------------------------------------");
        ProcessGridRules(currentGrid, newGrid);
        return newGrid;
    }

    private void ProcessGridRules(Cell[,] currentGrid, Cell[,] newGrid)
    {
        for (var x = 0; x < _grid.GridWidth; x++)
        {
            for (var y = 0; y < _grid.GridHeight; y++)
            {
                newGrid[x, y] = GetNewCellByConwaysRules(currentGrid, x, y);
            }
        }
    }

    private Cell GetNewCellByConwaysRules(Cell[,] currentGrid, int x, int y)
    {
        // Check if the indices are within the bounds of the grid
        if (x < 0 || x >= currentGrid.GetLength(0) || y < 0 || y >= currentGrid.GetLength(1))
        {
            // Handle the edge case, return null
            return null;
        }

        var liveNeighbors = _grid.CountLiveNeighbors(currentGrid, x, y);
        if (liveNeighbors == 0 || liveNeighbors == 1 || liveNeighbors > 3)
        {
            return null;
        }

        // Check the status of the current cell
        var stateOfCell = currentGrid[x, y]?.IsAlive ?? false;

        // If the cell is alive, it continues to live for the next generation if it has 2 or 3 live neighbors. 
        // If the cell is dead, it becomes a live cell only in the next generation if it has exactly 3 live neighbors.
        stateOfCell = stateOfCell 
            ? (liveNeighbors == 2 || liveNeighbors == 3) 
            : (liveNeighbors == 3);

        // Get a new cell from the pool and set its properties
        
        Color color;
        if (_grid.UseRandomColors)
        {
            //if the cell in the generation before was alive, keep the color
            if (currentGrid[x, y] != null && currentGrid[x, y].IsAlive)
            {
                color = currentGrid[x, y].Color;
            }
            else
                color = Color.Color8((byte)GD.RandRange(0, 255), (byte)GD.RandRange(0, 255), (byte)GD.RandRange(0, 255));
        }
        else
        {
            color = _grid.GetCellColor(liveNeighbors);
        }
        
        var cell = pool.GetCell();
        cell.LiveNeighbors = liveNeighbors;
        cell.Color = color;
        cell.IsAlive = stateOfCell;
        cell.Position = new Vector2(x, y);

        return cell;    
    }
}