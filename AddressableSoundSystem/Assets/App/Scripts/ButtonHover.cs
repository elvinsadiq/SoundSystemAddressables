using DynamicBox.EventManagement;
using DynamicBox.GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  public void OnPointerEnter(PointerEventData eventData)
  {
    Debug.Log("OnPointerHover");
    EventManager.Instance.Raise(new PlaySoundEffectEvent(0));
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    
  }
}
