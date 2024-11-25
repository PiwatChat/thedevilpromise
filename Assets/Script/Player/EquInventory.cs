using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquInventory : MonoBehaviour
{
    public Dictionary<string, Equipment> equipmentDict = new Dictionary<string, Equipment>();
    public Transform inventoryPanel;
    public EquItemSlot[] itemSlots;
    
    public void AddEquipment(Equipment equipment)
    {
        if (!equipmentDict.ContainsKey(equipment.name))
        {
            equipmentDict.Add(equipment.name, equipment);
        }
        
        else
        {
            Debug.Log("Item already exists in inventory!");
        }
        
        UpdateInventoryUI();
    }
    
    public void RemoveEquipment(Equipment equipment)
    {
        if (equipmentDict.ContainsKey(equipment.name))
        {
            equipmentDict.Remove(equipment.name);
            Debug.Log("Removed item: " + equipment.name);
        }
        else
        {
            Debug.Log("Item not found in inventory!");
        }

        UpdateInventoryUI();
    }
    
    void UpdateInventoryUI()
    {
        int index = 0;
        foreach (KeyValuePair<string, Equipment> kvp in equipmentDict)
        {
            if (index >= itemSlots.Length) break;
            EquItemSlot itemSlot = itemSlots[index];
            itemSlot.currentItem = kvp.Value;
            itemSlot.UpdateSlotUI();
            index++;
        }
    }
}
