using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;


public class PlayerModel : NetworkBehaviour
{
    #region VARIABLES
    public static PlayerModel local { get; private set; }
    
    public PlayerView view;
    public PlayerController controller;
    public PlayerWeapon weapon;
    public NetworkRunner runner;
        
    [Header("HEALTH")]
    public int maxHealth;
    //[Networked(OnChanged = nameof(OnLifeUpdate))]
    int _health { get; set; }
    [ReadOnly] public bool _dying = false;

    [Header("MOVEMENT")]
    public int speed;
    public int jumpHeight;
    public int dashForce;
    public bool limitDashing;
    public float dashCooldown;
    public NetworkRigidbody2D playerRB;
    [ReadOnly] public bool isAirborne;
    [ReadOnly] public bool hasDashed;

    [Header("JETPACK")]
    public int jetpackPower;
    public int jetpackDuration;
    public float jetpackCooldown;
    public float jetpackCooldownOnGround;
    [ReadOnly] public bool isRechargingJetpack;

    NetworkInputData _netInputs;

    Ray ray;
    RaycastHit hit;

    public event Action<float> OnLifeUpdate = delegate { };
    public event Action OnPlayerDespawn = delegate { };
    #endregion

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (/*Physics.Raycast(ray, 0.5f) &&*/ collision.gameObject.layer == 6) isAirborne = false;
    }

    private void Awake()
    {
        local = this;
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority) local = this;

        _dying = false;

        if (!limitDashing)
        {
            dashCooldown = 0;
            Destroy(view.dashAsset);
        }

        _health = maxHealth;
        view = GetComponent<PlayerView>();
        controller = GetComponent<PlayerController>();

        ray = new Ray(transform.position, Vector3.down);
    }

    public override void FixedUpdateNetwork()
    {
        if (!_dying && GetInput(out _netInputs))
        {
            controller.Move();

            if (_netInputs.isJumpPressed && !isAirborne) controller.Jump(jumpHeight);

            var rot = controller.GetAimingRotation();

            if(_netInputs.rotation != Quaternion.identity) transform.rotation = rot;

            if (_netInputs.isFirePressed && !weapon._isFiring) weapon.Fire();

            if(_netInputs.isDashPressed) controller.CheckForDash(dashForce);
        }

        //print($"{_netInputs.movementX} | {_netInputs.movementY} | {_netInputs.rotation}");
    }

    public float GetHealth(int healthChange = default) => RPC_GetHealth(healthChange);

    // Modifica la salud en base al valor recibido, da el feedback correspondiente y devuelve el resultado final.
    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public float RPC_GetHealth(int healthChange = default) 
    {
        if (healthChange != default) 
        {
            _health += healthChange;
            if (_health > maxHealth) _health = maxHealth;
            view.UpdateHealthBar();

            if (healthChange < 0 && _health > 0) StartCoroutine(DamageFeedback());
            else if(healthChange > 0) StartCoroutine(HealingFeedback());
        }

        if (_health <= 0) StartCoroutine(DeathFeedback());

        float health = _health;
        return health; 
    }

    #region FEEDBACKS
    IEnumerator DamageFeedback()
    {
        for (int i = 0; i < view.feedbackLength; i++)
        {
            view.mySprite.material.color = view.damageColor;
            yield return new WaitForSeconds(0.25f / view.feedbackSpeed);
            view.mySprite.material.color = view.originalColor;
            yield return new WaitForSeconds(0.25f / view.feedbackSpeed);
        }
    }

    IEnumerator HealingFeedback()
    {
        for (int i = 0; i < view.feedbackLength; i++)
        {
            view.mySprite.material.color = view.healingColor;
            yield return new WaitForSeconds(0.25f / view.feedbackSpeed);
            view.mySprite.material.color = view.originalColor;
            yield return new WaitForSeconds(0.25f / view.feedbackSpeed);
        }
    }

    IEnumerator DeathFeedback()
    {
        _dying = true;
        view.mySprite.material.color = view.deathColor;       
        yield return new WaitForSeconds(1f / view.feedbackSpeed);
        Runner.Despawn(this.Object);
        print("PERDISTE");
    }
    #endregion

    static void OnFiringChanged(Changed<PlayerModel> changed)
    {
        var updateFiring = changed.Behaviour.weapon._isFiring;
        changed.LoadOld();
        var oldFiring = changed.Behaviour.weapon._isFiring;
    }

    static void OnLifeChanged(Changed<PlayerModel> changed)
    {
        var updateLife = changed.Behaviour;
        updateLife.OnLifeUpdate(updateLife._health / updateLife.maxHealth);
    }
}
