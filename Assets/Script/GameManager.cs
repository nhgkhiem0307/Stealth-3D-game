using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject loseText;
    public Image fadeImage;
    public Slider enemyWatchingSlider;
    public TextMeshProUGUI gameText; //gametext 2
    public float fadeDuration = 1.5f;
    public float maxDetectionPercent;

    bool gameOver = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        enemyWatchingSlider.value = maxDetectionPercent;
        if (maxDetectionPercent > 0)
        {
            enemyWatchingSlider.gameObject.SetActive(true);
            gameText.gameObject.SetActive(true);
        }
        else
        {
            enemyWatchingSlider.gameObject.SetActive(false);
            gameText.gameObject.SetActive(false);
        }

        maxDetectionPercent = 0f; // reset mỗi frame
    }

    public void PlayerCaught()
    {
        if (gameOver) return;//tránh bug gọi hàm nhiều lần
        gameOver = true;

        loseText.SetActive(true);
        StartCoroutine(FadeAndRestart());
    }

    IEnumerator FadeAndRestart()
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void PlayerWin()
    {
        StartCoroutine(FadeAndRestart());
    }
}
