using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish Setup")]
    public GameObject[] fishPrefabs;           // All small fish prefabs
    public Transform[] spawnPoints;            // Optional spawn points
    public int initialFishCount = 10;          // Initial fish count

    [Header("Dynamic Spawning")]
    public int respawnThreshold = 3;           // When fish count <= this, respawn
    public Vector2Int respawnBatchRange = new Vector2Int(5, 8); // Spawn between 5–8
    public float checkInterval = 2f;           // How often to check fish count

    private FishQuestionDatabase db;
    private readonly List<GameObject> activeFish = new();

    private void Start()
    {
        // Check for manager and database
        if (FishQuestionManager.Instance == null)
        {
            Debug.LogError("FishQuestionManager instance not found!");
            return;
        }

        db = FishQuestionManager.Instance.questionDatabase;
        if (db == null)
        {
            Debug.LogError("FishQuestionDatabase not assigned in FishQuestionManager!");
            return;
        }

        db.LoadQuestions();

        // Spawn initial batch
        SpawnBatch(initialFishCount);

        // Start checking fish count
        StartCoroutine(CheckFishCountRoutine());
    }

    private IEnumerator CheckFishCountRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            // Clean up destroyed fish
            activeFish.RemoveAll(f => f == null);

            if (activeFish.Count <= respawnThreshold)
            {
                int spawnCount = Random.Range(respawnBatchRange.x, respawnBatchRange.y + 1);
                SpawnBatch(spawnCount);
                Debug.Log($"🐠 Spawned new batch ({spawnCount}) because only {activeFish.Count} left.");
            }
        }
    }

private void SpawnBatch(int count)
{
    // Determine how many questions are available
    int availableQuestions = db.QuestionsRemaining(); // we’ll add this function
    if (availableQuestions <= 0)
    {
        Debug.Log("No questions remaining — skipping fish spawn.");
        return;
    }

    // Limit spawn count to available questions
    count = Mathf.Min(count, availableQuestions);

    for (int i = 0; i < count; i++)
    {
        GameObject prefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
        Vector3 pos = (spawnPoints != null && spawnPoints.Length > 0)
            ? spawnPoints[Random.Range(0, spawnPoints.Length)].position
            : new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0);

        GameObject fish = Instantiate(prefab, pos, Quaternion.identity);
        activeFish.Add(fish);

        FishQuestionHolder holder = fish.GetComponent<FishQuestionHolder>();
        if (holder != null)
        {
            FishQuestion q = db.GetRandomQuestion();
            if (q == null)
            {
                Debug.LogWarning("No available questions — skipping assignment.");
                continue;
            }

            holder.question = q;
        }
    }
}

}
