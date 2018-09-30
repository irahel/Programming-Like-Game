using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizePanel : MonoBehaviour {

    
    public RectTransform rectTransformPanel;//recebe o painel 
    public RectTransform rectTransformImgBarra;//recebe a imagem da barra lateral, esse sera mudado a sua altura conforme o "rectTransformPanel"
    public LayoutElement bloco;//o bloco ou laço, ele recebera o tamanhanho dele mais o tamanho de seu painel. 
  

    private float larguraBarra, alturaBarra;
   
	
	// Update is called once per frame
	void Update () {
        larguraBarra = 18.5f;//largura da imagem "barra" 
        alturaBarra = rectTransformPanel.rect.height; //altura da imagem "barra"

        rectTransformImgBarra.sizeDelta = new Vector2(larguraBarra, alturaBarra); // modifica altura e largura da barra
        bloco.preferredHeight = rectTransformPanel.rect.height + 61; //modifica o tamanho do bloco

    }
}
