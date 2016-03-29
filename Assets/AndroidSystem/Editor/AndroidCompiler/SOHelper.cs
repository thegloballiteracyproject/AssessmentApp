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
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using UnityEditor;

public static class SOHelper
{
    public static void ExecuteProcessNoWait(string exeName, string argumentsString, string workingDir)
    {
        CreateProcess(exeName, argumentsString, workingDir);
    }

    public static bool ExecuteProcess(string exeName, string argumentsString, string workingDir, int timeout, bool printOutput)
    {
        Process proc = CreateProcess(exeName, argumentsString, workingDir);
        if (proc == null)
        {
            return false;
        }

        if (printOutput)
        {
            UnityEngine.Debug.Log("Executing process: " + exeName + " " + argumentsString);
        }

        return ExecutionMethod(proc, timeout, exeName, printOutput, null);
    }

    /// <summary>
    /// Executes the process.
    /// </summary>
    /// <returns><c>true</c>, if process was executed, <c>false</c> otherwise.</returns>
    /// <param name="exeName">Name of exe program</param>
    /// <param name="argumentsString">String with arguments in shell format</param>
    /// <param name="workingDir">Directory to program to be executed</param>
    /// <param name="timeout">Timeout to wait program to finish</param>
    public static void ExecuteProcess(string exeName, string argumentsString, string workingDir, int timeout, bool printOutput, Action<bool> completionCallback)
    {
        Process proc = CreateProcess(exeName, argumentsString, workingDir);
        if (proc == null && completionCallback != null)
        {
            completionCallback.Invoke(false);
            return;
        }

        if (printOutput)
        {
            UnityEngine.Debug.Log("Executing process: " + exeName + " " + argumentsString);
        }

        // Wait for process response in a new thread
        Thread th = new Thread(delegate ()
        {
            ExecutionMethod(proc, timeout, exeName, printOutput, completionCallback);
        });

        th.Start();
    }

    private static bool ExecutionMethod(Process p, int timeout, string exeName, bool printOutput, Action<bool> callback)
    {
        // Try waiting until process finish
        string output = "";
        string error = "";
        try
        {
            output = p.StandardOutput.ReadToEnd();
            error = p.StandardError.ReadToEnd();
            p.WaitForExit(timeout);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error waiting on process: " + error);
            UnityEngine.Debug.LogException(ex);

            if (callback != null) callback.Invoke(false);
            return false;
        }

#pragma warning disable 168
        // Kill process if exceed timeout
        if (p.HasExited == false)
        {
            try
            {
                UnityEngine.Debug.LogError("The process " + exeName + " is taking too much time, killing it");
                p.Kill();
            }
            catch (Exception ex)
            {
                // Ignore
            }
#pragma warning restore 168

            if (callback != null) callback.Invoke(false);
            return false;
        }

        // Disposing process
        int exitCode = p.ExitCode;
        bool success = exitCode == 0;
        p.Dispose();

        // Parse exit code
        if (!success)
        {
            if (!"".Equals(error))
            {
                UnityEngine.Debug.LogError(error);
            }
            else
            {
                UnityEngine.Debug.LogError("Process " + exeName + " failed with code " + exitCode);
            }

        }
        else
        {
            if (printOutput)
            {
                if (String.IsNullOrEmpty(output))
                    output = "Compilation of Android Java classes successful";

                UnityEngine.Debug.Log(output);
            }
        }

        // Return success
        if (callback != null) callback.Invoke(success);
        return success;
    }

    private static Process CreateProcess(string exeName, string argumentsString, string workingDir)
    {
        Process proc = new Process();
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.WorkingDirectory = workingDir;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.RedirectStandardError = true;
        proc.StartInfo.FileName = exeName;
        proc.StartInfo.Arguments = argumentsString;

        try
        {
            bool ok = proc.Start();
            if (!ok)
            {
                proc.Dispose();
                return null;
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error waiting on process " + exeName);
            UnityEngine.Debug.LogException(ex);

            proc.Dispose();
            return null;
        }

        return proc;
    }

    /// <summary>
    /// Search for java source files
    /// </summary>
    /// <param name="basePath">Directory to start search for java source files</param>
    /// <returns>List of source files found</returns>
    public static List<string> SearchFiles(string basePath, string fileExtension)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(basePath);
        return SearchFiles(dirInfo, fileExtension, basePath);
    }

    /// <summary>
    /// Search for java source files
    /// </summary>
    /// <param name="dirInfo">Info from directory to start search for java source files</param>
    /// <returns>List of source files found</returns>
    public static List<string> SearchFiles(DirectoryInfo dirInfo, string fileExtension, string basePath)
    {
        List<string> filePathList = new List<string>();

        FileInfo[] fileInfoArray = dirInfo.GetFiles(fileExtension);
        foreach (FileInfo fi in fileInfoArray)
        {
            string directory = fi.DirectoryName.Replace("\\", "/").Remove(0, basePath.Length);
            string filePath = directory + "/" + fi.Name;
            //filePath = filePath.Replace("\\", "/");
            filePathList.Add(filePath);
        }

        foreach (DirectoryInfo di in dirInfo.GetDirectories())
        {
            filePathList.AddRange(SearchFiles(di, fileExtension, basePath));
        }

        return filePathList;
    }

    private static bool IsFolder(UnityEngine.Object obj)
    {
        return Directory.Exists(AssetDatabase.GetAssetPath(obj));
    }
}