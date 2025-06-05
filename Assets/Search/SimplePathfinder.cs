using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SimplePathfinder : MonoBehaviour
{
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private int nodesExplored = 0;
    
    public Material lineMaterial;
    private List<Vector3> visitedPositions = new List<Vector3>();

    public enum HeuristicType { Manhattan, Euclidean, Diagonal }
    public enum SearchStrategy { DFS, BFS, Dijkstra, AStar }

    public HeuristicType heuristic = HeuristicType.Manhattan;
    public SearchStrategy strategy = SearchStrategy.AStar;

    [HideInInspector] public Tile startTile;
    [HideInInspector] public Tile goalTile;
    [HideInInspector] public bool isPathResolved = false;

    public GameObject arrowPrefab;
    public Transform arrowParent;

    public Text pathOutputText;

    public void OnFindPathButton()
    {
        if (startTile == null || goalTile == null)
        {
            Debug.LogWarning("Select a start and goal tile first.");
            return;
        }

        ResetArrows();
        ResetTileColors();

        isPathResolved = true;
        nodesExplored = 0;
        stopwatch.Reset();
        stopwatch.Start();

        switch (strategy)
        {
            case SearchStrategy.DFS: RunDFS(startTile, goalTile); break;
            case SearchStrategy.BFS: RunBFS(startTile, goalTile); break;
            case SearchStrategy.Dijkstra: RunDijkstra(startTile, goalTile); break;
            case SearchStrategy.AStar: RunAStar(startTile, goalTile); break;
        }

        stopwatch.Stop();
    }


    private void ResetArrows()
    {
        foreach (GameObject arrow in GameObject.FindGameObjectsWithTag("PathArrow"))
            Destroy(arrow);
    }

    private void ResetTileColors()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Tile"))
        {
            Tile tile = obj.GetComponent<Tile>();
            if (tile == null) continue;
            var rend = tile.GetComponent<Renderer>();
            switch (tile.tileType)
            {
                case TileType.Wall: rend.material.color = Color.black; break;
                case TileType.Swamp: rend.material.color = Color.yellow; break;
                default: rend.material.color = Color.white; break;
            }
        }
    }



    //private void RunBFS(Tile start, Tile goal)
    //{
    //    Queue<PathNode> queue = new Queue<PathNode>();
    //    HashSet<Tile> visited = new HashSet<Tile>();

    //    queue.Enqueue(new PathNode(start, null));

    //    while (queue.Count > 0)
    //    {
    //        var current = queue.Dequeue();
    //        if (visited.Contains(current.tile)) continue;

    //        visited.Add(current.tile);
    //        if (current.tile == goal)
    //        {
    //            ShowPath(ReconstructPath(current));
    //            return;
    //        }

    //        foreach (var neighbor in GetNeighbors(current.tile))
    //        {
    //            if (!visited.Contains(neighbor) && neighbor.tileType != TileType.Wall)
    //                queue.Enqueue(new PathNode(neighbor, current));
    //        }
    //    }

    //    Debug.LogWarning("BFS: No path found.");
    //}
    private void RunBFS(Tile start, Tile goal)
    {
        Queue<PathNode> queue = new Queue<PathNode>();
        HashSet<Tile> visited = new HashSet<Tile>();

        queue.Enqueue(new PathNode(start, null));
        visitedPositions.Clear();

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            nodesExplored++; //Increment nodesExplored in Algorithms

            if (visited.Contains(current.tile)) continue;

            visited.Add(current.tile);
            visitedPositions.Add(current.tile.transform.position + Vector3.up * 0.05f); // Track for line

            if (current.tile == goal)
            {
                ShowPath(ReconstructPath(current));
                DrawVisitedPath();  // Draw after reaching goal
                return;
            }

            foreach (var neighbor in GetNeighbors(current.tile))
            {
                if (!visited.Contains(neighbor) && neighbor.tileType != TileType.Wall)
                    queue.Enqueue(new PathNode(neighbor, current));
            }
        }

        DrawVisitedPath();  // Even if path not found, show visited traversal
        Debug.LogWarning("BFS: No path found.");
    }



    private void RunDFS(Tile start, Tile goal)
    {
        Stack<PathNode> stack = new Stack<PathNode>();
        HashSet<Tile> visited = new HashSet<Tile>();
        visitedPositions.Clear();

        stack.Push(new PathNode(start, null));

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            nodesExplored++; //Increment nodesExplored in Algorithms
            if (visited.Contains(current.tile)) continue;

            visited.Add(current.tile);
            visitedPositions.Add(current.tile.transform.position + Vector3.up * 0.05f);

            if (current.tile == goal)
            {
                ShowPath(ReconstructPath(current));
                DrawVisitedPath();
                return;
            }

            foreach (var neighbor in GetNeighbors(current.tile))
            {
                if (!visited.Contains(neighbor) && neighbor.tileType != TileType.Wall)
                    stack.Push(new PathNode(neighbor, current));
            }
        }

        DrawVisitedPath();
        Debug.LogWarning("DFS: No path found.");
    }



    //private void RunDijkstra(Tile start, Tile goal)
    //{
    //    List<PathNode> open = new List<PathNode> { new PathNode(start, null, 0f) };
    //    HashSet<Tile> closed = new HashSet<Tile>();

    //    while (open.Count > 0)
    //    {
    //        var current = open.OrderBy(n => n.gCost).First();
    //        if (current.tile == goal)
    //        {
    //            ShowPath(ReconstructPath(current));
    //            return;
    //        }

    //        open.Remove(current);
    //        closed.Add(current.tile);

    //        foreach (var neighbor in GetNeighbors(current.tile))
    //        {
    //            if (closed.Contains(neighbor) || neighbor.tileType == TileType.Wall) continue;

    //            float cost = current.gCost + GetTileCost(neighbor);
    //            var existing = open.FirstOrDefault(n => n.tile == neighbor);

    //            if (existing == null)
    //                open.Add(new PathNode(neighbor, current, cost));
    //            else if (cost < existing.gCost)
    //            {
    //                existing.gCost = cost;
    //                existing.parent = current;
    //            }
    //        }
    //    }

    //    Debug.LogWarning("Dijkstra: No path found.");
    //}

    private void RunDijkstra(Tile start, Tile goal)
    {
        List<PathNode> open = new List<PathNode> { new PathNode(start, null, 0f) };
        HashSet<Tile> closed = new HashSet<Tile>();
        visitedPositions.Clear();

        while (open.Count > 0)
        {
            var current = open.OrderBy(n => n.gCost).First();
            open.Remove(current);
            visitedPositions.Add(current.tile.transform.position + Vector3.up * 0.05f);
            closed.Add(current.tile);

            nodesExplored++; //Increment nodesExplored in Algorithms


            if (current.tile == goal)
            {
                ShowPath(ReconstructPath(current));
                DrawVisitedPath();
                return;
            }

            foreach (var neighbor in GetNeighbors(current.tile))
            {
                if (closed.Contains(neighbor) || neighbor.tileType == TileType.Wall) continue;

                float cost = current.gCost + GetTileCost(neighbor);
                var existing = open.FirstOrDefault(n => n.tile == neighbor);

                if (existing == null)
                    open.Add(new PathNode(neighbor, current, cost));
                else if (cost < existing.gCost)
                {
                    existing.gCost = cost;
                    existing.parent = current;
                }
            }
        }

        DrawVisitedPath();
        Debug.LogWarning("Dijkstra: No path found.");
    }

    //private void RunAStar(Tile start, Tile goal)
    //{
    //    List<PathNode> open = new List<PathNode> { new PathNode(start, null, 0f, Heuristic(start, goal)) };
    //    HashSet<Tile> closed = new HashSet<Tile>();

    //    while (open.Count > 0)
    //    {
    //        var current = open.OrderBy(n => n.FCost).First();
    //        if (current.tile == goal)
    //        {
    //            ShowPath(ReconstructPath(current));
    //            return;
    //        }

    //        open.Remove(current);
    //        closed.Add(current.tile);

    //        foreach (var neighbor in GetNeighbors(current.tile))
    //        {
    //            if (closed.Contains(neighbor) || neighbor.tileType == TileType.Wall) continue;

    //            float g = current.gCost + GetTileCost(neighbor);
    //            float h = Heuristic(neighbor, goal);
    //            var existing = open.FirstOrDefault(n => n.tile == neighbor);

    //            if (existing == null)
    //                open.Add(new PathNode(neighbor, current, g, h));
    //            else if (g < existing.gCost)
    //            {
    //                existing.gCost = g;
    //                existing.parent = current;
    //            }
    //        }
    //    }

    //    Debug.LogWarning("A*: No path found.");
    //}
    private void RunAStar(Tile start, Tile goal)
    {
        List<PathNode> open = new List<PathNode> { new PathNode(start, null, 0f, Heuristic(start, goal)) };
        HashSet<Tile> closed = new HashSet<Tile>();
        visitedPositions.Clear();

        while (open.Count > 0)
        {
            var current = open.OrderBy(n => n.FCost).First();
            open.Remove(current);
            visitedPositions.Add(current.tile.transform.position + Vector3.up * 0.05f);
            closed.Add(current.tile);

            nodesExplored++; //Increment nodesExplored in Algorithms


            if (current.tile == goal)
            {
                ShowPath(ReconstructPath(current));
                DrawVisitedPath();
                return;
            }

            foreach (var neighbor in GetNeighbors(current.tile))
            {
                if (closed.Contains(neighbor) || neighbor.tileType == TileType.Wall) continue;

                float g = current.gCost + GetTileCost(neighbor);
                float h = Heuristic(neighbor, goal);
                var existing = open.FirstOrDefault(n => n.tile == neighbor);

                if (existing == null)
                    open.Add(new PathNode(neighbor, current, g, h));
                else if (g < existing.gCost)
                {
                    existing.gCost = g;
                    existing.parent = current;
                }
            }
        }

        DrawVisitedPath();
        Debug.LogWarning("A*: No path found.");
    }



    private void DrawVisitedPath()
    {
        if (visitedPositions.Count < 2) return;

        GameObject lineObj = new GameObject("VisitedLine");
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.material = lineMaterial;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.positionCount = visitedPositions.Count;
        line.useWorldSpace = true;
        line.numCapVertices = 2;
        line.tag = "PathLine"; // Optional if you want to group/delete it easily

        line.SetPositions(visitedPositions.ToArray());
    }



    private List<Tile> ReconstructPath(PathNode node)
    {
        var path = new List<Tile>();
        while (node != null)
        {
            path.Insert(0, node.tile);
            node = node.parent;
        }
        return path;
    }

    //private void ShowPath(List<Tile> path)
    //{
    //    if (startTile) startTile.GetComponent<Renderer>().material.color = Color.green;
    //    if (goalTile) goalTile.GetComponent<Renderer>().material.color = Color.blue;

    //    for (int i = 0; i < path.Count - 1; i++)
    //    {
    //        var from = path[i];
    //        var to = path[i + 1];
    //        var dir = to.transform.position - from.transform.position;
    //        var rot = Quaternion.LookRotation(dir);
    //        GameObject arrow = Instantiate(arrowPrefab, from.transform.position + Vector3.up * 0.1f, rot);
    //        if (arrowParent) arrow.transform.parent = arrowParent;
    //    }
    //}

    private void ShowPath(List<Tile> path)
    {
        if (startTile) startTile.GetComponent<Renderer>().material.color = Color.green;
        if (goalTile) goalTile.GetComponent<Renderer>().material.color = Color.blue;

        float totalCost = 0f;
        string pathString = "";

        for (int i = 0; i < path.Count; i++)
        {
            Tile tile = path[i];
            pathString += tile.name;

            if (i < path.Count - 1)
                pathString += " -> ";

            if (i > 0) // Skip start tile cost
                totalCost += GetTileCost(tile);

            if (i < path.Count - 1)
            {
                Tile from = tile;
                Tile to = path[i + 1];
                Vector3 dir = to.transform.position - from.transform.position;
                Quaternion rot = Quaternion.LookRotation(dir);
                GameObject arrow = Instantiate(arrowPrefab, from.transform.position + Vector3.up * 0.05f, rot);
                if (arrowParent) arrow.transform.parent = arrowParent;
            }
        }

        Debug.Log($"Path: {pathString}");
        Debug.Log($"Total cost: {totalCost}");

        if (pathOutputText != null)
        {
            pathOutputText.text = $"Path:\n{pathString}\n\n" +
                                  $"Total cost: {totalCost}\n" +
                                  $"Nodes explored: {nodesExplored}\n" +
                                  $"Time taken: {stopwatch.ElapsedMilliseconds} ms";
        }


    }


    private float Heuristic(Tile a, Tile b)
    {
        int dx = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int dy = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
        return heuristic switch
        {
            HeuristicType.Euclidean => Mathf.Sqrt(dx * dx + dy * dy),
            HeuristicType.Diagonal => Mathf.Max(dx, dy),
            _ => dx + dy,
        };
    }

    private float GetTileCost(Tile tile)
    {
        return tile.tileType == TileType.Swamp ? 2f : 1f;
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            Vector2Int pos = tile.gridPosition + dir;
            var obj = GameObject.Find($"Tile_{pos.x}_{pos.y}");
            if (obj && obj.TryGetComponent(out Tile neighbor))
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private class PathNode
    {
        public Tile tile;
        public PathNode parent;
        public float gCost;
        public float hCost;

        public float FCost => gCost + hCost;

        public PathNode(Tile tile, PathNode parent, float g = 0, float h = 0)
        {
            this.tile = tile;
            this.parent = parent;
            gCost = g;
            hCost = h;
        }
    }

    //public void OnResetMap()
    //{
    //    foreach (GameObject tileObj in GameObject.FindGameObjectsWithTag("Tile"))
    //    {
    //        Tile tile = tileObj.GetComponent<Tile>();
    //        if (tile != null)
    //        {
    //            tile.tileType = TileType.Open;
    //            tile.GetComponent<Renderer>().material.color = Color.white;
    //        }
    //    }
    //    ResetArrows();
    //    ResetTileColors();



    //    //foreach (GameObject arrow in GameObject.FindGameObjectsWithTag("PathArrow"))
    //    //    Destroy(arrow);

    //    startTile = null;
    //    goalTile = null;
    //    isPathResolved = false;
    //}
    public void OnResetMap()
    {
        // Reset tile types and colors
        foreach (GameObject tileObj in GameObject.FindGameObjectsWithTag("Tile"))
        {
            Tile tile = tileObj.GetComponent<Tile>();
            if (tile != null)
            {
                tile.tileType = TileType.Open;
                tile.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        // Clear arrows
        foreach (GameObject arrow in GameObject.FindGameObjectsWithTag("PathArrow"))
            Destroy(arrow);

        // Clear visited path line
        GameObject oldLine = GameObject.Find("VisitedLine");
        if (oldLine) Destroy(oldLine);

        // Clear any debug output
        if (pathOutputText != null)
            pathOutputText.text = "";

        // Reset state
        startTile = null;
        goalTile = null;
        isPathResolved = false;
        visitedPositions.Clear();
    }

}





