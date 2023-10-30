using System;
using System.Collections.Generic;
using Godot;

namespace GameOfLife;

public partial class Grid: Node2D
{
	public bool IsMouseDown { get; set; }	
 
	public bool DrawDeadCell  { get;set;}
	private Color NewAliveColor { get; set; }

	private readonly Random _random = new ();
	private Vector2 _gridSize;

	public int CurrentStateIndex { get; set; }
	public MatrixManipulation MatrixManipulation { get; }
	public int GridWidth { get; set; }
	public int GridHeight { get; set; } 
	public bool IsPaused { get; set; }
	public double TimeElapsed { get; set; }
	public float UpdateTickRate { get; set; } = 0.5f;
	public bool ResetHigherGridArray { get; set; }
	public List<Cell[,]> ListOfCellArrayStates { get; set; } = new(); 
	public int BoxSize { get; set; } = 10;
	public bool DebugState { get; set; } = true;
	public bool DrawCopyBox { get; set; } = false;

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

	public void MirrorAndShift()
	{
		DrawCopyBox = !DrawCopyBox;
		PatternCreator.CopyPattern();
	}

	private void InitGrid()
	{
		var viewportSize = _gridSize;;
		GridWidth = (int)viewportSize.X / BoxSize;
		GridHeight = (int)viewportSize.Y / BoxSize;
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

				
				emptyCellGrid[i, j] = new Cell
				{
					Color = cellColor,
					IsAlive = isAlive,
					Position = new Vector2(i, j),
					State = "Alive"
					
				};
			}
		}
		
		var initialCellGrid = PatternCreator.CreatePattern(PatternCreator.Pattern.Star, emptyCellGrid);
		ListOfCellArrayStates.Add(initialCellGrid);
	}
	
	public int CountLiveNeighbors(Cell[,] grid, int x, int y)
	{
		var count = 0;
		var width = grid.GetLength(0);
		var height = grid.GetLength(1);

		//offsets for all 8 neighbors, cell at center
		(int, int)[] offsets =
		{
			(-1,-1), (0,-1), (1, -1), 
			(-1, 0),         (1,  0), 
			(-1, 1), (0, 1), (1,  1)
		};

		foreach (var offset in offsets)
		{
			var nx = (x + offset.Item1 + width) % width;
			var ny = (y + offset.Item2 + height) % height;

			if (grid[nx, ny].IsAlive)
				count++;
		}

		return count;
	}

	public void StepForward()
	{
		CurrentStateIndex++;
		
		if (CurrentStateIndex >= ListOfCellArrayStates.Count)
		{
			CurrentStateIndex = ListOfCellArrayStates.Count - 1;
		}
	}

	public void Rewind()
	{
		ResetHigherGridArray = true;
		
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
		var currentGridState = ListOfCellArrayStates[CurrentStateIndex];
		var patternWidth = pattern.Width;
		var patternHeight = pattern.Height;
		var patternCells = pattern.Cells;
		var mousePosition = GetGlobalMousePosition();
		var mousePositionX = (int) (mousePosition.X / BoxSize) - patternWidth / 2;
		var mousePositionY = (int) (mousePosition.Y / BoxSize) - patternHeight / 2;
		
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

		var currentGridState = ListOfCellArrayStates[CurrentStateIndex];
		
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
		if(IsPaused) return;
		
		var currentGridState = ListOfCellArrayStates[CurrentStateIndex];
		var newGridState = (Cell[,]) currentGridState.Clone();
		
		ListOfCellArrayStates.Add(newGridState);
		CurrentStateIndex++;

		// Optionally, limit the history to the last 1000 states
		if (ListOfCellArrayStates.Count <= 1000) return;
		
		ListOfCellArrayStates.RemoveAt(0);
		CurrentStateIndex--;
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

	public int CountElectronHeadNeighbors(Cell[,] currentGrid, int i, int i1)
	{
		throw new NotImplementedException();
	}
}