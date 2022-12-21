    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Networking;
    using XLua;
    
    [Serializable]
    public class GameVersion
    {
        public int version;
    }
    [LuaCallCSharp]
    public  class ResourcesManager:SingletonMono<ResourcesManager>
    {
     private   static AssetBundle uiLoadedAssetBundle;
     private   static AssetBundle imgLoadedAssetBundle;   
     private   static AssetBundle luaLoadedAssetBundle;
     protected override void InitAwake()
     {
         base.InitAwake();
      
     }

     private Dictionary<string, AssetBundle> assetBundles_dict;

     public  IEnumerator Hotfix()
     {
         GameVersion gameVersion=null;
         //判断是否要下载新版本
         bool Todownload = false;
         
         var versionUrl_local = new System.Uri(Path.Combine(Application.streamingAssetsPath, "version.json"));
         var versionLocalwww   = UnityWebRequest.Get(versionUrl_local);
         yield return versionLocalwww.SendWebRequest();
         //本地不存在version文件
         if (versionLocalwww.result != UnityWebRequest.Result.Success)
         {
             Todownload = true;
         }
         else
         {
             //开始和云端版本比较
           int  localVersion  =JsonUtility.FromJson<GameVersion>(versionLocalwww.downloadHandler.text).version;
           var versionUrl_remote = "http://localhost:8001/version.json";
           var versionwww_remote   = UnityWebRequest.Get(versionUrl_remote);
           yield return versionwww_remote.SendWebRequest();
           if (versionwww_remote.result != UnityWebRequest.Result.Success)
           {
               Todownload = false;
           }
           else
           {
               gameVersion = JsonUtility.FromJson<GameVersion>(versionwww_remote.downloadHandler.text);
               int  remoteVersion  =gameVersion.version;
               if (remoteVersion > localVersion)
               {
                   Todownload = true;
                   
               }
           }
         }
         //开始下载
         if (Todownload)
         {
             yield return DownLoadFromRemote("http://localhost:8001/","web.ab");
         }

         string str = JsonUtility.ToJson(gameVersion);
         StreamWriter sw = new StreamWriter(Path.Combine(Application.streamingAssetsPath, "version.json"));

         sw.Write(str);
         sw.Flush();
         sw.Close();
         yield return null;
     }

     IEnumerator DownLoadFromRemote(string url,string main_ab)
     {
         string url_main = url + main_ab;
         string localAbPath= Application.streamingAssetsPath+"/";
         
         var mainAbwww_remote   = UnityWebRequest.Get(new System.Uri(url_main));
         mainAbwww_remote.downloadHandler = new DownloadHandlerFile(localAbPath + main_ab);
         yield return mainAbwww_remote.SendWebRequest();
         if (mainAbwww_remote.result != UnityWebRequest.Result.Success)
         {
             Debug.Log("下载错误:"+mainAbwww_remote.error);
         }
         else
         {
             AssetBundle mainAB = AssetBundle.LoadFromFile(localAbPath+main_ab );
             
             //加载主包的mainfest文件
             AssetBundleManifest assetBundleManifest = mainAB.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
            //下载所以包
             foreach (var ab in assetBundleManifest.GetAllAssetBundles())
             {
                 string temp_url = url + ab;
                 var tempAbwww_remote   = UnityWebRequest.Get(new System.Uri(url_main));
                 tempAbwww_remote.downloadHandler = new DownloadHandlerFile(localAbPath + ab);
                 yield return tempAbwww_remote.SendWebRequest();
             }
         }
     }
     
     static ResourcesManager()
        {
            #if !UNITY_EDITOR
             uiLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "ui.ab"));
             imgLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "img.ab"));
             luaLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "lua.ab"));
            #endif
        }
        public static GameObject LoadGameObject(string path)
        {
            #if UNITY_EDITOR
                  return   Resources.Load<GameObject>("Prefabs/"+path);
            #else
                if (uiLoadedAssetBundle == null) {
                    Debug.Log("Failed to load AssetBundle!");
                    return null;
                }
                  var obj=  uiLoadedAssetBundle.LoadAsset<GameObject>(path);
                  return obj;
            #endif
            
        }
        public static Sprite LoadSprite(string path)
        {
    #if UNITY_EDITOR
                return Resources.Load<Sprite>("Sprites/items/"+ path);
    #else
            if (imgLoadedAssetBundle == null) {
                Debug.Log("Failed to load AssetBundle!");
                return null;
            }
            var obj=  imgLoadedAssetBundle.LoadAsset<Sprite>(path);
         
            return obj;
#endif
        }
        public static string LoadLua(string path)
        {
           
            if (luaLoadedAssetBundle == null) {
                Debug.Log("Failed to load AssetBundle!");
                return null;
            }
            var obj=  luaLoadedAssetBundle.LoadAsset<TextAsset>(path+".lua.txt");
            return obj.text;
        }
        public static string LoadPlayerPack()
        {
            string json = "";
            TextAsset text = Resources.Load<TextAsset>("Pack" );
            json = text.text;
            if (string.IsNullOrEmpty(json)) return null;
            return json;
        }
        public static void SavePlayerPack(string save)
        {
            
            //找到文件要存储的路径
            string filePath = Application.dataPath + "/Resources"+ "/Pack.txt";
            //创建一个文件流将字符串写入一个文件中
            StreamWriter sw = new StreamWriter(filePath);
            //开始写入
            sw.Write(save);
            //关闭文件流
            sw.Close();
             Debug.Log("已保存");  
        }


     
        
        void CreatFile(byte[] bytes,string savePath,string name)
        {
            Stream stream;
            stream = System.IO.File.Create(savePath +"/"+ name);
        
             stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            stream.Dispose();
        }
    }
