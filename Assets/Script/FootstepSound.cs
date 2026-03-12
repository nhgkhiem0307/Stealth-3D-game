using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Footstep Sounds")]
    public AudioClip walkSound;
    public AudioClip runSound;

    public float walkInterval = 0.5f;
    public float runInterval = 0.3f;

    float timer = 0f;

    public bool isWalking;
    public bool isRunning;

    [Header("Breathing Sounds")]
    public AudioClip calmBreath;
    public AudioClip heavyBreath;

    float breathTimer = 0f;
    float runTimer = 0f;

    void Update()
    {
        // FOOTSTEP
        if (isWalking || isRunning)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                if (isRunning)
                {
                    audioSource.PlayOneShot(runSound);
                    timer = runInterval;
                }
                else
                {
                    audioSource.PlayOneShot(walkSound);
                    timer = walkInterval;
                }
            }
        }
        else
        {
            timer = 0f;
        }

        if (isRunning)
        {
            runTimer += Time.deltaTime;//tgian cahy
        }
        else
        {
            runTimer = 0f;
        }

        // BREATHING
        if (isWalking || isRunning || !isRunning)
        {
            breathTimer -= Time.deltaTime;

            if (breathTimer <= 0f)
            {
                // nếu chạy đủ 1s mới thở dốc
                if (runTimer >= 1f)
                {
                    audioSource.PlayOneShot(heavyBreath);
                    breathTimer = heavyBreath.length;
                }
                else
                {
                    audioSource.PlayOneShot(calmBreath);
                    breathTimer = calmBreath.length;
                }
            }
        }
    }
}