using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;


public class SessionBrowser : MonoBehaviour
{
    [SerializeField] NetworkRunnerHandler _networkRunner;

    [SerializeField] TextMeshProUGUI _emptyText;

    [SerializeField] SessionItem _sessionItem;

    [SerializeField] VerticalLayoutGroup _parent;

    private void OnEnable()
    {
        _networkRunner.OnSessionListUpdate += ReceiveSessionList;
    }

    private void OnDisable()
    {
        _networkRunner.OnSessionListUpdate -= ReceiveSessionList;
    }

    void ReceiveSessionList(List<SessionInfo> allSessions)
    {
        ClearItemList();

        if (allSessions.Count <= 0) { NoSessionAvailable(); return; }

        foreach (var session in allSessions)
        {
            AddNewSessionItem(session);
        }
    }

    void ClearItemList()
    {
        foreach(GameObject item in _parent.transform) Destroy(item);

        _emptyText.gameObject.SetActive(false);
    }

    void NoSessionAvailable() { _emptyText.gameObject.SetActive(true); }

    void AddNewSessionItem(SessionInfo session)
    {
        var newItem = Instantiate(_sessionItem, _parent.transform);
        newItem.SetInfo(session, JoinSelectedSession);
    }

    void JoinSelectedSession(SessionInfo session)
    {
        _networkRunner.JoinSession(session);
    }
}
