using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class tester : MonoBehaviour
{
    public Texture2D image;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine("testCreated");
        }
    }
    public IEnumerator testCreated()
    {
        var content = image.EncodeToPNG();
        var file = new UnityGoogleDrive.Data.File() { Name = "test", Content = content };
        var request = GoogleDriveFiles.Create(file);
        yield return request.Send();
        print(request.IsError);
        print(request.ResponseData.Content);
        print(request.ResponseData.Id);
    }
}
