using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSceneSpawner : MonoBehaviour
{
    public GameObject groundPrefab;
    public GameObject agentPrefab;
    public GameObject waypointPrefab;
    public GameObject obstaclePrefab;

    public int numberOfWaypoints = 4;
    public int numberOfObstacles = 6;
    public Vector2 areaSize = new Vector2(20f, 20f);

    void Start()
    {
        // Spawn Ground
        GameObject ground = Instantiate(groundPrefab, Vector3.zero, Quaternion.identity);

        // Spawn Waypoints
        Transform[] waypoints = new Transform[numberOfWaypoints];
        for (int i = 0; i < numberOfWaypoints; i++)
        {
            Vector3 pos = RandomNavmeshPosition();
            GameObject wp = Instantiate(waypointPrefab, pos, Quaternion.identity);
            wp.name = $"Waypoint_{i}";
            waypoints[i] = wp.transform;
        }

        // Spawn Obstacles
        for (int i = 0; i < numberOfObstacles; i++)
        {
            Vector3 pos = RandomNavmeshPosition();
            Instantiate(obstaclePrefab, pos, Quaternion.identity);
        }

        // Spawn Agent
        Vector3 agentPos = RandomNavmeshPosition();
        GameObject agent = Instantiate(agentPrefab, agentPos, Quaternion.identity);

        // Assign Waypoints
        NavMeshPatrolAgent patrol = agent.GetComponent<NavMeshPatrolAgent>();
        patrol.waypoints = waypoints;
    }

    Vector3 RandomNavmeshPosition()
    {
        float x = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float z = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
        return new Vector3(x, 0, z);
    }
}
