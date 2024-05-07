using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerModel model;
    [ReadOnly] public float _jetpackDuration;

    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public float fuerzaDisparo = 10f;
    public float tiempoVidaBala = 2f;

    float movementX;
    float movementY;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6) model.isAirborne = false;
    }

    void Update()
    {
              
        if (Input.GetMouseButtonDown(0))
        {
            Disparar();
        }

        model.transform.position += new Vector3(GetMovementX(model.speed), 
                                                GetMovementY(model.jumpHeight, model.jetpackPower), 
                                                0) * Time.deltaTime;


        //Apunta al mouse
        target = cam.ScreenToWorldPoint(Input.mousePosition);

        float anguloRadianes = Mathf.Atan2(target.y - model.transform.position.y, target.x - model.transform.position.x);
        float anguloGrados = (180 / Mathf.PI) * anguloRadianes;
        model.transform.rotation = Quaternion.Euler(0, 0, anguloGrados);
    }

    float GetMovementX(float speed)
    {
        movementX = Input.GetAxis("Horizontal") * speed;
        return movementX;
    }

    float GetMovementY(float height, float power)
    {
        if (Input.GetKeyDown(KeyCode.Space) && model.isAirborne == false)
        {
            model.playerRB.AddForce(Vector3.up * height, ForceMode2D.Impulse);
            model.isAirborne = true;
        }

        if (model.isAirborne) StartCoroutine(UseJetpack(power));
        else if(!model.isRechargingJetpack) StartCoroutine(RechargeJetpack());

        return movementY;
    }
    
    IEnumerator UseJetpack(float power)
    {
        model.isRechargingJetpack = false;

        if (_jetpackDuration >= 0 )
        {
            if(Input.GetAxis("Vertical") != 0)
            {
                _jetpackDuration -= Time.deltaTime;
                movementY = Input.GetAxis("Vertical") * power;
                yield return null;
            }
        }
        else if (Input.GetAxis("Vertical") != 0)
        {
            movementY = 0;
            yield return new WaitForSeconds(model.jetpackCooldown);
            _jetpackDuration = model.jetpackDuration;          
        }

    }

    IEnumerator RechargeJetpack()
    {        
        movementY = 0;
        model.isRechargingJetpack = true;
        yield return new WaitForSeconds(model.jetpackCooldownOnGround);
        _jetpackDuration = model.jetpackDuration;
    }

    void Disparar()
    {
        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation);
        Rigidbody2D rb = bala.GetComponent<Rigidbody2D>();
        rb.AddForce(puntoDisparo.right * fuerzaDisparo, ForceMode2D.Impulse);

        Destroy(bala, tiempoVidaBala);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Escenario"))
        {
            Destroy(gameObject);
        }
    }
}
