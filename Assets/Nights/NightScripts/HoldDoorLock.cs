using UnityEngine;
using UnityEngine.InputSystem;

public class HoldDoorLock : MonoBehaviour
{
    [Header("Hold Settings")]
    public float holdTimeRequired = 3f;
    private float holdTimer = 0f;

    [HideInInspector] public bool isDoorClosed = false;

    [Header("Enemy Interaction")]
    public LinScript linEnemy;
    public LanScript lanEnemy;

    [Header("Door Visuals (Toggle Objects)")]
    public GameObject closedDoorVisual;
    public GameObject openDoorVisual;

    [Header("--- ZVUK ---")]
    public AudioSource doorAudioSource;
    public AudioClip slamCloseSound;    // Zvuk, který se pøehraje PØESNÌ po 3 sekundách

    private Collider2D myCollider;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        if (openDoorVisual != null) openDoorVisual.SetActive(true);
        if (closedDoorVisual != null) closedDoorVisual.SetActive(false);
        isDoorClosed = false;
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            if (myCollider != null && myCollider.OverlapPoint(clickPosition))
            {
                if (!isDoorClosed)
                {
                    holdTimer += Time.deltaTime;

                    if (holdTimer >= holdTimeRequired)
                    {
                        CloseDoor();
                    }
                }
            }
            else
            {
                if (holdTimer > 0 && !isDoorClosed) holdTimer = 0f;
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (isDoorClosed)
            {
                OpenDoor();
            }
            else
            {
                holdTimer = 0f;
            }
        }
    }

    void CloseDoor()
    {
        if (isDoorClosed) return;

        isDoorClosed = true;
        holdTimer = holdTimeRequired;

        // --- ZVUK: PRÁSKNUTÍ ---
        if (doorAudioSource != null && slamCloseSound != null)
        {
            doorAudioSource.PlayOneShot(slamCloseSound);
        }

        if (closedDoorVisual != null) closedDoorVisual.SetActive(true);
        if (openDoorVisual != null) openDoorVisual.SetActive(false);

        Debug.Log("DVEØE ZAVØENY!");

        if (linEnemy != null) linEnemy.StopKillRoutine();
        if (lanEnemy != null) lanEnemy.DoorWasClosed();
    }

    void OpenDoor()
    {
        if (!isDoorClosed) return;

        isDoorClosed = false;
        holdTimer = 0f;

        if (closedDoorVisual != null) closedDoorVisual.SetActive(false);
        if (openDoorVisual != null) openDoorVisual.SetActive(true);

        Debug.Log("DVEØE OTEVØENY.");

        if (linEnemy != null) linEnemy.Unblock();
        if (lanEnemy != null) lanEnemy.Unblock();
    }
}