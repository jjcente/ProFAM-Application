using UnityEngine;
using TMPro; // ✅ Use TextMeshPro

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform spawnPoint;
    public Transform counterPoint;
    public Transform exitPoint;
    public TextMeshProUGUI dialogueText; // ✅ FIXED type to TextMeshProUGUI

    private GameObject currentNPC;

    void Start()
    {
        SpawnNextNPC();
    }

    public void SpawnNextNPC()
    {
        if (currentNPC != null) return; // Prevent multiple spawns

        currentNPC = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);

        NPCController npcController = currentNPC.GetComponent<NPCController>();
        if (npcController != null)
        {
            npcController.counterPoint = counterPoint;
            npcController.exitPoint = exitPoint;
            npcController.dialogueText = dialogueText; // ✅ Works now
            npcController.spawner = this;
        }
    }
}
