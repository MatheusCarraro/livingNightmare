using System.Collections;
using System.Collections.Generic;
using AForge.Fuzzy;
using UnityEngine;

[System.Serializable]
public class LinguisticVariableData {
    //Info for Linguistic Variable
    public string name;

    float _start, _end;
    public float start {
        get { return _start; }
        set {
            if (_start != value) {
                if (value == _end) value = _end - (length / 100f);

                oldStart = _start;
                oldEnd = _end;
                _start = Mathf.Min(_end, value);
                _end = Mathf.Max(_end, value);
                foreach (FuzzySetData fsd in fuzzySets) {
                    fsd.tempGraph = fsd.NormalizeLineGraph();
                    fsd.graph.keys = fsd.tempGraph.keys;
                }
            }
        }
    }

    public float end {
        get { return _end; }
        set {
            if (_end != value) {
                if (value == _start) value = _start + (length / 100f);

                oldStart = _start;
                oldEnd = _end;
                _end = Mathf.Max(_start, value);
                _start = Mathf.Min(_start, value);
                foreach (FuzzySetData fsd in fuzzySets) {
                    fsd.tempGraph = fsd.NormalizeLineGraph();
                    fsd.graph.keys = fsd.tempGraph.keys;
                }
                oldStart = _start;
                oldEnd = _end;
            }
        }
    }

    public float length {
        get {
            return _end - _start;
        }
    }

    public List<FuzzySetData> fuzzySets;

    //Info for Editor
    public bool showInInspector, validName;
    public float oldStart, oldEnd;

    public float oldLength {
        get {
            return oldEnd - oldStart;
        }
    }

    public LinguisticVariableData() : this("NewLinguisticVariable", 0, 100) {}

    public LinguisticVariableData(string name) : this(name, 0, 100) {}

    public LinguisticVariableData(string name, float start, float end) {
        this.fuzzySets = new List<FuzzySetData>();
        this.name = name;
        this.oldStart = this.start = start;
        this.oldEnd = this.end = end;
        this.showInInspector = true;
    }

    public void AddFuzzySet(FuzzySetData fsd) {
        fsd.linguisticVariable = this;
        fuzzySets.Add(fsd);
    }

    public void ClearFuzzySets() {
        fuzzySets.RemoveRange(1, fuzzySets.Count - 1);
    }

    public LinguisticVariable GetLinguisticVariable() {
        if (!validName) return null;

        LinguisticVariable lv = new LinguisticVariable(name, start, end);

        foreach (FuzzySetData fsd in fuzzySets) {
            lv.AddLabel(fsd.GetFuzzySet());
        }

        return lv;
    }

    public bool ValidateFuzzySetName(string name) {

        List<FuzzySetData> fsdMatch = fuzzySets.FindAll(fsd => fsd.name == name);

        return fsdMatch.Count <= 1;
    }

    public LinguisticVariableData Copy() {
        LinguisticVariableData lvd = new LinguisticVariableData(name + "Copy", start, end);
        FIS.flData.linguisticVariables.Add(lvd);
        foreach (FuzzySetData fsd in fuzzySets) {
            FuzzySetData newFsd = fsd.Copy();
            string removeString = "Copy";
            int index = newFsd.name.IndexOf(removeString);
            newFsd.name = newFsd.name.Remove(index, removeString.Length);
            lvd.AddFuzzySet(newFsd);
        }
        return lvd;
    }

    public string[] GetFuzzySetNames() {
        string[] names = new string[fuzzySets.Count];

        for (int i = 0; i < names.Length; ++i) {
            names[i] = fuzzySets[i].name;
        }

        return names;
    }
}