using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileEditorUI : MonoBehaviour
{
    public GameObject popupPanel;
    public InputField inputField;
    public Tile currentTile;

    public void OpenPopup(Tile tile)
    {
        currentTile = tile;
        popupPanel.SetActive(true);
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }

    public void OnConfirm()
    {
        string entry = inputField.text.ToUpper();
        Renderer rend = currentTile.GetComponent<Renderer>();

        switch (entry)
        {
            case "S":
                currentTile.tileType = TileType.Open;
                FindObjectOfType<SimplePathfinder>().startTile = currentTile;
                rend.material.color = Color.green;
                break;
            case "G":
                currentTile.tileType = TileType.Open;
                FindObjectOfType<SimplePathfinder>().goalTile = currentTile;
                rend.material.color = Color.blue;
                break;
            case "W":
                currentTile.tileType = TileType.Wall;
                rend.material.color = Color.black;
                break;
            case "T":
                currentTile.tileType = TileType.Swamp;
                rend.material.color = Color.yellow;
                break;
            case "O":
                currentTile.tileType = TileType.Open;
                rend.material.color = Color.white;
                break;
            default:
                Debug.Log("Invalid input");
                break;
        }

        popupPanel.SetActive(false);
        currentTile = null;
    }
}
