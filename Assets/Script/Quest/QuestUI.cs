using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public GameObject questPanel;
    public GameObject questItemPrefab;

    private List<GameObject> questItems = new List<GameObject>();

    public void UpdateQuestList(List<Quest> quests)
    {
        foreach (GameObject item in questItems)
        {
            Destroy(item);
        }
        questItems.Clear();

        foreach (Quest quest in quests)
        {
            GameObject questItem = Instantiate(questItemPrefab, questPanel.transform);
            Text questText = questItem.GetComponentInChildren<Text>();

            questText.text = $"{quest.questName}: {quest.currentAmount}/{quest.targetAmount}";
            if (quest.isCompleted)
            {
                questText.color = Color.green;
            }

            questItems.Add(questItem);
        }
    }
}
