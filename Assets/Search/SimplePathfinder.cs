using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SimplePathfinder : MonoBehaviour
{
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

        switch (strategy)
        {
            case SearchStrategy.DFS: RunDFS(startTile, goalTile); break;
            case SearchStrategy.BFS: RunBFS(startTile, goalTile); break;
            case SearchStrategy.Dijkstra: RunDijkstra(startTile, goalTile); break;
            case SearchStrategy.AStar: RunAStar(startTile, goalTile); break;
        }
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

    private void RunDFS(Tile start, Tile goal)
    {
        Stack<PathNode> stack = new Stack<PathNode>();
        HashSet<Tile> visited = new HashSet<Tile>();

        stack.Push(new PathNode(start, null));

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (visited.Contains(current.tile)) continue;

            visited.Add(current.tile);
            if (current.tile == goal)
            {
                ShowPath(ReconstructPath(current));
                return;
            }

            foreach (var neighbor in GetNeighbors(current.tile))
            {
                if (!visited.Contains(neighbor) && neighbor.tileType != TileType.Wall)
                    stack.Push(new PathNode(neighbor, current));
            }
        }

        Debug.LogWarning("DFS: No path found.");
    }

    private void RunBFS(Tile start, Tile goal)
    {
        Queue<PathNode> queue = new Queue<PathNode>();
        HashSet<Tile> visited = new HashSet<Tile>();

        queue.Enqueue(new PathNode(start, null));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (visited.Contains(current.tile)) continue;

            visited.Add(current.tile);
            if (current.tile == goal)
            {
                ShowPath(ReconstructPath(current));
                return;
            }

            foreach (var neighbor in GetNeighbors(current.tile))
            {
                if (!visited.Contains(neighbor) && neighbor.tileType != TileType.Wall)
                    queue.Enqueue(new PathNode(neighbor, current));
            }
        }

        Debug.LogWarning("BFS: No path found.");
    }

    private void RunDijkstra(Tile start, Tile goal)
    {
        List<PathNode> open = new List<PathNode> { new PathNode(start, null, 0f) };
        HashSet<Tile> closed = new HashSet<Tile>();

        while (open.Count > 0)
        {
            var current = open.OrderBy(n => n.gCost).First();
            if (current.tile == goal)
            {
                ShowPath(ReconstructPath(current));
                return;
            }

            open.Remove(current);
            closed.Add(current.tile);

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

        Debug.LogWarning("Dijkstra: No path found.");
    }

    private void RunAStar(Tile start, Tile goal)
    {
        List<PathNode> open = new List<PathNode> { new PathNode(start, null, 0f, Heuristic(start, goal)) };
        HashSet<Tile> closed = new HashSet<Tile>();

        while (open.Count > 0)
        {
            var current = open.OrderBy(n => n.FCost).First();
            if (current.tile == goal)
            {
                ShowPath(ReconstructPath(current));
                return;
            }

            open.Remove(current);
            closed.Add(current.tile);

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

        Debug.LogWarning("A*: No path found.");
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
            pathOutputText.text = $"Path:\n{pathString}\n\nTotal cost: {totalCost}";
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

    public void OnResetMap()
    {
        foreach (GameObject tileObj in GameObject.FindGameObjectsWithTag("Tile"))
        {
            Tile tile = tileObj.GetComponent<Tile>();
            if (tile != null)
            {
                tile.tileType = TileType.Open;
                tile.GetComponent<Renderer>().material.color = Color.white;
            }
        }
        ResetArrows();
        ResetTileColors();

        //foreach (GameObject arrow in GameObject.FindGameObjectsWithTag("PathArrow"))
        //    Destroy(arrow);

        startTile = null;
        goalTile = null;
        isPathResolved = false;
    }

}








//public class SimplePathfinder : MonoBehaviour
//{
//    public enum HeuristicType
//    {
//        Manhattan,
//        Euclidean,
//        Diagonal
//    }

//    public enum SearchStrategy
//    {
//        DFS,
//        BFS,
//        Dijkstra,
//        AStar
//    }


//    [HideInInspector] public Tile startTile;
//    [HideInInspector] public Tile goalTile;
//    [HideInInspector] public bool isPathResolved = false;



//    public SearchStrategy strategy = SearchStrategy.AStar;
//    public Dropdown strategyDropdown;
//    public SimplePathfinder pathfinder;

//    public HeuristicType heuristic = HeuristicType.Manhattan;
//    public Color openColor = Color.cyan;
//    public Color closedColor = Color.gray;
//    //public Color pathColor = Color.yellow;

//    public GameObject arrowPrefab; // assign in Inspector
//    public Transform arrowParent;  // optional: to keep scene organized


//    void Start()
//    {
//        strategyDropdown.onValueChanged.AddListener(OnStrategyChange);
//    }

//    void OnStrategyChange(int index)
//    {
//        pathfinder.strategy = (SearchStrategy)index;
//    }

//    public void OnFindPathButton()
//    {
//        if (startTile != null && goalTile != null)
//        {
//            RunPathfinding(startTile, goalTile);
//        }
//        else
//        {
//            Debug.LogWarning("Please select a start and a goal tile.");
//        }
//    }

//    public void OnResetMap()
//    {
//        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Tile"))
//        {
//            Tile tile = obj.GetComponent<Tile>();
//            if (tile == null) continue;

//            tile.tileType = TileType.Open;
//            tile.GetComponent<Renderer>().material.color = Color.white;
//        }

//        foreach (GameObject arrow in GameObject.FindGameObjectsWithTag("PathArrow"))
//            Destroy(arrow);

//        startTile = null;
//        goalTile = null;
//        isPathResolved = false;
//    }



//    public void RunPathfinding(Tile startTile, Tile goalTile)
//    {
//        if (startTile == null || goalTile == null)
//        {
//            Debug.LogWarning("You must select a start and goal tile.");
//            return;
//        }

//        isPathResolved = true; // Prevent further edits after path is shown
//        foreach (GameObject arrow in GameObject.FindGameObjectsWithTag("PathArrow"))
//            Destroy(arrow);

//        ResetTiles();

//        switch (strategy)
//        {
//            case SearchStrategy.DFS:
//                RunDFS(startTile, goalTile);
//                break;
//            case SearchStrategy.BFS:
//                RunBFS(startTile, goalTile);
//                break;
//            case SearchStrategy.Dijkstra:
//                RunDijkstra(startTile, goalTile);
//                break;
//            case SearchStrategy.AStar:
//            default:
//                RunAStar(startTile, goalTile);
//                break;
//        }

//        // Reset Start/Goal After Each Run
//        startTile = null;
//        goalTile = null;

//    }


//    private float Heuristic(Tile a, Tile b)
//    {
//        int dx = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
//        int dy = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
//        switch (heuristic)
//        {
//            case HeuristicType.Euclidean:
//                return Mathf.Sqrt(dx * dx + dy * dy);
//            case HeuristicType.Diagonal:
//                return Mathf.Max(dx, dy);
//            case HeuristicType.Manhattan:
//            default:
//                return dx + dy;
//        }
//    }



//    private float GetTileCost(Tile tile)
//    {
//        return tile.tileType == TileType.Swamp ? 1f : 1f;
//    }

//    private List<Tile> ReconstructPath(PathNode node)
//    {
//        List<Tile> path = new List<Tile>();
//        while (node != null)
//        {
//            path.Insert(0, node.tile);
//            node = node.parent;
//        }
//        return path;
//    }

//    private List<Tile> GetNeighbors(Tile tile)
//    {
//        List<Tile> neighbors = new List<Tile>();
//        Vector2Int[] directions = new Vector2Int[]
//        {
//            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
//        };

//        foreach (Vector2Int dir in directions)
//        {
//            Vector2Int pos = tile.gridPosition + dir;
//            GameObject found = GameObject.Find($"Tile_{pos.x}_{pos.y}");
//            if (found != null && found.GetComponent<Tile>() != null)
//            {
//                neighbors.Add(found.GetComponent<Tile>());
//            }
//        }

//        return neighbors;
//    }

//    // clear previous pathfinding highlights before running A* again

//    private void ResetTiles()
//    {
//        foreach (GameObject tileObj in GameObject.FindGameObjectsWithTag("Tile"))
//        {
//            Tile tile = tileObj.GetComponent<Tile>();
//            if (tile != null)
//            {
//                Renderer rend = tile.GetComponent<Renderer>();
//                switch (tile.tileType)
//                {
//                    case TileType.Swamp:
//                        rend.material.color = Color.yellow;
//                        break;
//                    case TileType.Wall:
//                        rend.material.color = Color.black;
//                        break;
//                    default:
//                        rend.material.color = Color.white;
//                        break;
//                }
//            }
//        }
//    }

//    public void RunDFS(Tile startTile, Tile goalTile)
//    {
//        ResetTiles();
//        Stack<PathNode> stack = new Stack<PathNode>();
//        HashSet<Tile> visited = new HashSet<Tile>();

//        stack.Push(new PathNode { tile = startTile, parent = null });
//        visited.Add(startTile);

//        while (stack.Count > 0)
//        {
//            PathNode current = stack.Pop();

//            if (current.tile == goalTile)
//            {
//                ShowPath(ReconstructPath(current), startTile, goalTile);
//                return;
//            }

//            visited.Add(current.tile);
//            current.tile.GetComponent<Renderer>().material.color = closedColor;

//            foreach (Tile neighbor in GetNeighbors(current.tile))
//            {
//                if (!visited.Contains(neighbor) && neighbor.tileType != TileType.Wall)
//                {
//                    stack.Push(new PathNode
//                    {
//                        tile = neighbor,
//                        parent = current
//                    });
//                    visited.Add(neighbor);
//                    neighbor.GetComponent<Renderer>().material.color = openColor;
//                }
//            }
//        }

//        Debug.LogWarning("DFS: No path found.");
//    }

//    public void RunBFS(Tile startTile, Tile goalTile)
//    {
//        ResetTiles();
//        Queue<PathNode> queue = new Queue<PathNode>();
//        HashSet<Tile> visited = new HashSet<Tile>();

//        queue.Enqueue(new PathNode { tile = startTile, parent = null });
//        visited.Add(startTile);

//        while (queue.Count > 0)
//        {
//            PathNode current = queue.Dequeue();

//            if (current.tile == goalTile)
//            {
//                ShowPath(ReconstructPath(current), startTile, goalTile);
//                return;
//            }

//            visited.Add(current.tile);
//            current.tile.GetComponent<Renderer>().material.color = closedColor;

//            foreach (Tile neighbor in GetNeighbors(current.tile))
//            {
//                if (!visited.Contains(neighbor) && neighbor.tileType != TileType.Wall)
//                {
//                    queue.Enqueue(new PathNode
//                    {
//                        tile = neighbor,
//                        parent = current
//                    });
//                    visited.Add(neighbor);
//                    neighbor.GetComponent<Renderer>().material.color = openColor;
//                }
//            }
//        }

//        Debug.LogWarning("BFS: No path found.");
//    }


//    public void RunDijkstra(Tile startTile, Tile goalTile)
//    {
//        List<PathNode> openSet = new List<PathNode>();
//        HashSet<Tile> closedSet = new HashSet<Tile>();

//        PathNode startNode = new PathNode
//        {
//            tile = startTile,
//            gCost = 0,
//            hCost = 0 // not used
//        };

//        openSet.Add(startNode);

//        while (openSet.Count > 0)
//        {
//            PathNode current = openSet.OrderBy(n => n.gCost).First(); // just gCost
//            if (current.tile == goalTile)
//            {
//                ShowPath(ReconstructPath(current), startTile, goalTile);
//                return;
//            }

//            openSet.Remove(current);
//            closedSet.Add(current.tile);
//            current.tile.GetComponent<Renderer>().material.color = closedColor;

//            foreach (Tile neighbor in GetNeighbors(current.tile))
//            {
//                if (closedSet.Contains(neighbor) || neighbor.tileType == TileType.Wall)
//                    continue;

//                float tentativeG = current.gCost + GetTileCost(neighbor);
//                PathNode existing = openSet.FirstOrDefault(n => n.tile == neighbor);

//                if (existing == null)
//                {
//                    openSet.Add(new PathNode
//                    {
//                        tile = neighbor,
//                        parent = current,
//                        gCost = tentativeG,
//                        hCost = 0 // not used
//                    });
//                    neighbor.GetComponent<Renderer>().material.color = openColor;
//                }
//                else if (tentativeG < existing.gCost)
//                {
//                    existing.gCost = tentativeG;
//                    existing.parent = current;
//                }
//            }
//        }

//        Debug.LogWarning("Dijkstra: No path found.");
//    }


//    public void RunAStar(Tile startTile, Tile goalTile)
//    {
//        List<PathNode> openSet = new List<PathNode>();
//        HashSet<Tile> closedSet = new HashSet<Tile>();

//        PathNode startNode = new PathNode
//        {
//            tile = startTile,
//            gCost = 0,
//            hCost = Heuristic(startTile, goalTile)
//        };

//        openSet.Add(startNode);

//        while (openSet.Count > 0)
//        {
//            PathNode current = openSet.OrderBy(n => n.FCost).First();
//            if (current.tile == goalTile)
//            {
//                ShowPath(ReconstructPath(current), startTile, goalTile);
//                return;
//            }

//            openSet.Remove(current);
//            closedSet.Add(current.tile);
//            current.tile.GetComponent<Renderer>().material.color = closedColor;

//            foreach (Tile neighbor in GetNeighbors(current.tile))
//            {
//                if (closedSet.Contains(neighbor) || neighbor.tileType == TileType.Wall)
//                    continue;

//                float tentativeG = current.gCost + GetTileCost(neighbor);
//                PathNode existing = openSet.FirstOrDefault(n => n.tile == neighbor);

//                if (existing == null)
//                {
//                    openSet.Add(new PathNode
//                    {
//                        tile = neighbor,
//                        parent = current,
//                        gCost = tentativeG,
//                        hCost = Heuristic(neighbor, goalTile)
//                    });
//                    neighbor.GetComponent<Renderer>().material.color = openColor;
//                }
//                else if (tentativeG < existing.gCost)
//                {
//                    existing.gCost = tentativeG;
//                    existing.parent = current;
//                }
//            }
//        }

//        Debug.LogWarning("A*: No path found.");
//    }



//    private void ShowPath(List<Tile> path, Tile startTile, Tile goalTile)
//    {
//        // Color start and goal
//        startTile.GetComponent<Renderer>().material.color = Color.green;
//        goalTile.GetComponent<Renderer>().material.color = Color.blue;

//        // Draw arrows
//        for (int i = 0; i < path.Count - 1; i++)
//        {
//            Tile from = path[i];
//            Tile to = path[i + 1];
//            Vector3 dir = to.transform.position - from.transform.position;
//            Quaternion rot = Quaternion.LookRotation(dir);
//            GameObject arrow = Instantiate(arrowPrefab, from.transform.position + Vector3.up * 0.5f, rot);
//            if (arrowParent) arrow.transform.parent = arrowParent;
//        }
//    }

//}

