using Godot;
using System;

namespace GameOfLife
{
	public partial class GridRenderer : Node2D
	{
		private Grid _grid;

		public GridRenderer(Grid grid)
		{
			_grid = grid;
		}

		private void DrawBox(int x, int y, Color color)
		{
			if (x < 0 || x >= _grid.GridWidth || y < 0 || y >= _grid.GridHeight)
			{
				GD.PrintErr("Coordinates out of bounds");
				return;
			}

			DrawRect(new Rect2(x * _grid.BoxSize, y * _grid.BoxSize, _grid.BoxSize, _grid.BoxSize), color);
		}

		public override void _Draw()
		{
			// Check if _currentStateIndex is within valid bounds
			if (_grid._currentStateIndex >= 0 && _grid._currentStateIndex < _grid._gridCells.Count)
			{
				var currentGridState = _grid._gridCells[_grid._currentStateIndex];

				for (var x = 0; x < _grid.GridWidth; x++)
				{
					for (var y = 0; y < _grid.GridHeight; y++)
					{
						// Check if there is a cell at this position
						if (currentGridState[x, y] == null)
						{
							continue;
						}

						if (_grid._drawDeadCell)
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

			if (_grid.DebugState)
			{
				for (var x = 0; x <= _grid.GridWidth; x++)
				{
					DrawLine(new Vector2(x * _grid.BoxSize, 0), new Vector2(x * _grid.BoxSize, _grid.GridHeight * _grid.BoxSize),
						new Color(0, 0, 0));
				}

				for (var y = 0; y <= _grid.GridHeight; y++)
				{
					DrawLine(new Vector2(0, y * _grid.BoxSize), new Vector2(_grid.GridWidth * _grid.BoxSize, y * _grid.BoxSize),
						new Color(0, 0, 0));
				}
			}
		}
	}
}