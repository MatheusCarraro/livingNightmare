using System.Collections.Generic;
using UnityEngine;

namespace Utilities {
#if UNITY_EDITOR
	[ExecuteInEditMode]
#endif
	public class GridController : SingletonMonoBehaviour<GridController> {
		public int gridWidth,
		gridHeight;
		public Vector2 spacing = Vector2.one,
		offset = Vector2.zero;
		public GridPosition[, ] grid = new GridPosition[0, 0];

		public GridPosition startTile,
		goalTile;
		public bool interactable = false;

		public override void Awake() {
			base.Awake();
		}

		void Start() {

		}

		public void CreateGrid(int w, int h, bool[, ] allow = null) {
			gridWidth = w;
			gridHeight = h;
			grid = new GridPosition[h, w];
			for (int x = 0; x < w; ++x) {
				for (int y = 0; y < h; ++y) {
					GridPosition newPosition = new GridPosition();
					newPosition.SetPosition(x, y);
					if (allow != null) newPosition.allowMovement = allow[y, x];
					grid[y, x] = newPosition;
				}
			}
		}

		public void DestroyGrid() {
			foreach (Transform child in transform) Destroy(child.gameObject);
			grid = null;
			//Miscellaneous.Populate<GridPosition>(grid, null);
		}

		public static GridPosition GetPosition(Vector2Int position) {
			return instance.grid[position.y, position.x];
		}

		public static GridPosition RandomPosition() {
			Vector2Int position;

			do {
				position = new Vector2Int(Random.Range(0, instance.gridWidth), Random.Range(0, instance.gridHeight));
			} while (!instance.grid[position.y, position.x].allowMovement);

			return instance.grid[position.y, position.x];
		}

		public static int Heuristic(GridPosition a, GridPosition b) {
			// Manhattan distance on a square grid
			return Mathf.Abs(a.position.x - b.position.x) + Mathf.Abs(a.position.y - b.position.y);
		}

		public static int DiagonalDistance(GridPosition a, GridPosition b) {
			// D * max(dx, dy) + (D2-D) * min(dx, dy); D = D2 = 1;
			int dx = Mathf.Abs(a.position.x - b.position.x);
			int dy = Mathf.Abs(a.position.y - b.position.y);
			return Mathf.Max(dx, dy) * Mathf.Min(dx, dy);
		}

		public static List<GridPosition> AStar(Vector2Int start, Vector2Int goal, bool diagonal = false, System.Func<GridPosition, bool> isNeighborTest = null) {
			return AStar(instance.grid[start.y, start.x], instance.grid[goal.y, goal.x], diagonal, isNeighborTest);
		}

		public static List<GridPosition> AStar(Vector2Int start, GridPosition goal, bool diagonal = false, System.Func<GridPosition, bool> isNeighborTest = null) {
			return AStar(instance.grid[start.y, start.x], goal, diagonal, isNeighborTest);
		}

		public static List<GridPosition> AStar(GridPosition start, Vector2Int goal, bool diagonal = false, System.Func<GridPosition, bool> isNeighborTest = null) {
			return AStar(start, instance.grid[goal.y, goal.x], diagonal, isNeighborTest);
		}

		public static List<GridPosition> AStar(GridPosition start, GridPosition goal, bool diagonal = false, System.Func<GridPosition, bool> isNeighborTest = null) {
			PriorityQueue<GridPosition> frontier = new PriorityQueue<GridPosition>();
			Dictionary<GridPosition, GridPosition> came_from = new Dictionary<GridPosition, GridPosition>();
			Dictionary<GridPosition, int> cost_so_far = new Dictionary<GridPosition, int>();
			start.priority = 0;
			frontier.Enqueue(start);
			came_from[start] = null;
			cost_so_far[start] = 0;

			while (!frontier.Empty) {
				GridPosition current = frontier.Dequeue();
				if (current == goal) break;

				foreach (GridPosition next in current.FindNeighbors(diagonal ? Miscellaneous.coord8 : Miscellaneous.coord4, isNeighborTest)) {
					int new_cost = cost_so_far[current] + next.movementCost;
					if (!cost_so_far.ContainsKey(next) || new_cost < cost_so_far[next]) {
						cost_so_far[next] = new_cost;
						next.priority = new_cost + Heuristic(goal, next);
						frontier.Enqueue(next);
						came_from[next] = current;
					}
				}
			}

			return GetPath(start, goal, came_from);
		}

		static List<GridPosition> GetPath(GridPosition start, GridPosition goal, Dictionary<GridPosition, GridPosition> came_from) {
			List<GridPosition> path = new List<GridPosition>();
			GridPosition current = goal;
			while (current != start && came_from.ContainsKey(current)) {
				if (current.allowMovement) path.Insert(0, current);
				current = came_from[current];
			}

			if (path.Count > 0 && path[0] == null) path.Clear();
			else path.Remove(start);
			return path;
		}

		public static Vector3 GetWorldPosition(Vector2Int position) {
			return new Vector3(instance.offset.x + (position.x * instance.spacing.x), instance.offset.y + (position.y * instance.spacing.y));
		}

		public static Vector3 SnapWorldPosition(Vector3 position) {
			return new Vector3(instance.offset.x + (position.x * instance.spacing.x), instance.offset.y + (position.y * instance.spacing.y));
		}

		public static Vector2Int GetGridPosition(Vector3 position) {
			return new Vector2Int((int) Mathf.RoundToInt((position.x - instance.offset.x) / instance.spacing.x), (int) Mathf.RoundToInt((position.y - instance.offset.y) / instance.spacing.y));
		}

#if UNITY_EDITOR

		Vector3 GetWorldPositionEditor(Vector2Int position) {
			return new Vector3(offset.x + (position.x * spacing.x), offset.y + (position.y * spacing.y));
		}

		Vector3 SnapWorldPositionEditor(Vector3 position) {
			return new Vector3(offset.x + (position.x * spacing.x), offset.y + (position.y * spacing.y));
		}

		Vector2Int GetGridPositionEditor(Vector3 position) {
			return new Vector2Int((int) Mathf.RoundToInt((position.x - offset.x) / spacing.x), (int) Mathf.RoundToInt((position.y - offset.y) / spacing.y));
		}

		void OnDrawGizmos() {
			if (grid != null) {
				for (int i = 0; i < grid.GetLength(1); ++i) {
					for (int j = 0; j < grid.GetLength(0); ++j) {
						GridPosition gp = grid[j, i];
						Gizmos.DrawWireCube(GetWorldPositionEditor(gp.position), Vector3.one * Mathf.Min(spacing.x, spacing.y));
						if (!gp.allowMovement) Gizmos.color = new Color(1, 0, 0, 0.25f);
						else Gizmos.color = new Color(1, 1, 1, 0.25f);
						Gizmos.DrawCube(GetWorldPositionEditor(gp.position), Vector3.one * Mathf.Min(spacing.x, spacing.y));
						Gizmos.color = Color.white;
					}
				}
			}
		}
#endif
	}
}