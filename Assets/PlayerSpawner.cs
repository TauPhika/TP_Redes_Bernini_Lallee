using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;

    public GameObject[] spawningPoints;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Instantiate(player, spawningPoints[Random.Range(0, spawningPoints.Length)].transform);
        }
    }
}
