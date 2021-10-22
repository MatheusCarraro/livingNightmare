using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Utilities;

public class FuzzyLogicEditor : EditorWindow {

    List<EditorTab<FuzzyLogicEditor>> tabs = new List<EditorTab<FuzzyLogicEditor>>();
    int selectedTab, prevSelectedTab;
    Vector2 scrollPos;

    [MenuItem("Fuzzy Logic/Fuzzy Logic Editor")]
    static void ShowWindow() {
        FuzzyLogicEditor window = EditorWindow.GetWindow<FuzzyLogicEditor>("Fuzzy Logic Editor");
        window.minSize = new Vector2(850, 450);
    }

    void OnEnable() {
        string path = Path.Combine(Application.dataPath, "Fuzzy Logic Editor", "Resources");
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }

        if (!Miscellaneous.IsDirectoryEmpty(path) && File.Exists(Path.Combine(path, "Fuzzy Logic Data.asset"))) {
            FIS.flData = Resources.Load<FuzzyLogicData>("Fuzzy Logic Data");
        } else {
            FIS.flData = ScriptableObject.CreateInstance<FuzzyLogicData>();
            AssetDatabase.CreateAsset(FIS.flData, "Assets/Fuzzy Logic Editor/Resources/Fuzzy Logic Data.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        FIS.flData.Initialize();

        tabs.Add(new LinguisticVariablesTab(this));
        tabs.Add(new RulesTab(this));
        selectedTab = 0;
    }

    void OnGUI() {

        if (selectedTab >= 0 && selectedTab < tabs.Count) {
            var selectedEditor = tabs[selectedTab];
            if (selectedTab != prevSelectedTab) {
                selectedEditor.OnTabSelected();
                GUI.FocusControl(null);
            }
            selectedEditor.Draw();
            prevSelectedTab = selectedTab;
        }
        DrawMenuBar();
        EditorUtility.SetDirty(FIS.flData);
        Repaint();
    }

    void DrawMenuBar() {
        Rect menuBar = new Rect(0, 0, position.width, EditorStyles.toolbar.fixedHeight);

        GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
        selectedTab = GUILayout.Toolbar(selectedTab, new [] { "Fuzzy Sets", "Rules" }, EditorStyles.toolbarButton);
        GUILayout.EndArea();
    }
}