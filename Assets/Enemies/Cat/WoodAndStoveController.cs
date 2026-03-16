using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WoodAndStoveController : MonoBehaviour
{
    public enum ObjectType { Wood, Stove }
    [Header("Nastavení objektu")]
    public ObjectType type;

    [Header("Vizuály (Přiřaď JEN u Kamen!)")]
    public GameObject woodInHand;
    public GameObject stoveOff;
    public GameObject stoveOn;
    public float burnDuration = 10f;

    // STATICKÉ proměnné - vidí je i CatScript
    private static bool hasWood = false;
    private static bool isBurning = false;

    private Collider2D col;

    // Funkce, kterou volá Cat, aby zjistila, jestli se topí
    public static bool IsStoveBurning() { return isBurning; }

    private void Start()
    {
        col = GetComponent<Collider2D>();

        if (type == ObjectType.Stove)
        {
            if (stoveOff != null) stoveOff.SetActive(true);
            if (stoveOn != null) stoveOn.SetActive(false);
            if (woodInHand != null) woodInHand.SetActive(false);
        }
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (col != null && col.OverlapPoint(mousePos) && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (type == ObjectType.Wood) HandleWoodClick();
            else if (type == ObjectType.Stove) HandleStoveClick();
        }
    }

    private void HandleWoodClick()
    {
        if (isBurning) return;
        hasWood = !hasWood;
        UpdateWoodVisual();
    }

    private void HandleStoveClick()
    {
        if (hasWood && !isBurning) StartCoroutine(BurnRoutine());
    }

    private IEnumerator BurnRoutine()
    {
        isBurning = true;
        hasWood = false;

        WoodAndStoveController stove = (type == ObjectType.Stove) ? this : FindStove();
        if (stove != null)
        {
            if (stove.woodInHand != null) stove.woodInHand.SetActive(false);
            if (stove.stoveOff != null) stove.stoveOff.SetActive(false);
            if (stove.stoveOn != null) stove.stoveOn.SetActive(true);

            yield return new WaitForSeconds(burnDuration);

            if (stove.stoveOff != null) stove.stoveOff.SetActive(true);
            if (stove.stoveOn != null) stove.stoveOn.SetActive(false);
        }
        isBurning = false;
    }

    private void UpdateWoodVisual()
    {
        WoodAndStoveController stove = (type == ObjectType.Stove) ? this : FindStove();
        if (stove != null && stove.woodInHand != null) stove.woodInHand.SetActive(hasWood);
    }

    private WoodAndStoveController FindStove()
    {
        var scripts = Object.FindObjectsByType<WoodAndStoveController>(FindObjectsSortMode.None);
        foreach (var s in scripts) if (s.type == ObjectType.Stove) return s;
        return null;
    }
}