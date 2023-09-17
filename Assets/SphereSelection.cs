using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SphereSelection : MonoBehaviour
{

    [SerializeField] private GameObject car_2;
    [SerializeField] private GameObject car_1;
    [SerializeField] private string selectableTagL = "SelectableL" ;
    [SerializeField] private string selectableTagR = "SelectableR";
    [SerializeField] private TextMeshProUGUI txt;
    private int counter1=0 ;
    private int counter2 = 0;
    private Transform _selection;
    private void Update()
    {
        if (_selection != null)
        {
            
            _selection = null;
        }
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selection = hit.transform;
            if (selection.CompareTag(selectableTagL))
            {
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    txt.text = "Now please look right";
                    counter1++;
                }
                
            }
            else if (selection.CompareTag(selectableTagR))
            {
                txt.text = "Now please look left";
                counter2++;
            }
        }
        if (counter1 > 0 && counter2>0)
        {
            Destroy(car_1);
            Destroy(car_2);
            txt.text = "You may now cross the street \n next level is now loading";
            StartCoroutine(DelaySceneLoad());
           

        }
    }

    IEnumerator DelaySceneLoad()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("scene 2");
    }
}
