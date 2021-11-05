using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public KeyCode m_CreateKey = KeyCode.C;
    public KeyCode m_DeleteKey = KeyCode.D;
    public KeyCode m_DeleteAllKey = KeyCode.F;
    public string assetName;
    public string assetPath;

    public List<GameObject> m_ObjPool = new List<GameObject>();

    private void Update()
    {
        if (Input.GetKeyDown(m_CreateKey))
        {
            CreateSinglePrefab();
        }

        if (Input.GetKeyDown(m_DeleteKey))
        {
            DeleteSinglePrefab();
        }

        if (Input.GetKeyDown(m_DeleteAllKey))
        {
            DeleteAllPrefabs();
        }
    }

    private void CreateSinglePrefab()
    {
        System.Action<ResourceContainer> OnLoad = delegate (ResourceContainer container)
        {
            var obj = container.GetAsset<GameObject>(assetName);
            m_ObjPool.Add(obj);
            obj.transform.position = Random.insideUnitSphere * 2f;
        };
        ResourceManager.Instance.LoadAsset(assetPath, assetPath, typeof(GameObject), true, OnLoad);
    }

    private void DeleteSinglePrefab()
    {
        if (m_ObjPool.Count > 0)
        {
            var obj = m_ObjPool[0];
            var script = obj.GetComponent<ResourceObj>();
            script.OnRelease();
            GameObject.DestroyImmediate(obj);
            m_ObjPool.RemoveAt(0);
        }
    }

    private void DeleteAllPrefabs()
    {
        for (int i = 0; i < m_ObjPool.Count; i++)
        {
            GameObject.DestroyImmediate(m_ObjPool[i]);
        }
    }
}
