using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObj : MonoBehaviour
{
    private ResourceContainer m_Container;
    private bool mannualClearRefCount = false;
    
    public void OnInit(ResourceContainer container)
    {
        m_Container = container;
        container.AddRefCount();
    }

    public void OnRelease()
    {
        if (!mannualClearRefCount && m_Container != null)
        {
            m_Container.MinusRefCount();
            mannualClearRefCount = true;
        }

    }

    private void OnDestroy()
    {
        OnRelease();
    }
}
