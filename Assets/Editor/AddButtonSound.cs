using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class AddButtonSoundToProject
{
    [MenuItem("Tools/Audio/Add Button Sound To Entire Project")]
    public static void AddButtonSound()
    {
        int totalButtons = 0;

        //---------------------------------------
        // Scene
        //---------------------------------------

        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");

        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            var scene = EditorSceneManager.OpenScene(path);

            bool changed = false;

            Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);

            foreach (Button button in buttons)
            {
                if (button.GetComponent<UIButtonSound>() == null)
                {
                    Undo.AddComponent<UIButtonSound>(button.gameObject);
                    totalButtons++;
                    changed = true;
                }
            }

            if (changed)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);

                Debug.Log("Updated Scene : " + scene.name);
            }
        }

        //---------------------------------------
        // Prefab
        //---------------------------------------

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefab =
                PrefabUtility.LoadPrefabContents(path);

            bool changed = false;

            Button[] buttons = prefab.GetComponentsInChildren<Button>(true);

            foreach (Button button in buttons)
            {
                if (button.GetComponent<UIButtonSound>() == null)
                {
                    Undo.AddComponent<UIButtonSound>(button.gameObject);
                    totalButtons++;
                    changed = true;
                }
            }

            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(prefab, path);
                Debug.Log("Updated Prefab : " + prefab.name);
            }

            PrefabUtility.UnloadPrefabContents(prefab);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Done! Added UIButtonSound to {totalButtons} Buttons.");
    }
}