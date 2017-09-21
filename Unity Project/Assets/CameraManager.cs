using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.WebCam;
using System.Linq;
using System;


using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using UnityEngine.UI;
using Facebook;


public class CameraManager : MonoBehaviour {
    
    #region core

    TimeSpan ts = new TimeSpan(0, 0, 3);
    DateTime last; 

    // Use this for initialization
    void Start () {
        last = DateTime.Now;
        nameText = GameObject.Find("NameText");
    }
	
	// Update is called once per frame
	void Update () {
        if (DateTime.Now.Subtract(last) > ts && !photomode)
        {
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
            last = DateTime.Now;
        }
        if (changeText)
        {
            Debug.Log("changing name");
            changeText = false;
            UpdateName(name);
        }
    }

    #endregion core

    #region pictures

    Texture2D image;
    PhotoCapture photoCaptureObject = null;
    bool photomode = false;

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photomode = true;

        photoCaptureObject = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        Debug.Log("OnPhotoCaptureCreated");
        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            Debug.Log("photomode success");
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            // Create our Texture2D for use and set the correct resolution
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            image = new Texture2D(cameraResolution.width, cameraResolution.height);
            // Copy the raw image data into our target texture
            photoCaptureFrame.UploadImageDataToTexture(image);
            // Do as we wish with the texture such as apply it to a material, etc.

            Debug.Log("photo taken, sending to server");

            sendPhotoToServer();
        }
        // Clean up
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
        photomode = false;
    }

    #endregion pictures

    #region networking

    string url = "http://80f8b731.ngrok.io/recognize";

    async void sendPhotoToServer()
    {
        HttpContent bytesContent = new ByteArrayContent(image.EncodeToJPG());
        using (var client = new HttpClient())
        using (var formData = new MultipartFormDataContent())
        {
            try
            {
                formData.Add(bytesContent, "file", "hololensImage.jpg");
                HttpResponseMessage response = await client.PostAsync(url, formData);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.Log("failed");
                }
                Debug.Log("Success!");

                name = await response.Content.ReadAsStringAsync();
                Debug.Log(name);
                changeText = true;
            } catch (HttpRequestException e)
            {
                Debug.Log("HttpRequestException caught by black.");
                Debug.Log(e.StackTrace);
                Debug.Log(e.Message);
                Debug.Log(e.InnerException);
            }
        }
    }

    #endregion networking

    #region UI

    GameObject nameText;
    string name;
    bool changeText = false;

    void UpdateName(string name)
    {
        nameText.GetComponent<Text>().text = name;
    }

    #endregion UI

    #region fb

    void PostImage()
    {
        var fb = new FacebookMediaObject();
    }

    #endregion fb
}
