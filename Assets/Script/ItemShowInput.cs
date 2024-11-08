using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemShowInput : MonoBehaviour
{
    public GameObject popupText;
    public TMP_Text popupT;
    private PopupText popText;
    public string showInput;
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (popText == null)
            {
                popupT.text = showInput;
                GameObject popUp = Instantiate(popupText, transform.position, Quaternion.identity);
                popUp.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, -0.5f);
                
                popText = popUp.GetComponent<PopupText>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && popText != null)
        {
            popText.DestroyPopup();
            popText = null;
        }
    }
}
