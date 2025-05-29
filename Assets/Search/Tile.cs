using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public enum TileType { Open, Swamp, Wall }

public class Tile : MonoBehaviour
{
    public TileType tileType;
    public Vector2Int gridPosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    HandleClick();
                }
            }
        }
    }

    void HandleClick()
    {
        SimplePathfinder pathfinder = FindObjectOfType<SimplePathfinder>();
        if (pathfinder == null) return;

        // Allow reassignment of start and goal at any time
        if (Input.GetKey(KeyCode.S))
        {
            if (pathfinder.startTile != null && pathfinder.startTile != this)
                pathfinder.startTile.GetComponent<Renderer>().material.color = GetBaseColor(pathfinder.startTile);

            pathfinder.startTile = this;
            GetComponent<Renderer>().material.color = Color.green;
            return;
        }

        if (Input.GetKey(KeyCode.G))
        {
            if (pathfinder.goalTile != null && pathfinder.goalTile != this)
                pathfinder.goalTile.GetComponent<Renderer>().material.color = GetBaseColor(pathfinder.goalTile);

            pathfinder.goalTile = this;
            GetComponent<Renderer>().material.color = Color.blue;
            return;
        }

        // Terrain editing
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) // Ctrl-click = reset to Open
        {
            tileType = TileType.Open;
            GetComponent<Renderer>().material.color = Color.white;
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) // Shift + left click = Swamp
        {
            tileType = TileType.Swamp;
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else // Default left-click = Wall
        {
            tileType = TileType.Wall;
            GetComponent<Renderer>().material.color = Color.black;
        }
    }

    private Color GetBaseColor(Tile tile)
    {
        return tile.tileType switch
        {
            TileType.Swamp => Color.yellow,
            TileType.Wall => Color.black,
            _ => Color.white,
        };
    }
}



//public class Tile : MonoBehaviour
//{
//    public TileType tileType;
//    public Vector2Int gridPosition;



//    //void OnMouseDown()
//    //{
//    //    //Update the logic to ignore clicks on Tile_0_0 or Tile_4_4:

//    //    if (gameObject.name == "Tile_0_0" || gameObject.name == "Tile_4_4")
//    //        return;

//    //    //if (Input.GetMouseButton(1)) // Right-click to clear
//    //    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) // Ctrl-click to clear
//    //    {
//    //        tileType = TileType.Open;
//    //        GetComponent<Renderer>().material.color = Color.white;
//    //    }
//    //    else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) // Shift = swamp
//    //    {
//    //        tileType = TileType.Swamp;
//    //        GetComponent<Renderer>().material.color = Color.green;
//    //    }
//    //    else // Default = wall
//    //    {
//    //        tileType = TileType.Wall;
//    //        GetComponent<Renderer>().material.color = Color.black;
//    //    }

//    //    // If you want the path to automatically recalculate after a click:
//    //    // Rerun pathfinding (optional) 
//    //    SimplePathfinder pathfinder = FindObjectOfType<SimplePathfinder>();
//    //    Tile start = GameObject.Find("Tile_0_0")?.GetComponent<Tile>();
//    //    Tile goal = GameObject.Find("Tile_4_4")?.GetComponent<Tile>();
//    //    pathfinder.RunPathfinding(start, goal);
//    //}



//    private void OnMouseDown()
//    {
//        SimplePathfinder pathfinder = FindObjectOfType<SimplePathfinder>();
//        if (pathfinder == null) return;

//        // Skip if already running a search
//        if (pathfinder.isPathResolved) return;

//        // Select start and goal tiles after terrain is ready
//        if (pathfinder.startTile == null)
//        {
//            pathfinder.startTile = this;
//            GetComponent<Renderer>().material.color = Color.green;
//            return;
//        }
//        else if (pathfinder.goalTile == null && this != pathfinder.startTile)
//        {
//            pathfinder.goalTile = this;
//            GetComponent<Renderer>().material.color = Color.blue;
//            return;
//        }

//        // Terrain editing
//        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) // Ctrl-click to clear
//        {
//            tileType = TileType.Open;
//            GetComponent<Renderer>().material.color = Color.white;
//        }
//        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) // Shift = swamp
//        {
//            tileType = TileType.Swamp;
//            GetComponent<Renderer>().material.color = Color.yellow;
//        }
//        else // Default left-click = wall
//        {
//            tileType = TileType.Wall;
//            GetComponent<Renderer>().material.color = Color.black;
//        }
//    }








