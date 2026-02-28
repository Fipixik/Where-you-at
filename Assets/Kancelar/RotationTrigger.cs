using UnityEngine;
using UnityEngine.EventSystems;

public class RotationTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RoomRotator rotator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        rotator.RotateToBack();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rotator.RotateToFront();
    }
}