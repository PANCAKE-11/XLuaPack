    using System.IO;
    using UnityEngine;
   
    using XLua;
    
    [LuaCallCSharp]
    public static class ResourcesManager
    {
        public static GameObject LoadGameObject(string path)
        {
            return   Resources.Load<GameObject>(path);
        }
        public static Sprite LoadSprite(string path)
        {
           
            return   Resources.Load<Sprite>(path);
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
