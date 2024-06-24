using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES
    public PlayerModel model;
    public PlayerWeapon weapon;
    [ReadOnly] public float _jetpackDuration;

    float _movementX;
    float _movementY;
    bool _isJumpPressed;
    bool _isDashPressed;
    bool _isJetpackPressed;
    bool _isRechargingJetpack;
    [ReadOnly] public float _timeOnGround = 0;

    private Vector3 target;
    [ReadOnly] public Camera cam;

    NetworkInputData _inputData;
    #endregion

    private void Awake()
    {
        cam = FindObjectOfType<Camera>();
        _inputData = new NetworkInputData();
    }

    private void Start()
    {
        _jetpackDuration = model.jetpackDuration;
    }

    void Update()
    {
        if (!model._dying)
        {

            model.transform.position += new Vector3(GetMovementX(model.speed),
                                                    GetMovementY(model.jumpHeight, model.jetpackPower),
                                                    0) * Time.deltaTime;

            model.transform.rotation = GetAimingRotation();

            if (weapon.FiringInput() && !weapon._isFiring)
            {
                StartCoroutine(weapon.FireWeapon());
            }

            CheckForDash(model.dashForce);
        }
    }

    public NetworkInputData GetLocalInputs()
    {
        _inputData.movementX = _movementX;
        _inputData.movementY = _movementY;
        _inputData.isAirborne = model.isAirborne;
        _inputData.isDashPressed = _isDashPressed;
        _inputData.isJetpackPressed = _isJetpackPressed;
        _inputData.isJumpPressed = _isJumpPressed;
        _inputData.isFirePressed = weapon.FiringInput();
        _inputData.isFiring = weapon._isFiring;
        _inputData.isRechargingJetpack = model.isRechargingJetpack;

        _isDashPressed = _isJetpackPressed = _isJumpPressed = false;

        return _inputData;
    }

    #region MOVEMENT

    // Devuelve el movimiento normal en x
    float GetMovementX(float speed)
    {
        _movementX = Input.GetAxis("Horizontal") * speed;
        return _movementX;
    }

    // Devuelve el movimiento en Y, incluyendo salto y jetpack.
    float GetMovementY(float height, float power)
    {
        if (Input.GetKeyDown(KeyCode.Space) && !model.isAirborne)
        {
            model.playerRB.AddForce(Vector3.up * height, ForceMode2D.Impulse);
            model.isAirborne = true;
            _isJumpPressed = true;
        }

        if (model.isAirborne)
        {
            StartCoroutine(UseJetpack(power));
            RechargeJetpack(false);
        }
        else RechargeJetpack(true);


        return _movementY;
    }

    // Devuelve la rotacion en base a la posicion del mouse
    Quaternion GetAimingRotation()
    {
        target = cam.ScreenToWorldPoint(Input.mousePosition);

        float radians = Mathf.Atan2(target.y - model.transform.position.y, target.x - model.transform.position.x);
        float radToDegrees = (180 / Mathf.PI) * radians;
        return Quaternion.Euler(0, 0, radToDegrees);
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
                _movementY = Input.GetAxis("Vertical") * power;
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

            _movementY = 0;
        }
        else _timeOnGround = 0;
    }
    #endregion

    #region DASH
    float doubleTapSpeed = 0.5f;
    bool pressedFirstTime = false;
    float lastPressedTime;

    void CheckForDash(int force)
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
                    if (!model.hasDashed) { _isDashPressed = true; StartCoroutine(Dash(dir, force)); }
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

    IEnumerator Dash(Vector3 dir, float force)
    {
        model.hasDashed = true;
        model.playerRB.AddForce(dir * force, ForceMode2D.Impulse);
        model.view.UpdateDashImage(false);

        yield return new WaitForSeconds(model.dashCooldown);

        model.hasDashed = false;
        model.view.UpdateDashImage(true);
    }
    #endregion
}