using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class hud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt;
    System.Threading.Timer _timer;
    // Start is called before the first frame update
    void Start()
    {
        int secondsInterval = 30;
        _timer = new System.Threading.Timer(Tick, null, 0, secondsInterval * 1000);
        txt.text = "this is working";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Tick(object state)
    {
        AnalyzeScene();
    }

    void AnalyzeScene()
    {
        
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    PhotoCapture _photoCaptureObject = null;
    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        _photoCaptureObject = captureObject;

        //Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            string filename = string.Format(@"terminator_analysis.jpg");
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);
            _photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
        }
        else
        {
            
            txt.text = "Failed to scan";
        }
    }

    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            string filename = string.Format(@"terminator_analysis.jpg");
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

            //byte[] image = File.ReadAllBytes(filePath);
            //GetTagsAndFaces(image);
            //ReadWords(image);
        }
        else
        {

            txt.text = "Failed to scan";
        }
        _photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        _photoCaptureObject.Dispose();
        _photoCaptureObject = null;
    }
}
