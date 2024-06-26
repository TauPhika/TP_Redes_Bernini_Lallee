using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    #region VARIABLES
    public PlayerModel model;
    public PlayerWeapon weapon;
    [ReadOnly] public float _jetpackDuration;

    [ReadOnly] public float _timeOnGround = 0;

    private Vector3 target;
    [ReadOnly] public Camera cam;

    [ReadOnly] public NetworkInputData _netInputs;
    bool _isJumpPressed;
    bool _isJetpackPressed;
    [ReadOnly] public bool _isFirePressed;
    bool _isDashPressed;
    bool _waiting = true;
    [ReadOnly] public Vector3 dashDir;

    float _movementX;
    float _movementY;
    #endregion

    private void Awake()
    {
        _netInputs.waiting = _waiting;
        cam = FindObjectOfType<Camera>();
        _netInputs = new NetworkInputData();
    }

    private void Start()
    {
        _jetpackDuration = model.jetpackDuration;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isJumpPressed = true;
        }
        if (weapon.FiringInput()) _isFirePressed = true;

        var dir = CheckForDash(model.dashForce);

        if (dir != default) { _netInputs.dashDir = dir; _isDashPressed = true; }
    }

    public NetworkInputData GetLocalInputs()
    {
        _netInputs.isJumpPressed = _isJumpPressed; _isJumpPressed = false;
        _netInputs.isDashPressed = _isDashPressed; _isDashPressed = false;
        _netInputs.isJetpackPressed = _isJetpackPressed; _isJetpackPressed = false;
        _netInputs.isFirePressed = _isFirePressed; _isFirePressed = false;

        return _netInputs;
    }

    #region MOVEMENT

    // Devuelve el movimiento normal en x
    public float GetMovementX(float speed)
    {
        _netInputs.movementX = Input.GetAxis("Horizontal") * speed;
        return _netInputs.movementX;
    }

    // Devuelve el movimiento en Y, incluyendo salto y jetpack.
    public float GetMovementY(float height, float power)
    {
        if (model.isAirborne)
        {
            StartCoroutine(UseJetpack(power));
            RechargeJetpack(false);
        }
        else RechargeJetpack(true);


        return _netInputs.movementY;
    }

    public Vector3 Jump(float height)
    {
        model.isAirborne = true;
        model.playerRB.Rigidbody.AddForce(Vector3.up * height, ForceMode2D.Impulse);
        return Vector3.up * height;
    }

    // Devuelve la rotacion en base a la posicion del mouse
    public Quaternion GetAimingRotation()
    {
        target = PlayerModel.local.controller.cam.ScreenToWorldPoint(Input.mousePosition);

        float radians = Mathf.Atan2(target.y - model.transform.position.y, target.x - model.transform.position.x);
        float radToDegrees = (180 / Mathf.PI) * radians;
        _netInputs.rotation = Quaternion.Euler(0, 0, radToDegrees);
        return _netInputs.rotation;
    }


    public Vector3 Move()
    {
        var move = new Vector2(GetMovementX(model.speed),
                           GetMovementY(model.jumpHeight, model.jetpackPower)) * Time.fixedDeltaTime;

        PlayerModel.local.playerRB.Rigidbody.position += move;

        //if (move == Vector2.zero) PlayerModel.local.playerRB.Rigidbody.velocity = default;
        //else 
        //{
        //    PlayerModel.local.playerRB.Rigidbody.AddForce(Vector2.ClampMagnitude(move * model.speed, model.maxSpeed), ForceMode2D.Impulse);
        //} 

        return move;
    }
    #endregion

    #region JETPACK
    IEnumerator UseJetpack(float power)
    {
        model.isRechargingJetpack = false;

        if (Input.GetAxis("Vertical") != 0)
        {
            _isJetpackPressed = true;

            if (_jetpackDuration > 0)
            {
                _jetpackDuration -= Time.fixedDeltaTime;
                _netInputs.movementY = Input.GetAxis("Vertical") * power;
                yield return null;
            }
            else
            {
                // Recarga el jetpack automaticamente tras un cierto tiempo cuando esta vacio.
                yield return new WaitForSeconds(model.jetpackCooldown);
                _jetpackDuration = model.jetpackDuration;
            }
        }

    }

    // Recarga el jetpack cuando el jugador esta en contacto con el piso por un tiempo.
    void RechargeJetpack(bool recharging)
    {
        if (recharging)
        {
            _timeOnGround += Time.fixedDeltaTime;
            if (_timeOnGround >= model.jetpackCooldownOnGround)
            {
                _jetpackDuration = model.jetpackDuration;
                _timeOnGround = 0;
            }

            _netInputs.movementY = 0;
        }
        else _timeOnGround = 0;
    }
    #endregion

    #region DASH
    float doubleTapSpeed = 0.25f;
    bool pressedAFirstTime = false;
    bool pressedDFirstTime = false;
    float lastPressedTime;

    public Vector3 CheckForDash(int force)
    {
        Vector3 d = default;

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (pressedDFirstTime) // Chequeamos si el boton ya se presiono una vez
            {
                // Esto es cierto si presionamos dos veces dentro del tiempo determinado
                bool isDoublePress = Time.fixedTime - lastPressedTime <= doubleTapSpeed;

                if (isDoublePress)
                {
                    if (!model.hasDashed) { d = Vector3.right; }
                    pressedDFirstTime = false;
                }

            }
            else // Y, si no se presiono una vez...
            {
                pressedDFirstTime = true; // ...entonces esta es la primera vez
            }

            lastPressedTime = Time.fixedTime;

        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (pressedAFirstTime) // Lo mismo pero con A
            {
                bool isDoublePress = Time.fixedTime - lastPressedTime <= doubleTapSpeed;

                if (isDoublePress)
                {
                    if (!model.hasDashed) { d = Vector3.left; }
                    pressedAFirstTime = false;
                }

            }
            else
            {
                pressedAFirstTime = true;
            }

            lastPressedTime = Time.fixedTime;

        }


        if (pressedAFirstTime && Time.fixedTime - lastPressedTime > doubleTapSpeed)
        {
            pressedAFirstTime = false;
        }

        return d;
    }

    public IEnumerator Dash(Vector3 dir, float force)
    {
        //print("dasheando");

        model.hasDashed = true;
        model.playerRB.Rigidbody.AddForce(dir * force, ForceMode2D.Impulse);
        model.view.UpdateDashImage(false);

        yield return new WaitForSeconds(model.dashCooldown);

        //print("dasheandon't");

        model.hasDashed = false;
        model.view.UpdateDashImage(true);
    }
    #endregion
}