using System;
using System.Linq;
using System.Collections.Generic;

using ThirdParty.Utils;

using UnityEngine;

public class PathFinder
{
    /// <summary>
    /// Precomputed square root of 2 for diagonal movement cost.
    /// </summary>
    private const float sqrtOf2 = 1.4142135f;

    private readonly (Vector2Int, float)[] neighborsRelative = new (Vector2Int, float)[]
    {
        // cardinal neighbors
        (new Vector2Int(0, 1), 1.0f),
        (new Vector2Int(-1, 0), 1.0f),
        (new Vector2Int(0, -1), 1.0f),
        (new Vector2Int(1, 0), 1.0f),
        // diagonal neighbors
        (new Vector2Int(1, 1), sqrtOf2),
        (new Vector2Int(-1, 1), sqrtOf2),
        (new Vector2Int(1, -1), sqrtOf2),
        (new Vector2Int(-1, -1), sqrtOf2),
    };

    /// <summary>
    /// Grid representing whether each cell is walkable.
    /// </summary>
    private readonly bool[,] grid;

    /// <summary>
    /// The bounds of the grid in world coordinates.
    /// </summary>
    public Rect Bounds { get; }

    /// <summary>
    /// Number of cells alon the x and y axes.
    /// </summary>
    public Vector2Int Size { get; }

    /// <summary>
    /// Radius used for physics overlap checks when populating the grid.
    /// </summary>
    public float Radius { get; }
    
    /// <summary>
    /// Size of each cell in world units.
    /// </summary>
    public float CellSize { get; }

    /// <param name="bounds">World bounds of the grid.</param>
    /// <param name="cellSize">Size of each cell in world units.</param>
    /// <param name="radius">Radius used for physics overlap checks when populating the grid.</param>
    public PathFinder(Rect bounds, float cellSize, float radius)
    {
        Bounds = bounds;
        CellSize = cellSize;
        Radius = radius;
        Size = Vector2Int.RoundToInt(Bounds.size / CellSize);

        grid = new bool[Size.y, Size.x];
        CreateGrid();
    }

    /// <summary>
    /// Visualizes the grid in the Unity editor using Gizmos.  Each cell is displayed as a sphere (green if walkable,
    /// red otherwise).
    /// </summary>
    public void DrawGizmos()
    {
        foreach (var cell in GetCells())
        {
            Gizmos.color = IsCellWalkable(cell) ? Color.green : Color.red;
            Gizmos.DrawSphere(GetWorldPosition(cell), Radius);
        }
    }

    /// <summary>
    /// Finds a path (with post-processing for simplification) between two world-space points.
    /// </summary>
    /// <param name="start">The start position in world coordinates.</param>
    /// <param name="goal">The goal position in world coordinates.</param>
    /// <returns>A list of world positions representing the path.</returns>
    public IReadOnlyList<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        Vector2Int startCell = GetNearestCell(start);
        Vector2Int goalCell = GetNearestCell(goal);

        List<Vector2Int> gridPath = FindPath(startCell, goalCell);
        List<Vector2> path = ToWorldPositions(gridPath);
        List<Vector2> simplifiedPath = SimplifyPath(path, start, goal);

        return simplifiedPath;
    }

    /// <summary>
    /// Transform each cell in path into world position.
    /// </summary>
    private List<Vector2> ToWorldPositions(List<Vector2Int> path)
    {
        var resultPath = new List<Vector2>(path.Count);
     
        foreach (var cell in path)
            resultPath.Add(GetWorldPosition(cell));

        return resultPath;
    }

    /// <summary>
    /// Simplifies the grid path by checking line-of-sight between waypoints.
    /// </summary>
    /// <param name="path">A list of grid cells from start to goal.</param>
    /// <param name="start">Original start world position.</param>
    /// <param name="goal">Original goal world position.</param>
    /// <returns>A simplified world-space path.</returns>
    private List<Vector2> SimplifyPath(List<Vector2> path, Vector2 start, Vector2 goal)
    {
        var resultPath = new List<Vector2>() { start };
        Vector2 last = path[0];

        for (int i = 0; i < path.Count; i++)
        {
            if (IsLineWalkable(resultPath.Last(), path[i]))
                last = path[i];
            else
            {
                resultPath.Add(last);
                if (i + 1 != path.Count)
                    last = path[i + 1];
            }
        }

        if (!IsLineWalkable(resultPath.Last(), last))
            resultPath.Add(last);
        resultPath.Add(goal);
        
        return resultPath;
    }

    /// <summary>
    /// Checks whether the straight line between two world-space points is walkable
    /// (i.e., every cell along the line is walkable).
    /// </summary>
    /// <param name="start">Line start in world coordinates.</param>
    /// <param name="end">Line end in world coordinates.</param>
    /// <returns>True if the line is walkable, false otherwise.</returns>
    private bool IsLineWalkable(Vector2 start, Vector2 end)
    {
        if ((end - start).sqrMagnitude < Mathf.Epsilon)
            return true;

        float distance = Vector2.Distance(start, end);
        int sampleCount = Mathf.Max(1 ,(int)(distance / (CellSize)));
        Vector2 step = CellSize * (end - start).normalized;

        for (int i = 0; i < sampleCount; i++)
        {
            Vector2Int cell = GetNearestCell(start + i * step);

            if (!IsCellWalkable(cell))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Performs grid-based A* search to find a path from start to goal in grid cell indices.
    /// </summary>
    /// <param name="start">Start cell index.</param>
    /// <param name="goal">Goal cell index.</param>
    /// <returns>A list of grid cell indices representing the path.</returns>
    private List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        var frontier = new PriorityQueue<Vector2Int, float>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var costSoFar = new Dictionary<Vector2Int, float>();

        frontier.Enqueue(start, 0.0f);
        costSoFar.Add(start, 0.0f);

        bool pathFound = false;


        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == goal)
            {
                pathFound = true;
                break;
            }

            foreach (var (neighbor, cost) in GetNeighbors(current))
            {
                float newCost = costSoFar[current] + cost;

                if (costSoFar.TryGetValue(neighbor, out float currentCost) && newCost >= currentCost)
                    continue;

                costSoFar[neighbor] = newCost;
                cameFrom[neighbor] = current;
                frontier.Enqueue(neighbor, newCost + Vector2Int.Distance(neighbor, goal));
            }
        }

        if (!pathFound)
            throw new InvalidOperationException("No valid path exists between the specified points.");

        var path = new List<Vector2Int>() { goal };
        while (cameFrom.ContainsKey(path.Last()))
            path.Add(cameFrom[path.Last()]);
        path.Reverse();

        return path;
    }

    /// <summary>
    /// Populates the grid by performing physics overlap checks for each cell to determine if it is walkable.
    /// </summary>
    private void CreateGrid()
    {
        int layerMask = LayerMask.GetMask("Default");

        foreach (var cell in GetCells())
        {
            bool isWalkable = Physics2D.OverlapCircle(GetWorldPosition(cell), Radius, layerMask) == null;
            SetCellWalkable(cell, isWalkable);
        }
    }

    /// <summary>
    /// Enumerates all cell coordinates within the grid.
    /// </summary>
    /// <returns>An enumerable collection of cell coordinates.</returns>
    private IEnumerable<Vector2Int> GetCells()
    {
        for (int y = 0; y<Size.y; y++)
        {
            for (int x = 0; x<Size.x; x++)
            {
                yield return new Vector2Int(x, y);
            }
        }
    }

    /// <summary>
    /// Returns the neighbors (adjacent cells) for a given cell along with their movement cost.
    /// </summary>
    /// <param name="cell">The current cell.</param>
    /// <returns>An enumerable of neighbor cells and their cost.</returns>
    private IEnumerable<(Vector2Int, float)> GetNeighbors(Vector2Int cell)
    {
        var bounds = new RectInt(Vector2Int.zero, Size);

        foreach (var (relativeNeighbor, cost) in neighborsRelative)
        {
            Vector2Int neighbor = cell + relativeNeighbor;

            if (!bounds.Contains(neighbor))
                continue;
            if (!IsCellWalkable(neighbor))
                continue;

            yield return (neighbor, cost);
        }
    }

    /// <summary>
    /// Converts a cell's grid coordinates to its corresponding world position.
    /// </summary>
    /// <param name="cell">The grid coordinates of the cell.</param>
    /// <returns>The world position of the cell.</returns>
    private Vector2 GetWorldPosition(Vector2Int cell) => Bounds.min + (Vector2)cell * CellSize;

    /// <summary>
    /// Converts a world-space position to its nearest grid cell indices.
    /// </summary>
    /// <param name="position">The world position.</param>
    /// <returns>The grid cell indices closest to the given position.</returns>
    private Vector2Int GetNearestCell(Vector2 position)
    {
        Vector2Int cell = Vector2Int.RoundToInt((position - Bounds.min) / CellSize);
        cell = new Vector2Int
        (
            (int)Mathf.Clamp(cell.x, 0.0f, Size.x - 1),
            (int)Mathf.Clamp(cell.y, 0.0f, Size.y - 1)
        );

        return cell;
    }

    /// <summary>
    /// Checks whether a given cell is walkable.
    /// </summary>
    /// <param name="cell">Grid cell.</param>
    /// <returns>True if the cell is walkable; false otherwise.</returns>
    private bool IsCellWalkable(Vector2Int cell)
    {
        return grid[cell.y, cell.x];
    }

    /// <summary>
    /// Sets the walkability state for a given cell.
    /// </summary>
    /// <param name="cell">Grid cell.</param>
    /// <param name="walkable">True if the cell is walkable; false otherwise.</param>
    private void SetCellWalkable(Vector2Int cell, bool walkable)
    {
        grid[cell.y, cell.x] = walkable;
    }
}
