using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EquipmentType
{
    WEAPON,
    ARMOR,
    ACCESSORY
}

public class Equipment : MonoBehaviour
{
    public string name;
    public Sprite icon;
    public EquipmentType type;
    public int attackPower;
    public int defensePower;

    public Equipment(string name, Sprite icon, EquipmentType type, int attackPower, int defensePower)
    {
        this.name = name;
        this.icon = icon;
        this.type = type;
        this.attackPower = attackPower;
        this.defensePower = defensePower;
    }
}
