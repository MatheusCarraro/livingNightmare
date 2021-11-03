using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities {
    public static class EditorUtils {
        public static List<T> DrawOrderListButtons<T>(List<T> list, int index) {
            if (list.Count > 1) {
                T element = list[index];
                if (GUILayout.Button("↓", GUILayout.Width(20))) {
                    list.Remove(element);
                    list.Insert(Miscellaneous.Mod(index + 1, list.Count + 1), element);
                }
                if (GUILayout.Button("↑", GUILayout.Width(20))) {
                    list.Remove(element);
                    list.Insert(Miscellaneous.Mod(index - 1, list.Count + 1), element);
                }
            }

            return list;
        }
    }
}