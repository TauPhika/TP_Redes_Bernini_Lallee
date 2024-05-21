using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerView : MonoBehaviour
{
    #region VARIABLES
    PlayerModel model;

    [Header("CANVAS")]
    public GameObject healthBarAsset;
    public GameObject jetpackBarAsset;
    public GameObject dashAsset;
    public GameObject textAsset;
    public Canvas canvas;
    Image healthBar;
    Image jetpackBar;
    TextMeshProUGUI objectiveText;

    [Header("MATERIAL")]
    [ReadOnly] public SpriteRenderer mySprite;
    public Color damageColor;
    public Color healingColor;
    public Color deathColor;
    [Range(1, 10)]
    public int feedbackSpeed = 3;
    [Range(1, 5)]
    public int feedbackLength = 3;
    [ReadOnly] public Color originalColor;
    #endregion

    void Start()
    {
        model = gameObject.GetComponent<PlayerModel>();

        mySprite = gameObject.GetComponent<SpriteRenderer>();
        originalColor = mySprite.material.color;

        if(PlayerSpawner.instance.allPlayers.Count <= 1)
        {
            canvas = Instantiate(canvas);
            CreateObjectiveText();
            CreateDashImage();
            CreateHealthBar();
            CreateJetpackBar();
        }
    }

    void LateUpdate()
    {
        if (jetpackBar) UpdateJetpackBar();
    }

    #region CREATORS
    void CreateHealthBar()
    {
        healthBar = Instantiate(healthBarAsset, canvas.transform).GetComponent<Image>();
    }


    void CreateJetpackBar()
    {
        jetpackBar = Instantiate(jetpackBarAsset, canvas.transform).GetComponent<Image>();
    }

    void CreateDashImage()
    {
        dashAsset = Instantiate(dashAsset, canvas.transform);
    }

    void CreateObjectiveText()
    {
        objectiveText = Instantiate(textAsset, canvas.transform).GetComponent<TextMeshProUGUI>();
    }
    #endregion

    #region UPDATERS
    public void UpdateHealthBar()
    {
        healthBar.fillAmount = model.RPC_GetHealth() / model.maxHealth;
        print($"Player health: {model.RPC_GetHealth()}");
    }

    public void UpdateJetpackBar()
    {
        jetpackBar.fillAmount = model.controller._jetpackDuration / model.jetpackDuration;
    }

    public void UpdateDashImage(bool state)
    {
        dashAsset.SetActive(state);
    }
    #endregion

}
