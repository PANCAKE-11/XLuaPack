using System;
using UnityEngine;
using UnityEngine.EventSystems;
using XLua;

    [LuaCallCSharp]
    public class Dragable : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
    {
        public event Action<PointerEventData,Transform> onBeginDragEvent; 
        public event Action<PointerEventData,Transform> onDragEvent; 
        public event Action<PointerEventData,Transform> onEndDragEvent; 

            public event Action<PointerEventData,Transform> onPointerEnterEvent; 
            public event Action<PointerEventData> onPointerExitEvent; 
public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDragEvent?.Invoke(eventData,transform);
    
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDragEvent?.Invoke(eventData,transform);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDragEvent?.Invoke(eventData,transform);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnterEvent?.Invoke(eventData,transform);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExitEvent?.Invoke(eventData);
        }
    }
