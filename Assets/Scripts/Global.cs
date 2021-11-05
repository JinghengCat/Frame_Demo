using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    private static Global m_Instance;
    public static Global Instance
    {
        get
        {
            return m_Instance;
        }
    }
    
    public bool useAssetBundle = false;


    private void Awake()
    {
        m_Instance = this;
        OnInit();
    }

    private void OnInit()
    {
        ResourceManager.Instance.OnInit();
    }

    private void OnRelease()
    {

    }


}
