using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows.WebCam;
using System.Linq;
using System;
using System.IO;
using UnityEngine.Networking;
using UnityGoogleDrive;
using ExitGames.Client.Photon.StructWrapping;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class hud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt;
    [SerializeField] private TextMeshProUGUI analysis;
    private System.Threading.Timer _timer;
    PhotoCapture _photoCaptureObject = null;
    IEnumerator coroutine;
    public Texture2D image;

    [Header("Number of seconds between snapshots")]
    [Range(10, 60)]
    public int _loopSeconds = 5;

    public string _computerVisionKey = "2fc121cbfe2c4d74a8bcd3726ee16f30";
    public string _computerVisionEndpoint = "https://eastus.api.cognitive.microsoft.com/vision/v3.2/analyze?visualFeatures=Tags&language=en&model-version=latest";

   
    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(CoroLoop());

    }
    IEnumerator CoroLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_loopSeconds);
            AnalyzeScene();
        }
    }

    

    
    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        _photoCaptureObject = captureObject;

        // find the best supported resolution
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        // start the capture
        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {       // take the picture
            _photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {

            txt.text = "Failed to capture";
        }
    }
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        _photoCaptureObject.Dispose();
        _photoCaptureObject = null;
    }

    private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

            photoCaptureFrame.UploadImageDataToTexture(targetTexture);

            var imageBytes = targetTexture.EncodeToJPG();

            GetTagsAndFaces(imageBytes);

        }
        else
        {

            txt.text = "Failed to save";
        }
        _photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }
    void AnalyzeScene()
    {
        var content = image.EncodeToJPG();
        analysis.text= "CALCULATION PENDING";
        GetTagsAndFaces(content);
        //PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }



    public void GetTagsAndFaces(byte[] image)
    {
        try
        {
            coroutine = RunComputerVision(image);
            StartCoroutine(coroutine);
        }
        catch (Exception ex)
        {
            txt.text = "Failed to find faces and tags";
        }
    }

    IEnumerator RunComputerVision(byte[] image)
    {
        


        UnityWebRequest www = new UnityWebRequest(_computerVisionEndpoint, UnityWebRequest.kHttpVerbPOST);
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(image);
        uploadHandler.contentType = "application/octet-stream";
        www.uploadHandler = uploadHandler;
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/octet-stream");
        www.SetRequestHeader("Ocp-Apim-Subscription-Key", _computerVisionKey);
        

         yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {       // on error, show information and return
            txt.text = "Failed to www "+ www.result;
            yield break;
        }
        try
        {
            var resultObject = JsonUtility.FromJson<AnalysisResult>(www.downloadHandler.text);

            // show all the tags returned
            List<string> tags = new List<string>();
            foreach (var tag in resultObject.tags)
            {
                tags.Add(tag.name);
            }
            analysis.text = "ANALYSIS:\n***************\n" + string.Join("\n", tags.ToArray());
            if (tags.Contains("car"))
            {
                txt.text = "Warning, car in area";
                StartCoroutine(DelaySceneLoad());
                



            }
            else
            {
                txt.text = "Please look both ways before crossing the street";
            }
        }
        catch (Exception ex)
        {       // show error details in UI
            txt.text = "Failed";
        }
    }
    IEnumerator DelaySceneLoad()
    {
        EditorApplication.Beep();
        yield return new WaitForSeconds(1f);
        EditorApplication.Beep();
        yield return new WaitForSeconds(1f);
        EditorApplication.Beep();
    }



    public class AnalysisResult
    {
        public Tag[] tags;
        public Face[] faces;

    }

    [Serializable]
    public class Tag
    {
        public double confidence;
        public string hint;
        public string name;
    }

    [Serializable]
    public class Face
    {
        public int age;
        public FaceRectangle facerectangle;
        public string gender;
    }

    [Serializable]
    public class FaceRectangle
    {
        public int height;
        public int left;
        public int top;
        public int width;
    }

}
