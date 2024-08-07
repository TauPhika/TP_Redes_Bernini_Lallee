using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    PlayerModel p;

    private void Awake()
    {
        _waitingForPlayers = true;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.Topology == SimulationConfig.Topologies.Shared && runner.ActivePlayers.Count() < 3)
        {
            
            p = runner.Spawn(prefab: _playerPrefab,
                                 position: new Vector3(-6, -2, 0)  /*_spawningPoints[UnityEngine.Random.Range(0, _spawningPoints.Length)].transform.position*/,
                                 rotation: Quaternion.identity,
                                 inputAuthority: player);

            if (!p && PlayerModel.local) p = PlayerModel.local;

            if (!p) print("no apareci");
            else print(p.gameObject.transform.position);
            //PlayerModel.local = p;

            if(runner.ActivePlayers.Count() == 2) _waitingForPlayers = false;
        }
        else
        {
            print("Sesion Llena");
        }

        if (!p.myWaitingCanvas) p.myWaitingCanvas = Instantiate(waitingCanvas);
        p.myWaitingText = p.myWaitingCanvas.GetComponentInChildren<TextMeshProUGUI>();
        p.myWaitingText.text = "Successfully connected. \n Waiting for another player...";

        //var blocker = GameObject.Find("ScreenBlocker");
        //if (blocker) {print("blocker"); Destroy(blocker); }


        if (runner.ActivePlayers.Count() == 2 && !_waitingForPlayers)
        {
            p.myWaitingCanvas.SetActive(false);

            PlayerModel.local.controller._netInputs.waiting = false;
            p.controller._netInputs.waiting = false;

            _waitingForPlayers = false;
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!PlayerModel.local || _waitingForPlayers) return;

        if (!_localInputs) _localInputs = p.GetComponent<PlayerController>();
        input.Set(_localInputs.GetLocalInputs());
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
        print("A new player is trying to join.");

        if(runner.IsServer) p.myWaitingCanvas.SetActive(false);
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        PlayerModel.local.Disconnect();
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

        print("The other player disconnected, going back to the main menu");
        //_waitingForPlayers = true;
        //runner.Despawn(p.Object);
        //Destroy(p.gameObject);
        ////foreach (var p in p.allPlayers)
        ////{
        ////    p.Disconnect();
        ////}

        //SceneManager.LoadScene("MainMenu");
        //SessionBrowser.instance.ClearItemList(runner.SessionInfo);
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
