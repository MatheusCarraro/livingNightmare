using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuleData {

    public string id;
    public List<ClauseData> antecedents;
    public ClauseData consequent;

    //Info for Editor
    public bool showInInspector;

    public RuleData() {
        antecedents = new List<ClauseData>();
    }

    public void AddAntecedent(ClauseData newClause) {
        antecedents.Add(newClause);
    }

    public override string ToString() {
        if (antecedents == null || consequent == null)
            return string.Empty;

        string antecedentsString = string.Empty;

        foreach (ClauseData cd in antecedents)
            antecedentsString += cd.ToString();

        return "IF " + antecedentsString + " THEN " + consequent.ToString();
    }

    public RuleData Copy() {
        RuleData rd = new RuleData();

        foreach (ClauseData cd in antecedents)
            rd.antecedents.Add(cd.Copy());

        rd.consequent = consequent.Copy();
        rd.showInInspector = true;
        return rd;
    }
}