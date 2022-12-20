using System.Collections;
using System.Collections.Generic;
using CSObjectWrapEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;


public static class CreateAssetBundles 
{
    [MenuItem("MyBuild/All", false, 3)]
    public static void All()
    {

        Generator.GenAll();
        Debug.Log("Xlua生成代码清理完成");
        Generator.GenAll();
        Debug.Log("Xlua生成代码完成");
        BuildAllAssetBundles();
        Debug.Log("AB包构建完成");
    }
 
    static void BuildAllAssetBundles()
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
}
