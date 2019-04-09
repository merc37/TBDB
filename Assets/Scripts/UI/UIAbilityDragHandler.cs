using UnityEngine;
using UnityEngine.EventSystems;

public class UIAbilityDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(eventData.position.x, eventData.position.y, 1); ;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
    }
}
