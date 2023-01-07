using UnityEngine;
using XLua;
[LuaCallCSharp]
public enum ItemType
{
    拉面,
    宝石,
    卷轴,

    
}
    [LuaCallCSharp]
public class ItemInWorld:MonoBehaviour
{
    public ItemType itemType;
    public int num;
}
