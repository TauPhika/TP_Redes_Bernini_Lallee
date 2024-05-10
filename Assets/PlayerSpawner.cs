using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    
    public GameObject player;
    public GameObject waitingCanvas;

    public GameObject[] spawningPoints;
    [ReadOnly] public List<GameObject> allPlayers = new();

    private void Awake()
    {
        instance = this;
        waitingCanvas = Instantiate(waitingCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && allPlayers.Count < 4)
        {
            waitingCanvas.SetActive(false);
            var p = Instantiate(player, spawningPoints[Random.Range(0, spawningPoints.Length)].transform);
            allPlayers.Add(p);
        }
    }
}
