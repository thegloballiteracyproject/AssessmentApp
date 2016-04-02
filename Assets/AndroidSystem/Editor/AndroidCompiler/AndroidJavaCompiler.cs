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
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AndroidJavaCompiler : AbstractCompiler
{
    // Editor constants for configurations
    private static readonly string[] sdkEnvNames
                                = new string[] {    "ANDROID_SDK",
                                                    "ANDROID_SDK_ROOT"};

    private static string javaSDK;
    private static string androidSDK;

    private AndroidCompilerConfig config;

    // Android SDK path
    public static string AndroidSDKPath
    {
        get
        {
            string androidSDK = EditorPrefs.GetString(AndroidCompilerConstants.ANDROID_SDK).Replace("\\", "/");
            if (String.IsNullOrEmpty(androidSDK))
            {
                // First, try some of environment paths
                foreach (string var in sdkEnvNames)
                {
                    string sdkPath = Environment.GetEnvironmentVariable(var);
                    if (sdkPath != null)
                    {
                        androidSDK = sdkPath.Replace("\\", "/");
                        EditorPrefs.SetString(AndroidCompilerConstants.ANDROID_SDK, androidSDK);
                        break;
                    }
                }
            }
            return androidSDK;
        }
        set
        {
            EditorPrefs.SetString(AndroidCompilerConstants.ANDROID_SDK, value);
        }
    }

    private string AndroidJavaSourcesPath
    {
        get
        {
            return Application.dataPath + AndroidCompilerConstants.PLUGINS_JAVA_PATH;
        }
    }
    
    public AndroidJavaCompiler(AndroidCompilerConfig config)
    {
        this.config = config;
        ConfigurePath();
    }

    public static bool ShouldCompile(string assetPath)
    {
        return assetPath.Contains(".java") && assetPath.Contains(AndroidCompilerConstants.PLUGINS_JAVA_PATH);
    }

    public override bool ConfigurePath()
    {
        // Find Java path
        if (String.IsNullOrEmpty(javaSDK))
        {
            // First, try some of environment paths
            string[] jdkVarNames = new string[] {   "JAVA_HOME",
                                                    "JAVA_SDK",
                                                    "JDK_HOME"};

            foreach (string var in jdkVarNames)
            {
                string jdkPath = Environment.GetEnvironmentVariable(var);
                if (jdkPath != null)
                {
                    javaSDK = jdkPath.Replace("\\", "/");
                    break;
                }
            }

            if (String.IsNullOrEmpty(javaSDK))
            {
                // If not found, print an alert to user
                Debug.LogError("Java JDK Path not found. Please, configure Java JDK path in System Variables");
                return false;
            }
        }

        // Find Android SDK Path
        if (String.IsNullOrEmpty(androidSDK))
        {
            // First, try some of environment paths
            string[] sdkVarNames = new string[] {   "ANDROID_SDK",
                                                    "ANDROID_SDK_ROOT"};
            foreach (string var in sdkVarNames)
            {
                string sdkPath = Environment.GetEnvironmentVariable(var);
                if (sdkPath != null)
                {
                    androidSDK = sdkPath.Replace("\\", "/");
                    return true;
                }
            }

            if (String.IsNullOrEmpty(androidSDK))
            {
                // If not found, print an alert to user
                Debug.LogError("Android SDK Path not found. Please, configure Android SDK path in System Variables");
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
        }

        // First, search for java files
        List<string> sourceFiles = SOHelper.SearchFiles(AndroidJavaSourcesPath, "*.java");
        if (sourceFiles.Count <= 0)
        {
            if (completionCallback != null)
                completionCallback.Invoke(false);
            return;
        }

        // Then, compile all found source files
        bool compileOK = CompileJavaClasses(sourceFiles);
        if (!compileOK)
        {
            UnityEngine.Debug.LogError("Error compiling Android java classes, aborting");

            if (completionCallback != null)
                completionCallback.Invoke(false);
            return;
        }

        // Create JAR package with compiled files
        string jarPath = AndroidCompilerConstants.ANDROID_GENERATED_JAR_PATH;
        List<string> compiledFiles = SOHelper.SearchFiles(AndroidJavaSourcesPath, "*.class");
        bool jarOK = CreateJarPackage(jarPath, compiledFiles);
        if (!jarOK)
        {
            Debug.LogError("Error creating JAR package, aborting");

            if (completionCallback != null)
                completionCallback.Invoke(false);
            return;
        }

        CleanupFiles(compiledFiles);

        // Refresh assets database to show created JAR 
        if (updateAtFinish)
            AssetDatabase.Refresh();

        if (completionCallback != null)
            completionCallback.Invoke(true);
    }

    public override void CleanupFiles(List<string> files)
    {
        // Delete all compiled classes
        foreach (string file in files)
        {
            File.Delete(AndroidJavaSourcesPath + file);
        }
    }

    /// <summary>
    /// Compiles ".java" files into ".class" file
    /// </summary>
    /// <param name="javaFiles">List of path for ".java" source files</param>
    /// <returns><c>true</c>, if classes was compiled, <c>false</c> otherwise.</returns>
    private bool CompileJavaClasses(List<string> javaFiles)
    {
        if (javaFiles.Count > 0)
        {
            string files = "";
            foreach (string f in javaFiles)
            {
                files += f + " ";
            }

            string classpath = "";
            foreach (string c in config.classpath)
            {
                classpath += c + ";";
            }

            string args = " -cp " + classpath + " " + files;
            return SOHelper.ExecuteProcess("javac.exe", args, AndroidJavaSourcesPath, 1000, true);
        }

        return false;
    }

    /// <summary>
    /// Creates the jar package.
    /// </summary>
    /// <param name="outputJar">Output jar.</param>
    /// <param name="files">Files.</param>
    private bool CreateJarPackage(string outputJar, List<string> files)
    {
        if (outputJar.Length != 0)
        {
            string args = "cvf " + outputJar + " ";
            foreach (string file in files)
            {
                args += file + " ";
            }

            return SOHelper.ExecuteProcess("jar.exe", args, AndroidJavaSourcesPath, 1000, true);
        }

        return false;
    }
}