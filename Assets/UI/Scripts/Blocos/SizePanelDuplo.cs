using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizePanelDuplo : MonoBehaviour {


    public RectTransform rectTransformPanel;//recebe o painel 1
    public RectTransform rectTransformPanel2;//recebe o painel 2
    public RectTransform rectTransformImgBarra;//recebe a imagem da barra lateral, esse sera mudado a sua altura conforme o "rectTransformPanel"
    public RectTransform rectTransformImgBarra2;
    public LayoutElement bloco;//o bloco ou laço, ele recebera o tamanhanho dele mais o tamanho de seu painel. 


    private float larguraBarra, larguraBarra2, alturaBarra, alturaBarra2;


    // Update is called once per frame
    void Update()
    {
        larguraBarra = 22.5f;//largura da imagem "barra"
        larguraBarra2 = 1f;//largura da imagem "barra"
        alturaBarra = rectTransformPanel.rect.height; //altura da imagem "barra1"
        alturaBarra2 = rectTransformPanel2.rect.height; //altura da imagem "barra2"

        rectTransformImgBarra.sizeDelta = new Vector2(larguraBarra, alturaBarra); // modifica altura e largura da barra
        rectTransformImgBarra2.sizeDelta = new Vector2(larguraBarra2, alturaBarra2); // modifica altura e largura da barra2

        bloco.preferredHeight = rectTransformPanel2.rect.height + rectTransformPanel.rect.height + 97; //modifica o tamanho do bloco

    }
}
