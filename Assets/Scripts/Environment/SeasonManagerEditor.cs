using UnityEngine;
using UnityEditor;
using Environment;

[CustomEditor(typeof(SeasonManager))]
public class SeasonManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty sp = serializedObject.FindProperty("seasons");
        for(int i = 0; i < (int)SeasonManager.Season.Count; i++)
        {
            string name = ((SeasonManager.Season)i).ToString();
            EditorGUILayout.PropertyField(sp.GetArrayElementAtIndex(i), 
                new GUIContent(name));
        }

        

        SeasonManager manager = (SeasonManager)target;
        SeasonData data = manager.RuntimeData;

        if(data != null)
        {
            EditorGUILayout.LabelField("Runtime Data");
            SerializedObject so = new SerializedObject(data);
            EditorGUILayout.PropertyField(so.FindProperty("avgTemp"));
            EditorGUILayout.PropertyField(so.FindProperty("dayLength"));
            EditorGUILayout.PropertyField(so.FindProperty("sunColor"));

            so.ApplyModifiedProperties();

        }

        serializedObject.ApplyModifiedProperties();

    }
}
