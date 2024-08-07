using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using System.Linq;


public class SessionBrowser : MonoBehaviour
{
    public static SessionBrowser instance;
    [SerializeField] NetworkRunnerHandler _networkRunner;

    [SerializeField] TextMeshProUGUI _emptyText, _joiningText;

    [SerializeField] SessionItem _sessionItem;

    [SerializeField] VerticalLayoutGroup _parent;

    private void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        _joiningText.gameObject.SetActive(false);
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

    public void ClearItemList(SessionInfo info = default)
    {
        var itemList = _parent.transform.GetComponentsInChildren<SessionItem>().ToList();
        
        if (info != default) 
        {
            foreach (var item in itemList)
            {
                if(item.thisSession == info) Destroy(item);
            }

            if(itemList.Count <= 0) _emptyText.gameObject.SetActive(true);
        }
        else
        {
            foreach (var item in itemList) Destroy(item);

            _emptyText.gameObject.SetActive(false);
        }

    }

    void NoSessionAvailable() { _emptyText.gameObject.SetActive(true); }

    void AddNewSessionItem(SessionInfo session)
    {
        var newItem = Instantiate(_sessionItem, _parent.transform);
        newItem.SetInfo(session, JoinSelectedSession, NetworkRunnerHandler.instance.runner.ActivePlayers.Count() - 2 >= session.MaxPlayers);
    }

    void JoinSelectedSession(SessionInfo session)
    {
        _joiningText.gameObject.SetActive(true);
        _networkRunner.JoinSession(session);
    }
}
