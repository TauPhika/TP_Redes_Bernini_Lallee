using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using TMPro;
using System.Linq;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static PlayerSpawner instance;
    
    public PlayerModel player;
    public NetworkRunner runner;
    PlayerController _controller;
    public NetworkObject waitingCanvas;
    public TextMeshProUGUI waitingText;

    public GameObject[] spawningPoints;
    [Networked] public /*static*/ List<PlayerModel> allPlayers { get; set; } = new();

    private void Awake()
    {
        if(!instance) instance = this;
        if(!waitingCanvas.gameObject.activeInHierarchy) waitingCanvas = Instantiate(waitingCanvas);
        waitingText = waitingCanvas.GetComponentInChildren<TextMeshProUGUI>();
        waitingText.text = "Connecting to the servers...";

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

        if(runner.Topology == SimulationConfig.Topologies.Shared && allPlayers.Count < 3)
        {
            var localPlayer = runner.Spawn(player,
                                           spawningPoints[Random.Range(0, spawningPoints.Length)].transform.position,
                                           Quaternion.identity,
                                           runner.LocalPlayer);             

            _controller = localPlayer.GetComponent<PlayerController>();

            allPlayers.Add(localPlayer);
            allPlayers.Add(localPlayer);

            print($"Player {allPlayers.Count} has connected.");

            localPlayer.myWaitingText.text = "Successfully connected. Waiting for another player...";

            if (allPlayers.Count == 2)
            {
                foreach (var player in allPlayers)
                {
                    Destroy(player.myWaitingCanvas);
                    player.controller._netInputs.waiting = false;
                }
            }
        }
        else if(allPlayers.Count >= 3)
        {
            waitingText.text = "Connection failed. The lobby is already full.";
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
