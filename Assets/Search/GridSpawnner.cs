using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class GridSpawner : MonoBehaviour
{
    public GameObject tilePrefab; 
    public int width = 5;
    public int height = 5;
    public float spacing = 1.1f;

    public InputField widthInput;
    public InputField heightInput;

    private GameObject[,] tiles;

    public void GenerateGrid()
    {
        // Clear previous grid
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        int.TryParse(widthInput.text, out width);
        int.TryParse(heightInput.text, out height);

        tiles = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * spacing, 0, y * spacing);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                tile.name = $"Tile_{x}_{y}";

                Tile tileScript = tile.GetComponent<Tile>();
                tileScript.gridPosition = new Vector2Int(x, y);
                tileScript.tileType = TileType.Open;

                tile.GetComponent<Renderer>().material.color = Color.white;
                tile.tag = "Tile";

                tiles[x, y] = tile;
            }
        }
    }
}


//public class GridSpawner : MonoBehaviour
//{
//    public GameObject tilePrefab;
//    public int gridWidth = 5;
//    public int gridHeight = 5;
//    public float tileSpacing = 1.1f;

//    private TileType[,] layout = new TileType[5, 5]
//    {
//        { TileType.Open, TileType.Open, TileType.Open, TileType.Open, TileType.Open },
//        { TileType.Wall, TileType.Swamp, TileType.Open, TileType.Swamp, TileType.Open },
//        { TileType.Open, TileType.Wall,  TileType.Wall, TileType.Open,  TileType.Open },
//        { TileType.Open, TileType.Open,  TileType.Open, TileType.Open,  TileType.Open },
//        { TileType.Open, TileType.Open,  TileType.Open, TileType.Open,  TileType.Open }
//    };

//    void Start()
//    {
//        for (int y = 0; y < gridHeight; y++)
//        {
//            for (int x = 0; x < gridWidth; x++)
//            {
//                Vector3 pos = new Vector3(x * tileSpacing, 0, y * tileSpacing);
//                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);

//                Tile tileScript = tile.GetComponent<Tile>();
//                tileScript.gridPosition = new Vector2Int(x, y);
//                tileScript.tileType = layout[y, x];

//                Renderer rend = tile.GetComponent<Renderer>();
//                switch (tileScript.tileType)
//                {
//                    case TileType.Swamp: rend.material.color = Color.yellow; break;
//                    case TileType.Wall: rend.material.color = Color.black; break;
//                    default: rend.material.color = Color.white; break;
//                }

//                tile.name = $"Tile_{x}_{y}";
//            }
//        }

//        // ✅ Trigger pathfinding after all tiles are created
//        SimplePathfinder pathfinder = FindObjectOfType<SimplePathfinder>();
//        if (pathfinder != null)
//        {
//            Tile start = GameObject.Find("Tile_0_0")?.GetComponent<Tile>();
//            Tile goal = GameObject.Find("Tile_4_4")?.GetComponent<Tile>();
//            pathfinder.RunPathfinding(start, goal);
//        }
//    }


//}
