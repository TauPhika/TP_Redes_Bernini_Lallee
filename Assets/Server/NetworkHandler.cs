using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkRunner),typeof(NetworkSceneManagerDefault))]
public class NetworkHandler : MonoBehaviour
{
    [SerializeField] private NetworkRunner _networkRunner;   

    void Start()
    {
        if(_networkRunner == null)  _networkRunner = GetComponent<NetworkRunner>();

        var clientTask = InitializeGame(GameMode.Shared, SceneManager.GetActiveScene().buildIndex);
    }

    Task InitializeGame(GameMode gameMode, SceneRef sceneToLoad)
    {
        var sceneManager = GetComponent<NetworkSceneManagerDefault>();

        _networkRunner.ProvideInput = true;

        return _networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = gameMode,
            Scene = sceneToLoad,    
            SessionName = "TP1 Redes Bernini Lallee",
            SceneManager = sceneManager
        });
    }
}
