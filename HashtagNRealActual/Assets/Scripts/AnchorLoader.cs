using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

using NRKernal.Persistence;

public class AnchorLoader : MonoBehaviour
{
    // This is the object which you want to load
    public GameObject AnchorPrefab;
    // This is an anchor load/add tool using image tracking.
    private ImageTrackingAnchorTool m_ImageTrackingAnchorTool;

    private void Start()
    {
        m_ImageTrackingAnchorTool = gameObject.GetComponent<ImageTrackingAnchorTool>();
        m_ImageTrackingAnchorTool.OnAnchorLoaded += OnImageTrackingAnchorLoaded;
    }

    /// <summary>
    /// After local anchor been loaded
    /// </summary>
    /// <param name="key"></param>
    /// <param name="anchor"></param>
    private void OnImageTrackingAnchorLoaded(string key, NRWorldAnchor anchor)
    {
        // Load your prefab as a child of the anchor
        var go = Instantiate(AnchorPrefab);
        go.transform.SetParent(anchor.transform);
        go.name = key;
    }

    public void Load()
    {
        NRWorldAnchorStore.GetAsync(GetAnchorStoreCallBack);
    }

	private NRWorldAnchorStore m_NRWorldAnchorStore;
	private Dictionary<string, GameObject> m_AnchorPrefabDict = new Dictionary<string, GameObject>();
	private Dictionary<string, GameObject> m_LoadedAnchorDict = new Dictionary<string, GameObject>();
	private StringBuilder m_LogStr = new StringBuilder();
    private void GetAnchorStoreCallBack(NRWorldAnchorStore store)
    {
        if (store == null)
        {
            Debug.Log("store is null");
            return;
        }
        m_NRWorldAnchorStore = store;
        m_LogStr.AppendLine("Load map result: true");
        var keys = m_NRWorldAnchorStore.GetAllIds();
        if (keys != null)
        {
            foreach (var key in m_NRWorldAnchorStore.GetAllIds())
            {
                m_LogStr.AppendLine("Get a anchor from NRWorldAnchorStore  key: " + key);
                GameObject prefab;
                if (m_AnchorPrefabDict.TryGetValue(key, out prefab))
                {
                    var go = Instantiate(prefab);
                    m_NRWorldAnchorStore.Load(key, go);
                    m_LoadedAnchorDict[key] = go;
                }
            }
        }
    }
}