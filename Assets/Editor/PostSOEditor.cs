using UnityEditor;

[CustomEditor(typeof(PostSO))]
public class PostSOEditor : Editor
{
    private SerializedProperty scriptProp;
    private SerializedProperty postTypeProp;
    private SerializedProperty biasChoicesProp;
    private SerializedProperty sourceChoicesProp;
    private SerializedProperty malignChoicesProp;

    private void OnEnable()
    {
        scriptProp = serializedObject.FindProperty("m_Script");
        postTypeProp = serializedObject.FindProperty("postType");
        biasChoicesProp = serializedObject.FindProperty("BiasChoices");
        sourceChoicesProp = serializedObject.FindProperty("SourceChoices");
        malignChoicesProp = serializedObject.FindProperty("MalignChoices");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(scriptProp, true);

        // Manually draw postType so we can conditionally handle other fields
        EditorGUILayout.PropertyField(postTypeProp);
        PostType type = (PostType)postTypeProp.enumValueIndex;

        // Iterate over all remaining properties to preserve headers
        bool enterChildren = true;
        SerializedProperty prop = serializedObject.GetIterator();
        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;
            string propName = prop.name;

            // Skip script and postType (already drawn)
            if (propName == "m_Script" || propName == postTypeProp.name)
                continue;

            if(propName == "authorImage" && type != PostType.MalignChecker)
                continue;

            // Conditional: hide postHeader on MalignChecker
            if (propName == "postHeader" && type != PostType.SourceChecker)
                continue;

            // Skip all choice arrays; we'll draw the relevant one below
            if (propName == biasChoicesProp.name || propName == sourceChoicesProp.name || propName == malignChoicesProp.name)
                continue;
            EditorGUILayout.PropertyField(prop, true);
        }

        // Draw only the relevant choices array under its header
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

        switch (type)
        {
            case PostType.BiasChecker:
                EditorGUILayout.PropertyField(biasChoicesProp, true);
                break;
            case PostType.SourceChecker:
                EditorGUILayout.PropertyField(sourceChoicesProp, true);
                break;
            case PostType.MalignChecker:
                EditorGUILayout.PropertyField(malignChoicesProp, true);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
