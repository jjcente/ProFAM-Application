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
        // --- Mouse drag control ---
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0f;
            targetPosition = Camera.main.ScreenToWorldPoint(mousePos);
            targetPosition.z = 0f;
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        // --- Touch drag control ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                        touchPos.z = 0f;
                        targetPosition = touchPos;
                    }
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
