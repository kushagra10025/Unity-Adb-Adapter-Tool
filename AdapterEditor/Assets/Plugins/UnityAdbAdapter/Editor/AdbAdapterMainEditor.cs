using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class AdbAdapterMainEditor : EditorWindow
{
    [Shortcut("Tools/ADB Adapter/Open ADB Adapter Window",KeyCode.I,ShortcutModifiers.Control | ShortcutModifiers.Shift)]
    [MenuItem("Tools/ADB Adapter/Open ADB Adapter Window")]
    public static void OpenAdbAdapterMainWindow()
    {
        AdbAdapterMainEditor window = GetWindow<AdbAdapterMainEditor>();
        window.titleContent = new GUIContent("ADB Adapter");
        window.maxSize = new Vector2(300, 400);
        window.minSize = window.maxSize;
        window.ShowModalUtility();
    }

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        VisualTreeAsset mainEditorUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/UnityAdbAdapter/UI/AdbAdapterMain_UXML.uxml");

        if(mainEditorUXML == null)
        {
            Debug.Log("Cannot Load AdbAdapterMain_UXML.uxml. Please re-import or restore original Path!");
            return;
        }

        root.Add(mainEditorUXML.Instantiate());
    }
}
