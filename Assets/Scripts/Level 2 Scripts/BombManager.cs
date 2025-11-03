using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using NUnit.Framework;

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

    [Header("Win / Portal")]
    public GameObject portalPrefab;
    public Transform portalSpawnPoint;
    public float timeBeforePlayerMoves = 1f;
    public float playerMoveDuration = 1.2f;
    public string nextSceneName = "Level 3";
    public AudioClip winClip;

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

        //bombs.Remove(b);

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

        ResetLevel();
    }




void OnAllBombsDefused()
{
    Debug.Log("🎉 All bombs defused! Level success.");
    levelOver = true;

    // Fade out background music
    if (AudioManager.Instance != null)
        AudioManager.Instance.FadeOutMusic(1.5f);

    // Play win sound
    if (winClip != null)
        AudioManager.Instance.PlaySFX(winClip);

    // Begin win sequence
    var player = FindFirstObjectByType<PlayerMovementController>();
    if (player != null)
    {
        StartCoroutine(HandlePlayerWinSequence(player));
    }
    else
    {
        Debug.LogWarning("Player not found! Can't run win sequence.");
    }
}

IEnumerator HandlePlayerWinSequence(PlayerMovementController player)
{
    yield return new WaitForSeconds(timeBeforePlayerMoves);

    player.transform.position = player.spawnPoint.position;

    Vector3 exitPos = portalSpawnPoint != null ? portalSpawnPoint.position : player.spawnPoint.position + Vector3.right * 3f;
GameObject portalObj = Instantiate(portalPrefab, exitPos, Quaternion.identity);

    Portal portal = portalObj.GetComponent<Portal>();


    player.ForceFaceDirection(Vector2.right);

    yield return player.StartCoroutine(player.MoveToPortalAndWin(
        portal.transform, playerMoveDuration));

    yield return new WaitForSeconds(0.5f);
        Debug.Log("➡️ Loading next level...");
    
    if (portal != null)
        yield return portal.StartCoroutine(portal.ActivateAndLoadNextScene(0.1f, nextSceneName));
}



    public bool IsTimerRunning()
    {
        return timer > 0f && !levelOver;
    }

public void ResetLevelWithoutTimer()
{
    Debug.Log("Partial Reset (player death)");

    List<Bomb> newBombList = new List<Bomb>();

    foreach (var b in bombs.ToArray())
    {
        if (b == null) continue;

        switch (b.State)
        {
            case Bomb.BombState.Defused:
                // Keep defused bomb as-is (disabled, gray, no collider)
                newBombList.Add(b);
                break;

            case Bomb.BombState.Active:
                // Keep active bomb (player didn’t interact with it yet)
                newBombList.Add(b);
                break;

            case Bomb.BombState.Exploded:
                // Destroy and respawn exploded bomb
                Destroy(b.gameObject);
                Vector3 pos = GetRandomSpawnPosition();
                var newBomb = Instantiate(bombPrefab, pos, Quaternion.identity).GetComponent<Bomb>();
                newBombList.Add(newBomb);
                break;
        }
    }

    bombs = newBombList;

    levelOver = false;
    QuestionDatabase.Instance.ResetQuestions();

    Debug.Log($"After death: {bombs.Count} total bombs (Defused remain, Exploded respawned)");
}


public void ResetLevel()
{
    Debug.Log("Hard Reset");

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
