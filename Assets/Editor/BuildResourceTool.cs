using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class BuildSetting
{
    public string outputPath;
    public AssetBundleBuild[] buildSettings;
    public BuildAssetBundleOptions buildOption = BuildAssetBundleOptions.UncompressedAssetBundle;
    public BuildTarget buildTarget = BuildTarget.StandaloneWindows;
}


public class BuildResourceTool
{
    public static string inputPath = string.Format("Assets/AssetBundle");
    public static string outputPath = string.Format("Assets/Build");

    public static string inputFullPath = string.Format("{0}/AssetBundle", Application.dataPath);
    public static string outputFullPath = string.Format("{0}/../Build",Application.dataPath);

    private static BuildSetting InitBuildSetting()
    {
        var setting = new BuildSetting();

        var dirs = Directory.GetDirectories(inputFullPath);
        setting.buildSettings = new AssetBundleBuild[dirs.Length];
        setting.outputPath = outputFullPath;

        for (int i = 0; i < dirs.Length; i++)
        {
            var resDirPath= dirs[i].Replace("\\", "/");
            var resDirPathArr = resDirPath.Split('/');
            var resDirName = resDirPathArr[resDirPathArr.Length - 1];
            var buildDirPath = string.Format("{0}/{1}", outputFullPath, resDirName);


            ////创建文件夹
            //if (Directory.Exists(buildDirPath))
            //{
            //    Directory.Delete(buildDirPath, true);
            //}

            //Directory.CreateDirectory(buildDirPath);

            //获取资源文件夹下所有文件路径
            var resFilesPath = Directory.GetFiles(resDirPath);
            List<string> filePathes = new List<string>();
            for (int index = 0; index < resFilesPath.Length; index++)
            {
                var filePath = resFilesPath[index].Replace("\\", "/");
                if (filePath.EndsWith(".meta"))
                {
                    continue;
                }
                filePathes.Add(filePath);
            }

            //将所有文件写入设置
            var buildSettingData = new AssetBundleBuild();
            //buildSettingData.assetNames


            string[] assetNames = new string[filePathes.Count];
            string[] addressableNames = new string[filePathes.Count];

            for (int index = 0; index < filePathes.Count; index++)
            {
                var filePath = filePathes[index];
                var fileName = Path.GetFileName(filePath);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                var relativeFilePath = string.Format("{0}/{1}/{2}", inputPath, resDirName, fileName);
                assetNames[index] = relativeFilePath;
                addressableNames[index] = fileName;
            }

            buildSettingData.assetBundleName = resDirName.ToLower() + ".ab";
            //Debug.LogFormat("buildSettingData.assetBundleName = {0}", buildSettingData.assetBundleName);
            //buildSettingData.assetBundleVariant = "ab";
            buildSettingData.assetNames = assetNames;
            buildSettingData.addressableNames = assetNames;

            setting.buildSettings[i] = buildSettingData;
        }

        return setting;
    }
    [MenuItem("Build/BuildAssetBundle")]
    public static void BuildAssetBundle()
    {
        var setting = InitBuildSetting();
        LogSettings(setting);
        BuildPipeline.BuildAssetBundles(setting.outputPath, setting.buildSettings, setting.buildOption, setting.buildTarget);
        AssetDatabase.Refresh();
    }

    public static void LogSettings(BuildSetting setting)
    {
        Debug.Log("------------------------------------------------------------");
        Debug.LogFormat("OutPutPath = {0}, BuildOption = {1}, BuildTarget = {2}", setting.outputPath, setting.buildOption, setting.buildTarget);
        var buildSettings = setting.buildSettings;
        for (int i = 0; i < buildSettings.Length; i++)
        {
            Debug.LogFormat("Index = {0}, BundleName = {1}, Assets: ", i, buildSettings[i].assetBundleName);
            var assetNames = buildSettings[i].assetNames;
            var addressableNames = buildSettings[i].addressableNames;
            for (int j = 0; j < assetNames.Length; j++)
            {
                Debug.LogFormat("AssetName = {0}, AddressableName = {1}", assetNames[j], addressableNames[j]);
            }
        }
        Debug.Log("------------------------------------------------------------");
    }

}
