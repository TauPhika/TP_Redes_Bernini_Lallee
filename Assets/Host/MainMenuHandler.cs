using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] NetworkRunnerHandler _networkRunner;

    [Header("PANELS"), SerializeField] GameObject _joinLobbyPanel;
    [SerializeField] GameObject _joiningLobbyPanel, _sessionBrowserPanel, _hostSessionPanel;

    [Header("BUTTONS"), SerializeField] Button _joinLobbyButton;
    [SerializeField] Button _hostSessionLobbyButton, _hostSessionButton;

    [Header("INPUT FIELD"), SerializeField] TMP_InputField _inputSessionName;

    [Header("TEXT"), SerializeField] TextMeshProUGUI _statusText;
    [SerializeField] TextMeshProUGUI _creatingText;
    [SerializeField] string _sceneName;

    private void Start()
    {
        _joinLobbyPanel.SetActive(true);
        _joiningLobbyPanel.SetActive(false);
        _sessionBrowserPanel.SetActive(false);
        _hostSessionPanel.SetActive(false);
        _creatingText.gameObject.SetActive(false);

        _joinLobbyButton.onClick.AddListener(JoinLobby);
        _hostSessionLobbyButton.onClick.AddListener(HostSessionLobby);
        _hostSessionButton.onClick.AddListener(HostSession);

        _networkRunner.OnLobbyConnected += () =>
        {
            _joiningLobbyPanel.SetActive(false);
            _sessionBrowserPanel.SetActive(true);
        };
    }

    void JoinLobby() 
    {
        _networkRunner.JoinLobby();

        _joinLobbyPanel.SetActive(false);
        _joiningLobbyPanel.SetActive(true);
        StartCoroutine(JoiningText());
    }

    IEnumerator JoiningText()
    {
        var wait = new WaitForSeconds(0.2f);

        while (_joiningLobbyPanel.activeInHierarchy)
        {
            _statusText.text = "Joining Lobby."; yield return wait;
            _statusText.text = "Joining Lobby.."; yield return wait;
            _statusText.text = "Joining Lobby..."; yield return wait; 
        }
    }

    void HostSessionLobby()
    {
        _sessionBrowserPanel.SetActive(false);
        _hostSessionPanel.SetActive(true);
    }

    void HostSession()
    {
        _creatingText.gameObject.SetActive(true);
        _networkRunner.HostSession(_inputSessionName.text, _sceneName);
    }
}
