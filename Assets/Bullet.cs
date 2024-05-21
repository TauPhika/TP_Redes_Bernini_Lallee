using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    PlayerWeapon _thisPlayer;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Escenario")
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Player" && _thisPlayer.CanHurtItself(other, _thisPlayer.canHurtItself))
        {
            other.gameObject.GetComponent<PlayerModel>().GetHealth(-1);
            Destroy(gameObject);
        }
       
    }

    public GameObject SetPlayer(PlayerWeapon player) 
    { 
        if (_thisPlayer == null) _thisPlayer = player;
        return this.gameObject;
    }

}

