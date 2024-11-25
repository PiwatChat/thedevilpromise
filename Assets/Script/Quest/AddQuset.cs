using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddQuset : MonoBehaviour
{
    public string qusetName;
    public string qusetDescription;
    public int qusetAmount;
    
    private QuestManager questManager;
    
    void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Quest newQuest = new Quest(qusetName, qusetDescription, qusetAmount);
        questManager.AddQuest(newQuest);
        Destroy(gameObject);
    }
}
