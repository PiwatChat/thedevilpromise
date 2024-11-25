using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContextMenu : MonoBehaviour
{
    public GameObject contextMenu;
    public GameObject itemPrefab;
    
    private Character character;
    private Equipment equipment;
    private EquInventory equInventory;
    private EquItemSlot itemSlot;

    private void Start()
    {
        character = FindObjectOfType<Character>();
        equInventory = FindObjectOfType<EquInventory>();
        contextMenu.SetActive(false);
    }
    
    public void ShowContextMenu(Equipment item, Vector3 position,  EquItemSlot slot)
    {
        if (contextMenu.activeSelf)
        {
            CloseContextMenu();
        }
        else
        {
            contextMenu.SetActive(true);
            contextMenu.transform.position = position;
            equipment = item;
            itemSlot = slot;
        }
    }
    
    public void CloseContextMenu()
    {
        contextMenu.SetActive(false);
    }

    public void UseItem()
    {
        character.AddEquipment(equipment);
        equInventory.RemoveEquipment(equipment);
        
        if (itemSlot != null)
        {
            itemSlot.ClearSlot();
        }
        
        //Debug.Log("Used item: " + equipment.name);
    }

    public void DropItem()
    {
        equInventory.RemoveEquipment(equipment);
        
        if (itemSlot != null)
        {
            itemSlot.ClearSlot();
        }
        
        CreateDroppedItem();
        
        //Debug.Log("Dropped item: " + equipment.name);
    }
    
    private void CreateDroppedItem()
    {
        if (itemPrefab != null)
        {
            GameObject droppedItem = Instantiate(itemPrefab, character.transform.position, Quaternion.identity);
            Equipment newItem = droppedItem.GetComponent<Equipment>();

            if (newItem != null)
            {
                newItem.name = equipment.name;
                newItem.icon = equipment.icon;
                newItem.attackPower = equipment.attackPower;
                newItem.defensePower = equipment.defensePower;
                newItem.type = equipment.type;
            }
        }
        else
        {
            Debug.LogError("Item Prefab is not assigned!");
        }
    }
}
