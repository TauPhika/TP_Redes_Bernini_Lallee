using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    
    public GameObject player;

    public GameObject[] spawningPoints;
    public List<GameObject> allPlayers = new();

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            var p = Instantiate(player, spawningPoints[Random.Range(0, spawningPoints.Length)].transform);
            allPlayers.Add(p);
        }
    }
}
