    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using XLua;

    [LuaCallCSharp]
    public static class ExtraMethod
    {
        public static Button GetComponentButton(this Transform transform)
        {
            return transform.GetComponent<Button>();
            
        }
        public static CollisionCallback GetComponentCollisionCb(this Transform transform)
        {
         
            return transform.GetComponent<CollisionCallback>();
        }
        public static Image GetComponentImage(this Transform transform)
        {
            return transform.GetComponent<Image>();
        }
        public static ItemInWorld GetComponentItemInWorld(this Transform transform)
        {
            return transform.GetComponent<ItemInWorld>();
        }
        public static Text GetTextComponentInChildren(this Transform transform)
        {
            return transform.GetComponentInChildren<Text>();
        }
        public static Text GetTextComponent(this Transform transform)
        {
            return transform.GetComponent<Text>();
        }
        public static Dragable GetDragableComponentInChildren(this Transform transform)
        {
            return transform.GetComponentInChildren<Dragable>();
        }
    }
