using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int level = 1;
    public int experience;
    
    public int baseXP = 100;
    public float levelMultiplier = 1.5f;
    public int maxLevel = 100;
    //public int experienceToLevelUp;
    
    private Dictionary<int, int> experienceToLevelUp = new Dictionary<int, int>();

    public Dictionary<EquipmentType, Equipment> equippedItems;
    
    private Status status;

    void Start()
    {
        status = GetComponent<Status>();
        experience = 0;
        equippedItems = new Dictionary<EquipmentType, Equipment>();
        CalculateExperienceToLevelUp(50);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddExperience(50);
        }
    }
    
    public void CalculateExperienceToLevelUp(int maxLevel)
    {
        //experienceToLevelUp = Mathf.FloorToInt(baseXP * Mathf.Pow(level, levelMultiplier));
        experienceToLevelUp.Clear();

        for (int i = 1; i <= maxLevel; i++)
        {
            int xpForLevelUp = Mathf.FloorToInt(baseXP * MathF.Pow(i, levelMultiplier));
            experienceToLevelUp.Add(i, xpForLevelUp);
        }
    }
    
    public void Upgrade()
    {
        level++;
    }
    
    public void AddExperience(int exp)
    {
        experience += exp;
        
        /*while (experience >= experienceToLevelUp && level < maxLevel)
        {
            experience -= experienceToLevelUp;
            level++;
            CalculateExperienceToLevelUp();
            OnLevelUp();
        }*/

        while (experience >= experienceToLevelUp[level] && level < experienceToLevelUp.Count)
        {
            experience -= experienceToLevelUp[level];
            level++;
            OnLevelUp();
        }
    }

    public void AddEquipment(Equipment equipment)
    {
        if (equippedItems.ContainsKey(equipment.type))
        {
            equippedItems[equipment.type] = equipment;
        }
        else
        {
            equippedItems.Add(equipment.type, equipment);
        }
    }
    
    public void OnLevelUp()
    {
        Debug.Log("leveled up to level " + level);

        for (int i = 0; i < 5; i++)
        {
            status.UpgradeStatus(i);
        }
    }
}
