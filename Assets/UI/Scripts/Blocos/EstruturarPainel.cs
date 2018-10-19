using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EstruturarPainel : MonoBehaviour {

    public RectTransform pan;
    public GameObject pn;
 
    private string sent = "";
    private ArrayList ordenador,cochete;
    List<Bloco> aVerificar;
    List<int> teste;
    Bloco bl;
    public Bloco blocoVirgular;

	public GameObject plat;

    public string getString()
    {
        return sent;
    }

    private void Start()
    {
        teste = new List<int>();
        aVerificar = new List<Bloco>();
        ordenador = new ArrayList();
        cochete = new ArrayList();
        bl = new Bloco();
        //blocoVirgular = new Bloco();
    }

    public void verificar(){
        this.sent = "";
        foreach (Bloco obj in (pn.GetComponentsInChildren<Bloco>())){
            aVerificar.Add(obj);
        }

        //teste
        int kk = pan.GetChildCount();
        for (int d = 0; d < kk ; d++)
        {
            teste.Add(1);
        }

        montarE();	
        aVerificar.Clear();
        cochete.Clear();

		plat.GetComponent<DoorActivator> ().getEntry (this.sent);
		//plat.GetComponent<PlattaformTest> ().compiller (this.sent);
		Debug.Log ("Final");
		Debug.Log (this.sent);
		//Debug.Log ("Correct");
		//Debug.Log ("while(true){move(left,1);}");
    }

    private void montarE(){
        Stack blocos = new Stack();
        int numeroCochetes = 0;
        int numeroFilhos = 0;
        

        for (int i = 0; i < aVerificar.Count; i++){
            /*
             * ====ENQUANTO====
             * VERIFICA E CONDICIONA AS PROPRIEDADES PARA O ENQUANTO
             */

            if (aVerificar[i].nomedoBloco().Equals("enquanto")) {

                sent += "while("+ aVerificar[i].getEntrada() +"){";                
                numeroCochetes += 1;

                if (aVerificar[i].GetComponent<RectTransform>().GetChild(1).GetChild(1).childCount > 0){
                    numeroFilhos += aVerificar[i].GetComponent<RectTransform>().GetChild(1).GetChild(1).childCount;
                    Debug.Log("Numero de Filhos " + numeroFilhos.ToString());
                    if (i != 0) {
                        numeroFilhos--;
                    }

                } else if (aVerificar[i].GetComponent<RectTransform>().GetChild(1).GetChild(1).childCount == 0){
                    sent += "}";
                    numeroFilhos--;
                    numeroCochetes--;
                }
            /*
             * ====VARIAVEL====
             * VERIFICA E CONDICIONA AS PROPRIEDADES PARA VARIAVEL
             */
            }
            else if (aVerificar[i].nomedoBloco().Equals("var")){
                sent += "declaracao(" + aVerificar[i].getTipoVariavel() + ":" + aVerificar[i].getEntrada() + ");";            
                numeroFilhos--;
            }
			else if (aVerificar[i].nomedoBloco().Equals("mova")){
				sent += "move(" + aVerificar[i].getTipoVariavel().ToLower() + "," + aVerificar[i].getEntrada() + ")";            
				numeroFilhos--;
			}

            /*
             * ====COCHETE====
             * FECHA COCHETE DE UM CONJUNTO OU ESCOPO 
             */
            //Debug.Log("Numero de Filhos " + numeroFilhos.ToString());
            if (numeroFilhos == 0){
                for (int cont = 0; cont < numeroCochetes; cont++){
                    sent += "}";
                }
                
                numeroFilhos = 0;
                numeroCochetes = 0;
            }
           
            //Debug.Log(sent);
        }
    }

   private void montarEs()
    {
        int numeroCochetes = 0;
        int numeroFilhos = 0;

       if(teste[0] == 0 && teste != null){
            teste.RemoveAt(0);
        }
            //llll
            for (int i = 0; i < aVerificar.Count; i++)
            {
                /*
                 * ====ENQUANTO====
                 * VERIFICA E CONDICIONA AS PROPRIEDADES PARA O ENQUANTO
                 */

                if (aVerificar[i].nomedoBloco().Equals("enquanto"))
                {

                    sent += "enquanto(" + aVerificar[i].getEntrada() + "){";
                    numeroCochetes += 1;

                    if (aVerificar[i].GetComponent<RectTransform>().GetChild(1).GetChild(1).childCount > 0)
                    {
                    teste[0] += aVerificar[i].GetComponent<RectTransform>().GetChild(1).GetChild(1).childCount;
                        Debug.Log("Numero de Filhos " + numeroFilhos.ToString());
                        if (i >= 0)
                        {
                        teste[0]--;
                           
                        }

                    }
                    else if (aVerificar[i].GetComponent<RectTransform>().GetChild(1).GetChild(1).childCount == 0)
                    {
                        sent += "}";
                   // teste[0]--;
                    numeroCochetes--;
                    }
                    /*
                     * ====VARIAVEL====
                     * VERIFICA E CONDICIONA AS PROPRIEDADES PARA VARIAVEL
                     */
                }
                else if (aVerificar[i].nomedoBloco().Equals("var"))
                {
                    sent += "declaracao(" + aVerificar[i].getTipoVariavel() + ":" + aVerificar[i].getEntrada() + ");";
                teste[0]--;
            }

                /*
                 * ====COCHETE====
                 * FECHA COCHETE DE UM CONJUNTO OU ESCOPO 
                 */
                //Debug.Log("Numero de Filhos " + numeroFilhos.ToString());
                if (teste[0] == 0)
                {
                    for (int cont = 0; cont < numeroCochetes; cont++)
                    {
                        sent += "}";
                    }

                    //numeroFilhos = 0;
                    numeroCochetes = 0;
                }

               // Debug.Log(sent);
            }

        

   }

    private void motarEstrutura(Bloco est){

        Debug.Log(sent);
        /*
         * VERIFICA QUAL E O TIPO DE BLOCO
         */
        if (est.nomedoBloco().Equals("enquanto")) {
            sent += est.nomedoBloco()+"(){";

            Debug.Log(est.meuPainel().GetComponentsInChildren<Bloco>().Length);
            foreach (Bloco obj in (est.meuPainel().GetComponentsInChildren<Bloco>())){
                Debug.Log(obj.nomedoBloco());
                ordenador.Add(obj);
             }
           
            if (ordenador.Count > 0){
                for (int i = ordenador.Count - 1; 0 <= i; i--){
                    aVerificar.Add((Bloco)ordenador[i]);
                }
                ordenador.Clear();
            }
            
            

        } else if (est.nomedoBloco().Equals("se") ) {
            sent += est.nomedoBloco();
            
            foreach (Bloco obj in (est.meuPainel().GetComponentsInChildren<Bloco>())){
                ordenador.Add(obj);
             }

            if (ordenador.Count > 0){
                for (int i = ordenador.Count - 1; 0 <= i; i--){
                    aVerificar.Add((Bloco)ordenador[i]);
                }
                ordenador.Clear();
            };
            est.setBlocoVerificado(true);


        } else if (est.nomedoBloco().Equals("virgula")){
            sent += "}";
            aVerificar.RemoveAt(aVerificar.Count - 1);
        }

        

        if ( aVerificar.Count > 0){

            
            bl = (Bloco)aVerificar[aVerificar.Count - 1];
            
            aVerificar.RemoveAt(aVerificar.Count - 1);
            if (!bl.nomedoBloco().Equals("var") && !bl.nomedoBloco().Equals("virgula"))
                aVerificar.Add(blocoVirgular);//Adicionando um bloco quer dizer fecha virgular do enquanto;

            motarEstrutura(bl);

        }
        else{
            Debug.Log("Teste=" + sent);
            aVerificar.Clear();
        }
            
    }
}
