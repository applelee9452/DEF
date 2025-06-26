using UnityEditor;

class EditorListener
{
    [InitializeOnLoadMethod]
    static void OnProjectLoadedInEditor()
    {
        if (EditorContext.Instance == null)
        {
            new EditorContext();
        }
    }
}