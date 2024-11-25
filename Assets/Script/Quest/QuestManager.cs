using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public List<Quest> activeQuests;
    public QuestUI questUI;

    void Start()
    {
        questUI.UpdateQuestList(activeQuests);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateQuest("test", 1);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Quest newQuest = new Quest("Collect Apples", "Collect 10 apples from the forest.", 10);
            AddQuest(newQuest);
        }
    }

    public void AddQuest(Quest newQuest)
    {
        if (!activeQuests.Exists(q => q.questName == newQuest.questName))
        {
            activeQuests.Add(newQuest);
            Debug.Log($"Added New Quest: {newQuest.questName}");
            questUI.UpdateQuestList(activeQuests);
        }
        else
        {
            Debug.LogWarning($"Quest '{newQuest.questName}' already exists!");
        }

    }

    public void UpdateQuest(string questName, int progress)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.questName == questName && !quest.isCompleted)
            {
                quest.UpdateProgress(progress);
                Debug.Log($"Updated Quest: {quest.questName} | Progress: {quest.currentAmount}/{quest.targetAmount}");

                questUI.UpdateQuestList(activeQuests);

                if (quest.isCompleted)
                {
                    Debug.Log($"Quest '{quest.questName}' Completed!");
                    RemoveCompletedQuest(quest, 3f);
                }
                return;
            }
        }
    }
    
    public void RemoveCompletedQuest(Quest quest, float delay)
    {
        StartCoroutine(RemoveCompletedQuestCoroutine(quest, delay));
    }

    private IEnumerator RemoveCompletedQuestCoroutine(Quest quest, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (quest.isCompleted && activeQuests.Contains(quest))
        {
            activeQuests.Remove(quest);
            Debug.Log($"Removed Quest: {quest.questName}");
            questUI.UpdateQuestList(activeQuests);
        }
    }
}
