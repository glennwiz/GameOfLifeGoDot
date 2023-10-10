using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace GameOfLife;

public partial class GridManager : Node2D 
{
	
	private Grid _grid;
	private GridRenderer _renderer;
	private GridController _controller;
	private InputManager _inputManager;
	private PatternCreator _patternCreator;
	private MatrixManipulation _matrixManipulation = new MatrixManipulation();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{  
		_grid = new Grid(_matrixManipulation);
		_renderer = new GridRenderer(_grid);
		_controller = new GridController(_grid);
		_inputManager = new InputManager(_grid, _controller);
		_patternCreator = new PatternCreator(_grid);
            
		AddChild(_grid);
		AddChild(_renderer);
		AddChild(_controller);
		AddChild(_inputManager);
		AddChild(_patternCreator);
	}
}