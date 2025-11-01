using UnityEngine;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    public List<GameObject> npcs;     // All NPC prefabs or scene objects
    public Transform spawnPoint;      // Where each NPC will appear
    private int currentIndex = 0;
    private GameObject activeNPC;

    void Start()
    {
        // Deactivate all NPCs at start
        foreach (var npc in npcs)
        {
            npc.SetActive(false);
        }

        // 🔀 Randomize NPC order
        ShuffleNPCs();

        SpawnNextNPC(); // Start with the first one
    }

    public void SpawnNextNPC()
    {
        // If there’s still another NPC to spawn
        if (currentIndex < npcs.Count)
        {
            GameObject npc = npcs[currentIndex];
            npc.transform.position = spawnPoint.position; // Move to spawn point
            npc.SetActive(true);

            // Tell the NPC who the manager is
            NPCController controller = npc.GetComponent<NPCController>();
            if (controller != null)
            {
                controller.manager = this;
            }

            activeNPC = npc;
            currentIndex++;
        }
        else
        {
            Debug.Log("✅ All NPCs finished!");
        }
    }

    // 🔁 Function to shuffle NPC list
    private void ShuffleNPCs()
    {
        for (int i = 0; i < npcs.Count; i++)
        {
            GameObject temp = npcs[i];
            int randomIndex = Random.Range(i, npcs.Count);
            npcs[i] = npcs[randomIndex];
            npcs[randomIndex] = temp;
        }
    }
}
