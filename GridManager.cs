using Godot;
using System;
using System.Collections.Generic;

public partial class GridManager : Node2D
{
	Random rand = new ();
	private int _boxSize = 10;
	private bool _debugState = true;
	private int _currentStateIndex = 0; // The index of the current state in the gridStates list we keep track of 100 states
	private List<Cell[,]> _gridCells = new ();
	
	private int gridWidth { get; set;}
	private int gridHeight { get; set;}
	
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
		initialState[5, 5] = new Cell
		{
			IsAlive = true, 
			Position = new Vector2(5, 5)
		};
		_gridCells.Add(initialState);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.Pressed)
			{
				var mousePosition = mouseButtonEvent.Position;
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
		var randomCoords = GetRandomCoords();
		var randomX = randomCoords.Item1;
		var randomY = randomCoords.Item2;
		ToggleCell(new Vector2(randomX, randomY));
		
		//we need to update the screen every frame
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
		GD.Print("Current state index: " + _currentStateIndex);
		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				//Check if there is a cell at this position
				if (currentGridState[x, y] == null)
				{
					continue;
				}
				
				if (currentGridState[x, y].IsAlive)
				{
					DrawBox(x, y);
				}
			}
		}

		if (_debugState)
		{
			for (int x = 0; x <= gridWidth; x++)
			{
				DrawLine(new Vector2(x * _boxSize, 0), new Vector2(x * _boxSize, gridHeight * _boxSize), new Color(0, 0, 0));
			}

			for (int y = 0; y <= gridHeight; y++)
			{
				DrawLine(new Vector2(0, y * _boxSize), new Vector2(gridWidth * _boxSize, y * _boxSize), new Color(0, 0, 0));
			}
		}
	}
}