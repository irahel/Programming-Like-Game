using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Bloco : MonoBehaviour {

    public string nomeBloco;
    public GameObject objBloco, objPainel;
    public RectTransform recBloco;
    private bool blocoVerificado = false;
    [SerializeField]
    private InputField entrada;
    [SerializeField]
    private Dropdown dropdown;

    public string getEntrada(){
        return entrada.text;
    }

    public string nomedoBloco() {
        return this.nomeBloco;
    }
    
    public GameObject meuPainel() {
        return this.objPainel;
    }

    public bool getBlocoVerificado() {
        return this.blocoVerificado;
    }

    public string getTipoVariavel()
    {
        return dropdown.options[dropdown.value].text;
    }

    public void setBlocoVerificado(bool isVerificado){
        this.blocoVerificado = isVerificado;
    }
}
