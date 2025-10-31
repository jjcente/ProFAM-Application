using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BombManager : MonoBehaviour
{
    public static BombManager Instance { get; private set; }

    [Header("Bomb spawning")]
    public GameObject bombPrefab;
    public int bombCount = 5;
    public TilemapHelper tilemapHelper; // optional helper to pick walkable cells (see below)
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
        if (sharedTimerText) sharedTimerText.text = Mathf.CeilToInt(timer).ToString();
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
        // If you have a TilemapHelper that returns a walkable cell center, prefer that.
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
        bombs.Remove(b);
        if (defused >= bombCount)
        {
            levelOver = true;
            OnAllBombsDefused();
        }
    }

    void OnTimerExpired()
    {
        // time up -> fail
        Debug.Log("Time expired! Level failed.");
        // Option: trigger explode on remaining bombs
        foreach (var b in bombs.ToArray())
            b.Explode();
        // restart or show fail UI (your choice)
    }

    void OnAllBombsDefused()
    {
        Debug.Log("All bombs defused! Level success.");
        // show success UI / progress to next level
    }
}
