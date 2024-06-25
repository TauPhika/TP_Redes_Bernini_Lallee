using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using TMPro;
using System.Linq;


public class NetworkPlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] PlayerModel _playerPrefab;
    PlayerController _localInputs;
    GameObject[] _spawningPoints;
    public GameObject waitingCanvas;
    [HideInInspector] public TextMeshProUGUI waitingText;
    bool _waitingForPlayers = true;

    private void Awake()
    {
        _waitingForPlayers = true;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer || runner.Topology == SimulationConfig.Topologies.ClientServer)
        {
            var p = runner.Spawn(prefab: _playerPrefab,
                                 position: _spawningPoints[UnityEngine.Random.Range(0, _spawningPoints.Length)].transform.position,
                                 rotation: Quaternion.identity,
                                 inputAuthority: player);

            if (!p) print("no apareci");

            var blocker = GameObject.Find("ScreenBlocker");

            //if (!p.myWaitingCanvas) p.myWaitingCanvas = Instantiate(waitingCanvas);
            //p.myWaitingText = p.myWaitingCanvas.GetComponentInChildren<TextMeshProUGUI>();
            //p.myWaitingText.text = "Successfully connected. \n Waiting for another player...";

            if (blocker) Destroy(blocker);

            if (runner.ActivePlayers.Count() == 2)
            {
                //p.myWaitingCanvas.SetActive(false);
                _waitingForPlayers = false;
            }

        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!PlayerModel.local || _waitingForPlayers) return;

        if (!_localInputs) _localInputs = PlayerModel.local.GetComponent<PlayerController>();
        else input.Set(_localInputs.GetLocalInputs());
    }

    #region CALLBACKS
    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

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

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        print("y se marcho");
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

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }
    #endregion
}
