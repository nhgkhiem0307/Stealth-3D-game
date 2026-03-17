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
    public Image Eye;
    public TextMeshProUGUI gameText; //gametext 2
    public float fadeDuration = 1.5f;
    public float maxDetectionPercent;

    public GameObject pausePanel;
    bool isPaused = false;  
    bool gameOver = false;

    public string MenuScene = "MenuScene";
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        Color Eyec = Eye.color;


        enemyWatchingSlider.value = maxDetectionPercent;
        if (maxDetectionPercent > 0)
        {
            enemyWatchingSlider.gameObject.SetActive(true);
            gameText.gameObject.SetActive(true);
            Eyec.a = maxDetectionPercent /3;
            Eye.color = Eyec;
        }
        else
        {
            enemyWatchingSlider.gameObject.SetActive(false);
            gameText.gameObject.SetActive(false);
            Eyec.a = 0; 
            Eye.color = Eyec;

        }

        maxDetectionPercent = 0f; // reset mỗi frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    IEnumerator FadeAndRestart()
    {
        float t = 0f;
        Color fadeImagec = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeImagec.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = fadeImagec;
            yield return null;
        }


        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator FadeAndExit()
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(MenuScene);
    }
    
    
    
    public void PlayerCaught()
    {
        if (gameOver) return;
        gameOver = true;
        loseText.SetActive(true);
        StartCoroutine(FadeAndRestart());
    }
    public void PlayerWin()
    {
        StartCoroutine(FadeAndExit());
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        StartCoroutine(FadeAndRestart());
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(FadeAndExit());
    }
}
