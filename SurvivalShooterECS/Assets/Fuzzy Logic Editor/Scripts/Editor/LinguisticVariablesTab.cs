using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class LinguisticVariablesTab : EditorTab<FuzzyLogicEditor> {

    Vector2 scrollPosLV,
    scrollPosFS;
    LinguisticVariableData selectedLVD;
    GUIStyle bgLightStyle,
    fontStyle,
    foldoutStyle;
    Texture2D bgLight;

    Rect area;

    float wLV,
    wF;

    public LinguisticVariablesTab(FuzzyLogicEditor editor) : base(editor) {
        bgLight = Miscellaneous.MakeTex(2, 2, Miscellaneous.ColorHEX("AAAAAA"));
    }

    public override void Draw() {
        bgLightStyle = new GUIStyle(GUI.skin.box);
        bgLightStyle.normal.background = bgLight;
        fontStyle = new GUIStyle(EditorStyles.label);
        fontStyle.fontSize = 16;
        fontStyle.fixedHeight = 32;
        fontStyle.alignment = TextAnchor.UpperCenter;
        foldoutStyle = new GUIStyle(EditorStyles.foldoutHeader);
        foldoutStyle.normal.background = Resources.Load<Texture2D>("EditorUI/bg_HeaderCollapsed_Red");
        foldoutStyle.onNormal.background = Resources.Load<Texture2D>("EditorUI/bg_HeaderExpanded_Red");

        area = new Rect(0, EditorStyles.toolbar.fixedHeight, editor.position.width, editor.position.height - EditorStyles.toolbar.fixedHeight);
        wLV = (area.width / 3) + 2;
        wF = (area.width * (2f / 3)) - 2;
        GUILayout.Space(10);
        GUI.BeginGroup(area);
        EditorGUILayout.BeginHorizontal();

        DrawLinguisticVariablesList();

        DrawFuzzySetsList();

        EditorGUILayout.EndHorizontal();
        GUI.EndGroup();
        Handles.BeginGUI();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(wLV, EditorStyles.toolbar.fixedHeight), new Vector2(wLV, editor.position.height));
        Handles.color = Color.white;
        Handles.EndGUI();
    }

    void DrawLinguisticVariablesList() {
        selectedLVD = null;

        Rect r = EditorGUILayout.BeginVertical(GUILayout.Width(wLV), GUILayout.Height(area.height));

        EditorGUILayout.LabelField("Linguistic Variables", fontStyle);

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(wLV), GUILayout.Height(EditorStyles.toolbar.fixedHeight));

        if (GUILayout.Button("New", EditorStyles.toolbarButton)) {
            LinguisticVariableData newLV = new LinguisticVariableData("NewLinguisticVariable" + FIS.flData.linguisticVariables.Count);
            FIS.flData.linguisticVariables.Add(newLV);
            newLV.AddFuzzySet(new FuzzySetData("NewFuzzySet" + newLV.fuzzySets.Count));
        }
        if (GUILayout.Button("Clear", EditorStyles.toolbarButton)) {
            FIS.flData.linguisticVariables.Clear();
            FIS.flData.rules.Clear();
        } //SaveTexBuitin();

        EditorGUILayout.EndHorizontal();

        if (FIS.flData.linguisticVariables.Count > 0) {
            GUILayout.BeginArea(new Rect(15, 55, wLV - 15, area.height - 55));

            scrollPosLV = EditorGUILayout.BeginScrollView(scrollPosLV, GUILayout.Width(wLV - 15));

            GUILayout.Space(10);

            for (int i = 0; i < FIS.flData.linguisticVariables.Count; ++i) {
                LinguisticVariableData lvd = FIS.flData.linguisticVariables[i];
                lvd.validName = FIS.flData.ValidateLinguisticVariableName(lvd.name);

                EditorGUILayout.BeginVertical(GUILayout.Width(wLV - 35));
                lvd.showInInspector = EditorGUILayout.BeginFoldoutHeaderGroup(lvd.showInInspector, Miscellaneous.Truncate(lvd.name, (int) (wLV / 8.75f)), lvd.validName ? EditorStyles.foldoutHeader : foldoutStyle, delegate(Rect position) {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, DeleteLinguisticVariable, lvd);
                    menu.AddItem(new GUIContent("Duplicate"), false, DuplicateLinguisticVariable, lvd);
                    menu.DropDown(position);
                });

                if (lvd.showInInspector) {

                    selectedLVD = lvd;
                    foreach (LinguisticVariableData lv in FIS.flData.linguisticVariables)
                        if (lv != lvd) lv.showInInspector = false;

                    Rect r2 = EditorGUILayout.BeginVertical();
                    GUI.Box(new Rect(r2.x + 2, r2.y - 1, r2.width - 4, r2.height + 2), "", bgLightStyle);

                    EditorGUILayout.LabelField("Name:");
                    lvd.name = EditorGUILayout.DelayedTextField(lvd.name).Trim();
                    lvd.name = lvd.name.Replace(" ", "");

                    if (!lvd.validName) {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Linguistic Variables must have unique names!", MessageType.Error);
                    } else {
                        EditorGUILayout.LabelField("Start:");
                        lvd.start = EditorGUILayout.DelayedFloatField(lvd.start);
                        EditorGUILayout.LabelField("End:");
                        lvd.end = EditorGUILayout.DelayedFloatField(lvd.end);
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.EndVertical();

                } else {
                    EditorGUILayout.Space();
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        } else {
            EditorGUILayout.HelpBox("Add Linguistic Variables Here...", MessageType.Info);
        }

        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();
    }

    void DrawFuzzySetsList() {
        EditorGUILayout.BeginVertical(GUILayout.Width(wF), GUILayout.Height(area.height));

        EditorGUILayout.LabelField("Fuzzy Sets", fontStyle);

        GUILayout.Space(10);

        if (selectedLVD != null) {

            if (!selectedLVD.validName) {
                EditorGUILayout.HelpBox("Linguistic Variables must have unique names!", MessageType.Error);
            } else {

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(wF), GUILayout.Height(EditorStyles.toolbar.fixedHeight));

                if (GUILayout.Button("New", EditorStyles.toolbarButton)) {
                    selectedLVD.AddFuzzySet(new FuzzySetData("NewFuzzySet" + selectedLVD.fuzzySets.Count));
                }
                if (GUILayout.Button("Clear", EditorStyles.toolbarButton)) {
                    selectedLVD.ClearFuzzySets();
                }

                EditorGUILayout.EndHorizontal();

                if (selectedLVD.fuzzySets.Count > 0) {
                    GUILayout.BeginArea(new Rect(wLV + 15, 55, wF - 15, area.height - 55));

                    scrollPosFS = EditorGUILayout.BeginScrollView(scrollPosFS);
                    GUILayout.Space(10);

                    for (int i = 0; i < selectedLVD.fuzzySets.Count; ++i) {
                        FuzzySetData fsd = selectedLVD.fuzzySets[i];

                        fsd.validName = selectedLVD.ValidateFuzzySetName(fsd.name);

                        EditorGUILayout.BeginVertical(GUILayout.Width(wF - 35));
                        fsd.showInInspector = EditorGUILayout.BeginFoldoutHeaderGroup(fsd.showInInspector, Miscellaneous.Truncate(fsd.name, (int) (wF / 7.75f)), fsd.validName ? EditorStyles.foldoutHeader : foldoutStyle, delegate(Rect position) {
                            var menu = new GenericMenu();
                            if (selectedLVD.fuzzySets.Count > 1) {
                                menu.AddItem(new GUIContent("Delete"), false, DeleteFuzzySet, fsd);
                            }
                            menu.AddItem(new GUIContent("Duplicate"), false, DuplicateFuzzySet, fsd);
                            menu.DropDown(position);
                        });

                        if (fsd.showInInspector) {

                            Rect r = EditorGUILayout.BeginHorizontal();
                            GUI.Box(new Rect(r.x + 2, r.y - 1, r.width - 4, r.height + 2), "", bgLightStyle);

                            if (!fsd.validName) {
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Name:");
                                fsd.name = EditorGUILayout.TextField(fsd.name);
                                fsd.name = fsd.name.Replace(" ", "");
                                EditorGUILayout.Space();
                                EditorGUILayout.HelpBox("Linguistic Variables must have unique names!", MessageType.Error);

                                EditorGUILayout.EndVertical();
                            } else {
                                EditorGUILayout.BeginVertical(GUILayout.Width(wF / 2));

                                EditorGUILayout.LabelField("Name:");
                                fsd.name = EditorGUILayout.TextField(fsd.name);
                                fsd.name = fsd.name.Replace(" ", "");

                                EditorGUILayout.Space();

                                EditorGUILayout.LabelField("Membership Function: ");
                                fsd.membershipFunction = (MembershipFunctionType) EditorGUILayout.EnumPopup(fsd.membershipFunction);
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();

                                for (int j = 0; j < fsd.values.Length; ++j) {
                                    float newValue = EditorGUILayout.DelayedFloatField(fsd.values[j]);
                                    if (fsd.values[j] != newValue) {
                                        fsd.values[j] = Mathf.Clamp(newValue, j > 0 ? fsd.values[j - 1] + 0.01f : fsd.linguisticVariable.start, j < fsd.values.Length - 1 ? fsd.values[j + 1] - 0.01f : fsd.linguisticVariable.end);
                                        Keyframe k = fsd.tempGraph.keys[j + 1];
                                        fsd.tempGraph.RemoveKey(j + 1);
                                        fsd.tempGraph.AddKey(fsd.values[j], k.value);
                                    }
                                }

                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space();
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.BeginVertical();

                                DrawGraphField(fsd);

                                EditorGUILayout.EndVertical();
                            }

                            EditorGUILayout.EndHorizontal();
                        } else {
                            EditorGUILayout.Space();
                        }

                        EditorGUILayout.EndFoldoutHeaderGroup();
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndScrollView();
                    GUILayout.EndArea();
                } else {
                    EditorGUILayout.HelpBox("Add Fuzzy Sets for " + selectedLVD.name + " Here...", MessageType.Info);
                }
            }
        } else if (FIS.flData.linguisticVariables.Count > 0) {
            EditorGUILayout.HelpBox("Select an Linguistic Variable...", MessageType.Info);
        } else {
            EditorGUILayout.HelpBox("Add at least one Linguistic Variable...", MessageType.Warning);
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    void DrawGraphField(FuzzySetData fsd) {

        if (fsd.tempGraph.keys.Length != fsd.values.Length + 2) {
            fsd.tempGraph.keys = fsd.graph.keys;
        }

        for (int i = 1; i < fsd.values.Length + 1; ++i) {

            Keyframe k = fsd.tempGraph.keys[i];

            float y = 0;

            if (i == 1) {
                y = fsd.membershipFunction == MembershipFunctionType.TrapezoidalRight ? 1 : 0;
            } else if (i == fsd.values.Length) {
                y = fsd.membershipFunction == MembershipFunctionType.TrapezoidalLeft ? 1 : 0;
            } else {
                y = 1;
            }

            fsd.tempGraph.RemoveKey(i);
            fsd.tempGraph.AddKey(k.time, y);
            fsd.values[i - 1] = k.time;
        }

        for (int i = 0; i < fsd.tempGraph.length; ++i) {
            AnimationUtility.SetKeyLeftTangentMode(fsd.tempGraph, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(fsd.tempGraph, i, AnimationUtility.TangentMode.Linear);
        }

        fsd.graph.keys = fsd.tempGraph.keys;

        fsd.tempGraph = EditorGUILayout.CurveField(fsd.tempGraph, Color.green, new Rect(selectedLVD.start, 0, selectedLVD.length, 1), GUILayout.Height(105));

        EditorGUILayout.Space();
    }

    void DeleteLinguisticVariable(object obj) {
        if (!(obj is LinguisticVariableData)) return;

        LinguisticVariableData lvd = obj as LinguisticVariableData;
        FIS.flData.linguisticVariables.Remove(lvd);
    }

    void DuplicateLinguisticVariable(object obj) {
        if (!(obj is LinguisticVariableData)) return;

        LinguisticVariableData lvd = obj as LinguisticVariableData;
        lvd.showInInspector = false;
        lvd.Copy();
    }

    void DeleteFuzzySet(object obj) {
        if (!(obj is FuzzySetData) || selectedLVD == null) return;

        FuzzySetData fsd = obj as FuzzySetData;
        selectedLVD.fuzzySets.Remove(fsd);
    }

    void DuplicateFuzzySet(object obj) {
        if (!(obj is FuzzySetData) || selectedLVD == null) return;

        FuzzySetData fsd = obj as FuzzySetData;
        selectedLVD.AddFuzzySet(fsd.Copy());
    }

    void SaveTexBuitin() {
        if (GUILayout.Button("Save")) {
            GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
            Texture2D texture = style.normal.background;
            // Create a temporary RenderTexture of the same size as the texture
            RenderTexture tmp = RenderTexture.GetTemporary(
                texture.width,
                texture.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit(texture, tmp);

            // Backup the currently set RenderTexture
            RenderTexture previous = RenderTexture.active;
            // Set the current RenderTexture to the temporary one we created
            RenderTexture.active = tmp;
            // Create a new readable Texture2D to copy the pixels to it

            //foldoutStyle.normal.background.isReadable = true;
            Texture2D tex = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

            // Copy the pixels from the RenderTexture to the new Texture
            tex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            tex.Apply();

            // Reset the active RenderTexture
            RenderTexture.active = previous;
            // Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary(tmp);

            //tex.SetPixels(foldoutStyle.normal.background.GetPixels());
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/../" + texture.name + ".png", bytes);
        }
    }
}