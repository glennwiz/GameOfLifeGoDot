using Godot;
using System;
using System.Collections.Generic;

public partial class GridManager : Node2D
{
	private bool debug = true;
	private int gridWidth { get; set;}
	private int gridHeight { get; set;}
	private int boxSize = 10;
	private List<Cell[,]> gridCells = new List<Cell[,]>();
	private int currentStateIndex = 0; // The index of the current state in the gridStates list we keep track of 100 states
	Random rand = new Random();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Ready!!");
		InitGrid();
	}
	
	private void InitGrid()
	{
		var viewportSize = GetViewportRect().Size;
		gridWidth = (int)viewportSize.X / boxSize;
		gridHeight = (int)viewportSize.Y / boxSize;
		var initialState = new Cell[gridWidth, gridHeight];
		initialState[5, 5] = new Cell() { IsAlive = true, Position = new Vector2(5, 5) };
		gridCells.Add(initialState);
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

		var currentGridState = gridCells[currentStateIndex];
		
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

		var currentGridState = gridCells[currentStateIndex];

		var color = currentGridState[x, y].Color;
		
		DrawRect(new Rect2(x * boxSize, y * boxSize, boxSize, boxSize), color);
	}	

	public Tuple<int, int> GetRandomCoords()
	{
		var randomX = new Random().Next(0, gridWidth);
		var randomY = new Random().Next(0, gridHeight);
		return Tuple.Create(randomX, randomY);
	}

	public void SaveState()
	{
		var currentGridState = gridCells[currentStateIndex];
		var newGridState = (Cell[,]) currentGridState.Clone();
		gridCells.Add(newGridState);
		currentStateIndex++;

		// Optionally, limit the history to the last 100 states
		if (gridCells.Count > 100)
		{
			gridCells.RemoveAt(0);
			currentStateIndex--;
		}
	}
	
	public override void _Draw()
	{
		var currentGridState = gridCells[currentStateIndex];
		GD.Print("Current state index: " + currentStateIndex);
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

		if (debug)
		{
			for (int x = 0; x <= gridWidth; x++)
			{
				DrawLine(new Vector2(x * boxSize, 0), new Vector2(x * boxSize, gridHeight * boxSize), new Color(0, 0, 0));
			}

			for (int y = 0; y <= gridHeight; y++)
			{
				DrawLine(new Vector2(0, y * boxSize), new Vector2(gridWidth * boxSize, y * boxSize), new Color(0, 0, 0));
			}
		}
	}
}