using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fuzzy Logic Data", menuName = "Fuzzy Logic/Fuzzy Logic Data", order = 1)]
public class FuzzyLogicData : ScriptableObject {

    public List<LinguisticVariableData> linguisticVariables;
    public List<RuleData> rules;

    public void Initialize() {
        if (linguisticVariables == null)
            linguisticVariables = new List<LinguisticVariableData>();
        if (rules == null)
            rules = new List<RuleData>();
    }

    public bool ValidateLinguisticVariableName(string name) {

        List<LinguisticVariableData> lvdMatch = linguisticVariables.FindAll(lvd => lvd.name == name);

        return lvdMatch.Count <= 1;
    }

    public LinguisticVariableData GetLinguisticVariable(string name) {
        return linguisticVariables.Find(lv => lv.name == name);
    }

    public string[] GetVariablesNames() {
        string[] names = new string[linguisticVariables.Count];

        for (int i = 0; i < names.Length; ++i) {
            names[i] = linguisticVariables[i].name;
        }

        return names;
    }
}