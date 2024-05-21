using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static PlayerSpawner instance;
    
    public PlayerModel player;
    public NetworkRunner runner;
    PlayerController _controller;
    public GameObject waitingCanvas;

    public GameObject[] spawningPoints;
    [ReadOnly] public List<PlayerModel> allPlayers = new();

    private void OnEnable()
    {
        if (runner) runner.AddCallbacks(this);
    }

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

            var localPlayer = Instantiate(player,
                                           spawningPoints[Random.Range(0, spawningPoints.Length)].transform.position,
                                           Quaternion.identity);

            _controller = localPlayer.GetComponent<PlayerController>();

            allPlayers.Add(localPlayer);
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        print("upsn't");

        if (runner.Topology == SimulationConfig.Topologies.Shared)
        {

        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!PlayerModel.local || !_controller) return; 

        input.Set(_controller.GetLocalInputs());
    }

    #region UNUSED
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //print("b");
        throw new System.NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //print("a");
        throw new System.NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new System.NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        //print("d");
        throw new System.NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new System.NotImplementedException();
    }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        //print("c");
        throw new System.NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new System.NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data)
    {
        throw new System.NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new System.NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new System.NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new System.NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        print("ups");
        throw new System.NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new System.NotImplementedException();
    }
    #endregion

}
