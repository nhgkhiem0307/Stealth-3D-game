using UnityEngine;

public class SetActiveByKeycode : MonoBehaviour
{
    public GameObject targetObject;
    public KeyCode key = KeyCode.Escape;

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}