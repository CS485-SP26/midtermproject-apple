using UnityEngine;
using UnityEditor;
using Environment;

[CustomEditor(typeof(SeasonManager))]
public class SeasonManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("sunLight"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("seasonLabel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("particleController"));
        EditorGUILayout.Space();

        SerializedProperty sp = serializedObject.FindProperty("seasons");
        for(int i = 0; i < (int)SeasonManager.Season.Count; i++)
        {
            string name = ((SeasonManager.Season)i).ToString();
            EditorGUILayout.PropertyField(sp.GetArrayElementAtIndex(i), 
                new GUIContent(name));
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startingSeason"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startingDay"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startingDayNumber"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("daysPerSeason"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime State", EditorStyles.boldLabel);
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentSeasonType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentDayOfWeek"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentTimeOfDay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentDayNumber"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeOfDayProgress"));
        }

        SeasonManager manager = (SeasonManager)target;
        SeasonData data = manager.RuntimeData;

        if(data != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime Season Data", EditorStyles.boldLabel);
            SerializedObject so = new SerializedObject(data);
            so.Update();
            EditorGUILayout.PropertyField(so.FindProperty("avgTemp"));
            EditorGUILayout.PropertyField(so.FindProperty("dayLength"));
            EditorGUILayout.PropertyField(so.FindProperty("sunColor"));

            so.ApplyModifiedProperties();

        }

        serializedObject.ApplyModifiedProperties();

    }
}
