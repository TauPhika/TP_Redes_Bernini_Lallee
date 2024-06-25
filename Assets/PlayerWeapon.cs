using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class PlayerWeapon : MonoBehaviour
{
    #region VARIABLES
    [ReadOnly] public PlayerModel _model;

    [Header("BULLET")]
    public GameObject bulletPrefab;
    public Transform bulletOrigin;
    [SerializeField] private float bulletPower = 10f;
    [SerializeField] private float bulletLifeTime = 2f;
    [Range(60, 600)] public int bulletsPerMinute;
    [SerializeField] public TextMeshPro reloadText;

    [Header("EXTRAS")]
    public bool fullAuto;
    public bool canHurtItself;
    [ReadOnly] public bool _isFiring;
    [Networked(OnChanged = nameof(Fire))] public bool fire { get; set; }
    #endregion

    private void Awake()
    {
        _isFiring = false;
        _model = gameObject.GetComponent<PlayerModel>();

        reloadText = Instantiate(new GameObject(),
                                _model.gameObject.transform.position + new Vector3(4.2f, 0.1f, 0),
                                Quaternion.identity,
                                _model.gameObject.transform).
                                AddComponent<TextMeshPro>();

        reloadText.fontSize = 5;
    }

    private void Update()
    {
        reloadText.transform.position = _model.gameObject.transform.position + new Vector3(4.2f, 0.1f, 0);
        reloadText.transform.rotation = Quaternion.identity;
    }

    public bool CanHurtItself(Collider2D other, bool itCan)
    {
        if (itCan) return true; else return other.gameObject != gameObject;
    }

    public bool FiringInput()
    {
        if (fullAuto) return Input.GetMouseButton(0); else return Input.GetMouseButtonDown(0);
    }

    public void Fire() { StartCoroutine(FireWeapon()); }

    public IEnumerator FireWeapon()
    {
        _isFiring = true;

        GameObject bullet = _model.runner.Spawn(bulletPrefab, bulletOrigin.transform.position, bulletOrigin.transform.rotation).
                            GetComponent<Bullet>().SetPlayer(this);

        NetworkRigidbody2D rb = bullet.GetComponent<NetworkRigidbody2D>();
        rb.Rigidbody.AddForce(bulletOrigin.transform.right * bulletPower, ForceMode2D.Impulse);

        Destroy(bullet, bulletLifeTime);

        if (bulletsPerMinute <= 140 && !fullAuto) reloadText.text = "Rechambering...";

        yield return new WaitForSeconds(60f / bulletsPerMinute);
        reloadText.text = "";
        _isFiring = false;
    }
}
