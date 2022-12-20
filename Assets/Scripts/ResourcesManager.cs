    using System.IO;
    using UnityEngine;
   
    using XLua;
    
    [LuaCallCSharp]
    public static class ResourcesManager
    {
     private   static AssetBundle uiLoadedAssetBundle;
     private   static AssetBundle imgLoadedAssetBundle;   
     private   static AssetBundle luaLoadedAssetBundle;   


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
    }
