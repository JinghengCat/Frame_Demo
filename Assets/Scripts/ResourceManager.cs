using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;


public class Task
{
    public string bundlePath;
    public string assetPath;
    public bool onlyLoadBundle;
    public bool needAutoRelease;
    public Type assetType;
    public Action<ResourceContainer> OnLoadFinish;
}


public class ResourceManager
{
    #region Singleton
    private static ResourceManager _instance;
    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourceManager();
            }
            return _instance;
        }
    }
    #endregion

    private Dictionary<string, ResourceContainer> m_ResourceDict = new Dictionary<string, ResourceContainer>();
    private string m_ResourceRootPath;

    private int m_Worker = 12;
    private List<Coroutine> m_WorkCo = new List<Coroutine>();
    private Queue<Task> m_WorkTaskQueue = new Queue<Task>();


    public bool useAssetBundle
    {
        get
        {
            return Global.Instance.useAssetBundle;
        }
    }



    public void OnInit()
    {
        OnInitResourcesPath();
        OnInitWorkFlow();
    }

    private void OnInitResourcesPath()
    {
        m_ResourceRootPath = string.Format("{0}/{1}/", Application.dataPath, "AssetBundle");
        //Debug.Log("Init ResourceRootPath = " + m_ResourceRootPath);
    }

    private void OnInitWorkFlow()
    {
        for (int i = 0; i < m_Worker; i++)
        {
            m_WorkCo.Add(Global.Instance.StartCoroutine(DoTask()));
        }

        Global.Instance.StartCoroutine(AutoReleaseResource());
    }

    private IEnumerator DoTask()
    {
        yield return null;
    }

    private IEnumerator AutoReleaseResource()
    {
        //因为foreach中不能移除元素所以添加一个移除的List
        var list = new List<string>();
        while (true)
        {
            list.Clear();
            foreach (var item in m_ResourceDict)
            {
                var container = item.Value;
                if (container.loadState == LoadState.Loaded && container.RefCount <= 0 && container.autoRelease)
                {
                    container.LeftSecToRelease -= 1;
                    if (container.LeftSecToRelease <= 0)
                    {
                        UnLoadContainer(container);
                        list.Add(item.Key);
                    }
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                m_ResourceDict.Remove(list[i]);
            }
            yield return new WaitForSeconds(1);
        }
    }

    

    public void OnRelease()
    {

    }

    public void OnUpdate(float deltaTime)
    {

    }

    /// <summary>
    /// 在编辑器和设备上都调用此接口
    /// </summary>
    /// <param name="assetPath">相对于AssetBundle文件夹下的路径</param>
    /// <param name="bundlePath">相对于Assets/AssetBundle下的路径</param>
    /// <param name="assetType"></param>
    /// <param name="isAsync"></param>
    /// <param name="OnLoad"></param>
    public void LoadAsset(string assetPath, string bundlePath, Type assetType, bool isAsync, Action<ResourceContainer> OnLoad)
    {
#if UNITY_EDITOR
        if (useAssetBundle)
        {
            LoadAssetImp(assetPath, bundlePath, assetType, isAsync, OnLoad);
        }
        else
        {
            LoadAssetImp(assetPath, assetPath, assetType, isAsync, OnLoad);
        }
#else

#endif
    }

    public void UnLoadContainer(ResourceContainer container)
    {
        container.loadState = LoadState.UnLoaded;
        
        if (!useAssetBundle)
        {
            Debug.LogFormat("Release Container {0}", container.bundlePath);
            return;
        }
    }

    public ResourceContainer LoadAssetImp(string assetPath, string bundlePath, Type assetType, bool isAsync,  Action<ResourceContainer> OnLoad, bool onlyLoadBundle = false, bool needAutoRelease = true)
    {
        if (string.IsNullOrEmpty(bundlePath))
        {
            Debug.LogError("BundlePath Cant be Empty Or Null");
            return null;
        }

        ResourceContainer container = null;
        if (!m_ResourceDict.TryGetValue(bundlePath, out container))
        {
            container = new ResourceContainer();
            m_ResourceDict.Add(bundlePath, container);
            container.bundlePath = bundlePath;

            if (useAssetBundle)
            {
                //加载依赖，走相同流程
            }
        }

        Task task = new Task();
        task.assetPath = assetPath;
        task.bundlePath = bundlePath;
        task.assetType = assetType;
        task.OnLoadFinish = OnLoad;
        task.onlyLoadBundle = onlyLoadBundle;
        task.needAutoRelease = needAutoRelease;

        if (isAsync)
        {
            m_WorkTaskQueue.Enqueue(task);
        }
        else
        {
            if (useAssetBundle)
            {
                LoadAssetInAssetBundle(task);
            }
            else
            {
                LoadAssetInEditor(task);
            }
        }

        return container;
    }
    private void LoadAssetInEditor(Task task)
    {
        bool loadResult = m_ResourceDict.TryGetValue(task.bundlePath, out var container);
        if (loadResult)
        {
            container.loadState = LoadState.Loaded;
        }

        task.OnLoadFinish?.Invoke(container);
    }

    private void LoadAssetInAssetBundle(Task task)
    {

    }

    private IEnumerator LoadAssetInAssetBundleAsync(Task task)
    {
        yield return null;
    }
}
