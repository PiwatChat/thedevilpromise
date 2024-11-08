using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenDoor : MonoBehaviour
{
    public Image fadeImage;
    public GameObject warpPosition;
    public float fadeDuration = 1.5f;
    public Text warpText;
    public bool doorOpen = false;
    
    private GameObject player;
    private Inventory inventory;
    private bool playerInZone = false;
    
    public string keyItemName = "Key";

    private void Start()
    {
        // อาจจะต้องการเพิ่มการค้นหา Inventory ที่นี่
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            inventory = player.GetComponent<Inventory>();
            playerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }

    private void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            if (inventory != null && inventory.items.ContainsKey(keyItemName))
            {
                inventory.UseItem(keyItemName);
                doorOpen = true;
                StartCoroutine(WarpPlayer());
            }
            else if (inventory != null && doorOpen)
            {
                StartCoroutine(WarpPlayer());
            }
            else
            {
                warpText.text = "You must have the key to open this door.";
                StartCoroutine(TextWarpPlayer());
                Debug.Log("คุณต้องมีกุญแจในการเปิดประตูนี้");
            }
        }
    }

    private IEnumerator TextWarpPlayer()
    {
        yield return new WaitForSeconds(3f);
        warpText.text = "";
    }
    
    private IEnumerator WarpPlayer()
    {
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(0, 0, 0, 1);

        while (elapsedTime < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        player.transform.position = warpPosition.transform.position;
        Time.timeScale = 0;
        
        yield return new WaitForSecondsRealtime(2.5f);
        Time.timeScale = 1;
        
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeImage.color = Color.Lerp(targetColor, startColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
