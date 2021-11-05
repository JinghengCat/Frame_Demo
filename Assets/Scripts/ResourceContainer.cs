using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum LoadState
{
    UnLoaded,
    Loading,
    Loaded
}

public class ResourceContainer
{

    public AssetBundle bundle;
    public bool autoRelease = true;
    public LoadState loadState = LoadState.UnLoaded;
    public string bundlePath;
    public int LeftSecToRelease
    {
        get; set;
    } = 5;

    #region Ref Count
    private int m_RefCount = 0;
    public int RefCount { get { return m_RefCount; } }
    public void AddRefCount()
    {
        m_RefCount++;
    }

    private bool HasReference()
    {
        if (m_RefCount > 0)
        {
            return true;
        }
        else
        {
            if (m_RefCount < 0)
            {
                Debug.LogErrorFormat("RefCount Logic Error");
            }
            return false;
        }
    }
    public void MinusRefCount()
    {
        m_RefCount--;

        HasReference();
    }

    public void MinusRefCount(int count)
    {
        m_RefCount -= count;

        HasReference();
    }
    #endregion

    public T GetAsset<T>(string assetName) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (Global.Instance.useAssetBundle)
        {

        }
        else
        {
            return GetAssetInEditor<T>(assetName);
        }
#else

#endif
        return null;
    }



#if UNITY_EDITOR
    private T GetAssetInEditor<T>(string assetName) where T : UnityEngine.Object
    {
        var theType = typeof(T);
        if (theType == typeof(GameObject))
        {
            return GetGameObject(assetName) as T;
        }

        return null;
    }

    private GameObject GetGameObject(string assetName)
    {
        var path = (string.Format("{0}/{1}.prefab", bundlePath, assetName));
        GameObject asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        var obj = GameObject.Instantiate<GameObject>(asset);

        var script = obj.AddComponent<ResourceObj>();
        script.OnInit(this);
        return obj;
    }

#endif
}
