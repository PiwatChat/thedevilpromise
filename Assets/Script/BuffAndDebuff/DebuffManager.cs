using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DebuffManager : MonoBehaviour
{
    public List<BuffDebuff> debuffs; // List สำหรับเก็บ Debuff
    public Text debuffText; // Text สำหรับแสดงสถานะ Debuff

    private Status status;
    private SpriteRenderer spriteRenderer;
    private Color originalColor; // เก็บสีเดิมของ Player

    private Dictionary<DebuffType, float> debuffCooldownTimers = new Dictionary<DebuffType, float>();
    private List<ActiveDebuff> activeDebuffs = new List<ActiveDebuff>();

    void Start()
    {
        status = GetComponent<Status>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        // อัปเดตคูลดาวน์ของ Debuff
        UpdateCooldownTimers();
        UpdateActiveDebuffs();
        UpdateStatusText();
    }

    public void ApplyDebuff(BuffDebuff debuff)
    {
        if (CanUseDebuff(debuff.debuffType))
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue <= debuff.probability)
            {
                switch (debuff.debuffType)
                {
                    case DebuffType.Poison:
                        activeDebuffs.Add(new ActiveDebuff(DebuffType.Poison, debuff.value, debuff.duration));
                        break;
                    case DebuffType.Weakness:
                        status.strength = Mathf.Max(0, status.strength - (int)debuff.value);
                        break;
                    case DebuffType.Slow:
                        status.agility = Mathf.Max(0, status.agility - (int)debuff.value);
                        break;
                }
                debuffCooldownTimers[debuff.debuffType] = debuff.cooldown;

                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.red;
                }
                Debug.Log($"Debuff {debuff.debuffType} applied");
            }
        }
    }

    private bool CanUseDebuff(DebuffType type)
    {
        return !debuffCooldownTimers.ContainsKey(type) || debuffCooldownTimers[type] <= 0;
    }

    private void UpdateCooldownTimers()
    {
        List<DebuffType> keys = new List<DebuffType>(debuffCooldownTimers.Keys);
        foreach (var key in keys)
        {
            if (debuffCooldownTimers[key] > 0)
                debuffCooldownTimers[key] -= Time.deltaTime;
        }
    }

    private void UpdateActiveDebuffs()
    {
        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            ActiveDebuff activeDebuff = activeDebuffs[i];
            activeDebuff.remainingDuration -= Time.deltaTime;

            if (activeDebuff.debuffType == DebuffType.Poison)
            {
                ApplyPoisonDamage(activeDebuff.damagePerSecond);
            }

            if (activeDebuff.remainingDuration <= 0)
            {
                activeDebuffs.RemoveAt(i);
            }
        }

        if (spriteRenderer != null && activeDebuffs.Count == 0)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void ApplyPoisonDamage(float damagePerSecond)
    {
        status.health -= damagePerSecond * Time.deltaTime;
        Debug.Log($"Applying poison damage: -{damagePerSecond * Time.deltaTime} HP");
        if (status.health <= 0)
        {
            Debug.Log("Character is dead.");
        }
    }

    private void UpdateStatusText()
    {
        debuffText.text = "Active Debuffs:\n";
        foreach (var debuff in activeDebuffs)
        {
            debuffText.text += $"{debuff.debuffType}: Time Left - {debuff.remainingDuration:0.0} s\n";
        }
    }
}

public class ActiveDebuff
{
    public DebuffType debuffType;
    public float damagePerSecond;
    public float remainingDuration;

    public ActiveDebuff(DebuffType debuffType, float damagePerSecond, float duration)
    {
        this.debuffType = debuffType;
        this.damagePerSecond = damagePerSecond;
        this.remainingDuration = duration;
    }
}
