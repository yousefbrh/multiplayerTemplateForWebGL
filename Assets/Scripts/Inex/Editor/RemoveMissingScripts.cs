using UnityEditor;
using UnityEngine;

namespace Inex.Editor
{
    public class RemoveMissingScripts : MonoBehaviour
    {
        [MenuItem("/Inex/GameObjects/Remove Missing Scripts")]
        static void Remove()
        {
            var go = Selection.gameObjects;

            // loop over selected objects
            foreach (var g in go)
            {
                Remove(g);
            }
            Debug.Log("Removed Missing Scripts");
        }

        private static void Remove(GameObject g)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(g); // remove missing scripts
    
            // Now recurse through each child GO (if there are any):
            foreach (Transform childT in g.transform)
            {
                //Debug.Log("Searching " + childT.name  + " " );
                Remove(childT.gameObject);
            }
        }
    }
}