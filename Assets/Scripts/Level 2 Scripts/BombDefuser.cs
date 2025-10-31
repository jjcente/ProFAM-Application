using UnityEngine;

public class BombDefuser : MonoBehaviour
{
     public float interactRange = 1.2f;
    public LayerMask bombLayer;
    private Bomb nearestBomb;

    void Update()
    {
        FindNearestBomb();

        // Keyboard interact (editor)
        if (Input.GetKeyDown(KeyCode.E) && nearestBomb != null)
            StartQuestionFor(nearestBomb);

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
        if (nearestBomb != null) StartQuestionFor(nearestBomb);
    }

    void StartQuestionFor(Bomb b)
    {
        // Example hardcoded question — replace with your question source
        string q = "What is 9 + 13?";
        string[] answers = new string[] { "22","21","20","23" };
        int correct = 0;
        QuestionManager.Instance.AskQuestion(b, q, answers, correct);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
