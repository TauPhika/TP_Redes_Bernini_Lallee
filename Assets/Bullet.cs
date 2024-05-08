using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public PlayerWeapon thisPlayer;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Escenario")
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Player" && thisPlayer.CanHurtItself(other, thisPlayer.canHurtItself))
        {
            other.gameObject.GetComponent<PlayerModel>().GetHealth(-1);
            Destroy(gameObject);
        }
       
    }

    public GameObject SetPlayer(PlayerWeapon player) 
    { 
        if (thisPlayer == null) thisPlayer = player;
        return this.gameObject;
    }

}

