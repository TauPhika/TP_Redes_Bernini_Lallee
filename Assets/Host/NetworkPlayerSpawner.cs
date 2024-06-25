using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using TMPro;


public class NetworkPlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] PlayerModel _playerPrefab;
    PlayerController _localInputs;
    GameObject[] _spawningPoints;
    public GameObject waitingCanvas;
    [HideInInspector] public TextMeshProUGUI waitingText;


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {              
        if (runner.IsServer)
        {
            var p = runner.Spawn(prefab : _playerPrefab, 
                                 position : _spawningPoints[UnityEngine.Random.Range(0, _spawningPoints.Length)].transform.position,
                                 rotation : Quaternion.identity,
                                 inputAuthority : player);

            var blocker = GameObject.Find("ScreenBlocker");           

            if (!p.myWaitingCanvas) p.myWaitingCanvas = Instantiate(waitingCanvas);
            p.myWaitingText = p.myWaitingCanvas.GetComponentInChildren<TextMeshProUGUI>();
            p.myWaitingText.text = "Successfully connected. \n Waiting for another player...";

            if (blocker) Destroy(blocker);

        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!PlayerModel.local) return;

        if (!_localInputs) _localInputs = PlayerModel.local.GetComponent<PlayerController>();
        else input.Set(_localInputs.GetLocalInputs());
    }

    #region CALLBACKS
    public void OnConnectedToServer(NetworkRunner runner)
    {
        print("Successfully joined the game.");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        print("Failed to join the game.");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        print("A new player is trying to join.");
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

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        _spawningPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        print("shutting down");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }
    #endregion
}
