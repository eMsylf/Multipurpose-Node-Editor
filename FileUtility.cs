using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class FileUtility
{
    /// <summary>
    /// Checks if the entered folders exist and creates a folder in the Assets folder for each folder string entered. The first folder is placed inside the Assets folder. Every consecutive folder entered is placed inside the previous folder. For example: "Characters", "Materials" would result in a folder with the path Assets/Characters/Materials/
    /// </summary>
    /// <param name="folders">Cascading subfolders. For example: "Characters", "Materials"</param>
    /// <returns>The folder path including the last forward slash (/). For example: Assets/Characters/Materials/</returns>
    public static string EnsureFolderIsInAssets(params string[] folders)
    {
        string completeFolderPath = "Assets";
        for (int i = 0; i < folders.Length; i++)
        {
            if (!AssetDatabase.IsValidFolder(completeFolderPath + "/" + folders[i]))
            {
                AssetDatabase.CreateFolder(completeFolderPath, folders[i]);
                Debug.Log("Folder created: " + completeFolderPath);
            }

            completeFolderPath += "/" + folders[i];
        }
        return completeFolderPath + "/";
    }
}
