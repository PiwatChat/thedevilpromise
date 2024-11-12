using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : MonoBehaviour
{
    public Image equipmentImage;
    public TMP_Text equipmentText;
    public EquipmentType uiType;
    
    private Equipment equipment;

    void Start()
    {
        UpdateVisibility();
    }

    void Update()
    {
        
    }

    public void UpdateEquipmentUI(Equipment eq)
    {
        if (eq.type == uiType)
        {
            equipment = eq;
            equipmentText.text = uiType.ToString();
            equipmentImage.sprite = eq.icon;
            gameObject.SetActive(true);
        }
    }
    
    public void ClearEquipmentUI()
    {
        equipment = null;
        equipmentImage.sprite = null;
        equipmentText.text = "";
        
        UpdateVisibility();
    }
    
    private void UpdateVisibility()
    { 
        gameObject.SetActive(equipment != null);
    }
}
