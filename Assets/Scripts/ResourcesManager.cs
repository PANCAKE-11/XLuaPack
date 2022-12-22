﻿// #define NOTUSEAB
 using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Networking;
 using UnityEngine.SceneManagement;
 using UnityEngine.UI;
    using XLua;

    [Serializable]
    public class GameVersion
    {
        public int version;
    }
    [LuaCallCSharp]
    public  class ResourcesManager:SingletonMono<ResourcesManager>
    {

     private Slider slider;
     private static Dictionary<string, AssetBundle>dict_assetBundles;
     private static AssetBundleManifest _assetBundleManifest;
 
     private IEnumerator Start()
     {
         dict_assetBundles = new Dictionary<string, AssetBundle>();
         slider= GameObject.FindObjectOfType<Slider>();
         yield return Hotfix();
         yield return new WaitForSeconds(0.5f);
         AssetBundle mainAB = AssetBundle.LoadFromFile(Application.streamingAssetsPath+"/web.ab");
         //加载主包的mainfest文件
         _assetBundleManifest = mainAB.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
         LoadScene();
         
      
       
     }

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
             int localVersion = 0;
             //开始和云端版本比较
             if (!string.IsNullOrEmpty(versionLocalwww.downloadHandler.text))
             {
                   localVersion  =JsonUtility.FromJson<GameVersion>(versionLocalwww.downloadHandler.text).version;
             }
             
             var versionUrl_remote = "http://localhost:8001/version.json";
        
             var versionwww_remote   = UnityWebRequest.Get(versionUrl_remote);
        

             yield return versionwww_remote.SendWebRequest();
             slider.value =1 /  (float)7;
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
         else
         {
             slider.value = 1;
         }
         AssetBundle.UnloadAllAssetBundles(true); 
         // string str = JsonUtility.ToJson(gameVersion);
         // StreamWriter sw = new StreamWriter(Path.Combine(Application.streamingAssetsPath, "version.json"));
         //
         // sw.Write(str);
         // sw.Flush();
         // sw.Close();
         yield return null;
     }

     IEnumerator DownLoadFromRemote(string url,string main_ab)
     {
         
         string url_main = url + main_ab;
         
         string localAbPath= Application.streamingAssetsPath+"/";
         var mainAbwww_remote   = UnityWebRequest.Get(new System.Uri(url_main));
         mainAbwww_remote.downloadHandler = new DownloadHandlerFile(localAbPath + main_ab);
         yield return mainAbwww_remote.SendWebRequest();
         slider.value =2/  (float)7;
         if (mainAbwww_remote.result != UnityWebRequest.Result.Success)
         {
             Debug.Log("下载错误:"+mainAbwww_remote.error);
         }
         else
         {
             AssetBundle mainAB = AssetBundle.LoadFromFile(localAbPath+main_ab );
             //加载主包的mainfest文件
             var _assetBundleManifest = mainAB.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
            //下载所以包
             foreach (var ab in _assetBundleManifest.GetAllAssetBundles())
             {
                 string temp_url = url + ab;
                 var tempAbwww_remote   = UnityWebRequest.Get(new System.Uri(temp_url));
                 tempAbwww_remote.downloadHandler = new DownloadHandlerFile(localAbPath + ab);
                 tempAbwww_remote.SendWebRequest();
                 while (!tempAbwww_remote.isDone)
                 {
                     yield return null;
                     slider.value += tempAbwww_remote.downloadProgress /
                         _assetBundleManifest.GetAllAssetBundles().Length + 2;
                 }
                     
             }
         }
     }
     
  
        public static GameObject LoadPrefab(string path)
        {
            string abName = "prefab.ab";
            #if NOTUSEAB
                  return   Resources.Load<GameObject>("Prefabs/"+path);
            #else
                LoadAB(abName);
                
                var obj=   dict_assetBundles[abName].LoadAsset<GameObject>(path);
                return obj;
            #endif
            
        }

        private static void LoadAB(string abName)
        {
            if (!dict_assetBundles.ContainsKey(abName))
            {
                AssetBundle t = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, abName));
                dict_assetBundles[abName] = t;

                foreach (var dependency in _assetBundleManifest.GetDirectDependencies(abName))
                {
                    LoadAB(dependency);
                }
            }
        }

        public static Sprite LoadSprite(string path)
        {
            string abName = "img.ab";
    #if NOTUSEAB
                return Resources.Load<Sprite>("Sprites/items/"+ path);
    #else
            LoadAB(abName);
                
            var obj=   dict_assetBundles[abName].LoadAsset<Sprite>(path);
            return obj;
#endif
        }
        public static string LoadLua(string path)
        {
            path = path + ".lua.txt";
           #if NOTUSEAB
             string absPath = Application.dataPath + "/Scripts/lua/" + path + ".lua.txt";
             return   File.ReadAllText(absPath);
       
           #else
            string abName = "lua.ab";
            LoadAB(abName);
            
            var obj=   dict_assetBundles[abName].LoadAsset<TextAsset>(path);
            return obj.text;
        #endif
        }
        public static string LoadPlayerPack()
        {
            string json = "";
            TextAsset text = Resources.Load<TextAsset>("Pack" );
            json = text.text;
            if (string.IsNullOrEmpty(json)) return null;
            return json;
        }

        public static void LoadScene()
        {
            string abName = "scene.ab";
            LoadAB(abName);
          
          print(dict_assetBundles[abName].isStreamedSceneAssetBundle);
            
                foreach (var name in   dict_assetBundles[abName].GetAllAssetNames())
                {
                    print(name);
                }
          
            
            var secnes= dict_assetBundles[abName].GetAllScenePaths();
 
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
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
