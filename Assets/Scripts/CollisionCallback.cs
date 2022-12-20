using System;

using UnityEngine;
using XLua;

[LuaCallCSharp]
public class CollisionCallback:MonoBehaviour
{
    public event Action<Collider> triggerEnterEvent;

		

    public void OnTriggerEnter(Collider collider) {
	   
        triggerEnterEvent?.Invoke(collider);
			
    }
	
    
}