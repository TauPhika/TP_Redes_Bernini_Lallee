using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public GameObject balaPrefab;
    public Transform puntoDisparo;

    [SerializeField]
    private float fuerzaDisparo = 10f;
    [SerializeField]
    private float tiempoVidaBala = 2f;

    public bool canHurtItself;

    public bool CanHurtItself(Collider2D other, bool itCan)
    {
        if (itCan) return true; else return other.gameObject != gameObject;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Disparar();
        }
    }

    void Disparar()
    {
        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation).GetComponent<Bullet>().SetPlayer(this);
        Rigidbody2D rb = bala.GetComponent<Rigidbody2D>();
        rb.AddForce(puntoDisparo.right * fuerzaDisparo, ForceMode2D.Impulse);

        Destroy(bala, tiempoVidaBala);
    }
}
