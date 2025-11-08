using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerDragController : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Vector3 targetPosition;
    private PlayerGrowth playerGrowth;

    private bool canOpenQuestion = true;
    public float questionCooldown = 2f;

    private bool isDragging = false;
    private Vector3 dragStartPosition; // for detecting real drag
    private float dragThreshold = 0.2f; // how far the finger must move to start dragging

    public static bool IsInCooldown { get; private set; } = false;

    void Start()
    {
        targetPosition = transform.position;
        playerGrowth = GetComponent<PlayerGrowth>();
    }

    void Update()
    {
        if (FishQuestionManager.IsQuestionActive || FeaturePanelManager.IsFeatureActive || PauseMenu.isPaused)
            return;

        HandleInput();
        MovePlayer();
        FlipSprite();
    }

    private void HandleInput()
    {
        if (EventSystem.current == null)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE
        // --- Mouse drag control (no tap movement) ---
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            dragStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragStartPosition.z = 0f;
            isDragging = false; // not yet dragging until moved enough
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = 0f;

            // Only start dragging after the mouse has moved past a small threshold
            if (!isDragging && Vector3.Distance(dragStartPosition, currentPos) > dragThreshold)
                isDragging = true;

            if (isDragging)
                targetPosition = currentPos;
        }

        if (Input.GetMouseButtonUp(0))
            isDragging = false;
#endif

#if UNITY_ANDROID || UNITY_IOS
        // --- Touch drag control (no tap movement) ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            touchPos.z = 0f;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        dragStartPosition = touchPos;
                        isDragging = false;
                    }
                    break;

                case TouchPhase.Moved:
                    // Only move if finger moves beyond small threshold
                    if (!isDragging && Vector3.Distance(dragStartPosition, touchPos) > dragThreshold)
                        isDragging = true;

                    if (isDragging)
                        targetPosition = touchPos;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }
#endif
    }

    private void MovePlayer()
    {
        if (isDragging)
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void FlipSprite()
    {
        if (playerGrowth == null)
            return;

        float direction = (targetPosition.x > transform.position.x) ? 1f : -1f;
        playerGrowth.FlipDirection(direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canOpenQuestion || FishQuestionManager.IsQuestionActive ||
            FeaturePanelManager.IsFeatureActive || PauseMenu.isPaused ||
            FishQuestionManager.IsInCooldown)
            return;

        FishAudioManager.Instance.PlayPlayerBite();

        FishQuestionHolder smallFish = collision.GetComponent<FishQuestionHolder>();
        if (smallFish != null)
        {
            FishQuestionManager.Instance.ShowQuestion(smallFish.question, smallFish);
            StartCoroutine(QuestionCooldown());
        }
    }

    private IEnumerator QuestionCooldown()
    {
        canOpenQuestion = false;
        yield return new WaitForSeconds(questionCooldown);
        canOpenQuestion = true;
    }
}
