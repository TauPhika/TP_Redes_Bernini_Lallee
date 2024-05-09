using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerModel model;
    public PlayerWeapon weapon;
    [ReadOnly] public float _jetpackDuration;

    float _movementX;
    float _movementY;
    [ReadOnly] public float _timeOnGround = 0;

    private Vector3 target;

    [ReadOnly] public Camera cam;


    private void Awake()
    {
        cam = FindObjectOfType<Camera>();
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
                StartCoroutine(weapon.Disparar());
            }
        }
    }

    float GetMovementX(float speed)
    {
        _movementX = Input.GetAxis("Horizontal") * speed;
        return _movementX;
    }

    float GetMovementY(float height, float power)
    {
        if (Input.GetKeyDown(KeyCode.Space) && !model.isAirborne)
        {
            model.playerRB.AddForce(Vector3.up * height, ForceMode2D.Impulse);
            model.isAirborne = true;
        }

        if (model.isAirborne)
        {
            StartCoroutine(UseJetpack(power));
            RechargeJetpack(false);
        }
        else RechargeJetpack(true);


        return _movementY;
    }

    Quaternion GetAimingRotation()
    {
        //Apunta al mouse
        target = cam.ScreenToWorldPoint(Input.mousePosition);

        float radians = Mathf.Atan2(target.y - model.transform.position.y, target.x - model.transform.position.x);
        float degrees = (180 / Mathf.PI) * radians;
        return Quaternion.Euler(0, 0, degrees);
    }
    
    IEnumerator UseJetpack(float power)
    {
        model.isRechargingJetpack = false;

        if (Input.GetAxis("Vertical") != 0)
        {
            if (_jetpackDuration > 0)
            {
                _jetpackDuration -= Time.deltaTime;
                _movementY = Input.GetAxis("Vertical") * power;
                yield return null;
            }
            else
            {
                //_movementY = 0;
                yield return new WaitForSeconds(model.jetpackCooldown);
                _jetpackDuration = model.jetpackDuration;
            }
        }

    }

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

}
