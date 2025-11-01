using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BombManager : MonoBehaviour
{
    public static BombManager Instance { get; private set; }

    [Header("Bomb spawning")]
    public GameObject bombPrefab;
    public int bombCount = 5;
    public TilemapHelper tilemapHelper;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;

    [Header("Shared timer")]
    public float sharedTime = 60f;
    public TMP_Text sharedTimerText;

    private float timer;
    private List<Bomb> bombs = new List<Bomb>();
    private int defused = 0;
    private bool levelOver = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timer = sharedTime;
        SpawnBombs();
        UpdateTimerUI();
    }

    void Update()
    {
        if (levelOver) return;

        timer -= Time.deltaTime;
        UpdateTimerUI();

        if (timer <= 0f)
        {
            levelOver = true;
            OnTimerExpired();
        }
    }

    void UpdateTimerUI()
{
    if (!sharedTimerText) return;

    int minutes = Mathf.FloorToInt(timer / 60f);
    int seconds = Mathf.FloorToInt(timer % 60f);

    sharedTimerText.text = string.Format("{0}:{1:00}", minutes, seconds);
}

    void SpawnBombs()
    {
        for (int i = 0; i < bombCount; i++)
        {
            Vector3 pos = GetRandomSpawnPosition();
            var go = Instantiate(bombPrefab, pos, Quaternion.identity);
            var bomb = go.GetComponent<Bomb>();
            if (bomb != null) bombs.Add(bomb);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (tilemapHelper != null)
        {
            Vector3Int cell = tilemapHelper.GetRandomWalkableCell();
            return tilemapHelper.cellToWorldCenter(cell);
        }

        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector3(x, y, 0f);
    }

    public void OnBombDefused(Bomb b)
    {
        defused++;
        if (defused >= bombCount)
        {
            levelOver = true;
            OnAllBombsDefused();
        }
    }

    public void OnBombExploded(Bomb b)
    {
        Debug.Log($"💥 Bomb exploded: {b.name}");

        bombs.Remove(b);

        Debug.Log("💀 A bomb exploded! Mission failed!");

    }

void OnTimerExpired()
{
    Debug.Log("⏰ Time expired! Restarting full round...");

    foreach (var b in bombs.ToArray())
        b.Explode();

    // Reset timer
    timer = sharedTime;
    levelOver = false;

    // Respawn player and bombs
    var player = FindFirstObjectByType<PlayerMovementController>();
    if (player != null)
        player.Respawn();

    ResetLevelWithoutTimer();
}




    void OnAllBombsDefused()
    {
        Debug.Log("All bombs defused! Level success.");
    }

    public bool IsTimerRunning()
    {
        return timer > 0f && !levelOver;
    }

public void ResetLevelWithoutTimer()
{
    Debug.Log("🔄 Soft resetting level (timer unaffected)");

    // Clear existing bombs
    foreach (var b in bombs.ToArray())
    {
        if (b != null)
            Destroy(b.gameObject);
    }
    bombs.Clear();

    defused = 0;
    levelOver = false;
    
    QuestionDatabase.Instance.ResetQuestions();


    // Respawn bombs
    SpawnBombs();
}
}
