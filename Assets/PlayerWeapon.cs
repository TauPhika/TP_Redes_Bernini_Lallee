using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    PlayerModel _model;

    [Header("BULLET")]
    public GameObject bulletPrefab;
    public Transform bulletOrigin;
    [SerializeField]
    private float bulletPower = 10f;
    [SerializeField]
    private float bulletLifeTime = 2f;
    [Range(60,600)]
    public int bulletsPerMinute;

    [Header("EXTRAS")]
    public bool fullAuto;
    public bool canHurtItself;
    [ReadOnly] public bool _isFiring;

    private void Awake()
    {
        _isFiring = false;
        _model = gameObject.GetComponent<PlayerModel>();
    }

    public bool CanHurtItself(Collider2D other, bool itCan)
    {
        if (itCan) return true; else return other.gameObject != gameObject;
    }

    public bool FiringInput()
    {
        if (fullAuto) return Input.GetMouseButton(0); else return Input.GetMouseButtonDown(0);
    }

    public IEnumerator Disparar()
    {
        _isFiring = true;

        GameObject bullet = Instantiate(bulletPrefab, bulletOrigin.position, bulletOrigin.rotation).GetComponent<Bullet>().SetPlayer(this);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(bulletOrigin.right * bulletPower, ForceMode2D.Impulse);

        Destroy(bullet, bulletLifeTime);

        yield return new WaitForSeconds(60f/bulletsPerMinute);
        _isFiring = false;
    }
}
