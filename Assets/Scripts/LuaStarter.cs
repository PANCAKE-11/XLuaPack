using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
using System.IO;
using UnityEngine.UI;

public class LuaStarter : MonoBehaviour
{
     static LuaEnv luaEnv = new LuaEnv();
     static float lastGCTime = 0;
    private const float GCInterval = 1;//1 second 
    
    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptEnv;



    void Awake()
    {
        
        gameObject.GetComponentInChildren<Button>();
        scriptEnv = luaEnv.NewTable();
        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
       LuaTable meta = luaEnv.NewTable();
       meta.Set("__index", luaEnv.Global);
       scriptEnv.SetMetaTable(meta);
       meta.Dispose();

       scriptEnv.Set("self", this);

    luaEnv.AddLoader((ref string filename) =>
       {
           return System.Text.Encoding.UTF8.GetBytes(ResourcesManager.LoadLua(filename));
       });

       
       luaEnv.DoString("require 'Main'", "LuaTestScript", scriptEnv);

       Action luaAwake = scriptEnv.Get<Action>("awake");
       scriptEnv.Get("start", out luaStart);
       scriptEnv.Get("update", out luaUpdate);
       scriptEnv.Get("ondestroy", out luaOnDestroy);

       luaAwake?.Invoke();
    }

    // Use this for initialization
    void Start()
    {
        
       if (luaStart != null)
       {
           luaStart();
       }
    }

    // Update is called once per frame
    void Update()
    {
        luaUpdate?.Invoke();
        if (!(Time.time - LuaStarter.lastGCTime > GCInterval)) return;
        luaEnv.Tick();
       LuaStarter.lastGCTime = Time.time;
    }

    void OnDestroy()
    {
        luaOnDestroy?.Invoke();
       luaOnDestroy = null;
       luaUpdate = null;
       luaStart = null;
       scriptEnv.Dispose();
      
    }
}
