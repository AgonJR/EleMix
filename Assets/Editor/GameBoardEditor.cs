using UnityEditor;

[CustomEditor(typeof(GameBoard))]
public class GameBoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ManagerBoard boardManager = (ManagerBoard)target;

        base.OnInspectorGUI();

        // EditorGUILayout.Space();
        // EditorGUILayout.LabelField("Debug");
        // boardManager.DebugMode = EditorGUILayout.Toggle("Debug Mode", boardManager.DebugMode);

        // if (boardManager.DebugMode)
        // {
        //     boardManager.DebugSpawnRandom = EditorGUILayout.Toggle("Spawn Random", boardManager.DebugSpawnRandom);
        // }

        // if (GUI.changed)
        //     EditorUtility.SetDirty(boardManager);
    }
}