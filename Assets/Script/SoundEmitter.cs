using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public static System.Action<Vector3, float> OnSoundEmitted;

    public static void EmitSound(Vector3 position, float radius)
    {
        OnSoundEmitted?.Invoke(position, radius);
    }
}