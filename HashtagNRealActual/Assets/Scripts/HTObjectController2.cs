using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class HTObjectController2 : MonoBehaviour
{
    public static bool canSwapHashtag = true;

    bool init = false;
    bool lastTracking = false;
    public Transform worldParent;
    public static GameObject currentHashtagParent;
    public static int currentHashtag;

    public GameObject[] worldMap;

    void Start()
    {
        worldParent.gameObject.SetActive(false);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!init)
        {
            init = true;
            worldParent.gameObject.SetActive(true);
            SetHashtag("hacktothefuture");
        }
#endif

#if !UNITY_EDITOR
        if (ImageTrackerTest.worldAnchor == null)
            return;
#endif

        if (!init)
        {
            worldParent.gameObject.SetActive(true);
            init = true;
            worldParent.position = new Vector3(ImageTrackerTest.worldAnchor.position.x, 0f, ImageTrackerTest.worldAnchor.position.z);
            worldParent.rotation = Quaternion.Euler(worldParent.eulerAngles.x, ImageTrackerTest.worldAnchor.eulerAngles.y, worldParent.eulerAngles.z);
            SetHashtag("hacktothefuture");
        } else {
           if(currentHashtagParent != null) {
                if(!canSwapHashtag) {
                    //currentHashtagParent.transform.position = Vector3.Lerp(currentHashtagParent.transform.position, new Vector3(currentHashtagParent.transform.position.x, -200f, currentHashtagParent.transform.position.z), 16f * Time.deltaTime);
                    currentHashtagParent.transform.localScale = Vector3.Lerp(currentHashtagParent.transform.localScale, Vector3.zero, 16f * Time.deltaTime);
                } else {
                    //currentHashtagParent.transform.localPosition = Vector3.Lerp(currentHashtagParent.transform.localPosition, Vector3.zero, 16f * Time.deltaTime);
                    currentHashtagParent.transform.localScale = Vector3.Lerp(currentHashtagParent.transform.localScale, Vector3.one, 16f * Time.deltaTime);
                }
           } 
        }
    }

    public void SetHashtag(string ht)
    {
        StartCoroutine(SetHashtagRoutine(ht));
    }

    private IEnumerator SetHashtagRoutine(string ht) {
        canSwapHashtag = false;

        yield return new WaitForSeconds(0.7f);

        if(currentHashtagParent != null)
            currentHashtagParent.SetActive(false);
        
        if (ht.ToLower() == "hacktothefuture")
        {
            worldMap[0].SetActive(true);
            currentHashtagParent = worldMap[0];
            currentHashtag = 0;
        }
        else if (ht.ToLower() == "mitmedialab")
        {
            worldMap[1].SetActive(true);
            currentHashtagParent = worldMap[1];
            currentHashtag = 1;
        }
        else if (ht.ToLower() == "socialgood")
        {
            worldMap[2].SetActive(true);
            currentHashtagParent = worldMap[2];
            currentHashtag = 2;
        }

        canSwapHashtag = true;
    }

}
