using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject[] fishPrefabs; // All small fish prefabs
    public Transform[] spawnPoints;   // Optional spawn points
    public int fishCount = 10;       // Number of fish to spawn

    void Start()
    {
        // Ensure FishQuestionManager exists
        if (FishQuestionManager.Instance == null)
        {
            Debug.LogError("FishQuestionManager instance not found!");
            return;
        }

        var db = FishQuestionManager.Instance.questionDatabase;

        // Ensure database exists
        if (db == null)
        {
            Debug.LogError("FishQuestionDatabase not assigned in FishQuestionManager!");
            return;
        }

        // Load hardcoded questions
        db.LoadQuestions();

        // Spawn fishes
        SpawnFishes(db);
    }

    void SpawnFishes(FishQuestionDatabase db)
    {
        for (int i = 0; i < fishCount; i++)
        {
            // Pick a random fish prefab
            GameObject prefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];

            // Determine position
            Vector3 pos;
            if (spawnPoints != null && spawnPoints.Length > 0)
                pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            else
                pos = new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0);

            // Instantiate fish
            GameObject fish = Instantiate(prefab, pos, Quaternion.identity);

            // Assign a random question
            FishQuestionHolder holder = fish.GetComponent<FishQuestionHolder>();
            if (holder != null)
            {
                // Always get a valid question
                if (db.AllQuestionsUsed())
                    db.ResetQuestions();

                holder.question = db.GetRandomQuestion();
            }
        }
    }
}
