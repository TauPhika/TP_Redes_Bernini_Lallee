using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class SessionItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _sessionName, _playerAmount;
    [SerializeField] Button _joinSession;
    
    public void SetInfo(SessionInfo sessionInfo, Action<SessionInfo> onClick)
    {
        _sessionName.text = sessionInfo.Name;

        if (sessionInfo.PlayerCount < sessionInfo.MaxPlayers)
        {
            _playerAmount.text = $"{sessionInfo.PlayerCount} / {sessionInfo.MaxPlayers}";
            _joinSession.enabled = true;
        }
        else 
        {
            _playerAmount.text = "FULL";
            _joinSession.enabled = false;
        }

        _joinSession.onClick.AddListener(() => onClick(sessionInfo));

    }
}
