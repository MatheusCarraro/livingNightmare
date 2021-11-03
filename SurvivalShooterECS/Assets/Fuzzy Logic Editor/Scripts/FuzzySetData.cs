using System.Collections;
using System.Collections.Generic;
using AForge.Fuzzy;
using UnityEngine;

[System.Serializable]
public class FuzzySetData {
    //Info for Linguistic Variable
    public string name;
    public float[] values;

    //Info for Editor
    public bool showInInspector, validName;
    public AnimationCurve graph, tempGraph;
    int linguisticVariableID;
    public LinguisticVariableData linguisticVariable {
        get { return linguisticVariableID >= 0 ? FIS.flData.linguisticVariables[linguisticVariableID] : null; }
        set {
            if (linguisticVariableID < 0) {
                linguisticVariableID = FIS.flData.linguisticVariables.IndexOf(value);
                tempGraph = GetLineGraph();
                graph = GetLineGraph();
            } else {
                linguisticVariableID = FIS.flData.linguisticVariables.IndexOf(value);
            }
        }
    }

    public MembershipFunctionType _membershipFunction;
    public MembershipFunctionType membershipFunction {
        get { return _membershipFunction; }
        set {
            if (value != _membershipFunction) {
                _membershipFunction = value;
                switch (_membershipFunction) {
                    case MembershipFunctionType.TrapezoidalLeft:
                    case MembershipFunctionType.TrapezoidalRight:
                        values = new float[2];
                        break;
                    case MembershipFunctionType.Triangular:
                        values = new float[3];
                        break;
                    case MembershipFunctionType.Trapezoidal:
                        values = new float[4];
                        break;
                }
                if (linguisticVariable != null) {
                    tempGraph = GetLineGraph();
                    graph = GetLineGraph();
                }
            }
        }
    }

    public FuzzySetData() : this("NewFuzzySet", MembershipFunctionType.Triangular) {}

    public FuzzySetData(string name) : this(name, MembershipFunctionType.Triangular) {}

    public FuzzySetData(string name, MembershipFunctionType membershipFunction) {
        this.name = name;
        this.linguisticVariableID = -1;
        this._membershipFunction = membershipFunction;
        
        switch (this._membershipFunction) {
            case MembershipFunctionType.TrapezoidalLeft:
            case MembershipFunctionType.TrapezoidalRight:
                this.values = new float[2];
                break;
            case MembershipFunctionType.Triangular:
                this.values = new float[3];
                break;
            case MembershipFunctionType.Trapezoidal:
                this.values = new float[4];
                break;
        }
        
        this.showInInspector = true;
    }

    public FuzzySet GetFuzzySet() {
        if (!validName) return null;

        TrapezoidalFunction tf = null;

        switch (this.membershipFunction) {
            case MembershipFunctionType.TrapezoidalLeft:
                tf = new TrapezoidalFunction(values[0], values[1], TrapezoidalFunction.EdgeType.Left);
                break;
            case MembershipFunctionType.TrapezoidalRight:
                tf = new TrapezoidalFunction(values[0], values[1], TrapezoidalFunction.EdgeType.Right);
                break;
            case MembershipFunctionType.Triangular:
                tf = new TrapezoidalFunction(values[0], values[1], values[2]);
                break;
            case MembershipFunctionType.Trapezoidal:
                tf = new TrapezoidalFunction(values[0], values[1], values[2], values[3]);
                break;
        }

        return new FuzzySet(name, tf);
    }

    public AnimationCurve GetLineGraph() {
        AnimationCurve line = new AnimationCurve();
        Keyframe[] keys = new Keyframe[values.Length + 2];
        keys[0] = new Keyframe(linguisticVariable.start - linguisticVariable.length, membershipFunction == MembershipFunctionType.TrapezoidalRight ? 1 : 0);
        for (int i = 1; i < keys.Length - 1; ++i) {
            float x = (i * (linguisticVariable.length / (keys.Length - 1))) + linguisticVariable.start, y = 0;
            if (i == 1) {
                y = membershipFunction == MembershipFunctionType.TrapezoidalRight ? 1 : 0;
            } else if (i == keys.Length - 2) {
                y = membershipFunction == MembershipFunctionType.TrapezoidalLeft ? 1 : 0;
            } else {
                y = 1;
            }
            keys[i] = new Keyframe(x, y);
        }
        keys[keys.Length - 1] = new Keyframe(linguisticVariable.end + linguisticVariable.length, membershipFunction == MembershipFunctionType.TrapezoidalLeft ? 1 : 0);

        line.keys = keys;

        return line;
    }

    public AnimationCurve NormalizeLineGraph() {
        AnimationCurve line = new AnimationCurve();

        Keyframe[] keys = new Keyframe[values.Length + 2];
        keys[0] = new Keyframe(linguisticVariable.start - linguisticVariable.length, membershipFunction == MembershipFunctionType.TrapezoidalRight ? 1 : 0);

        for (int i = 1; i < keys.Length - 1; ++i) {
            float x = (((graph.keys[i].time - linguisticVariable.oldStart) / linguisticVariable.oldLength) * linguisticVariable.length) + linguisticVariable.start, y = 0;

            if (i == 1) {
                y = membershipFunction == MembershipFunctionType.TrapezoidalRight ? 1 : 0;
            } else if (i == keys.Length - 2) {
                y = membershipFunction == MembershipFunctionType.TrapezoidalLeft ? 1 : 0;
            } else {
                y = 1;
            }
            keys[i] = new Keyframe(x, y);
        }

        keys[keys.Length - 1] = new Keyframe(linguisticVariable.end + linguisticVariable.length, membershipFunction == MembershipFunctionType.TrapezoidalLeft ? 1 : 0);

        line.keys = keys;

        return line;
    }

    public FuzzySetData Copy() {
        FuzzySetData fsd = new FuzzySetData(name + "Copy", membershipFunction);
        fsd.linguisticVariable = linguisticVariable;
        for (int i = 1; i < values.Length + 1; ++i) {
            fsd.values[i - 1] = values[i - 1];
            Keyframe k = fsd.tempGraph.keys[i];
            fsd.tempGraph.RemoveKey(i);
            fsd.tempGraph.AddKey(fsd.values[i - 1], k.value);
        }
        fsd.graph.keys = fsd.tempGraph.keys;
        return fsd;
    }
}

[System.Serializable]
public enum MembershipFunctionType {
    Triangular,
    Trapezoidal,
    TrapezoidalLeft,
    TrapezoidalRight
}