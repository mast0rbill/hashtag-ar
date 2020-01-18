using NRKernal;
using UnityEngine;

public class GlassEventTest : MonoBehaviour
{
    void Start()
    {
        GlassEventProxy.Instance.OnGlassPutOn += OnGlassPutOn;
        GlassEventProxy.Instance.OnGlassPutOff += OnGlassPutOff;
        GlassEventProxy.Instance.OnGlassIDResponse += OnGlassIDResponse;
    }

    private void Update()
    {
        if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            GlassEventProxy.Instance.RequestGlassID();
        }
    }

    private void OnGlassIDResponse(string msg)
    {
        Debug.LogError("..............OnGlassIDResponse............... :" + msg);
    }

    private void OnGlassPutOff(string msg)
    {
        Debug.LogError("..............OnGlassPutOff...............");
    }

    private void OnGlassPutOn(string msg)
    {
        Debug.LogError("..............OnGlassPutOn...............");
    }
}
