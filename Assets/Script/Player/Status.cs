using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Status : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float mana;
    public int strength;
    public int agility;
    public int intelligence;
    
    public GameObject UiStatus;

    public TMP_Text textHealth;
    public TMP_Text textMana;
    public TMP_Text textStrength;
    public TMP_Text textAgility;
    public TMP_Text textIntelligence;
    
    private PlayerManager playerManager;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        UiStatus.SetActive(false);
    }
    
    public Status(float health, float maxHealth, float mana, int strength, int agility, int intelligence)
    {
        this.health = health;
        this.maxHealth = maxHealth;
        this.mana = mana;
        this.strength = strength;
        this.agility = agility;
        this.intelligence = intelligence;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UiStatus.SetActive(!UiStatus.activeSelf);
            OpenUiStatus();
        }
    }

    private void OpenUiStatus()
    {
        textHealth.text = "Health : "+health.ToString();
        textMana.text = "Mana : " + mana.ToString();
        textStrength.text = "Strength : " + strength.ToString();
        textAgility.text = "Agility : " + agility.ToString();
        textIntelligence.text = "Intelligence : " + intelligence.ToString();
    }

    /*public void ApplyStatusEffect(StatusEffect effect)
    {
        effect.Apply(this);
    }*/
    
    public void UpgradeStatus(int upgradeType)
    {
        switch (upgradeType)
        {
            case 0:
                maxHealth += 10;
                playerManager.healthSlider.maxValue = maxHealth;
                break;

            case 1:
                strength += 2;
                break;

            case 2:
                agility += 2;
                break;

            case 3:
                intelligence += 2;
                break;

            case 4:
                mana += 10;
                break;

            default:
                Debug.LogWarning("Invalid upgrade type");
                break;
        }
    }
    
    public void ResetHealth()
    {
        health = maxHealth;
    }
}
