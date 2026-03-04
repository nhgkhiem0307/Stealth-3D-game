using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExitDoor : MonoBehaviour
{
    public float openTime = 3f;
    public Slider progressBar;
    public TextMeshProUGUI gameText;

    float holdTimer = 0f;
    bool playerInRange = false;
    PlayerMovement player;

    void Start()
    {
        gameText.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
        progressBar.value = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerMovement p = other.GetComponent<PlayerMovement>();

        if (p != null)
        {
            player = p;
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            playerInRange = false;
            holdTimer = 0f;
            progressBar.value = 0f;
            progressBar.gameObject.SetActive(false);
            gameText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!playerInRange) return;

        bool hasKey =
            player != null &&
            player.heldObject != null &&
            player.heldObject.GetComponent<KeyItem>() != null;

        if (!hasKey)
        {
            gameText.gameObject.SetActive(true);
            gameText.text = "A key is missing";
            return;
        }

        gameText.gameObject.SetActive(true);
        gameText.text = "Hold E to exit";
        progressBar.gameObject.SetActive(true);

        if (Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;
            progressBar.value = holdTimer / openTime;

            if (holdTimer >= openTime)
            {
                GameManager.instance.PlayerWin();
            }
        }
        else
        {
            holdTimer = 0f;
            progressBar.value = 0f;
        }
    }
}
