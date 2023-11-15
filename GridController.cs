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
        if (!(_grid.TimeElapsed >= _grid.UpdateTickRate)) return;
        
        PerformGridUpdate();
        _grid.TimeElapsed = 0.0;
    }
    
    private void PerformGridUpdate()
    {
        _grid.ListOfCellArrayStates[_grid.CurrentStateIndex] = ApplyConwaysRules(_grid.ListOfCellArrayStates[_grid.CurrentStateIndex]);
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
                newGrid[x, y] = GetNewCellByConwaysRules(currentGrid, x, y);
            }
        }
    }

    private Cell GetNewCellByConwaysRules(Cell[,] currentGrid, int x, int y)
    {
        var liveNeighbors = _grid.CountLiveNeighbors(currentGrid, x, y);
        if (y >= currentGrid.GetLength(1) || x >= currentGrid.GetLength(0)) return pool.GetCell(); 
        
        var newCell = InitializeCell(liveNeighbors, currentGrid[x, y]?.IsAlive ?? false, x, y);
        return newCell;
    }
    
    private Cell GetNewCellByBriansBrainRules(Cell[,] currentGrid, int x, int y)
    {
        var liveNeighbors = _grid.CountLiveNeighbors(currentGrid, x, y); // Assuming this function can count states other than "Alive"
        if (y >= currentGrid.GetLength(1) || x >= currentGrid.GetLength(0)) return pool.GetCell();// Presuming the Cell constructor initializes to Dead

        var currentCell = currentGrid[x, y];
        var newCell = pool.GetCell(); // Initialize as Dead by default
    
        if (currentCell.State == "Alive") {
            newCell.State = "Dying";
            newCell.IsAlive = true;
        }
        else if (currentCell.State == "Dying") {
            newCell.State = "Dead";
            newCell.IsAlive = false;
        }
        else if (currentCell.State == "Dead" && liveNeighbors == 2) {
            newCell.State = "Alive";
            newCell.IsAlive = true;
        }
        else {
            newCell.State = currentCell.State;
            newCell.IsAlive = currentCell.IsAlive;
        }

        return newCell;
    }
    
    private Cell InitializeCell(int liveNeighbors, bool isAlive, int x, int y)
    {
        var cellColor = _grid.GetCellColor(liveNeighbors);

        var cell = pool.GetCell();
        cell.LiveNeighbors = liveNeighbors;
        cell.Color = cellColor;
        cell.IsAlive = isAlive ? liveNeighbors is 2 or 3 : (liveNeighbors == 3);
        cell.Position = new Vector2(x, y);
        
        return cell;    
    }
}