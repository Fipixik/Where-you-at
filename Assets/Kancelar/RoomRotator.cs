using UnityEngine;

public class RoomRotator : MonoBehaviour
{
    public Vector3 officePos = new Vector3(0, 0, -10); // Pozícia u stola
    public Vector3 backPos = new Vector3(0, -15, -10); // Pozícia vzadu
    public float speed = 5f;

    private Vector3 targetPos;
    public bool isFacingBack = false;

    void Start()
    {
        targetPos = officePos;
    }

    void Update()
    {
        // Plynulý presun kamery na cie¾ovú pozíciu
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
    }

    public void RotateToBack()
    {
        targetPos = backPos;
        isFacingBack = true;
    }

    public void RotateToFront()
    {
        targetPos = officePos;
        isFacingBack = false;
    }
}