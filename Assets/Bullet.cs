using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    PlayerWeapon _thisPlayer = null;
    [Range(2, 10)] public int damage;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Escenario")
        {
            Runner.Despawn(this.Object);
        }
        else if (other.gameObject.tag == "Player" && _thisPlayer.CanHurtItself(other, _thisPlayer.canHurtItself))
        {
            var otherModel = other.gameObject.GetComponent<PlayerModel>();
            var newHealth = otherModel.GetHealth(-(damage / 2));

            _thisPlayer._model.view.UpdateHealthBar(otherModel);

            otherModel.healthText.text = (newHealth - damage).ToString();
            print($"Other health: {newHealth}");

            StartCoroutine(otherModel.DamageFeedback());

            if (newHealth <= 0) { _thisPlayer._model.GameOver(true); Runner.Shutdown();}
            Runner.Despawn(this.Object);
        }
       
    }

    public GameObject SetPlayer(PlayerWeapon player) 
    { 
        if (_thisPlayer == null) _thisPlayer = player;
        return this.gameObject;
    }

}

