using System;
using System.IO;
using UnityEngine;

public class FileManagement : Singleton<FileManagement> {

    private string path;

    public void Initialize()
    {
        path = Application.persistentDataPath;
    }

    #region File/Directory management
    private bool CheckDirectory(string directory)
    {
        if (Directory.Exists(path + "/" + directory))
            return true;
        else
            return false;
    }

    private void CreateDirectory(string directory)
    {
        print("Creating directory: " + directory);
        Directory.CreateDirectory(path + "/" + directory);
    }

    public bool CheckFile(string filePath)
    {
        if (File.Exists(path + "/" + filePath))
            return true;
        else
            return false;
    }
 
    public void UpdateFile(string directory, string filename, string filetype, string fileData, string mode)
    {
        print("Updating " + directory + "/" + filename + "." + filetype);
        if (!CheckDirectory(directory))
            CreateDirectory(directory);
        if (CheckFile(directory + "/" + filename + "." + filetype) == false)
            mode = "replace";
        if (mode == "replace")
            File.WriteAllText(path + "/" + directory + "/" + filename + "." + filetype, fileData);
        if (mode == "append")
            File.AppendAllText(path + "/" + directory + "/" + filename + "." + filetype, fileData);
    }
   
    public string ReadFile(string directory, string filename, string filetype)
    {
        print("Reading " + directory + "/" + filename + "." + filetype);
        if (!CheckDirectory(directory))
            CreateDirectory(directory);
        if (CheckFile(directory + "/" + filename + "." + filetype) == true)
            return File.ReadAllText(path + "/" + directory + "/" + filename + "." + filetype);
        else
        {
            TextAsset targetFile = Resources.Load(filename) as TextAsset;
            string tempData = targetFile.ToString();
            UpdateFile(directory, filename, filetype, tempData, "replace");
            return tempData;
        }
    }

    #endregion
}
