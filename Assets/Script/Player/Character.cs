using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Character : MonoBehaviour
{
    public TMP_Text levelText;
    public int level = 1;
    public int skillPoint = 0;
    public int experience;
    
    public int baseXP = 100;
    public float levelMultiplier = 1.5f;
    public int maxLevel = 10;
    public EquipmentUI[] equipmentUI;
    public GameObject[] upStatus;

    public Dictionary<EquipmentType, Equipment> equippedItems = new Dictionary<EquipmentType, Equipment>();
    
    private Dictionary<int, int> experienceToLevelUp = new Dictionary<int, int>();
    private Status status;
    private Equipment equipment;
    public EquInventory equInventory;

    void Start()
    {
        status = GetComponent<Status>();
        experience = 0;
        CalculateExperienceToLevelUp(maxLevel);
        UpdateTextLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddExperience(50);
        }
        
        if (Input.GetKeyDown(KeyCode.E) && equipment != null)
        {
            equInventory.AddEquipment(equipment);
            Destroy(equipment.gameObject);
            equipment = null;
        }
    }

    private void UpdateTextLevel()
    {
        levelText.text = "Lvl." + level.ToString();
    }
    
    public void CalculateExperienceToLevelUp(int maxLevel)
    {
        experienceToLevelUp.Clear();

        for (int i = 1; i <= maxLevel; i++)
        {
            int xpForLevelUp = Mathf.FloorToInt(baseXP * MathF.Pow(i, levelMultiplier));
            experienceToLevelUp.Add(i, xpForLevelUp);
        }
    }
    
    public void AddExperience(int exp)
    {
        experience += exp;

        while (experience >= experienceToLevelUp[level] && level < experienceToLevelUp.Count)
        {
            experience -= experienceToLevelUp[level];
            level++;
            
            skillPoint++;
            
            OnLevelUp();
            
            for (int i = 0; i < upStatus.Length; i++)
            {
                upStatus[i].SetActive(true);
            }

            UpdateTextLevel();
        }
    }

    public void AddEquipment(Equipment equipment)
    {
        /*if (equippedItems.ContainsKey(equipment.type))
        {
            Debug.Log(equipment.type);
            equInventory.AddEquipment(equippedItems[equipment.type]);
            equippedItems[equipment.type] = equipment;
        }
        else
        {
            equippedItems.Add(equipment.type, equipment);
        }*/
        
        if (equippedItems.ContainsKey(equipment.type))
        {
            Equipment oldEquipment = equippedItems[equipment.type];
            UpdateStatus(-oldEquipment.attackPower, -oldEquipment.defensePower);
        
            equInventory.AddEquipment(oldEquipment);
        }

        equippedItems[equipment.type] = equipment;
        UpdateStatus(equipment.attackPower, equipment.defensePower);

        for (int i = 0; i < equipmentUI.Length; i++)
        {
            equipmentUI[i].UpdateEquipmentUI(equipment);
            Debug.Log(equipment.type);
        }
    }
    
    private void UpdateStatus(int attackPower, int defensePower)
    {
        status.strength += attackPower;
        status.intelligence += defensePower;
    }
    
    public void OnLevelUp()
    {
        Debug.Log("leveled up to level " + level);
        
        for (int i = 0; i < 5; i++)
        {
            status.UpgradeStatus(i);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Equipment eq = other.GetComponent<Equipment>();
        if (eq != null)
        {
            equipment = eq;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        Equipment eq = other.GetComponent<Equipment>();
        if (eq != null)
        {
            eq = null;
        }
    }
}
