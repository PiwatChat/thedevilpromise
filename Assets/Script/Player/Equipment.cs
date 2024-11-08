using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    WEAPON,
    ARMOR,
    ACCESSORY
}

public class Equipment : MonoBehaviour
{
    public string name;
    public EquipmentType type;
    public int attackPower;
    public int defensePower;

    public Equipment(string name, EquipmentType type, int attackPower, int defensePower)
    {
        this.name = name;
        this.type = type;
        this.attackPower = attackPower;
        this.defensePower = defensePower;
    }
}
