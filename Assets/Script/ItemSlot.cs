using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image itemIcon;
    public Text itemQuantityText;
    public KeyCode useKey;
    private Inventory inventory;

    private Item currentItem;

    private void Awake()
    {
        UpdateSlotVisibility();
        inventory = FindObjectOfType<Inventory>();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(useKey))
        {
            UseItem();
        }
    }

    public void SetItem(Item newItem)
    {
        currentItem = newItem;
        itemIcon.sprite = currentItem.itemIcon;
        itemQuantityText.text = $"{currentItem.itemName} (x{currentItem.quantity})"; 
        
        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemIcon.sprite = null;
        itemQuantityText.text = "";
        
        UpdateSlotVisibility();
    }
    
    private void UpdateSlotVisibility()
    { 
        gameObject.SetActive(currentItem != null);
    }
    
    private void UseItem()
    {
        if (currentItem.itemName == "Key")
        {
            return;
        }
        inventory.UseItem(currentItem.itemName);
    }
}
