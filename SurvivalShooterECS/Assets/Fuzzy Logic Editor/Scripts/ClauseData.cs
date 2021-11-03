using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClauseData {
    public bool not, isBlock;
    public OperatorType operatorToNext;
    public int linguisticVariableID;
    public LinguisticVariableData linguisticVariable {
        get { return linguisticVariableID >= 0 ? FIS.flData.linguisticVariables[linguisticVariableID] : null; }
        set {
            linguisticVariableID = FIS.flData.linguisticVariables.IndexOf(value);
        }
    }
    
    public int fuzzySetID;
    public FuzzySetData fuzzySet {
        get { return fuzzySetID >= 0 ? linguisticVariable.fuzzySets[fuzzySetID] : null; }
        set {
            fuzzySetID = linguisticVariable != null ? linguisticVariable.fuzzySets.IndexOf(value) : -1;
        }
    }
    public List<ClauseData> clauses;

    //Info for Editor
    public bool showInInspector, hover;

    public ClauseData(bool isBlock = false) : this(isBlock, null, null) {}

    public ClauseData(bool isBlock, LinguisticVariableData linguisticVariable, FuzzySetData fuzzySet) : base() {
        this.linguisticVariable = linguisticVariable;
        this.fuzzySet = fuzzySet;
        this.isBlock = isBlock;
        if (isBlock) {
            clauses = new List<ClauseData>();
        }
        not = false;
        operatorToNext = OperatorType.None;
    }

    public void AddAntecedent(ClauseData newClause) {
        clauses.Add(newClause);
    }

    public override string ToString() {
        string OPERATOR = (operatorToNext != OperatorType.None ? " " + operatorToNext.ToString() + " " : "");
        if (isBlock) {
            if (clauses.Count == 0)
                return string.Empty;

            string block = string.Empty;

            foreach (ClauseData cd in clauses) {
                block += cd.ToString();
            }

            if (clauses.Count > 1 || not)
                block = "( " + block + " )";

            return (not ? "NOT " : "") + block + OPERATOR;
        } else {
            string IS = " IS " + (not ? "NOT " : "");

            return linguisticVariable.name + IS + fuzzySet.name + OPERATOR;
        }
    }

    public ClauseData Copy() {
        ClauseData cd = new ClauseData(isBlock, linguisticVariable, fuzzySet);
        cd.not = not;
        cd.operatorToNext = operatorToNext;
        if (isBlock) {
            foreach (var iCd in clauses) {
                cd.clauses.Add(iCd.Copy());
            }
        }

        return cd;
    }
}

[System.Serializable]
public enum OperatorType {
    None,
    AND,
    OR
}