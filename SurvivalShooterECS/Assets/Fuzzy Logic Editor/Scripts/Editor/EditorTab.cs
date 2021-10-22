using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class EditorTab<T> {
    protected readonly T editor;

    protected EditorTab(T editor) {
        this.editor = editor;
    }

    public virtual void OnTabSelected() {}

    public virtual void Draw() {}
}