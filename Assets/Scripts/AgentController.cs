using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    public Transform exit;
    public float speed = 5f;

    void Update()
    {
        Vector3 dir = (exit.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

}
