using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public PlayerView view;
    
    [Header("HEALTH")]
    public int maxHealth;
    int _health;

    [Header("MOVEMENT")]
    public int speed;
    public int jumpHeight;
    public Rigidbody2D playerRB;
    [ReadOnly] public bool isAirborne;

    [Header("JETPACK")]
    public int jetpackPower;
    public int jetpackDuration;
    public float jetpackCooldown;
    public float jetpackCooldownOnGround;
    [ReadOnly] public bool isRechargingJetpack;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6) isAirborne = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        _health = maxHealth;
        view = GetComponent<PlayerView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetHealth(int healthChange = default) 
    {
        if (healthChange != default) 
        {
            _health += healthChange;
            view.UpdateHealthBar();
            print($"Player health: {_health}");
        } 
        return _health; 
    }
}
