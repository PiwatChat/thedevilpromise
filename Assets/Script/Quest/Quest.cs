using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest : MonoBehaviour
{
    public string questName;
    public string description;
    public int targetAmount;
    public int currentAmount;
    public bool isCompleted;

    public Quest(string name, string desc, int target)
    {
        questName = name;
        description = desc;
        targetAmount = target;
        currentAmount = 0;
        isCompleted = false;
    }

    public void UpdateProgress(int amount)
    {
        currentAmount += amount;
        if (currentAmount >= targetAmount)
        {
            isCompleted = true;
            Debug.Log($"Quest '{questName}' Completed!");
        }
    }
}
