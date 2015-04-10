using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseMonoBehaviour), true)]
public class LineBuilderComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ((BaseMonoBehaviour)target).OnInspectorGUI();
    }
}

public class BaseMonoBehaviour : MonoBehaviour
{
    public virtual void OnInspectorGUI()
    {
    }
}