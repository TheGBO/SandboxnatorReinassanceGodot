using System.Collections.Generic;
using Godot;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Util.Log;

namespace NullCyan.Util.IO;

public class ResourceIO
{

    /// <summary>
    /// Loads the item data from game content folder in order to register them.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static List<Resource> HandleResourcePath<T>(string path)
    {
        List<Resource> resources = [];

        DirAccess dirAccess = DirAccess.Open(path);
        if (dirAccess == null) { return null; }

        string[] files = dirAccess.GetFiles();
        if (files == null) { return null; }

        string remapSuffix = ".remap";
        string importSuffix = ".import";
        string uidSuffix = ".uid";

        foreach (string fileName in files)
        {
            string loadFileName = fileName;
            if (fileName.Contains(remapSuffix))
            {
                NcLogger.Log($"REMAP FOUND {fileName}", NcLogger.LogType.Register);
                loadFileName = StringUtils.TrimSuffix(fileName, remapSuffix);
            }

            if (!fileName.Contains(importSuffix) && !fileName.Contains(uidSuffix))
            {
                string resPath = path + "/" + loadFileName;
                Resource loadedRes = GD.Load<Resource>(resPath);
                if (loadedRes.GetType() == typeof(T))
                {
                    resources.Add(loadedRes);
                }
            }
        }

        return resources;
    }

    public static List<Resource> GetResources<T>(string contentPath)
    {
        List<Resource> totalResources = [];
        string path = contentPath;
        NcLogger.Log("Item content path:" + contentPath, NcLogger.LogType.Register);
        NcLogger.Log("Loading vanilla item data...", NcLogger.LogType.Register);
        using var dir = DirAccess.Open(path);
        if (dir != null)
        {
            string[] directories = dir.GetDirectories();
            //Go inside each directorie for a deeper search regarding resources
            foreach (string resDirectory in directories)
            {
                //underscore filters out abstract items
                if (!resDirectory.StartsWith("_"))
                {
                    string absoluteResDirectory = path + "/" + resDirectory;
                    List<Resource> currentLocalResources = HandleResourcePath<T>(absoluteResDirectory);
                    NcLogger.Log($"Found {typeof(T).Name} resource(s) at {absoluteResDirectory}:", NcLogger.LogType.Register);
                    foreach (Resource res in currentLocalResources)
                    {
                        totalResources.Add(res);
                    }
                }
            }
            return totalResources;
        }
        else
        {
            NcLogger.Log($"The path: \"{path}\" returned null on file access attempt!", NcLogger.LogType.Error);
        }
        return null;
    }
}