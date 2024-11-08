using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public int quantity;
    public bool canUse;
    public float dropChance;
    public GameObject itemPrefab;
    
    public Item(string name, Sprite icon, int qty, bool useable, float chance,GameObject prefab)
    {
        itemName = name;
        itemIcon = icon;
        quantity = qty;
        canUse = useable;
        dropChance = chance;
        itemPrefab = prefab;
    }
}
