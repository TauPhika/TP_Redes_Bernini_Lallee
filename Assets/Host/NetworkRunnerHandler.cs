using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;

public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkRunner _networkPrefab;
    [HideInInspector] public NetworkRunner runner;

    public event Action OnLobbyConnected = delegate { };
    public event Action<List<SessionInfo>> OnSessionListUpdate = delegate { };

    public static NetworkRunnerHandler instance;

    void Awake() { instance = this; }

    #region LOBBY
    public void JoinLobby()
    {
        
        if (runner) Destroy(runner);
        runner = Instantiate(_networkPrefab);

        //instance = this;

        runner.AddCallbacks(this);

        var clientTask = JoinLobbyTask();
    }

    async Task JoinLobbyTask()
    {
        var result = await runner.JoinSessionLobby(SessionLobby.Custom, "Main Lobby");

        if (result.Ok) OnLobbyConnected(); else print("Malio sal");
    }

    #endregion

    #region SESSION MANAGEMENT

    public void HostSession(string sessionName, string sceneName)
    {
        var clientTask = InitializeGame(gameMode: GameMode.Shared,
                                        gameName: sessionName,
                                        sceneToLoad : SceneUtility.GetBuildIndexByScenePath($"Scenes/{sceneName}"));
    }
    
    public void JoinSession(SessionInfo session)
    {
        print("Uniendose");
        var clientTask = InitializeGame(gameMode : GameMode.Shared, 
                                        gameName : session.Name);
    }

    async Task InitializeGame(GameMode gameMode, string gameName = "New session", SceneRef? sceneToLoad = null)
    {
        var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();

        runner.ProvideInput = true;
        
        var gameArgs = new StartGameArgs()
        {
            GameMode = gameMode,
            SessionName = gameName,
            Scene = sceneToLoad,
            CustomLobbyName = "Main Lobby",
            SceneManager = sceneManager,
            PlayerCount = 2
        };

        if(gameName != "New session" && sceneToLoad != null) print("Initializing...");

        var result = await runner.StartGame(gameArgs);

        if (result.Ok)
        {
            if (gameArgs.GameMode == GameMode.Shared) print($"Created new {gameName} in {gameArgs.CustomLobbyName} successfully.");
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
        print("Successfully connected to the game.");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        print("Failed to join the game.");
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

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
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
    #endregion
}
