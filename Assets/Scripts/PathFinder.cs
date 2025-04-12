using System.Collections.Generic;

using UnityEngine;

public class PathFinder
{
    /// <summary>
    /// Grid representing walkability status of each cell.
    /// </summary>
    private readonly bool[,] grid;
    
    public Rect Bounds { get; }

    /// <summary>
    /// Number of cells alon the x and y axes.
    /// </summary>
    public Vector2Int Size { get; }
    
    /// <summary>
    /// Radius of circle used for physics overlap checks to determine if a cell is walkable.
    /// </summary>
    public float Radius { get; }
    
    /// <summary>
    /// Size of each cell in world units.
    /// </summary>
    public float CellSize { get; }

    /// <param name="cellSize">Size of each cell in world units.</param>
    /// <param name="radius">
    /// Radius of circle used for physics overlap checks to determine if a cell is walkable.
    /// </param>
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
    /// Draws gizmos in the Unity editor to visualize the grid. Each cell is represented
    /// by a sphere colored green if walkable or red if not.
    /// </summary>
    public void DrawGizmos()
    {
        foreach (var cell in GetCells())
        {
            Gizmos.color = this[cell] ? Color.green : Color.red;
            Gizmos.DrawSphere(GetWorldPosition(cell), 0.05f);
        }
    }

    public IReadOnlyList<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        /*
         * 1. find nearest cells to start and goal
         * 2. find path from start-cell to goal-cell
         * 3. add start before start-cell and goal after goal-cell
         * 4. simplify the path
         */

        // TODO

        return null;
    }

    /// <summary>
    /// Populates the grid by performing physics overlap checks for each cell to determine
    /// if it is walkable.
    /// </summary>
    private void CreateGrid()
    {
        int layerMask = LayerMask.GetMask("Default");

        foreach (var cell in GetCells())
        {
            this[cell] = Physics2D.OverlapCircle(GetWorldPosition(cell), Radius, layerMask) == null;
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
    /// Converts a cell's grid coordinates to its corresponding world position.
    /// </summary>
    /// <param name="cell">The grid coordinates of the cell.</param>
    /// <returns>The world position of the cell.</returns>
    private Vector2 GetWorldPosition(Vector2Int cell) => Bounds.min + (Vector2)cell * CellSize;

    /// <summary>
    /// Gets or sets the walkability status of a specific cell in the grid.
    /// </summary>
    /// <param name="cell">The grid coordinates of the cell.</param>
    /// <returns><c>true</c> if the cell is walkable; otherwise, <c>false</c>.</returns>
    private bool this[Vector2Int cell]
    {
        get => grid[cell.y, cell.x];
        set => grid[cell.y, cell.x] = value;
    }
}
