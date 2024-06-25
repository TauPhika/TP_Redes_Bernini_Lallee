using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class PlayerView : MonoBehaviour
{
    #region VARIABLES
    PlayerModel model;

    [Header("CANVAS")]
    //public GameObject healthBarAsset;
    public GameObject jetpackBarAsset;
    public GameObject dashAsset;
    public GameObject textAsset;
    public Canvas canvas;
    Image healthBar;
    Image jetpackBar;
    [ReadOnly] public TextMeshProUGUI objectiveText;

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
    public List<Color> playerColors;
    #endregion

    void Awake()
    {
        model = gameObject.GetComponent<PlayerModel>();

        mySprite = gameObject.GetComponent<SpriteRenderer>();

    }

    void LateUpdate()
    {
        model.healthText.transform.position = model.gameObject.transform.position;
        model.healthText.transform.rotation = Quaternion.identity;
        if (jetpackBar) UpdateJetpackBar();
    }

    #region CREATORS
    public void BuildUI()
    {
        if (PlayerModel.local == model && !model.controller._netInputs.waiting)
        {
            canvas = Instantiate(canvas);
            CreateObjectiveText();
            CreateDashImage();
            CreateHealthBar();
            CreateJetpackBar();
            model.controller._netInputs.waiting = true;
        }
    }

    void CreateHealthBar()
    {
        //healthBar = Instantiate(healthBarAsset, canvas.transform).GetComponent<Image>();

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
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void UpdateHealthBar(PlayerModel model)
    {
        var health = model.GetHealth();
        //healthBar.fillAmount = health / model.maxHealth;
        model.healthText.text = health.ToString();
        print($"Player health: {health}");
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
