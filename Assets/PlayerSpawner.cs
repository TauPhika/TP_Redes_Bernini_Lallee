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
            
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        print("se conecto");

        if(runner.Topology == SimulationConfig.Topologies.Shared)
        {
            waitingCanvas.SetActive(false);

            var localPlayer = runner.Spawn(player,
                                           spawningPoints[Random.Range(0, spawningPoints.Length)].transform.position,
                                           Quaternion.identity,
                                           runner.LocalPlayer);

            _controller = localPlayer.GetComponent<PlayerController>();

            allPlayers.Add(localPlayer);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        print("input");

        if (!PlayerModel.local || !_controller) return; 

        input.Set(_controller.GetLocalInputs());
    }

    #region UNUSED
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }
    #endregion

}
