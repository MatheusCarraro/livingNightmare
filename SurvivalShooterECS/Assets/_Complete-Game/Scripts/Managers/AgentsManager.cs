using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Newtonsoft.Json;

public class AgentsManager : MonoBehaviour
{
    public static AgentsManager instance = null;
    public List<GameObject> activeAgents = new List<GameObject>();

    public string emotionList;

    public static Color agentColor;



   void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addActiveAgents (GameObject gameObject) {
        activeAgents.Add(gameObject);
    }

    public void removeActiveAgents (GameObject gameObject) {
        activeAgents.Remove(gameObject);
    }

    public void updateEmotionalState () {
        var engine = Manager.instance.engine;
        foreach (var item in activeAgents)
        {
            // Debug.Log(item.name);
            if (item.name == "ZomBunny(Clone)") {
                var agente = "coelho" + item.GetInstanceID();
                engine.SetGlobalValue("id", agente);
                var msgJson = engine.Evaluate("estadoEmocional('coelho')");
                if (msgJson.ToString() != "[]"){
                    setAttributesGain(item, msgJson.ToString());
                }
            }
            if (item.name == "ZomBear(Clone)") {
                var agente = "urso" + item.GetInstanceID();
                engine.SetGlobalValue("id", agente);
                var msgJson = engine.Evaluate("estadoEmocional('urso')");
                if (msgJson.ToString() != "[]"){
                    setAttributesGain(item, msgJson.ToString());
                }
            }
            if (item.name == "Hellephant(Clone)") {
                var agente = "elefante" + item.GetInstanceID();
                engine.SetGlobalValue("id", agente);
                var msgJson = engine.Evaluate("estadoEmocional('elefante')");
                if (msgJson.ToString() != "[]"){
                    setAttributesGain(item, msgJson.ToString());
                }
            }
        }
    }

       private void setAttributesGain(GameObject gameObject, string json) {
        // Debug.Log($"setAttributesGain: {json}");
        string strongestEmo = setFuzzyInput(json);
        // Debug.Log(($"setAttributesGain strongestEmo: {strongestEmo}"));

        var movimento = FIS.Evaluate("movimento");
        var dano = FIS.Evaluate("dano");
        var veloAtaque = FIS.Evaluate("veloAtaque");
        Debug.Log($"Emoção {gameObject.name}: {json}");
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
    private string setFuzzyInput(string json) {
        string [] emotions = {"joy", "relief", "distress", "disappointment", "fear-confirmed", "anger", "satisfaction"};
        // Debug.Log($"setFuzzyInput: {json}");

        var emotionList = JsonConvert.DeserializeObject<List<dynamic>>(json);
        
        foreach (var item in emotions)
        {
            var emoIntensity = emotionList.Find(x => x.name == item);

            if (emoIntensity is null) {
                FIS.SetInput(item, 0f);
            } else {
                FIS.SetInput(item, (float) emoIntensity.intensity);
                // Debug.Log($"Emoção {emoIntensity.name}: {emoIntensity.intensity}");
            }
        }
        
        return emotionList[0].name.ToString();
    }

    private void setCharactersColor(string strongestEmo) {
        // Debug.Log("MAIOR EMOÇÃO: "+strongestEmo);
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
            default:
                agentColor = new Color32(255, 255, 255, 255);
                break;
        }
    }

    public void agentIsDead(GameObject gameObject) {
        var engine = Manager.instance.engine;
        if (gameObject.name == "ZomBunny(Clone)") {
            // Crença = coelhoMortoUrso
            engine.Execute("gamygdalaEngine.appraiseBelief(0.75, 'jogador', ['coelhoMorrerUrso', 'jogadorPontuar'], [1, 1], true)");
            // Crença = coelhoMortoElefante
            engine.Execute("gamygdalaEngine.appraiseBelief(0.75, 'jogador', ['coelhoMorrerElefante'], [1], true)");
            Manager.coelho = 0;
        }

        if (gameObject.name == "ZomBear(Clone)") {
            // Crença = ursoMortoCoelho
            engine.Execute("gamygdalaEngine.appraiseBelief(0.9, 'jogador', ['ursoMorrerCoelho', 'jogadorPontuar'], [1, 1], true)");
            // Crença = ursoMortoElefante
            engine.Execute("gamygdalaEngine.appraiseBelief(0.75, 'jogador', ['coelhoMorrerElefante'], [1], true)");
            Manager.urso = 0;
            
        }

        if (gameObject.name == "Hellephant(Clone)") {
            Manager.elefante = 0;
        }

        removeActiveAgents(gameObject);
        updateEmotionalState();
    }

    public void agentTakeDamage(GameObject gameObject, CompleteProject.PlayerHealth playerHealth ) {
        var engine = Manager.instance.engine;
        // Debug.Log(gameObject.GetInstanceID());
        if (gameObject.name == "ZomBunny(Clone)") {
            // Seta globalmente o agente atacado
            var agente = "coelho" + gameObject.GetInstanceID();
            engine.SetGlobalValue("id", agente);

            if (playerHealth.currentHealth <= 30) {
                engine.Execute("appraise('vidaJogador30', 'coelho')");
            }

            engine.Execute("appraise('atacouJogador', 'coelho')");
            // Debug.Log($"{agente} atacou o jogador");
            var msgJson = engine.Evaluate("estadoEmocional('coelho')");
            // Debug.Log($"agentTakeDamage {msgJson}");
        }

        if (gameObject.name == "ZomBear(Clone)") {
            // Seta globalmente o agente atacado
            var agente = "urso" + gameObject.GetInstanceID();
            engine.SetGlobalValue("id", agente);

            engine.Execute("appraise('atacouJogador', 'urso')");
            // Debug.Log($"{agente} atacou o jogador");
            var msgJson = engine.Evaluate("estadoEmocional('urso')");

        }

        if (gameObject.name == "Hellephant(Clone)") {
            // Seta globalmente o agente atacado
            var agente = "elefante" + gameObject.GetInstanceID();
            engine.SetGlobalValue("id", agente);

            engine.Execute("appraise('atacouJogador', 'elefante')");
            // Debug.Log($"{agente} atacou o jogador");
            var msgJson = engine.Evaluate("estadoEmocional('elefante')");
        }

        updateEmotionalState();
    }

    public void agentAttack(GameObject gameObject) {
        var engine = Manager.instance.engine;
        if (gameObject.name == "ZomBunny(Clone)") {
            // Seta globalmente o agente atacado
            var agente = "coelho" + gameObject.GetInstanceID();
            engine.SetGlobalValue("id", agente);
            // Crença = serAtacadoCoelho
            engine.Execute("appraise('serAtacado', 'coelho')");

            if (Manager.elefante == 1) {
                // Crença = coelhoSofreDanoElefante
                engine.Execute("gamygdalaEngine.appraiseBelief(1, 'jogador', ['coelhoMorrerElefante'], [0.5], true)");
                // Debug.Log("Crença coelhoSofreDanoElefante acionada.");
            }
            // Crença = coelhoSofreDanoUrso
            engine.Execute("gamygdalaEngine.appraiseBelief(1, 'jogador', ['coelhoMorrerUrso', 'jogadorPontuar'], [0.5, 0.5], true)");
            // Debug.Log("Crença coelhoSofreDanoUrso acionada.");

            // // Printa no console o estado emocional do coelho
            // var msgJson = engine.Evaluate("estadoEmocional('coelho')");
            // Manager.setAttributesGain(enemyHealth.gameObject, msgJson.ToString());
        }

        if (gameObject.name == "ZomBear(Clone)") {
            // Seta globalmente o agente atacado
            var agente = "urso" + gameObject.GetInstanceID();
            engine.SetGlobalValue("id", agente);

            // Crença = serAtacadoUrso
            engine.Execute("appraise('serAtacado', 'urso')");

            // Crença = ursoSofreDanoCoelho
            engine.Execute("gamygdalaEngine.appraiseBelief(1, 'jogador', ['coelhoMorrerUrso', 'jogadorPontuar'], [0.5, 0.5], true)");
            // Debug.Log("Crença ursoSofreDanoCoelho acionada.");

            if (Manager.elefante == 1) {
                // Crença = ursoSofreDanoElefante
                engine.Execute("gamygdalaEngine.appraiseBelief(1, 'jogador', ['ursoMorrerElefante'], [0.5], true)");
                // Debug.Log("Crença ursoSofreDanoElefante acionada.");
            }
            // // Printa no console o estado emocional do coelho
            // var msgJson = engine.Evaluate("estadoEmocional('urso')");
            // Manager.setAttributesGain(enemyHealth.gameObject, msgJson.ToString());
        }

        if (gameObject.name == "Hellephant(Clone)") {
            // Seta globalmente o agente atacado
            var agente = "elefante" + gameObject.GetInstanceID();
            engine.SetGlobalValue("id", agente);
            // Crença = serAtacadoElefante
            engine.Execute("appraise('serAtacado', 'elefante')");

            // var msgJson = engine.Evaluate("estadoEmocional('elefante')");
            // Manager.setAttributesGain(enemyHealth.gameObject, msgJson.ToString());
        }
        updateEmotionalState();
    }
}