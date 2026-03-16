using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightFollow : MonoBehaviour
{
    private Camera mainCam;

    [Header("Nastavení plynulosti")]
    public float smoothTime = 0.15f; // Čas, za který baterka "doletí" k myši (v sekundách)

    private Vector3 currentVelocity = Vector3.zero; // Unity si sem ukládá aktuální rychlost

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // 1. Pozice myši
        Vector2 mouseInput = Mouse.current.position.ReadValue();

        // 2. Přepočet na svět
        Vector3 targetWorldPos = mainCam.ScreenToWorldPoint(new Vector3(mouseInput.x, mouseInput.y, 10f));
        targetWorldPos.z = 0f;

        // 3. SMOOTH DAMP: Tohle je ten "tlumič"
        // transform.position - odkud jdeme
        // targetWorldPos - kam jdeme
        // ref currentVelocity - pomocná proměnná pro plynulost
        // smoothTime - jak dlouho to trvá (zkus 0.1 až 0.3)
        transform.position = Vector3.SmoothDamp(transform.position, targetWorldPos, ref currentVelocity, smoothTime);
    }
}