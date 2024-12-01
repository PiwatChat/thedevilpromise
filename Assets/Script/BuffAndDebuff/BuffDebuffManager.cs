using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Random = UnityEngine.Random;
using static UnityEngine.Rendering.DebugUI;


public enum EffectType
{
    Buff,
    Debuff
}

public enum BuffType
{
    Heal,           // เพิ่มพลังชีวิต
    Shield,         // เพิ่มโล่หรือการป้องกัน
    Strength,       // เพิ่มพลังโจมตี
    Agility,        // เพิ่มความเร็ว
    Intelligence    // เพิ่มพลังพิเศษหรือสติปัญญา
}

public enum DebuffType
{
    Poison,         // ติดพิษ ลดพลังชีวิตอย่างต่อเนื่อง
    Slow,           // ลดความเร็ว
    Weakness,       // ลดพลังโจมตี
    Blindness       // ลดความแม่นยำในการโจมตี
}

[System.Serializable]
public class BuffDebuff
{
    public EffectType effectType;    // เลือกระหว่าง Buff หรือ Debuff
    public BuffType buffType;        // ประเภทของ Buff เช่น Heal, Strength, Speed ฯลฯ
    public DebuffType debuffType;    // ประเภทของ Debuff เช่น Poison, Slow, Weakness ฯลฯ
    public float value;              // ค่าของ Buff/Debuff เช่น 20% ของการเพิ่มหรือลดพลัง
    public float duration;           // ระยะเวลาของ Buff/Debuff ที่มีผล (หน่วยเป็นวินาที)
    public float cooldown;           // ระยะเวลาคูลดาวน์ของ Buff/Debuff หลังใช้งาน (หน่วยเป็นวินาที)
    public float probability = 1.0f; // โอกาสในการติด Debuff (ค่าเริ่มต้นคือ 100% หรือ 1.0)
    public KeyCode keyCode;          // ปุ่มคีย์บอร์ดที่ใช้เปิดใช้งาน Buff (ใช้กับ Buff เท่านั้น)

    public BuffDebuff(EffectType effectType = EffectType.Buff, BuffType buffType = BuffType.Heal, DebuffType debuffType = DebuffType.Poison, float value = 0, float duration = 0, float cooldown = 0, float probability = 1.0f, KeyCode keyCode = KeyCode.None)
    {
        this.effectType = effectType;
        this.buffType = buffType;
        this.debuffType = debuffType;
        this.value = value;
        this.duration = duration;
        this.cooldown = cooldown;
        this.probability = probability;
        this.keyCode = keyCode;
    }
}
public class BuffDebuffManager : MonoBehaviour
{
    
    public enum EntityType { Player, Enemy }
    public EntityType entityType;

    public List<BuffDebuff> buffs;    // List สำหรับเก็บ Buff
    public List<BuffDebuff> debuffs;  // List สำหรับเก็บ Debuff
    public Text buffText;             // Text สำหรับแสดงสถานะ Buff
    public Text debuffText;           // Text สำหรับแสดงสถานะ Debuff

    public GameObject StrengthBuffEffect;
    public GameObject AgilityBuffEffect;
    public GameObject IntelligenceBuffEffect;

    public GameObject poisonDebuffEffect;
    public GameObject slowDebuffEffect;
    public GameObject weaknessDebuffEffect;



    private Status status;
    private PlayerManager playerManager;
    private float originalStrength; // เก็บค่า Strength ดั้งเดิม
   
    private Color originalColor;      // เก็บสีเดิมของ Player
    private float originalAgility; // เก็บค่า Agility ต้นฉบับ
    private float originalIntelligence; // เก็บค่า Intelligence ต้นฉบับ

    private Dictionary<BuffType, float> buffCooldownTimers = new Dictionary<BuffType, float>();
    private Dictionary<BuffType, float> buffDurationTimers = new Dictionary<BuffType, float>(); // เวลาที่ Buff จะหมด
    private Dictionary<DebuffType, float> debuffCooldownTimers = new Dictionary<DebuffType, float>();
    private List<ActiveDebuff> activeDebuffs = new List<ActiveDebuff>();

    private bool hasActiveBuff = false; // ตรวจสอบว่า Buff กำลังทำงานอยู่หรือไม่

    void Start()
    {
        status = GetComponent<Status>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
       
        //ApplyBuff(BuffType.Strength);

       
        if (status != null)
        {
            originalStrength = status.strength; // เก็บค่า Strength ดั้งเดิม
            originalAgility = status.agility; // เก็บค่า Agility ต้นฉบับ
            originalIntelligence = status.intelligence; // เก็บค่า Intelligence ต้นฉบับ
        }
       
        if (player != null)
        {
            playerManager = player.GetComponent<PlayerManager>();
        }

    }

    void Update()
    {
        foreach (var buff in buffs)
        {

            if (Input.GetKeyDown(buff.keyCode)) // ตรวจสอบ KeyCode ที่ตั้งไว้ใน Inspector
            {
                Debug.Log($"Key {buff.keyCode} pressed for Buff {buff.buffType}");
                ApplyBuff(buff.buffType);
            }


            // ไม่ต้องตรวจสอบ HasValue อีกต่อไป
            if (buffCooldownTimers.ContainsKey(buff.buffType))
            {
                float cooldownTime = buffCooldownTimers[buff.buffType];
                float durationTime = buffDurationTimers.ContainsKey(buff.buffType) ? buffDurationTimers[buff.buffType] : 0;

                buffText.text += $"{buff.buffType}: ";
                if (durationTime > 0)
                {
                    buffText.text += $"Time Left - {durationTime:0.0} s ";
                }
                if (cooldownTime > 0)
                {
                    buffText.text += $"(Cooldown - {cooldownTime:0.0} s)";
                }
                buffText.text += "\n";

               
            }
        }

        // อัปเดตคูลดาวน์ของ Buff และ Debuff
        UpdateCooldownTimers();

        // อัปเดตการทำงานของ Buff และ Debuff ที่กำลังทำงานอยู่
        UpdateActiveBuffs();
        UpdateActiveDebuffs();

        // อัปเดตข้อความแสดงสถานะ Buff และ Debuff
        UpdateStatusText();
    }

    public void ApplyBuff(BuffType type)
    {
        foreach (var buff in buffs)
        {
            if (buff.buffType == type && CanUseBuff(type))
            {
                switch (buff.buffType)
                {
                    case BuffType.Heal:
                        status.health = Mathf.Min(status.health + status.maxHealth * buff.value / 100, status.maxHealth);
                        break;
                    case BuffType.Shield:
                        // เพิ่มการป้องกัน
                        break;
                    case BuffType.Strength:
                        status.strength += (int)buff.value;
                        // เริ่ม Coroutine เพื่อลดค่า Strength กลับหลังจาก Buff หมดเวลา
                        StartCoroutine(RemoveBuffAfterDuration(buff));
                        Instantiate(StrengthBuffEffect, transform.position, Quaternion.identity, transform);
                        break;
                    case BuffType.Agility:
                        status.agility += (int)buff.value;
                        StartCoroutine(RemoveBuffAfterDuration(buff));
                        Instantiate(AgilityBuffEffect, transform.position, Quaternion.identity, transform);
                        break;
                    case BuffType.Intelligence:
                        status.intelligence += (int)buff.value;
                        StartCoroutine(RemoveBuffAfterDuration(buff));
                        Instantiate(IntelligenceBuffEffect, transform.position, Quaternion.identity, transform);
                        break;
                }
                buffCooldownTimers[type] = buff.cooldown;
                buffDurationTimers[type] = buff.duration;  // ตั้งเวลาของ Buff

                hasActiveBuff = true;

                Debug.Log($"Buff {type} applied");
            }
        }
    }

   public void ApplyDebuff(DebuffType type)
{
    foreach (var debuff in debuffs)
    {
        if (debuff.debuffType == type && CanUseDebuff(type))
        {
            // ตรวจสอบโอกาสในการติด debuff
            float randomValue = Random.Range(0f, 1f); // สุ่มค่าระหว่าง 0 ถึง 1
            if (randomValue <= debuff.probability)   // หากค่านี้อยู่ในช่วงโอกาส จะติด debuff
            {
                switch (debuff.debuffType)
                {
                    case DebuffType.Poison:
                        activeDebuffs.Add(new ActiveDebuff(DebuffType.Poison, debuff.value, debuff.duration));
                        Instantiate(poisonDebuffEffect, transform.position, Quaternion.identity, transform);
                        break;
                    case DebuffType.Weakness:
                        status.strength = Mathf.Max(0, status.strength - (int)debuff.value);
                        activeDebuffs.Add(new ActiveDebuff(DebuffType.Weakness, 0, debuff.duration));
                        Instantiate(weaknessDebuffEffect, transform.position, Quaternion.identity, transform);
                        break;
                    case DebuffType.Slow:
                        status.agility = Mathf.Max(0, status.agility - (int)debuff.value);
                        activeDebuffs.Add(new ActiveDebuff(DebuffType.Slow, 0, debuff.duration));
                        Instantiate(slowDebuffEffect, transform.position, Quaternion.identity, transform);
                        break;
                }
                debuffCooldownTimers[type] = debuff.cooldown;

                Debug.Log($"Debuff {debuff.debuffType} applied to Player");
            }
        }
    }
    }

    private bool CanUseBuff(BuffType type)
    {
        return !buffCooldownTimers.ContainsKey(type) || buffCooldownTimers[type] <= 0;
    }

    private bool CanUseDebuff(DebuffType type)
    {
        return !debuffCooldownTimers.ContainsKey(type) || debuffCooldownTimers[type] <= 0;
    }

    private void UpdateCooldownTimers()
    {
        // ลดค่า Timer ของ Buff
        List<BuffType> buffKeys = new List<BuffType>(buffCooldownTimers.Keys); // เก็บคีย์ของ Buff แยกออกมา
        foreach (var key in buffKeys)
        {
            if (buffCooldownTimers[key] > 0)
                buffCooldownTimers[key] -= Time.deltaTime;
        }

        // ลดค่า Timer ของ Buff Duration
        List<BuffType> durationKeys = new List<BuffType>(buffDurationTimers.Keys); // เก็บคีย์ของ Buff Duration แยกออกมา
        foreach (var key in durationKeys)
        {
            if (buffDurationTimers[key] > 0)
                buffDurationTimers[key] -= Time.deltaTime;
        }

        // ลดค่า Timer ของ Debuff
        List<DebuffType> debuffKeys = new List<DebuffType>(debuffCooldownTimers.Keys); // เก็บคีย์ของ Debuff แยกออกมา
        foreach (var key in debuffKeys)
        {
            if (debuffCooldownTimers[key] > 0)
                debuffCooldownTimers[key] -= Time.deltaTime;
        }
    }

    private void UpdateActiveBuffs()
    {
        hasActiveBuff = false;  // รีเซ็ตค่านี้สำหรับตรวจสอบในรอบถัดไป
    }

    private void UpdateActiveDebuffs()
    {
        bool hasActiveDebuff = false;

        for (int i = activeDebuffs.Count - 1; i >= 0; i--)
        {
            ActiveDebuff activeDebuff = activeDebuffs[i];
            activeDebuff.remainingDuration -= Time.deltaTime;

            if (activeDebuff.debuffType == DebuffType.Poison)
            {
                ApplyPoisonDamage(activeDebuff.damagePerSecond);
            }

            // ลบ debuff ออกจาก list เมื่อหมดเวลา
            if (activeDebuff.remainingDuration <= 0)
            {
                // รีเซ็ตค่ากลับสู่ค่าเดิม
                if (activeDebuff.debuffType == DebuffType.Weakness)
                {
                    status.strength = (int)originalStrength;
                    Debug.Log($"Debuff {DebuffType.Weakness} expired. Strength reset to original value.");
                }
                else if (activeDebuff.debuffType == DebuffType.Slow)
                {
                    status.agility = (int)originalAgility;
                    Debug.Log($"Debuff {DebuffType.Slow} expired. Agility reset to original value.");
                }
                activeDebuffs.RemoveAt(i);
            }
            else
            {
                hasActiveDebuff = true;  // ยังมี debuff ที่ทำงานอยู่
            }
        }

    }

    private float poisonTimer = 0f;

    private void ApplyPoisonDamage(float damagePerSecond)
    {
        poisonTimer += Time.deltaTime;

        // เมื่อเวลาผ่านไปครบ 1 วินาที ลดเลือดตาม damagePerSecond
        if (poisonTimer >= 1f)
        {
            status.health -= damagePerSecond;
            Debug.Log("Applying poison damage: -" + damagePerSecond + " HP");
            poisonTimer = 0f;

            if (status.health <= 0)
            {
               
                Debug.Log("Character is dead.");
                RemoveAllBuffsAndDebuffs(); // เรียกใช้ฟังก์ชันลบ Buff ทั้งหมด
               /* if (playerManager != null)
                {
                    playerManager.Die();   //เรียกใช้เมทอด Die เมื่อชน
                }*/


            }
        }
    }

  

    private void UpdateStatusText()
    {
        // อัปเดตข้อความ Buff
        buffText.text = "Active Buffs:\n";
        foreach (var buff in buffs)
        {
            if (buffCooldownTimers.ContainsKey(buff.buffType))
            {
                float cooldownTime = buffCooldownTimers[buff.buffType];
                float durationTime = buffDurationTimers.ContainsKey(buff.buffType) ? buffDurationTimers[buff.buffType] : 0;

                buffText.text += $"{buff.buffType}: ";
                if (durationTime > 0)
                {
                    buffText.text += $"Time Left - {durationTime:0.0} s ";
                }
                if (cooldownTime > 0)
                {
                    buffText.text += $"(Cooldown - {cooldownTime:0.0} s)";
                }
                buffText.text += "\n";
            }
        }




        // อัปเดตข้อความ Debuff
        debuffText.text = "Active Debuffs:\n";
        foreach (var debuff in activeDebuffs)
        {
            debuffText.text += $"{debuff.debuffType}: Time Left - {debuff.remainingDuration:0.0} s\n";
        }
    }

    private class ActiveDebuff
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

    private IEnumerator RemoveBuffAfterDuration(BuffDebuff buff)
    {
        yield return new WaitForSeconds(buff.duration); // รอจนกระทั่ง Buff หมดเวลา

        if (buff.buffType == BuffType.Strength)
        {
            // คืนค่า Strength กลับเป็นค่าดั้งเดิม
            status.strength = (int)originalStrength;
            Debug.Log($"Buff {buff.buffType} expired. Strength reset to original value.");
        }
        if(buff.buffType == BuffType.Agility)
        {
            status.agility = (int)originalAgility;
            Debug.Log($"Buff {buff.buffType} expired. Agility  reset to original value.");
        }
        if (buff.buffType == BuffType.Intelligence)
        {
            status.intelligence = (int)originalIntelligence;
            Debug.Log($"Buff {buff.buffType} expired. Intelligence  reset to original value.");
        }
        
    }

    // ฟังก์ชันสำหรับลบ Buff และ Debuff ทั้งหมด
    private void RemoveAllBuffsAndDebuffs()
    {
        // ลบและรีเซ็ต Buff
        foreach (var buff in buffs)
        {
            switch (buff.buffType)
            {
                case BuffType.Strength:
                    status.strength = (int)originalStrength;            
                    break;
                case BuffType.Agility:
                    status.agility = (int)originalAgility;            
                    break;
                case BuffType.Intelligence:
                    status.intelligence = (int)originalIntelligence;                    
                    break;
                    // เพิ่มกรณีอื่น ๆ ตาม Buff ที่มี
            }
        }

        // เคลียร์ลิสต์ Buff และ Timer ต่าง ๆ
        buffs.Clear();
        buffCooldownTimers.Clear();
        buffDurationTimers.Clear();

        // ลบและรีเซ็ต Debuff
        foreach (var debuff in activeDebuffs)
        {
            switch (debuff.debuffType)
            {
                case DebuffType.Weakness:
                    status.strength = (int)originalStrength;                  
                    break;
                case DebuffType.Slow:
                    status.agility = (int)originalAgility;                  
                    break;
                
                    // เพิ่มกรณีอื่น ๆ ตาม Debuff ที่มี
            }
        }

        // เคลียร์ลิสต์ Debuff และ Timer ต่าง ๆ
        activeDebuffs.Clear();
        debuffCooldownTimers.Clear();

        Debug.Log("All buffs and debuffs removed due to player death.");


      
    }
}



[CustomPropertyDrawer(typeof(BuffDebuff))]
public class BuffDebuffEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // กำหนดตำแหน่งเริ่มต้น
        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // ดึงค่าต่าง ๆ จาก BuffDebuff
        SerializedProperty effectTypeProp = property.FindPropertyRelative("effectType");
        SerializedProperty buffTypeProp = property.FindPropertyRelative("buffType");
        SerializedProperty debuffTypeProp = property.FindPropertyRelative("debuffType");
        SerializedProperty valueProp = property.FindPropertyRelative("value");
        SerializedProperty durationProp = property.FindPropertyRelative("duration");
        SerializedProperty cooldownProp = property.FindPropertyRelative("cooldown");
        SerializedProperty probabilityProp = property.FindPropertyRelative("probability");
        SerializedProperty keyCodeProp = property.FindPropertyRelative("keyCode");

        // แสดงฟิลด์สำหรับ Effect Type
        EditorGUI.PropertyField(singleFieldRect, effectTypeProp);
        singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // แสดงฟิลด์ตาม Effect Type ที่เลือก
        if (effectTypeProp.enumValueIndex == (int)EffectType.Buff)
        {
            // แสดงฟิลด์สำหรับ Buff
            EditorGUI.PropertyField(singleFieldRect, buffTypeProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(singleFieldRect, valueProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(singleFieldRect, durationProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(singleFieldRect, cooldownProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(singleFieldRect, probabilityProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(singleFieldRect, keyCodeProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        else if (effectTypeProp.enumValueIndex == (int)EffectType.Debuff)
        {
            // แสดงฟิลด์สำหรับ Debuff
            EditorGUI.PropertyField(singleFieldRect, debuffTypeProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(singleFieldRect, valueProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(singleFieldRect, durationProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(singleFieldRect, cooldownProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(singleFieldRect, probabilityProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        EditorGUI.EndProperty();
    }

    // กำหนดความสูงของ PropertyDrawer
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lineCount = 2; // สำหรับ EffectType และ Property Field

        SerializedProperty effectTypeProp = property.FindPropertyRelative("effectType");

        if (effectTypeProp.enumValueIndex == (int)EffectType.Buff)
        {
            lineCount += 6; // Buff มี 6 ฟิลด์
        }
        else if (effectTypeProp.enumValueIndex == (int)EffectType.Debuff)
        {
            lineCount += 5; // Debuff มี 5 ฟิลด์
        }

        return lineCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
    }
}

