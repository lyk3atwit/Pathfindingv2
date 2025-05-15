using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // This script allows the player to move using keyboard input (WASD or Arrow Keys)
    // Attach it to a Player GameObject (e.g., a Capsule or Cube)

    // Speed at which the player moves
    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get horizontal input (A/D or Left/Right keys)
        float h = Input.GetAxis("Horizontal");

        // Get vertical input (W/S or Up/Down keys)
        float v = Input.GetAxis("Vertical");

        // Create a movement vector based on input
        // Y is 0 because we don't want vertical movement (in the air)
        Vector3 movement = new Vector3(h, 0, v) * speed * Time.deltaTime;

        // Move the player in world space (not relative to its own rotation)
        transform.Translate(movement, Space.World);
    }
}
