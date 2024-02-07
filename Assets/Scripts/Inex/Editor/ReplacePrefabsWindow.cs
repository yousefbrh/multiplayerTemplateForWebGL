using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Inex.Editor
{
    public class ReplacePrefabsWindow : OdinEditorWindow
    {
        [MenuItem("Inex/Tools/Replace Prefabs")]
        private static void OpenWindow()
        {
            GetWindow<ReplacePrefabsWindow>().Show();
        }

        // array of game objects to be replaced
        [SerializeField] private GameObject[] objectsToReplace;

        // a game object to replace the objects in the array
        [SerializeField, PropertySpace(10)] private GameObject replacementObject;

        [Button(ButtonSizes.Large), PropertySpace(15)]
        public void ReplacePrefab()
        {
            // loop through the array of objects to be replaced
            foreach (GameObject obj in objectsToReplace)
            {
                ReplacePrefab(obj, replacementObject);
            }

            // clear the array of objects to be replaced
            objectsToReplace = null;
        }

        public static void ReplacePrefab(GameObject oldPrefab, GameObject newPrefab)
        {
            // instantiate the replacement prefab
            var replacement = PrefabUtility.InstantiatePrefab(newPrefab) as GameObject;
            // set the position, rotation and scale of the replacement object to match the object to be replaced
            replacement.transform.position = oldPrefab.transform.position;
            replacement.transform.rotation = oldPrefab.transform.rotation;
            replacement.transform.localScale = oldPrefab.transform.localScale;
            replacement.transform.parent = oldPrefab.transform.parent;
            replacement.SetActive(oldPrefab.activeSelf);
            replacement.name = oldPrefab.name;

            // destroy the object to be replaced
            DestroyImmediate(oldPrefab);
        }
    }
}