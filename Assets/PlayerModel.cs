using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using TMPro;
using System.Linq;


public class PlayerModel : NetworkBehaviour
{
    #region VARIABLES
    public static PlayerModel local { get; private set; }
    
    public PlayerView view;
    public PlayerController controller;
    public PlayerWeapon weapon;
    //public NetworkRunner runner;
        
    [Header("HEALTH")]
    public int maxHealth;
    [Networked(OnChanged = nameof(OnLifeChanged))]
    int _health { get; set; }
    [ReadOnly] public bool _dying = false;
    public TextMeshPro healthText { get; set; } = default;

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
    [ReadOnly] public GameObject myWaitingCanvas;
    [ReadOnly] public TextMeshProUGUI myWaitingText;
    [ReadOnly] public List<PlayerModel> allPlayers;

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

        view.mySprite.material.color = Color.cyan;
        allPlayers = FindObjectsOfType<PlayerModel>().ToList();
        foreach (var p in allPlayers)
        {
            if (p != this) view.mySprite.material.color = Color.red;
        }
        view.originalColor = view.mySprite.material.color;

        _dying = false;

        if (!limitDashing)
        {
            dashCooldown = 0;
            Destroy(view.dashAsset);
        }

        _health = maxHealth;

        view = GetComponent<PlayerView>();

        controller = GetComponent<PlayerController>();
        controller._netInputs.waiting = true;


        healthText = gameObject.GetComponentInChildren<TextMeshPro>();
        ray = new Ray(transform.position, Vector3.down);
    }

    public override void FixedUpdateNetwork()
    {
        if (!_dying && GetInput(out _netInputs))
        {
            if (!controller._netInputs.waiting) view.BuildUI();
            
            controller.Move();

            if (_netInputs.isJumpPressed && !isAirborne) controller.Jump(jumpHeight);

            var rot = controller.GetAimingRotation();

            if(_netInputs.rotation != Quaternion.identity) transform.rotation = rot;

            if (_netInputs.isFirePressed && !weapon._isFiring) weapon.Fire();

            if(_netInputs.isDashPressed) StartCoroutine(controller.Dash(controller.dashDir, dashForce));

            //healthText.text = _health.ToString();
        }

        //print($"{_netInputs.movementX} | {_netInputs.movementY} | {_netInputs.rotation}");
    }

    public float GetHealth(int healthChange = default) { RPC_GetHealth(healthChange); return _health; }

    // Modifica la salud en base al valor recibido, da el feedback correspondiente y devuelve el resultado final.
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_GetHealth(int healthChange = default) 
    {
        if (healthChange != default) 
        {
            _health += healthChange;
            if (_health > maxHealth) _health = maxHealth;
            view.UpdateHealthBar(this);

            if (healthChange < 0 && _health > 0) StartCoroutine(DamageFeedback());

            else if(healthChange > 0) StartCoroutine(HealingFeedback());
        }

        if (_health <= 0) StartCoroutine(DeathFeedback());
    }

    public void Disconnect()
    {
        if (!Object.HasInputAuthority)
        {
            NetworkRunnerHandler.instance.runner.Despawn(Object);
            NetworkRunnerHandler.instance.runner.Disconnect(Object.InputAuthority);
        }

        gameObject.SetActive(false);
        
    }

    #region FEEDBACKS
    public IEnumerator DamageFeedback()
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
        GameOver(false);
        print("PERDISTE");
    }

    public void GameOver(bool won) => RPC_GameOver(won);

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_GameOver(bool won)
    {
        myWaitingCanvas = Runner.Spawn(PlayerSpawner.instance.waitingCanvas).gameObject;
        myWaitingCanvas.SetActive(true);
        myWaitingText = myWaitingCanvas.GetComponentInChildren<TextMeshProUGUI>();
        if (won) myWaitingText.text = "Congratulations, you won!"; else myWaitingText.text = "You lost. Game Over.";
        Instantiate(myWaitingCanvas);
        Runner.Despawn(this.Object);
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
