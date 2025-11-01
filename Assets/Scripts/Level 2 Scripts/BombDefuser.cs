using UnityEngine;

public class BombDefuser : MonoBehaviour
{
    public float interactRange = 1.2f;
    public LayerMask bombLayer;
    private Bomb nearestBomb;

    public AudioClip defuseSfx;

    void Update()
    {
        FindNearestBomb();

        // Keyboard interact (editor)
        if (Input.GetKeyDown(KeyCode.E) && nearestBomb != null)
        {
            Debug.Log("E key pressed — attempting to start defuse question!");
            StartQuestionFor(nearestBomb);
        }

        // Optionally hook a UI Interact button for mobile to call StartInteraction() from OnClick
    }

    void FindNearestBomb()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, bombLayer);
        nearestBomb = null;
        float best = float.MaxValue;
        foreach (var c in hits)
        {
            float d = Vector2.SqrMagnitude((Vector2)c.transform.position - (Vector2)transform.position);
            if (d < best)
            {
                best = d;
                nearestBomb = c.GetComponent<Bomb>();
            }
        }
    }

    public void StartInteraction() // hook this to an on-screen Interact button
    {
        if (nearestBomb != null)
        {
            Debug.Log("UI Interact button pressed — starting question!");
            AudioManager.Instance.PlaySFX(defuseSfx);
            StartQuestionFor(nearestBomb);
        }
    }


    void StartQuestionFor(Bomb b)
    {
       if (QuestionDatabase.Instance.AllQuestionsUsed())
        QuestionDatabase.Instance.ResetQuestions(); // Reset so questions can repeat

    Question q = QuestionDatabase.Instance.GetRandomQuestion();
    if (q != null)
        QuestionManager.Instance.AskQuestion(b, q);
    else
        Debug.LogWarning("No questions available even after reset!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
