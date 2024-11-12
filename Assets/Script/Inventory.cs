using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, Item> items = new Dictionary<string, Item>();
    public ItemSlot[] itemSlots;
    public bool itemPicked = false;
    public Text testInventory;
    
    private PlayerManager playerManager;
    private Status status;
    private int maxItems;
    
    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        status = GetComponent<Status>();
        maxItems = itemSlots.Length;
    }

    public void AddItem(Item newItem)
    {
        itemPicked = true;
        if (items.ContainsKey(newItem.itemName))
        {
            items[newItem.itemName].quantity += newItem.quantity;
        }
        else
        {
            if (items.Count >= maxItems)
            {
                Debug.Log($"ไม่สามารถเพิ่ม {newItem.itemName} ได้ เพราะไม่มีช่องว่างใน inventory (สูงสุด {maxItems})");
                testInventory.text = $"cannot store {newItem.itemName} is possible because there are no spaces in the inventory.";
                StartCoroutine(TextWarpPlayer());
                itemPicked = false;
                return;
            }
            items.Add(newItem.itemName, newItem);
        }

        UpdateInventoryUI();
    }
    
    private IEnumerator TextWarpPlayer()
    {
        yield return new WaitForSeconds(3f);
        testInventory.text = "";
    }
    
    public void UseItem(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            Item item = items[itemName];
            
            if (!item.canUse)
            {
                Debug.Log($"ไม่สามารถใช้งาน {itemName} ได้ เพราะไม่ใช่ไอเท็มที่สามารถใช้ได้");
                return;
            }
            
            if (item.quantity > 0)
            {
                if (item.itemName == "HP")
                {
                    status.health += 35;
                }
                
                item.quantity--;
                
                if (item.quantity <= 0)
                {
                    items.Remove(itemName);
                }

                UpdateInventoryUI();
            }
            else
            {
                Debug.Log($"ไม่สามารถใช้งาน {itemName} ได้ เพราะจำนวนเหลือ 0");
            }
        }
        else
        {
            Debug.Log($"ไม่พบไอเท็ม {itemName} ใน inventory");
        }
    }

    public void UpdateInventoryUI()
    {
        int index = 0;
        foreach (var item in items.Values)
        {
            if (index < itemSlots.Length)
            {
                itemSlots[index].SetItem(item);
                index++;
            }
            else
            {
                break;
            }
        }
        
        for (; index < itemSlots.Length; index++)
        {
            itemSlots[index].ClearSlot();
        }
    }
}
