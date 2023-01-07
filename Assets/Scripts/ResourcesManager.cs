#define NOTUSEAB
 using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Networking;
 using UnityEngine.SceneManagement;
 using UnityEngine.UI;
    using XLua;


    [LuaCallCSharp]
    public static  class ResourcesManager
    {

        private static Dictionary<string, AssetBundle> dict_assetBundles = new Dictionary<string, AssetBundle>();
     private static AssetBundleManifest _assetBundleManifest;
 

     public static AssetBundleManifest LoadABManifest(AssetBundle ab)
     {
         return  ab.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
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
            if (_assetBundleManifest == null)
            {
                AssetBundle mainAB = AssetBundle.LoadFromFile(Application.streamingAssetsPath+"/web.ab");
                _assetBundleManifest = mainAB.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
         
            }
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
            string absPath = Application.dataPath + "/Scripts/lua/" + path;
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
        
        public static void SaveVersion(string save)
        {
            
            StreamWriter sw = new StreamWriter(Path.Combine(Application.streamingAssetsPath, "version.json"));
            
            sw.Write(save);
            sw.Flush();
            sw.Close();
        }
     
    }
