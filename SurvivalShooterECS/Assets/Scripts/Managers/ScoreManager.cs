using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static int score;


    Text text;


    void Awake ()
    {
        text = GetComponent <Text> ();
        score = 0;

        var engine = new Jurassic.ScriptEngine();
        Debug.Log(engine.Evaluate("5 * 10 + 2"));
    }


    void Update ()
    {
        text.text = "Score: " + score;
    }
}
