using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class GridManager : Node2D
{
	Random rand = new ();
	private int _boxSize = 10;
	float updateTickRate = 0.5f;
	private double _timeElapsed = 0.0;
	private bool _debugState = true;
	private int _currentStateIndex = 0; // The index of the current state in the gridStates list we keep track of 100 states
	private List<Cell[,]> _gridCells = new ();

	private int gridWidth;
	private int gridHeight;

	public bool drawDeadCell = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Ready!!");
		InitGrid();
	}
	
	private void InitGrid()
	{
		var viewportSize = GetViewportRect().Size;
		gridWidth = (int)viewportSize.X / _boxSize;
		gridHeight = (int)viewportSize.Y / _boxSize;
		var initialState = new Cell[gridWidth, gridHeight];
		
		for (var i = 0; i < gridWidth; i++)
		{
			for (var j = 0; j < gridHeight; j++)
			{
				var t = (float)i / (gridWidth - 1);
				var cellColor = Colors.Black;
					
				initialState[i, j] = new Cell
				{
					Color = cellColor,
					IsAlive = rand.Next(0, 2) == 1,
					Position = new Vector2(i, j)
				};
			}
		}
		_gridCells.Add(initialState);
	}

	private Color RandomColor()
	{
		return new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
	}

	private bool _isMouseDown = false;

	public override void _Input(InputEvent @event)
	{
		if(Input.IsKeyPressed(Key.Up))
		{
			updateTickRate -= 0.1f;
		}
		if(Input.IsKeyPressed(Key.Down))
		{
			updateTickRate += 0.1f;
		}
		if(Input.IsKeyPressed(Key.Left))
		{
			// create the walker pattern at mouse position
			
			
		}
		if(Input.IsKeyPressed(Key.Right))
		{
			GD.Print("Right");
		}		
		
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.DoubleClick)
			{
				_isMouseDown = false;
				return;
			}

			if (mouseButtonEvent.Pressed)
			{
				_isMouseDown = mouseButtonEvent.Pressed;
				if (_isMouseDown)
				{
					var mousePosition = mouseButtonEvent.Position;
					var x = (int) (mousePosition.X / _boxSize);
					var y = (int) (mousePosition.Y / _boxSize);
					ToggleCell(new Vector2(x, y));
					QueueRedraw();
				}
				if (!_isMouseDown)
				{
					_isMouseDown = false;
				}
			}
		}
		else if (@event is InputEventMouseMotion mouseMotionEvent)
		{
			if (_isMouseDown) // If the mouse button is being held, then draw at the new position
			{
				
				var mousePosition = mouseMotionEvent.Position;
				var x = (int) (mousePosition.X / _boxSize);
				var y = (int) (mousePosition.Y / _boxSize);

				ToggleCell(new Vector2(x, y));
				QueueRedraw();
			}
		}
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_timeElapsed += delta;
		if(_timeElapsed >= updateTickRate)  
		{
			_gridCells[_currentStateIndex] = ApplyConwaysRules(_gridCells[_currentStateIndex]);
			_timeElapsed = 0.0;
		}
		
		QueueRedraw();
	}

	public void ToggleCell(Vector2 cellCoords)
	{
		var x = (int) cellCoords.X;
		var y = (int) cellCoords.Y;

		if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
		{
			GD.PrintErr("Coordinates out of bounds");
			return;
		}

		var currentGridState = _gridCells[_currentStateIndex];
		
		if(currentGridState[x, y] == null)
		{
			currentGridState[x, y] = new Cell()
			{
				IsAlive = true, 
				Position = new Vector2(x, y),
				Color = new Color((float) rand.NextDouble(), (float) rand.NextDouble(), (float) rand.NextDouble())
			};
			return;
		}
		
		currentGridState[x, y].IsAlive = !currentGridState[x, y].IsAlive;
	}

	private void DrawBox(int x, int y)
	{
		if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
		{
			GD.PrintErr("Coordinates out of bounds");
			return;
		}

		var currentGridState = _gridCells[_currentStateIndex];

		var color = currentGridState[x, y].Color;
		
		DrawRect(new Rect2(x * _boxSize, y * _boxSize, _boxSize, _boxSize), color);
	}	

	public Tuple<int, int> GetRandomCoords()
	{
		var randomX = new Random().Next(0, gridWidth);
		var randomY = new Random().Next(0, gridHeight);
		return Tuple.Create(randomX, randomY);
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
	
	public override void _Draw()
	{
		var currentGridState = _gridCells[_currentStateIndex];
		for (var x = 0; x < gridWidth; x++)
		{
			for (var y = 0; y < gridHeight; y++)
			{
				//Check if there is a cell at this position
				if (currentGridState[x, y] == null)
				{
					continue;
				}

				if (drawDeadCell)
				{
					if (true)
					{
						DrawBox(x, y);
					}
				}
				else
				{
					if (currentGridState[x, y].IsAlive)
					{
						DrawBox(x, y);
					}
				}
			}
		}

		if (_debugState)
		{
			for (var x = 0; x <= gridWidth; x++)
			{
				DrawLine(new Vector2(x * _boxSize, 0), new Vector2(x * _boxSize, gridHeight * _boxSize), new Color(0, 0, 0));
			}

			for (var y = 0; y <= gridHeight; y++)
			{
				DrawLine(new Vector2(0, y * _boxSize), new Vector2(gridWidth * _boxSize, y * _boxSize), new Color(0, 0, 0));
			}
		}
	}



	public Cell[,] ApplyConwaysRules(Cell[,] currentGrid)
	{
		var newGrid = new Cell[gridWidth, gridHeight];

		for (var x = 0; x < gridWidth; x++)
		{
			for (var y = 0; y < gridHeight; y++)
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

	private static Color GetCellColor(int liveNeighbors)
	{
		System.Collections.Generic.Dictionary<int, Color> colorMap = new()
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

	public int CountLiveNeighbors(Cell[,] grid, int x, int y)
	{
		var count = 0;
		var width = grid.GetLength(0);
		var height = grid.GetLength(1);

		// Define offsets for all 8 neighbors
		int[] xOffset = { -1, 0, 1, -1, 1, -1, 0, 1 };
		int[] yOffset = { -1, -1, -1, 0, 0, 1, 1, 1 };

		for (int i = 0; i < 8; i++)
		{
			int nx = x + xOffset[i];
			int ny = y + yOffset[i];

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
}