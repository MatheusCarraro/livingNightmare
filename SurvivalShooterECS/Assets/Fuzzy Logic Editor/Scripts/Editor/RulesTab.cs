using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class RulesTab : EditorTab<FuzzyLogicEditor> {

    Vector2 scrollPosRV,
    scrollPosANT;
    RuleData selectedRD;
    GUIStyle toolbarHoverStyle,
    toolbarNormalStyle,
    fontStyle;

    Rect area;

    float wRL,
    wR;

    public RulesTab(FuzzyLogicEditor editor) : base(editor) {}

    public override void Draw() {
        fontStyle = new GUIStyle(EditorStyles.label);
        fontStyle.fontSize = 16;
        fontStyle.fixedHeight = 32;
        fontStyle.alignment = TextAnchor.UpperCenter;
        toolbarNormalStyle = new GUIStyle(EditorStyles.toolbarButton);
        toolbarNormalStyle.onHover.background = Resources.Load<Texture2D>("EditorUI/toolbar button_Blue");
        toolbarHoverStyle = new GUIStyle(EditorStyles.toolbarButton);
        toolbarHoverStyle.onNormal.background = Resources.Load<Texture2D>("EditorUI/toolbar button_Blue");
        toolbarHoverStyle.hover.background = Resources.Load<Texture2D>("EditorUI/toolbar button_Blue");
        toolbarHoverStyle.onHover.background = Resources.Load<Texture2D>("EditorUI/toolbar button_Blue");

        area = new Rect(0, EditorStyles.toolbar.fixedHeight, editor.position.width, editor.position.height - EditorStyles.toolbar.fixedHeight);
        wRL = (area.width / 3) + 2;
        wR = (area.width * (2f / 3)) - 2;
        GUILayout.Space(10);
        GUI.BeginGroup(area);
        EditorGUILayout.BeginHorizontal();

        DrawRulesList();
        DrawRule();

        EditorGUILayout.EndHorizontal();
        GUI.EndGroup();
        Handles.BeginGUI();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(wRL, EditorStyles.toolbar.fixedHeight), new Vector2(wRL, editor.position.height));
        Handles.DrawLine(new Vector2(wRL, area.height * (2f / 3) + 17), new Vector2(wRL + wR, area.height * (2f / 3) + 17));
        Handles.color = Color.white;
        Handles.EndGUI();
    }

    void DrawRulesList() {
        selectedRD = null;

        Rect r = EditorGUILayout.BeginVertical(GUILayout.Width(wRL), GUILayout.Height(area.height));

        EditorGUILayout.LabelField("Rules", fontStyle);

        GUILayout.Space(10);

        if (FIS.flData.linguisticVariables.Count < 1) {
            EditorGUILayout.HelpBox("Add Linguistic Variables.", MessageType.Warning);
        } else {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(wRL), GUILayout.Height(EditorStyles.toolbar.fixedHeight));

            if (GUILayout.Button("New", EditorStyles.toolbarButton)) {
                RuleData rd = new RuleData();
                //rd.antecedents = new ClauseData(true);
                //rd.antecedents.showInInspector = true;
                rd.consequent = new ClauseData();
                rd.consequent.linguisticVariable = FIS.flData.linguisticVariables[FIS.flData.linguisticVariables.Count - 1];
                rd.consequent.fuzzySet = rd.consequent.linguisticVariable.fuzzySets[0];
                FIS.flData.rules.Add(rd);
            }
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton)) {
                FIS.flData.rules.Clear();
            }

            EditorGUILayout.EndHorizontal();

            if (FIS.flData.rules.Count > 0) {
                GUILayout.BeginArea(new Rect(15, 55, wRL - 15, area.height - 55));

                scrollPosRV = EditorGUILayout.BeginScrollView(scrollPosRV, GUILayout.Width(wRL - 15));

                GUILayout.Space(10);

                for (int i = 0; i < FIS.flData.rules.Count; ++i) {
                    RuleData rd = FIS.flData.rules[i];

                    rd.id = "Rule " + i;
                    EditorGUILayout.BeginVertical(GUILayout.Width(wRL - 35));
                    EditorGUILayout.BeginHorizontal();
                    rd.showInInspector = GUILayout.Toggle(rd.showInInspector, Miscellaneous.Truncate(rd.id, (int) (wRL / 8.75f)), EditorStyles.toolbarButton);;
                    if (EditorGUILayout.DropdownButton(new GUIContent(EditorStyles.foldoutHeaderIcon.normal.background), FocusType.Passive, EditorStyles.toolbarButton, GUILayout.Width(30))) {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Delete"), false, DeleteRule, rd);
                        menu.AddItem(new GUIContent("Duplicate"), false, DuplicateRule, rd);
                        Vector2 position = Event.current.mousePosition;
                        menu.DropDown(new Rect(position.x, position.y, 0, 0));
                    }
                    EditorGUILayout.EndHorizontal();

                    if (rd.showInInspector) {

                        selectedRD = rd;
                        foreach (RuleData rD in FIS.flData.rules)
                            if (rD != rd) rD.showInInspector = false;
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndScrollView();
                GUILayout.EndArea();
            } else {
                EditorGUILayout.HelpBox("Add Rules Here...", MessageType.Info);
            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();
    }

    void DrawRule() {
        EditorGUILayout.BeginVertical(GUILayout.Width(wR), GUILayout.Height(area.height));

        EditorGUILayout.LabelField("Conditions", fontStyle);

        GUILayout.Space(10);

        DrawAntecedents();
        DrawConsequent();

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    void DrawAntecedents() {
        EditorGUILayout.BeginVertical(GUILayout.Width(wR), GUILayout.Height(area.height * (2f / 3) - 55));

        if (selectedRD != null) {

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(wR), GUILayout.Height(EditorStyles.toolbar.fixedHeight));

            if (GUILayout.Button("New", EditorStyles.toolbarButton)) {

                ClauseData cd = new ClauseData();
                cd.linguisticVariable = FIS.flData.linguisticVariables[0];
                cd.fuzzySet = cd.linguisticVariable.fuzzySets[0];
                //selectedRD.antecedents.showInInspector = true;
                selectedRD.AddAntecedent(cd);
            }
            if (GUILayout.Button("New (Block)", EditorStyles.toolbarButton)) {
                //selectedRD.antecedents.showInInspector = true;
                selectedRD.AddAntecedent(new ClauseData(true));
            }
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton)) {
                selectedRD.antecedents.Clear();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.BeginArea(new Rect(wRL + 15, 55, wR - 15, (area.height * (2f / 3)) - 55));

            scrollPosANT = EditorGUILayout.BeginScrollView(scrollPosANT);
            GUILayout.Space(10);

            for (int i = 0; i < selectedRD.antecedents.Count; ++i)
                DrawClauses(selectedRD.antecedents[i], selectedRD.antecedents);

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();

        } else {
            EditorGUILayout.HelpBox("Select an Rule...", MessageType.Info);
        }

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    void DrawConsequent() {

        EditorGUILayout.BeginVertical(GUILayout.Width(wR), GUILayout.Height(area.height * (1f / 3)));

        GUILayout.Space(30);

        EditorGUILayout.LabelField("Consequent", fontStyle);

        GUILayout.Space(10);

        if (selectedRD != null) {

            GUILayout.BeginArea(new Rect(wRL + 15, (area.height * (2f / 3)) + 45, wR - 30, area.height * (1f / 3) - 45));
            EditorGUILayout.BeginHorizontal();

            selectedRD.consequent = DrawSingleClause(selectedRD.consequent, null);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            EditorGUILayout.LabelField("Rule String", fontStyle);

            GUILayout.Space(10);
            GUILayout.TextField(selectedRD.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));;

            //if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            //    GUIUtility.keyboardControl = 0;

            GUILayout.EndArea();

        } else {
            EditorGUILayout.HelpBox("Select an Rule...", MessageType.Info);
        }

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

    }

    void DrawClauses(ClauseData cd, List<ClauseData> list) {
        EditorGUILayout.BeginVertical(GUILayout.Width(wR - 35));
        if (!cd.isBlock) {
            EditorGUILayout.BeginHorizontal();

            cd = DrawSingleClause(cd, list);

            EditorGUILayout.EndHorizontal();
        } else {
            EditorGUILayout.BeginHorizontal();
            cd.not = GUILayout.Toggle(cd.not, cd.not ? "NOT" : "NOT?", EditorStyles.toolbarButton, GUILayout.Width(50));

            cd.showInInspector = GUILayout.Toggle(cd.showInInspector, "(", cd.hover ? toolbarHoverStyle : toolbarNormalStyle);

            cd.hover = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);

            if (list != null && EditorGUILayout.DropdownButton(new GUIContent(EditorStyles.foldoutHeaderIcon.normal.background), FocusType.Passive, EditorStyles.toolbarButton, GUILayout.Width(30))) {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Add Clause"), false, AddClause, cd);
                menu.AddItem(new GUIContent("Add Block"), false, AddBlock, cd);
                menu.AddItem(new GUIContent("Delete"), false, DeleteClause, new ClauseInfo(cd, list));
                menu.AddItem(new GUIContent("Duplicate"), false, DuplicateClause, new ClauseInfo(cd, list));
                Vector2 position = Event.current.mousePosition;
                menu.DropDown(new Rect(position.x, position.y, 0, 0));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (cd.showInInspector) {
                for (int i = 0; i < cd.clauses.Count; ++i)
                    DrawClauses(cd.clauses[i], cd.clauses);
            }

            EditorGUILayout.BeginHorizontal();

            cd.showInInspector = GUILayout.Toggle(cd.showInInspector, ")", cd.hover ? toolbarHoverStyle : EditorStyles.toolbarButton);

            //cd.hover = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);

            if (list != null)
                if (list.Count == 1) {
                    cd.operatorToNext = OperatorType.None;
                } else if (list[list.Count - 1] != cd) {
                if (cd.operatorToNext == OperatorType.None)
                    cd.operatorToNext = OperatorType.AND;

                cd.operatorToNext = (OperatorType) EditorGUILayout.EnumPopup((Operator) cd.operatorToNext, EditorStyles.toolbarPopup, GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();

    }

    ClauseData DrawSingleClause(ClauseData cd, List<ClauseData> list) {
        int id = EditorGUILayout.Popup(cd.linguisticVariableID, FIS.flData.GetVariablesNames(), EditorStyles.toolbarPopup);
        LinguisticVariableData newLV = FIS.flData.linguisticVariables[id];

        if (GUILayout.Button(cd.not ? " IS NOT " : " IS ", EditorStyles.toolbarButton, GUILayout.Width(50))) {
            cd.not = !cd.not;
        }

        id = newLV != cd.linguisticVariable ? 0 : EditorGUILayout.Popup(newLV.fuzzySets.IndexOf(cd.fuzzySet), newLV.GetFuzzySetNames(), EditorStyles.toolbarPopup);

        cd.linguisticVariable = newLV;
        cd.fuzzySet = cd.linguisticVariable.fuzzySets[id];

        if (list != null) {
            if (list.Count == 1) {
                cd.operatorToNext = OperatorType.None;
            } else if (list[list.Count - 1] != cd) {
                if (cd.operatorToNext == OperatorType.None)
                    cd.operatorToNext = OperatorType.AND;

                cd.operatorToNext = (OperatorType) EditorGUILayout.EnumPopup((Operator) cd.operatorToNext, EditorStyles.toolbarPopup, GUILayout.Width(50));
            }

            if (EditorGUILayout.DropdownButton(new GUIContent(EditorStyles.foldoutHeaderIcon.normal.background), FocusType.Passive, EditorStyles.toolbarButton, GUILayout.Width(30))) {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, DeleteClause, new ClauseInfo(cd, list));
                menu.AddItem(new GUIContent("Duplicate"), false, DuplicateClause, new ClauseInfo(cd, list));
                Vector2 position = Event.current.mousePosition;
                menu.DropDown(new Rect(position.x, position.y, 0, 0));
            }
        }

        return cd;
    }

    void DeleteRule(object obj) {
        if (!(obj is RuleData)) return;

        RuleData rd = obj as RuleData;
        FIS.flData.rules.Remove(rd);
    }

    void DuplicateRule(object obj) {
        if (!(obj is RuleData)) return;

        RuleData rd = obj as RuleData;
        rd.showInInspector = false;
        FIS.flData.rules.Add(rd.Copy());
    }

    void AddClause(object obj) {
        if (!(obj is ClauseData)) return;

        ClauseData cd = obj as ClauseData;
        cd.showInInspector = true;
        ClauseData newCd = new ClauseData();
        newCd.linguisticVariable = FIS.flData.linguisticVariables[0];
        newCd.fuzzySet = newCd.linguisticVariable.fuzzySets[0];
        cd.AddAntecedent(newCd);
    }

    void AddBlock(object obj) {
        if (!(obj is ClauseData)) return;

        ClauseData cd = obj as ClauseData;
        cd.showInInspector = true;
        cd.AddAntecedent(new ClauseData(true));
    }

    void DeleteClause(object obj) {
        if (!(obj is ClauseInfo)) return;

        (obj as ClauseInfo).list.Remove((obj as ClauseInfo).cd);
    }

    void DuplicateClause(object obj) {
        if (!(obj is ClauseInfo)) return;

        (obj as ClauseInfo).list.Add((obj as ClauseInfo).cd.Copy());
    }

    // void DeleteFuzzySet(object obj) {
    //     if (!(obj is FuzzySetData) || selectedLVD == null) return;

    //     FuzzySetData fsd = obj as FuzzySetData;
    //     selectedLVD.fuzzySets.Remove(fsd);
    // }

    // void DuplicateFuzzySet(object obj) {
    //     if (!(obj is FuzzySetData) || selectedLVD == null) return;

    //     FuzzySetData fsd = obj as FuzzySetData;
    //     selectedLVD.AddFuzzySet(fsd.Copy());
    // }

    enum Operator {
        AND = 1,
        OR
    }

    class ClauseInfo {
        public ClauseData cd;
        public List<ClauseData> list;

        public ClauseInfo(ClauseData cd, List<ClauseData> list) {
            this.cd = cd;
            this.list = list;
        }
    }
}