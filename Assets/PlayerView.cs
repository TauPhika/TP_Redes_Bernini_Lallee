using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    PlayerModel model;
    public GameObject healthBarAsset;
    public Canvas canvas;
    Image healthBar;
    
    // Start is called before the first frame update
    void Start()
    {
        model = gameObject.GetComponent<PlayerModel>();
        canvas = Instantiate(canvas);
        CreateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateHealthBar()
    {
        healthBar = Instantiate(healthBarAsset, canvas.transform).GetComponent<Image>();
        print(healthBar.fillAmount);
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = model.GetHealth() / model.maxHealth;
        print(healthBar.fillAmount);
    }
}
