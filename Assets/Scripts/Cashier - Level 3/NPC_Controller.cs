using UnityEngine;
using TMPro;
using System.Collections;

public class NPCController : MonoBehaviour
{
    public Transform counterPoint;
    public Transform exitPoint;
    public float moveSpeed = 2f;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueCanvas; // ✅ Add reference to the entire dialogue box canvas
    public NPCSpawner spawner;

    private bool movingToCounter = true;
    private bool interacted = false;
    private bool isInteracting = false;

    private string[] dialogues = new string[]
    {
        "Hello! I'd like to buy some items.",
        "Hey there! Do you have any discounts today?",
        "Can I get a bag for this, please?",
        "I think I left my wallet in the car... oops.",
        "That'll be all for today, thank you!"
    };

    void Start()
    {
        // 🔹 Make sure the dialogue box starts hidden
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);
    }

    void Update()
    {
        if (!isInteracting)
        {
            if (movingToCounter && !interacted)
            {
                MoveTo(counterPoint.position);

                // When NPC reaches the counter
                if (Vector2.Distance(transform.position, counterPoint.position) < 0.05f)
                {
                    StartCoroutine(Interact());
                }
            }
            else if (interacted)
            {
                MoveTo(exitPoint.position);

                // When NPC reaches exit
                if (Vector2.Distance(transform.position, exitPoint.position) < 0.05f)
                {
                    if (spawner != null)
                        spawner.SpawnNextNPC();

                    Destroy(gameObject);
                }
            }
        }
    }

    void MoveTo(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    IEnumerator Interact()
    {
        isInteracting = true;
        movingToCounter = false;

        // 🎬 Pick random dialogue
        string selectedDialogue = dialogues[Random.Range(0, dialogues.Length)];

        // 🗨️ Show dialogue box
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(true);
        }

        if (dialogueText != null)
        {
            dialogueText.text = selectedDialogue;
        }

        // ⏳ Wait 3 seconds
        yield return new WaitForSeconds(3f);

        // 💬 Hide dialogue box
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
        }

        interacted = true;
        isInteracting = false;
    }
}
