using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
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

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetHealth() { return _health; }
}
