using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ExtFile
{
    /// <summary>
    /// read every line in the file
    /// </summary>
    public static void ReadTextFile(string filePath)
    {
        StreamReader file = new StreamReader(filePath);

        while (!file.EndOfStream)
        {
            //string line = file.ReadLine();
        }

        file.Close();
    }

    /*
    //get full string of spine.atlas.txt
    string atlasContent = File.ReadAllText(atlasPath);
    ReadTextFile(atlasPath);
    //finaly, update the atrlas with renamed name
    File.WriteAllText(atlasPath, atlasContent);
     */

    /*
    var bodyPartAcceptedRootPath = $"{bodyPartAccepted}/";
    var bodyPartSpritesPath = $"{resourcesRootPath}{genreRootPath}{bodyPartAcceptedRootPath}";

    var sprites = Resources.LoadAll<Sprite>(bodyPartSpritesPath);

     * */

    #region SaveTo

    /// <summary>
    /// Creates a directory at <paramref name="folder"/> if it doesn't exist
    /// </summary>
    /// <param name="folder"></param>
    public static void CreateDirectoryIfNotExists(this string folder)
    {
        if (string.IsNullOrEmpty(folder))
            return;

        string path = Path.GetDirectoryName(folder);
        if (string.IsNullOrEmpty(path))
            return;

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    /// <summary>
    /// Saves the data to a file
    /// </summary>
    /// <param name="data"></param>
    /// <param name="path"></param>
    public static void SaveTo(this string data, string path, bool append)
    {
        // exit if no data or no filename
        if ((string.IsNullOrEmpty(data)) || (string.IsNullOrEmpty(path)))
            return;

        // create the folder if it doesn't exist
        path.CreateDirectoryIfNotExists();

        if (append)
        {
            File.AppendAllText(path, data);
            return;
        }

        // save the data
        File.WriteAllText(path, data);
    }

    // SaveTo
    #endregion

    #region SaveToPersistentDataPath

    /// <summary>
    /// Saves the data to the PersistentDataPath, which is a directory where your application can store user specific 
    /// data on the target computer. This is a recommended way to store files locally for a user like highscores or savegames. 
    /// C:\Users\Hed\AppData\LocalLow\DefaultCompany\EkkoUnity\
    /// </summary>
    /// <param name="data">data to save</param>
    /// <param name="folderName">OPTIONAL - sub folder name (ex. DataFiles\SavedGames</param>
    /// <param name="filename">the filename (ex. SavedGameData.xml)</param>
    public static void SaveToPersistentDataPath(this string data, string folderName, string filename, bool append = false)
    {
        // exit if no data or no filename
        if ((string.IsNullOrEmpty(data)) || (string.IsNullOrEmpty(filename)))
            return;

        string path = string.IsNullOrEmpty(folderName) ?
            Path.Combine(Application.persistentDataPath, filename) :
            Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename);

        // save the data
        SaveTo(data, path, append);
    }

    // SaveToPersistentDataPath
    #endregion

    #region SaveToDataPath

    /// <summary>
    /// Saves the data to the DataPath, which points to your asset/project directory. This directory is typically read-only after
    /// your game has been compiled. Use SaveToDataPath only from Editor scripts
    /// </summary>
    /// <param name="data">data to save</param>
    /// <param name="folderName">OPTIONAL - sub folder name (ex. DataFiles\SavedGames</param>
    /// <param name="filename">the filename (ex. SavedGameData.xml)</param>
    public static void SaveToDataPath(this string data, string folderName, string filename, bool append = false)
    {
        // exit if no data or no filename
        if ((string.IsNullOrEmpty(data)) || (string.IsNullOrEmpty(filename)))
            return;

        string path = string.IsNullOrEmpty(folderName) ?
            Path.Combine(Application.dataPath, filename) :
            Path.Combine(Path.Combine(Application.dataPath, folderName), filename);

        // save the data
        SaveTo(data, path, append);
    }

    // SaveToDataPath
    #endregion

    #region LoadFrom

    /// <summary>
    /// Loads the data as a string 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string LoadFrom_AsString(this string path)
    {
        // exit if no path
        if (string.IsNullOrEmpty(path))
            return null;

        // exit if the file doesn't exist
        if (!File.Exists(path))
            return null;

        // read the file
        return File.ReadAllText(path);
    }

    /// <summary>
    /// Loads the data as a byte array 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] LoadFrom_AsBytes(this string path)
    {
        // exit if no path
        if (string.IsNullOrEmpty(path))
            return null;

        // exit if the file doesn't exist
        if (!File.Exists(path))
            return null;

        // read the file
        return File.ReadAllBytes(path);
    }

    // LoadFrom
    #endregion

    #region LoadFromPeristantDataPath

    /// <summary>
    /// Loads the data from PersistantDataPath as a string 
    /// </summary>
    /// <param name="folderName">OPTIONAL - sub folder name (ex. DataFiles\SavedGames</param>
    /// <param name="filename">the filename (ex. SavedGameData.xml)</param>
    public static string LoadFromPeristantDataPath_AsString(this string filename, string folderName)
    {
        // exit if no path
        if (string.IsNullOrEmpty(filename))
            return null;

        // build the path
        string path = string.IsNullOrEmpty(folderName) ?
            Path.Combine(Application.persistentDataPath, filename) :
            Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename);

        // load the data
        return LoadFrom_AsString(path);
    }

    /// <summary>
    /// Loads the data from PersistantDataPath as a byte array 
    /// </summary>
    /// <param name="folderName">OPTIONAL - sub folder name (ex. DataFiles\SavedGames</param>
    /// <param name="filename">the filename (ex. SavedGameData.xml)</param>
    public static byte[] LoadFromPeristantDataPath_AsBytes(this string filename, string folderName)
    {
        // exit if no path
        if (string.IsNullOrEmpty(filename))
            return null;

        // build the path
        string path = string.IsNullOrEmpty(folderName) ?
            Path.Combine(Application.persistentDataPath, filename) :
            Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename);

        // load the data
        return LoadFrom_AsBytes(path);
    }

    // LoadFromPeristantDataPath
    #endregion

    #region LoadFromDataPath

    /// <summary>
    /// Loads the data from PersistantDataPath as a string 
    /// </summary>
    /// <param name="folderName">OPTIONAL - sub folder name (ex. DataFiles\SavedGames</param>
    /// <param name="filename">the filename (ex. SavedGameData.xml)</param>
    public static string LoadFromDataPath_AsString(this string filename, string folderName)
    {
        // exit if no path
        if (string.IsNullOrEmpty(filename))
            return null;

        // build the path
        string path = string.IsNullOrEmpty(folderName) ?
            Path.Combine(Application.dataPath, filename) :
            Path.Combine(Path.Combine(Application.dataPath, folderName), filename);

        // load the data
        return LoadFrom_AsString(path);
    }

    /// <summary>
    /// Loads the data from PersistantDataPath as a byte array 
    /// </summary>
    /// <param name="folderName">OPTIONAL - sub folder name (ex. DataFiles\SavedGames</param>
    /// <param name="filename">the filename (ex. SavedGameData.xml)</param>
    public static byte[] LoadFromDataPath_AsBytes(this string filename, string folderName)
    {
        // exit if no path
        if (string.IsNullOrEmpty(filename))
            return null;

        // build the path
        string path = string.IsNullOrEmpty(folderName) ?
            Path.Combine(Application.dataPath, filename) :
            Path.Combine(Path.Combine(Application.dataPath, folderName), filename);

        // load the data
        return LoadFrom_AsBytes(path);
    }

    // LoadFromDataPath
    #endregion
}
