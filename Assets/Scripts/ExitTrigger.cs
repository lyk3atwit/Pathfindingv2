using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (other.gameObject.name == "Player" || other.gameObject.name == "Agent")
        {
            Debug.Log($"{other.gameObject.name} reached exit.");

            if (currentScene == "MainScene")
            {
                SceneManager.LoadScene("Level2"); // Go to Level2 if still in MainScene
            }
            else
            {
                other.transform.position = new Vector3(0, 1, 0); // Reset position in Level2 or beyond
                Debug.Log($"{other.gameObject.name} reset to start position.");
            }
        }
    }
}

