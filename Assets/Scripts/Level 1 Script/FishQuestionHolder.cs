using UnityEngine;

public class FishQuestionHolder : MonoBehaviour
{

     public float pointValue = 1f;
    public FishQuestion question;

      public void IncreaseValue()
    {
        pointValue += 0.5f;
    }
}
