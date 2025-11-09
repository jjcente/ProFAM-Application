using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

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

    private BombDefusedCounter bombDefusedCounter;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timer = sharedTime;
        SpawnBombs();
        UpdateTimerUI();

        bombDefusedCounter = FindFirstObjectByType<BombDefusedCounter>();
        if (bombDefusedCounter != null)
        {
            bombDefusedCounter.Initialize(bombCount);
        }
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

        // ✅ Update UI Counter
        if (bombDefusedCounter != null)
            bombDefusedCounter.OnBombDefused(defused);

        if (defused >= bombCount)
        {
            levelOver = true;
            OnAllBombsDefused();
        }
    }

    public void OnBombExploded(Bomb b)
    {
        Debug.Log($"💥 Bomb exploded: {b.name}");
        Debug.Log("💀 A bomb exploded! Mission failed!");
    }

    void OnTimerExpired()
    {
        Debug.Log("⏰ Time expired! Restarting full round...");

        foreach (var b in bombs.ToArray())
            b.Explode();

        timer = sharedTime;
        levelOver = false;

        var player = FindFirstObjectByType<PlayerMovementController>();
        if (player != null)
            player.Respawn();

        ResetLevel();
    }

    void OnAllBombsDefused()
    {
        Debug.Log("🎉 All bombs defused! Level success.");
        levelOver = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutMusic(1.5f);

        if (winClip != null)
            AudioManager.Instance.PlaySFX(winClip);

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

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            LoadingScreenManager.LoadSceneByName(nextSceneName);
        }
        else
        {
            Debug.LogWarning("⚠️ No next scene name set in BombManager!");
        }
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
                    newBombList.Add(b);
                    break;

                case Bomb.BombState.Active:
                    newBombList.Add(b);
                    break;

                case Bomb.BombState.Exploded:
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

        foreach (var b in bombs.ToArray())
        {
            if (b != null)
                Destroy(b.gameObject);
        }
        bombs.Clear();

        defused = 0;
        levelOver = false;

        QuestionDatabase.Instance.ResetQuestions();
        SpawnBombs();

        // Reset counter UI
        if (bombDefusedCounter != null)
            bombDefusedCounter.Initialize(bombCount);
    }
}
