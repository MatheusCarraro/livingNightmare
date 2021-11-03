using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.AI;

public class Manager : MonoBehaviour
{
    public static Manager instance = null;

    public static int coelho;
    
    public static int urso;

    public static int elefante;

    public static Color agentColor;

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
        var gamygdalaAddress = Application.persistentDataPath + "/Gamygdala.js";
        engine.ExecuteFile(@gamygdalaAddress);
        engine.SetGlobalValue("console", new Jurassic.Library.FirebugConsole(engine));
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time >= update) {            
            engine.Execute("gamygdalaEngine.decayAll()");
            update += interval;
        }
    }

    public static void setAttributesGain(GameObject gameObject, string json) {
        string strongestEmo = setFuzzyInput(json);

        var movimento = FIS.Evaluate("movimento");
        var dano = FIS.Evaluate("dano");
        var veloAtaque = FIS.Evaluate("veloAtaque");
        Debug.Log($"Emoção: {json}");
        Debug.Log($"movimento: {movimento}");
        Debug.Log($"dano: {dano}");
        Debug.Log($"veloAtaque: {veloAtaque}");


        gameObject.GetComponent<CompleteProject.EnemyAttack>().attackDamage = gameObject.GetComponent<CompleteProject.EnemyAttack>().attackDamageDefault * dano;
        gameObject.GetComponent<CompleteProject.EnemyAttack>().timeBetweenAttacks = gameObject.GetComponent<CompleteProject.EnemyAttack>().timeBetweenAttacks / veloAtaque;
        gameObject.GetComponent<NavMeshAgent>().speed = 3 * movimento;
        gameObject.GetComponent<NavMeshAgent>().angularSpeed = 120 * movimento;
        gameObject.GetComponent<NavMeshAgent>().acceleration = 8 * movimento;
        setCharactersColor(strongestEmo);
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].SetColor("_EmissionColor", agentColor);
        gameObject.GetComponentInChildren<Light>().color = agentColor;
    }
    public static string setFuzzyInput(string json) {
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
        
        return emotionList[0].name.ToString();
    }

    public static void setCharactersColor(string strongestEmo) {


        switch (strongestEmo)
        {
            case "joy":
                agentColor = new Color32(255, 255, 84, 255);
                break;
            case "relief":
                agentColor = new Color32(89, 189, 255, 255);
                break;
            case "distress":
                agentColor = new Color32(0, 0, 199, 255);
                break;
            case "disappointment":
                agentColor = new Color32(140, 140, 255, 255);
                break;
            case "fear-confirmed":
                agentColor = new Color32(0, 128, 0, 255);
                break;
            case "anger":
                agentColor = new Color32(255, 0, 0, 255);
                break;
            case "satisfaction":
                agentColor = new Color32(255, 232, 84, 255);
                break;
        }
    }
}
