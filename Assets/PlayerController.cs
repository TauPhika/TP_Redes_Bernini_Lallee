using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public class PlayerController : MonoBehaviour
{
    #region VARIABLES
    public PlayerModel model;
    public PlayerWeapon weapon;
    [ReadOnly] public float _jetpackDuration;

    [ReadOnly] public float _timeOnGround = 0;

    private Vector3 target;
    [ReadOnly] public Camera cam;

    NetworkInputData _netInputs;
    bool _isJumpPressed;
    bool _isJetpackPressed;
    [ReadOnly] public bool _isFirePressed;
    bool _isDashPressed;
    #endregion

    private void Awake()
    {
        cam = FindObjectOfType<Camera>();
        _netInputs = new NetworkInputData();
    }

    private void Start()
    {
        _jetpackDuration = model.jetpackDuration;
    }

    void Update()
    {
        
    }

    public NetworkInputData GetLocalInputs()
    {
        _netInputs.isJumpPressed = _isJumpPressed; _isJumpPressed = false;
        _netInputs.isDashPressed = _isDashPressed; _isDashPressed = false;
        _netInputs.isJetpackPressed = _isJetpackPressed; _isJetpackPressed = false;
        _netInputs.isFirePressed = _isFirePressed; _isFirePressed = false;

        print(_isJumpPressed);

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
        _isJumpPressed = true;
        model.isAirborne = true;
        model.playerRB.Rigidbody.AddForce(Vector3.up * height, ForceMode2D.Impulse);
        return Vector3.up * height;
    }

    // Devuelve la rotacion en base a la posicion del mouse
    public Quaternion GetAimingRotation()
    {
        target = cam.ScreenToWorldPoint(Input.mousePosition);

        float radians = Mathf.Atan2(target.y - model.transform.position.y, target.x - model.transform.position.x);
        float radToDegrees = (180 / Mathf.PI) * radians;
        _netInputs.rotation = Quaternion.Euler(0, 0, radToDegrees);
        return _netInputs.rotation;
    }


    public Vector3 Move()
    {
        var move = new Vector3(GetMovementX(model.speed),
                           GetMovementY(model.jumpHeight, model.jetpackPower),
                           0) * Time.deltaTime;

        model.transform.position += move;

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
                _jetpackDuration -= Time.deltaTime;
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
            _timeOnGround += Time.deltaTime;
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
    float doubleTapSpeed = 0.5f;
    bool pressedFirstTime = false;
    float lastPressedTime;

    public void CheckForDash(int force)
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A))
        {
            Vector3 dir;

            if (Input.GetKeyDown(KeyCode.D)) dir = Vector3.right;
            else dir = Vector3.left;

            if (pressedFirstTime) // Chequeamos si el boton ya se presiono una vez
            {
                // Esto es cierto si presionamos dos veces dentro del tiempo determinado
                bool isDoublePress = Time.time - lastPressedTime <= doubleTapSpeed;

                if (isDoublePress)
                {
                    if (!model.hasDashed) { _isDashPressed = true; StartCoroutine(Dash(dir, force));}
                    pressedFirstTime = false;
                }

            }
            else // Y, si no se presiono una vez...
            {
                pressedFirstTime = true; // ...entonces esta es la primera vez
            }

            lastPressedTime = Time.time;
        }


        if (pressedFirstTime && Time.time - lastPressedTime > doubleTapSpeed)
        {
            // Si presionamos una vez pero despues no volvemos a hacerlo, nos olvidamos de esa primera vez.
            pressedFirstTime = false;
        }


    }

    public IEnumerator Dash(Vector3 dir, float force)
    {
        model.hasDashed = true;
        model.playerRB.Rigidbody.AddForce(dir * force, ForceMode2D.Impulse);
        model.view.UpdateDashImage(false);

        yield return new WaitForSeconds(model.dashCooldown);

        model.hasDashed = false;
        model.view.UpdateDashImage(true);
    }
    #endregion
}


