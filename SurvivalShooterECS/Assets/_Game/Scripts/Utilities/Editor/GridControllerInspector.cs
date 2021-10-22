using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities {
    [CustomEditor(typeof(GridController))]
    public class GridControllerInspector : Editor {

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            GridController gc = (GridController) target;

            if (GUILayout.Button("Show/Hide Grid")) {
                if (gc.grid != null) {
                    gc.DestroyGrid();
                } else {
                    gc.CreateGrid(gc.gridWidth, gc.gridHeight);
                }
            }
        }
    }
}