using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace GameOfLife;

public partial class GridManager : Node2D 
{
	private readonly Random _random = new ();
	private int _boxSize = 10;
	private float _updateTickRate = 0.5f;
	private double _timeElapsed;
	private bool _debugState = true;
	private int _currentStateIndex; 
	private readonly List<Cell[,]> _gridCells = new ();
	private bool _isMouseDown;	
	private bool _isPaused;
	
	private int _gridWidth;
	private int _gridHeight;

	private bool _drawDeadCell = true;
	private Color _newAliveColor = Colors.Yellow;

	private MatrixManipulation _matrixManipulation = null;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_matrixManipulation = new();
		GD.Print("Ready!!");
		InitGrid();
	}
	
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

	private Color RandomColor()
	{
		return new Color((float)_random.NextDouble(), (float)_random.NextDouble(), (float)_random.NextDouble());
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isPaused) return;
		
		_timeElapsed += delta;
		if (_timeElapsed >= _updateTickRate)
		{
			_gridCells[_currentStateIndex] = ApplyConwaysRules(_gridCells[_currentStateIndex]);
			SaveState();
			_timeElapsed = 0.0;
		}

		QueueRedraw();
	}

	[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
	public override void _Input(InputEvent @event)
	{
		if (Input.IsKeyPressed(Key.Space))
		{
			_isPaused = !_isPaused;
		}
		
		if (_isPaused) {
			if (Input.IsKeyPressed(Key.Left)) {
				Rewind();
			}
			if (Input.IsKeyPressed(Key.Right)) {
				StepForward(); 
			}
		}

		// Handle 'S' key press to clear cells and reset speed
		if (Input.IsKeyPressed(Key.S))
		{
			_matrixManipulation.ClearGrid(_gridCells);
			ResetUpdateTickRate();

			QueueRedraw();
		}

		if (Input.IsKeyPressed(Key.Q))
		{
			_drawDeadCell = !_drawDeadCell;
		}

		if (Input.IsKeyPressed(Key.Up))
		{
			_updateTickRate -= 0.1f;
		}

		if (Input.IsKeyPressed(Key.Down))
		{
			_updateTickRate += 0.1f;
		}

		// Handle mouse input to toggle cells
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

		if (Input.IsKeyPressed(Key.Key1))
		{
			// create the walker pattern at mouse position
			var gliderCells = new[,]
			{
				{false, true , false},
				{false, false, true},
				{true , true , true}
			};

			var glider = new Pattern(3, 3, gliderCells);
			DrawPattern(glider);
		}

		if (Input.IsKeyPressed(Key.Key2))
		{
			const bool f = false; //too easier read the pattern in the grid
		
			// create a Gosper's glider gun at mouse position
			var gosperCells =  new[,]
			{
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

			var rotated = MatrixManipulation.RotateMatrix90(gosperCells);
			
			var gosperGliderGun = new Pattern(36, 9, rotated);
			DrawPattern(gosperGliderGun);
		}

		if (Input.IsKeyPressed(Key.Right))
		{
			GD.Print("Right");
		}
	}

	private void StepForward()
	{
		_currentStateIndex++;
		
		if (_currentStateIndex >= _gridCells.Count)
		{
			_currentStateIndex = _gridCells.Count - 1;
		}
		
		QueueRedraw();
	}

	private void Rewind()
	{
		_currentStateIndex--;
		
		if (_currentStateIndex < 0)
		{
			_currentStateIndex = 0;
		}
		
		QueueRedraw();
	}

	private void DrawPattern(Pattern pattern)
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

	private void DrawBox(int x, int y, Color color)
	{
		if (x < 0 || x >= _gridWidth || y < 0 || y >= _gridHeight)
		{
			GD.PrintErr("Coordinates out of bounds");
			return;
		}

		DrawRect(new Rect2(x * _boxSize, y * _boxSize, _boxSize, _boxSize), color);
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
	
	public override void _Draw()
	{
		// Check if _currentStateIndex is within valid bounds
		if (_currentStateIndex >= 0 && _currentStateIndex < _gridCells.Count)
		{
			var currentGridState = _gridCells[_currentStateIndex];
        
			for (var x = 0; x < _gridWidth; x++)
			{
				for (var y = 0; y < _gridHeight; y++)
				{
					// Check if there is a cell at this position
					if (currentGridState[x, y] == null)
					{
						continue;
					}

					if (_drawDeadCell)
					{
						// Use the cell's color when drawing
						DrawBox(x, y, currentGridState[x, y].Color);
					}
					else
					{
						if (currentGridState[x, y].IsAlive)
						{
							// Use the cell's color when drawing
							DrawBox(x, y, currentGridState[x, y].Color);
						}
					}
				}
			}
		}

		if (_debugState)
		{
			for (var x = 0; x <= _gridWidth; x++)
			{
				DrawLine(new Vector2(x * _boxSize, 0), new Vector2(x * _boxSize, _gridHeight * _boxSize), new Color(0, 0, 0));
			}

			for (var y = 0; y <= _gridHeight; y++)
			{
				DrawLine(new Vector2(0, y * _boxSize), new Vector2(_gridWidth * _boxSize, y * _boxSize), new Color(0, 0, 0));
			}
		}
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
	
	// Default speed value
	private const float DefaultUpdateTickRate = 0.5f;
	
	
	public void ResetUpdateTickRate()
	{
		_updateTickRate = DefaultUpdateTickRate;
	}
	
	public class Pattern
	{
		public int Width { get; } // Width of the pattern grid
		public int Height { get; } // Height of the pattern grid
		public bool[,] Cells { get; } // 2D array representing the pattern's initial state

		public Pattern(int width, int height, bool[,] cells)
		{
			Width = width;
			Height = height;
			Cells = cells;
		}
	}
}