using UnityEngine;

public class CameraTeleporter : MonoBehaviour
{
    [Header("Cílové Souřadnice")]
    public float targetX = 0f;
    public float targetY = 0f;

    [Header("Nastavení pohybu")]
    [Tooltip("Pokud je true, kamera tam skočí hned. Jinak poletí plynule.")]
    public bool instantTeleport = true;
    public float transitionSpeed = 5f;

    private Camera mainCam;
    private bool isMoving = false;
    private Vector3 finalTargetPos;

    void Start()
    {
        mainCam = Camera.main;
    }

    // Volá se při kliknutí na objekt s Colliderem
    void OnMouseDown()
    {
        // Z osu si vezmeme z aktuální kamery (aby neujela do hloubky, typicky -10)
        finalTargetPos = new Vector3(targetX, targetY, mainCam.transform.position.z);

        if (instantTeleport)
        {
            mainCam.transform.position = finalTargetPos;
        }
        else
        {
            isMoving = true;
        }
    }

    void Update()
    {
        if (!instantTeleport && isMoving)
        {
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, finalTargetPos, transitionSpeed * Time.deltaTime);

            // Zastavení pohybu při dojezdu
            if (Vector3.Distance(mainCam.transform.position, finalTargetPos) < 0.01f)
            {
                mainCam.transform.position = finalTargetPos;
                isMoving = false;
            }
        }
    }
}