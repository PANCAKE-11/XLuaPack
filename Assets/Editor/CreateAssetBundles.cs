using System.Collections;
using System.Collections.Generic;
using CSObjectWrapEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;


public static class CreateAssetBundles 
{
    [MenuItem("MyBuild/All", false, 4)]
    public static void All()
    {

        Generator.ClearAll();
        Debug.Log("Xlua生成代码清理完成");
        Generator.GenAll();
        Debug.Log("Xlua生成代码完成");
        BuildAllAssetBundles_loaclWeb();
        Debug.Log("本地资源服务器AB包构建完成");
    }
    [MenuItem("MyBuild/streamingAssets", false, 1)]
    static void BuildAllAssetBundles_stearming()
    {
        string assetBundleDirectory = Application.streamingAssetsPath;
        if(!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, 
                                        BuildAssetBundleOptions.None, 
                                        BuildTarget.StandaloneWindows);
    }
    [MenuItem("MyBuild/loaclWeb", false, 2)]
    static void BuildAllAssetBundles_loaclWeb()
    {
        string assetBundleDirectory = "S:/MyWebServer_43467/web";
        if(!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, 
            BuildAssetBundleOptions.None, 
            BuildTarget.StandaloneWindows);

        System.IO.File.Move("S:/MyWebServer_43467/web/web", "S:/MyWebServer_43467/web/web.ab");
    }
}
