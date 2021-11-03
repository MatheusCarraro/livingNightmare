using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities {
	public class GridPosition : IComparable<GridPosition> {
		public Vector2Int position = Vector2Int.zero;

		public Vector3 worldPosition {
			get {
				return GridController.GetWorldPosition(position);
			}
		}
		public bool allowMovement = true;
		public int movementCost = 1;
		public int priority = 0;
		Color myColor = Color.white;

		public void SetPosition(int x, int y) {
			position.Set(x, y);
		}

		public GridPosition[] FindNeighbors(int[, ] coords = null, Func<GridPosition, bool> isNeighborTest = null/*GridPosition goal = null */) {
			if (coords == null) coords = Miscellaneous.coord4;
			List<GridPosition> neighbors = new List<GridPosition>();
			for (int i = 0; i < coords.GetLength(0); ++i) {

				int x = position.x + coords[i, 1], y = position.y + coords[i, 0];

				if (!(x >= 0 && x < GridController.instance.gridWidth && y >= 0 && y < GridController.instance.gridHeight)) continue;

				if (isNeighborTest == null || isNeighborTest(GridController.instance.grid[y, x])/*goal == GridController.instance.grid[y, x] */) {
					neighbors.Add(GridController.instance.grid[y, x]);
				}
			}
			return neighbors.ToArray();
		}

		public GridPosition[] GetBorderArea(int radius) {

			List<GridPosition> neighbors = new List<GridPosition>();
			int _x = -radius;
			for (int i = 0; i < radius * 4; i++) {

				_x += (i % 2 == 0 ? 0 : 1);
				int _y = (radius - Mathf.Abs(_x)) * (i % 2 == 0 ? -1 : 1);
				int x = position.x + _x;
				int y = position.y + _y;

				if (!(x >= 0 && x < GridController.instance.gridWidth && y >= 0 && y < GridController.instance.gridHeight)) continue;

				if (GridController.instance.grid[y, x].allowMovement)
					neighbors.Add(GridController.instance.grid[y, x]);
			}
			Debug.Log("Tiles in Border: " + neighbors.Count);
			return neighbors.ToArray();
		}

		public GridPosition[] GetInArea(int radius) {

			List<GridPosition> neighbors = new List<GridPosition>();

			for (int j = -radius; j <= radius; j++) {
				for (int i = -radius; i <= radius; i++) {

					int x = position.x + i;
					int y = position.y + j;

					if ((Mathf.Abs(i + j) > radius || (i == 0 && j == 0)) || !(x >= 0 && x < GridController.instance.gridWidth && y >= 0 && y < GridController.instance.gridHeight)) continue;

					if (GridController.instance.grid[y, x] != this && GridController.instance.grid[y, x].allowMovement)
						neighbors.Add(GridController.instance.grid[y, x]);
				}
			}
			Debug.Log("Tiles in Area: " + neighbors.Count);
			return neighbors.ToArray();
		}

		public bool IsAccessible(int[, ] coords = null) {
			if (!allowMovement) return false;
			if (coords == null) coords = Miscellaneous.coord4;
			for (int i = 0; i < coords.GetLength(0); ++i) {

				int x = position.x + coords[i, 1], y = position.y + coords[i, 0];

				if (!(x >= 0 && x < GridController.instance.gridWidth && y >= 0 && y < GridController.instance.gridHeight)) continue;

				if (GridController.instance.grid[y, x].allowMovement) return true;
			}
			return false;
		}

		public int CompareTo(GridPosition other) {
			if (this.priority < other.priority) return -1;
			if (this.priority == other.priority) return 0;
			return 1;
		}
	}
}