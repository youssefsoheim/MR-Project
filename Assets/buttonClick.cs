using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class buttonClick : MonoBehaviour
{
    public GameObject chair;
    public int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(counter)
        {
            case 0: 
                counter++;
                break;
        }

    }
}
