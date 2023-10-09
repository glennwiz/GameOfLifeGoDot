using Godot;
using System;

namespace GameOfLife
{
	public class GridRenderer : Node2D
	{
		private Grid _grid;

		public GridRenderer(Grid grid)
		{
			_grid = grid;
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
					DrawLine(new Vector2(x * _boxSize, 0), new Vector2(x * _boxSize, _gridHeight * _boxSize),
						new Color(0, 0, 0));
				}

				for (var y = 0; y <= _gridHeight; y++)
				{
					DrawLine(new Vector2(0, y * _boxSize), new Vector2(_gridWidth * _boxSize, y * _boxSize),
						new Color(0, 0, 0));
				}
			}
		}
	}
}