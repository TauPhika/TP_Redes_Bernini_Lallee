using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerModel : NetworkBehaviour
{
    #region VARIABLES
    public static PlayerModel local { get; private set; }
    
    public PlayerView view;
    public PlayerController controller;
    public PlayerWeapon weapon;
        
    [Header("HEALTH")]
    public int maxHealth;
    int _health;
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
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (/*Physics.Raycast(ray, 0.5f) &&*/ collision.gameObject.layer == 6) isAirborne = false;
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
        PlayerSpawner.instance.OnInput(Runner, new NetworkInput());
        print("x");
    }

    public void Update()
    {
        if (!_dying)
        {
            var move = controller.Move();

            //if (_netInputs.movementX != 0 || _netInputs.isJetpackPressed || _netInputs.movementY != 0) controller.Move();

            //if (_netInputs.isJumpPressed && !isAirborne) controller.Jump(jumpHeight);

            var rot = controller.GetAimingRotation();

            if(_netInputs.rotation != Quaternion.identity) transform.rotation = rot;

            if (weapon.FiringInput() && !weapon._isFiring)
            {
                controller._isFirePressed = true;
                weapon.fire = true;
            }

            if(_netInputs.isDashPressed) controller.CheckForDash(dashForce);
        }

        print($"{_netInputs.movementX} | {_netInputs.movementY} | {_netInputs.rotation}");
    }

    // Modifica la salud en base al valor recibido, da el feedback correspondiente y devuelve el resultado final.
    public float GetHealth(int healthChange = default) 
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
        Destroy(gameObject);
        print("PERDISTE");
    }
    #endregion
}
