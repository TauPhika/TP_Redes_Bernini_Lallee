using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerHost : NetworkBehaviour
{
    //public static PlayerHost Local;
    //public event Action OnPlayerDespawn;

    //public override void Spawned()
    //{
    //    if (Object.HasInputAuthority) 
    //    {
    //        Local = this;
    //        GetComponent<SpriteRenderer>().material.color = Color.red;
    //    }
    //    else
    //    {
    //        GetComponent<SpriteRenderer>().material.color = Color.cyan;
    //    }
    //}

    //public override void Despawned(NetworkRunner runner, bool hasState)
    //{
    //    OnPlayerDespawn();
    //}
}
