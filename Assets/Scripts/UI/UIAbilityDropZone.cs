using UnityEngine.EventSystems;
using UnityEngine;
using EventManagers;
using Events;

public class UIAbilityDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Transform dragTransform = eventData.pointerDrag.transform;
        Transform dragTransformParent = dragTransform.parent;
        int dragParentIndex = dragTransformParent.GetSiblingIndex();

        Transform dropTransform = null;
        Transform dropTransformParent = null;

        for(int i = 0; i < transform.childCount; i++)
        {
            if(dragParentIndex == i)
            {
                continue;
            }

            if(RectTransformUtility.RectangleContainsScreenPoint(transform.GetChild(i) as RectTransform, Input.mousePosition))
            {
                dropTransformParent = transform.GetChild(i);
                if(dropTransformParent.childCount != 0)
                {
                    dropTransform = dropTransformParent.GetChild(0);
                }
                break;
            }
        }

        if(dropTransformParent == null)
        {
            return;
        }

        ParamsObject dragParamsObj = new ParamsObject(dragTransform.GetComponent<UIAbilityReceptacle>().Ability.transform)
        {
            Int = dropTransformParent.GetSiblingIndex()
        };

        if(dropTransform != null)
        {
            ParamsObject dropParamsObj = new ParamsObject(dropTransform.GetComponent<UIAbilityReceptacle>().Ability.transform)
            {
                Int = dragParentIndex
            };
            GlobalEventManager.TriggerEvent(InventoryGlobalEvents.OnInventorySetAbility, dropParamsObj);
        }

        GlobalEventManager.TriggerEvent(InventoryGlobalEvents.OnInventorySetAbility, dragParamsObj);
    }
}
