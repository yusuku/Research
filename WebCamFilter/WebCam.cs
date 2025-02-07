using UnityEngine;
using System;
using System.Collections;
using UnityEngine;
using NUnit.Framework;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static Unity.Sentis.Model;


public class WebCamFilter : MonoBehaviour
{
    public Material Sharpenmat;
    public RenderTexture outCam;
    WebCamTexture webcamTexture;

    void Start()
    {

        InitializeCamera();
    }
    private void Update()
    {
        Graphics.Blit(webcamTexture, outCam,Sharpenmat);
    }
    void OnDestroy()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }
    }
    private void InitializeCamera()
    {
        string devname="";
        foreach(var dev in WebCamTexture.devices)
        {
            if (dev.name == "RICOH THETA UVC")
            {
                Debug.Log(dev.name);
                devname=dev.name;
            }
        }
        if(devname == "") Debug.Assert(false);
        webcamTexture = new WebCamTexture(devname);

        webcamTexture.Play();
    }
}
