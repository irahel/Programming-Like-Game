using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstruturarPainell : MonoBehaviour {

    public RectTransform pan;
    public GameObject pn;
    private List<Rect> estruturaMontada, aVerificar;

 

    public void verificar()
    {
        pn.GetComponent<Rect>();

        for (int i = 0; i < pan.childCount; i++)
        {
            
        }


        Debug.Log("OK"+pan.childCount);
    }

    private void motarEstrutura()
    {

    }
    
}
