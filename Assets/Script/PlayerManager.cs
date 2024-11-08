using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public float soundCombatCooldown;
    public float currentGroundSoundCount;
    public Slider healthSlider;
    public TMP_Text damageText;
    public GameObject popUpDamagePrefab;
    public Animator animator;
    public AudioClip forestSounds;
    public AudioClip villageSounds;
    public AudioClip mansionSounds;
    public AudioClip fightingSounds;
    public AudioSource audioSource;
    public Vector3 savedPosition;
    public GameObject UIRespawn;
    public float currentHealth;
    
    private PlayerMovement player;
    private Status playerStatus;
    private Inventory inventory;
    private Item itemToCollect;
    private bool isNotHit = false;
    private float groundSoundCount = 1f;
    private bool isInCombat = false;
    
    void Start()
    {
        inventory = GetComponent<Inventory>();
        player = GetComponent<PlayerMovement>();
        playerStatus = GetComponent<Status>();
        healthSlider.maxValue = playerStatus.maxHealth;
        healthSlider.value = playerStatus.health;
        currentHealth = playerStatus.health;
        PlayBackgroundSound();
    }
    
    void Update()
    {
        healthSlider.value = playerStatus.health;
        
        if (Input.GetKeyDown(KeyCode.E) && itemToCollect != null)
        {
            inventory.AddItem(itemToCollect);
            if (inventory.itemPicked)
            {
                Destroy(itemToCollect.gameObject);
                itemToCollect = null;
            }
        }
    }
    
    public void TakeDamage(float amount, Vector2 knockbackDirection)
    {
        if (player.isDashing || isNotHit)
        {
            return;
        }
        
        player.audioSourceCombat.PlayOneShot(player.takeDamageSound);
        
        playerStatus.health -= amount;
        currentHealth = playerStatus.health;
        StartCoroutine(SoundCombat());
        StartCoroutine(IsHit());
        
        if (player.isHeavyAttacking == false)
        {
            Camera.main.GetComponent<CameraFollow>().Shake();
            animator.SetTrigger("hitPlayer");
        }
        player.Knockback(knockbackDirection);
        damageText.text = "-"+amount.ToString();
        
        GameObject damagePopUp = Instantiate(popUpDamagePrefab, transform.position, Quaternion.identity);
        
        damagePopUp.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, -0.5f);
        
        playerStatus.health = Mathf.Clamp(playerStatus.health, 0, playerStatus.maxHealth);
        
        if (playerStatus.health <= 0)
        {
            Die();
        }
        
    }

    private IEnumerator IsHit()
    {
        isNotHit = true;
        yield return new WaitForSeconds(1.5f);
        isNotHit = false;
    }


    private void Die()
    {
        StartCoroutine(DieTo());
        Debug.Log("Player has died!");
    }

    private IEnumerator DieTo()
    {
        animator.SetBool("isDeath", true);
        player.enabled = false;
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0;
        UIRespawn.SetActive(true);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            itemToCollect = item;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            itemToCollect = null;
        }
    }
    private IEnumerator SoundCombat()
    {
        StartCombat();
    
        float timer = soundCombatCooldown;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
    
        StopCombat();
    }
    
    private void StartCombat()
    {
        if (!isInCombat)
        {
            isInCombat = true;
            
            if (audioSource.clip != fightingSounds)
            {
                audioSource.Stop();
                audioSource.clip = fightingSounds;
                audioSource.Play();
            }
        }
    }
    
    private void StopCombat()
    {
        if (isInCombat)
        {
            isInCombat = false;
            audioSource.Stop();
            PlayBackgroundSound();
        }
    }
    
    public void PlayBackgroundSound()
    {
        groundSoundCount = currentGroundSoundCount;
        
        switch (groundSoundCount)
        {
            case 1:
                audioSource.clip = forestSounds;
                audioSource.loop = true;
                audioSource.Play();
                break;
            case 2:
                audioSource.Stop();
                audioSource.clip = villageSounds;
                audioSource.loop = true;
                audioSource.Play();
                break;
            case 3:
                audioSource.clip = mansionSounds;
                audioSource.loop = true;
                audioSource.Play();
                break;
        }
    }
    
    public void SavePosition(Vector3 position)
    {
        savedPosition = position;
        PlayerPrefs.SetFloat("SavedPosX", position.x);
        PlayerPrefs.SetFloat("SavedPosY", position.y);
        PlayerPrefs.SetFloat("SavedPosZ", position.z);
        PlayerPrefs.Save();
    }
    
    public void LoadPosition()
    {
        UIRespawn.SetActive(false);
        animator.SetBool("isDeath", false);
        player.enabled = true;
        Time.timeScale = 1;
        playerStatus.ResetHealth();
        float x = PlayerPrefs.GetFloat("SavedPosX", transform.position.x);
        float y = PlayerPrefs.GetFloat("SavedPosY", transform.position.y);
        float z = PlayerPrefs.GetFloat("SavedPosZ", transform.position.z);
        savedPosition = new Vector3(x, y, z);
        transform.position = savedPosition;
    }
}
