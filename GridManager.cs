using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace GameOfLife;

public partial class GridManager : Node2D 
{
	private readonly Random _random = new ();

	public float _updateTickRate = 0.5f;
	public double _timeElapsed;
	public bool _debugState = true;
	public bool _isMouseDown;	
	public bool _isPaused;
	public bool _drawDeadCell = true;
	public Color _newAliveColor = Colors.Yellow;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_matrixManipulation = new();
		GD.Print("Ready!!");
		InitGrid();
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
	
	// Default speed value
	private const float DefaultUpdateTickRate = 0.5f;
	
	public void ResetUpdateTickRate()
	{
		_updateTickRate = DefaultUpdateTickRate;
	}
}