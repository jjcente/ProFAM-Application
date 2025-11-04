using UnityEngine;
using System;

public class FishQuestionHolder : MonoBehaviour
{
    public FishQuestion question;
    public float pointValue = 1f;
    public event Action<FishQuestionHolder> OnFishDestroyed;

    private void OnDestroy()
    {
        OnFishDestroyed?.Invoke(this);
    }
}
