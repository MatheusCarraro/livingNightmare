using System;
using System.Collections;
using System.Collections.Generic;
using AForge.Fuzzy;
using UnityEngine;
using Utilities;

public class FIS : SingletonMonoBehaviour<FIS> {

    public static FuzzyLogicData flData;
    InferenceSystem IS;

    public bool initializeOnAwake;

    public override void Awake() {
        base.Awake();

        if (initializeOnAwake)
            Initialize();
    }

    public void LoadFuzzyLogicData() {
        flData = Resources.Load<FuzzyLogicData>("Fuzzy Logic Data");

        if (!flData)
            throw new Exception("Fuzzy Logic Data not defined!");
    }

    public static void Initialize() {
        LoadInferenceSystem();
        LoadRules();
    }

    public static void LoadInferenceSystem() {
        instance.LoadFuzzyLogicData();

        Database fuzzyDB = new Database();

        foreach (LinguisticVariableData lvd in flData.linguisticVariables) {
            fuzzyDB.AddVariable(lvd.GetLinguisticVariable());
        }

        instance.IS = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000));
    }

    public static void LoadRules() {
        if (instance.IS == null)
            throw new Exception("Inference System not defined!");

        foreach (RuleData rd in flData.rules) {
            instance.IS.NewRule(rd.id, rd.ToString());
        }
    }

    public static void SetInput(string variableName, float value) {
        instance.IS.SetInput(variableName, value);
    }

    public static float Evaluate(string variableName) {
        try {
            return instance.IS.Evaluate(variableName);
        } catch {
            return flData.GetLinguisticVariable(variableName).start;
        }
    }
}