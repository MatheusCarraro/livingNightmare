using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuzzification : FIS
{   

    // Start is called before the first frame update
    void Start()
    {
        FIS.Initialize();
        FIS.SetInput("alegria", 0f);
        FIS.SetInput("alivio", 0f);
        FIS.SetInput("angustia", 0f);
        FIS.SetInput("frustracao", 0f);
        FIS.SetInput("medosConfirmados", 0f);
        FIS.SetInput("raiva", 0f);
        FIS.SetInput("satisfacao", 0.75f);
        Debug.Log("Movimento " + FIS.Evaluate("movimento"));
        Debug.Log("Dano " + FIS.Evaluate("dano"));
        Debug.Log("VeloAtaque " + FIS.Evaluate("veloAtaque"));
    }


    // Update is called once per frame
    void Update()
    {

    }

    void calculaFuzzy(){
        
    }
}

