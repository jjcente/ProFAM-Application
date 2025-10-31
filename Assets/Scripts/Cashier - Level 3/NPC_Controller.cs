using UnityEngine;
using TMPro;
using System.Collections;

public class NPCController : MonoBehaviour
{
    public Transform counterPoint;
    public Transform exitPoint;
    public float moveSpeed = 2f;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueCanvas;
    public NPCManager manager;

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

                if (Vector2.Distance(transform.position, counterPoint.position) < 0.05f)
                {
                    StartCoroutine(Interact());
                }
            }
            else if (interacted)
            {
                MoveTo(exitPoint.position);

                if (Vector2.Distance(transform.position, exitPoint.position) < 0.05f)
                {
                    // When NPC reaches exit, spawn next one and remove self
                    if (manager != null)
                        manager.SpawnNextNPC();

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

        string selectedDialogue = dialogues[Random.Range(0, dialogues.Length)];

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = selectedDialogue;

        yield return new WaitForSeconds(3f);

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        interacted = true;
        isInteracting = false;
    }
}
