using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using System;

public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkRunner _networkPrefab;
    NetworkRunner _currentNetwork;

    public event Action OnLobbyConnected = delegate { };
    public event Action<List<SessionInfo>> OnSessionListUpdate = delegate { };

    #region LOBBY
    public void JoinLobby()
    {
        if (_currentNetwork) Destroy(_currentNetwork);
        _currentNetwork = Instantiate(_networkPrefab);

        _currentNetwork.AddCallbacks(this);

        var clientTask = JoinLobbyTask();
    }

    async Task JoinLobbyTask()
    {
        var result = await _currentNetwork.JoinSessionLobby(SessionLobby.Custom, "Main Lobby");

        if (result.Ok) OnLobbyConnected(); else print("Malio sal");
    }

    #endregion

    #region SESSION MANAGEMENT

    public void HostSession(string sessionName, string sceneName)
    {
        var clientTask = InitializeGame(gameMode: GameMode.Host,
                                        gameName: sessionName,
                                        sceneToLoad : SceneUtility.GetBuildIndexByScenePath($"Scenes/{sceneName}"));
    }
    
    public void JoinSession(SessionInfo session)
    {
        var clientTask = InitializeGame(gameMode : GameMode.Client, 
                                        gameName : session.Name);
    }

    async Task InitializeGame(GameMode gameMode, string gameName = "New session", SceneRef? sceneToLoad = null)
    {
        var sceneManager = _currentNetwork.GetComponent<NetworkSceneManagerDefault>();

        _currentNetwork.ProvideInput = true;
        
        var gameArgs = new StartGameArgs()
        {
            GameMode = gameMode,
            SessionName = gameName,
            Scene = sceneToLoad,
            CustomLobbyName = "Main Lobby",
            SceneManager = sceneManager,
            PlayerCount = 4
        };

        if(gameName != "New session" && sceneToLoad != null) print("Initializing...");

        var result = await _currentNetwork.StartGame(gameArgs);

        if (result.Ok)
        {
            if (gameArgs.GameMode == GameMode.Host) print($"Created new {gameName} in {gameArgs.CustomLobbyName} successfully.");
            else print($"Joined {gameName} in {gameArgs.CustomLobbyName} successfully.");
        }
        else print("nop");
    }


    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        OnSessionListUpdate(sessionList);
    }

    #endregion

    #region CALLBACKS
    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        print("shutting down");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }
    #endregion
}
