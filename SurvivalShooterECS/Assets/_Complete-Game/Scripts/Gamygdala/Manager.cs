using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Manager : MonoBehaviour
{
    public static Manager instance = null;

    public static int coelho;
    
    public static int urso;

    public static int elefante;

    public Jurassic.ScriptEngine engine = new Jurassic.ScriptEngine();
    float update;
    int interval;
    


    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        update = 0.0f;
        interval = 2;
        coelho = 0;
        urso = 0;
        elefante = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        engine.ExecuteFile(@"Assets\_Complete-Game\Scripts\Gamygdala\Gamygdala.js");
        engine.SetGlobalValue("console", new Jurassic.Library.FirebugConsole(engine));
        // engine.Execute("gamygdalaEngine.setDecay(0)");
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time >= update) {            
            engine.Execute("gamygdalaEngine.decayAll()");

            update += interval;
        }
    }

    public static void setAttributesGain(string id, string json) {
        setFuzzyInput(json);

        var movimento = FIS.Evaluate("movimento");
        var dano = FIS.Evaluate("dano");
        var veloAtaque = FIS.Evaluate("veloAtaque");

        Debug.Log($"Movimento: {movimento}");
        Debug.Log($"Dano: {dano}");
        Debug.Log($"Velocidade de Ataque: {veloAtaque}");
    }
    public static void setFuzzyInput(string json) {
        string [] emotions = {"joy", "relief", "distress", "disappointment", "fear-confirmed", "anger", "satisfaction"};

        var emotionList = JsonConvert.DeserializeObject<List<dynamic>>(json);
        
        foreach (var item in emotions)
        {
            var emoIntensity = emotionList.Find(x => x.name == item);

            if (emoIntensity is null) {
                FIS.SetInput(item, 0f);
            } else {
                FIS.SetInput(item, (float) emoIntensity.intensity);
            }
        }
    }
}
