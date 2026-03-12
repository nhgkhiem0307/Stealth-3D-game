using UnityEngine;

public class SetActive : MonoBehaviour
{
    public GameObject gameObject; 

    public void OpenObject()
    {
        gameObject.SetActive(true);
    }

    public void CloseObject()
    {
        gameObject.SetActive(false);
    }
}