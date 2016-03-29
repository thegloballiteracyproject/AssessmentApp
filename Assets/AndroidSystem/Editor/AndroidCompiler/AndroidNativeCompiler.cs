//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.34014
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AndroidNativeCompiler : AbstractCompiler
{
    // Android NDK path
    private static string androidNDK;
    public static string AndroidNDKPath
    {
        get
        {
            string path = EditorPrefs.GetString(AndroidCompilerConstants.ANDROID_NDK).Replace("\\", "/");
            if (String.IsNullOrEmpty(path))
            {

            }
            return path;
        }
        set
        {
            EditorPrefs.SetString(AndroidCompilerConstants.ANDROID_NDK, value);
        }
    }

    private string AndroidNativeSourcesPath
    {
        get
        {
            return AndroidCompilerConstants.APPLICATION_DATA_PATH + AndroidCompilerConstants.PLUGINS_JNI_PATH;
        }
    }


    private AndroidCompilerConfig config;

    public AndroidNativeCompiler (AndroidCompilerConfig config)
    {
        this.config = config;

        androidNDK = null;
        ConfigurePath();
    }

    public static bool ShouldCompile(string assetPath)
    {
        return (assetPath.Contains(".cpp") || assetPath.Contains(".h")) && assetPath.Contains(AndroidCompilerConstants.PLUGINS_JNI_PATH);
    }

    public override bool ConfigurePath()
    {
        // This configuration doesn't exists in Unity3D.
        // Shall be done by our custom UI.
        if (String.IsNullOrEmpty(androidNDK))
        {
            // First, try some of environment paths
            string[] ndkVarNames = new string[] {   "ANDROID_NDK",
                                                    "ANDROID_NDK_ROOT",
                                                    "NDK_PATH",
                                                    "NDK_ROOT",
                                                    "NDKROOT"};
            foreach (string var in ndkVarNames)
            {
                string ndkPath = Environment.GetEnvironmentVariable(var);
                if (ndkPath != null)
                {
                    androidNDK = ndkPath.Replace("\\", "/");
                    return true;
                }
            }

            if (String.IsNullOrEmpty(androidNDK))
            {
                // If not found, print an alert to user
                UnityEngine.Debug.Log("Android NDK Path not found. Please, configure NDK path in System Variables");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Creates the java package.
    /// </summary>
    public override void GeneratePackage(bool updateAtFinish, Action<bool> completionCallback)
    {
        if (!ConfigurePath())
        {
            if (completionCallback != null)
                completionCallback.Invoke(false);

            return;
        }

        // Use correctly Application.mk, that contains the targets for
        // Android native libraries
        File.Delete(AndroidNativeSourcesPath + "Application.mk");
#if UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6
		File.Copy(AndroidNativeSourcesPath + "Application.mk.unity4", AndroidNativeSourcesPath + "Application.mk");
#else
        File.Copy(AndroidNativeSourcesPath + "Application.mk.unity5", AndroidNativeSourcesPath + "Application.mk");
#endif

        string ndk_build = androidNDK + "/ndk-build.cmd";
        bool compilationOK = SOHelper.ExecuteProcess(ndk_build, " ", AndroidCompilerConstants.ANDROID_PLUGINS_PATH, 5000, true);
        if (!compilationOK)
        {
            //UnityEngine.Debug.LogError("Error compiling	Android native code, aborting");
            if (completionCallback != null)
                completionCallback.Invoke(false);

            return;
        }

        // Copy generated library to correct folder
        // to be recognized by Unity 4.x and earlier
#if UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6
		File.Copy(GetAndroidPluginsPath() + "libs/armeabi-v7a/libunityandroidsystem.so", 
		          GetAndroidPluginsPath() + "libunityandroidsystem.so",
		          true);
#endif

        CleanupFiles(null);

        // Refresh assets database to show native libraries
        if (updateAtFinish)
            AssetDatabase.Refresh();

        if (completionCallback != null)
            completionCallback.Invoke(true);
    }

    public override void CleanupFiles(List<string> files)
    {
        // Delete "obj" folder
        string objFolder = AndroidCompilerConstants.ANDROID_NATIVE_OBJECTS_PATH;
        if (Directory.Exists(objFolder))
            Directory.Delete(objFolder, true);
    }
}