using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquItemSlot : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon;
    public Equipment currentItem;
    public ItemContextMenu itemContextMenu;
    
    void Start()
    {
        itemIcon.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log("Right" + currentItem.name);
            itemContextMenu.ShowContextMenu(currentItem, Input.mousePosition, this);
        }
    }
    
    public void UpdateSlotUI()
    {
        itemIcon.sprite = currentItem.icon;
        itemIcon.enabled = true;
    }
    
    public void SetCurrentItem(Equipment item)
    {
        currentItem = item;
    }
    
    public void ClearSlot()
    {
        currentItem = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }
}
