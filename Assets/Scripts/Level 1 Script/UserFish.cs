using UnityEngine;
using UnityEngine.EventSystems; // <-- needed for IsPointerOverGameObject
using System.Collections;

public class PlayerDragController : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Vector3 targetPosition;
    private PlayerGrowth playerGrowth;

    private bool canOpenQuestion = true;   // cooldown control
    public float questionCooldown = 2f;


public static bool IsInCooldown { get; private set; } = false;


    void Start()
    {
        targetPosition = transform.position; // initialize

            playerGrowth = GetComponent<PlayerGrowth>();

    }

    void Update()
    {
    if (FishQuestionManager.IsQuestionActive || FeaturePanelManager.IsFeatureActive || PauseMenu.isPaused)
            return;
        
        HandleInput();    // <--- call it here
        MovePlayer();
        FlipSprite();
    }

    // <--- PUT THE METHOD HERE
    private void HandleInput()
    {
        if (EventSystem.current == null)
            return; // avoid null reference if EventSystem is missing

        // Editor: mouse input
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 0f;
            targetPosition = Camera.main.ScreenToWorldPoint(mousePos);
            targetPosition.z = 0f;
        }

        // Mobile: touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchPos.z = 0f;
                targetPosition = touchPos;
            }
        }
    }

    private void MovePlayer()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

  private void FlipSprite()
{
    if (playerGrowth == null)
        return;

    // Get direction: 1 = right, -1 = left
    float direction = (targetPosition.x > transform.position.x) ? 1f : -1f;

    // Flip using PlayerGrowth so it keeps its current size
    playerGrowth.FlipDirection(direction);
}

       private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!canOpenQuestion || FishQuestionManager.IsQuestionActive || FeaturePanelManager.IsFeatureActive || PauseMenu.isPaused || FishQuestionManager.IsInCooldown)
            return; // prevent re-trigger spam
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
