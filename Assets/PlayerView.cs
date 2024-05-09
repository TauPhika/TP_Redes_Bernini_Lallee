using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    PlayerModel model;

    [Header("CANVAS")]
    public GameObject healthBarAsset;
    public GameObject jetpackBarAsset;
    public Canvas canvas;
    Image healthBar;
    Image jetpackBar;

    [Header("MATERIAL")]
    [ReadOnly] public SpriteRenderer mySprite;
    public Color damageColor;
    public Color healingColor;
    public Color deathColor;
    [Range(1,10)]
    public int feedbackSpeed = 3;
    [Range(1, 5)]
    public int feedbackLength = 3;
    [ReadOnly] public Color originalColor;

    void Start()
    {
        model = gameObject.GetComponent<PlayerModel>();

        mySprite = gameObject.GetComponent<SpriteRenderer>();
        originalColor = mySprite.material.color;

        canvas = Instantiate(canvas);
        CreateHealthBar();
        CreateJetpackBar();

    }

    void LateUpdate()
    {
        if(jetpackBar) UpdateJetpackBar();
    }

    void CreateHealthBar()
    {
        healthBar = Instantiate(healthBarAsset, canvas.transform).GetComponent<Image>();
    }


    void CreateJetpackBar() 
    {
        jetpackBar = Instantiate(jetpackBarAsset, canvas.transform).GetComponent<Image>();
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = model.GetHealth() / model.maxHealth;
        print($"Player health: {model.GetHealth()}");
    }

    public void UpdateJetpackBar()
    {
        jetpackBar.fillAmount = model.controller._jetpackDuration / model.jetpackDuration;
    }
}
