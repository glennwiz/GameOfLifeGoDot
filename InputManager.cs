using System.Diagnostics.CodeAnalysis;
using Godot;

namespace GameOfLife;

public partial class InputManager : Node2D
{
	private readonly Grid _grid;
	private readonly GridController _controller;

	public InputManager(Grid grid, GridController controller)
	{
		_grid = grid;
		_controller = controller;
	}

	[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
	public override void _Input(InputEvent @event)
	{
		if (Input.IsKeyPressed(Key.Space))
		{
			_grid.IsPaused = !_grid.IsPaused;
		}
		
		if (_grid.IsPaused) {
			if (Input.IsKeyPressed(Key.Left)) {
				_grid.Rewind();
			}
			if (Input.IsKeyPressed(Key.Right)) {
				_grid.StepForward(); // You'll need to implement this
			}
		}

		// Handle 'S' key press to clear cells and reset speed
		if (Input.IsKeyPressed(Key.S))
		{
			_grid._matrixManipulation.ClearGrid(_grid._gridCells);
			_grid.ResetUpdateTickRate();

			QueueRedraw();
		}

		if (Input.IsKeyPressed(Key.Q))
		{
			_grid._drawDeadCell = !_grid._drawDeadCell;
		}

		if (Input.IsKeyPressed(Key.Up))
		{
			_grid.UpdateTickRate -= 0.1f;
		}

		if (Input.IsKeyPressed(Key.Down))
		{
			_grid.UpdateTickRate += 0.1f;
		}

		// Handle mouse input to toggle cells
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.DoubleClick)
			{
				_grid._isMouseDown = false;
				return;
			}

			if (mouseButtonEvent.Pressed)
			{
				_grid._isMouseDown = mouseButtonEvent.Pressed;
				if (_grid._isMouseDown)
				{
					var mousePosition = mouseButtonEvent.Position;
					var x = (int) (mousePosition.X / _grid.BoxSize);
					var y = (int) (mousePosition.Y / _grid.BoxSize);
					_grid.ToggleCell(new Vector2(x, y));
					QueueRedraw();
				}

				if (!_grid._isMouseDown)
				{
					_grid._isMouseDown = false;
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

			var glider = new PatternCreator.Pattern(3, 3, gliderCells);
			_grid.DrawPattern(glider);
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
			
			var gosperGliderGun = new PatternCreator.Pattern(36, 9, rotated);
			_grid.DrawPattern(gosperGliderGun);
		}

		if (Input.IsKeyPressed(Key.Right))
		{
			GD.Print("Right");
		}
	}
}