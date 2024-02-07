using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Inex.Editor
{
    public class SearchAndReplacePrefabs : OdinEditorWindow
    {
        [MenuItem("Inex/Tools/Search and Replace Prefabs")]
        private static void OpenWindow()
        {
            GetWindow<SearchAndReplacePrefabs>().Show();
        }

        [PropertySpace(10), SerializeField] GameObject objectToSearch;

        [InfoBox("Everything will be converted to lowercase. you don't need to think about case sensitivity.")]
        [SerializeField,PropertySpace(15),ListDrawerSettings(ShowPaging = false)] SearchAndReplaceItem[] items = Array.Empty<SearchAndReplaceItem>();


        [PropertySpace(30), Button(ButtonSizes.Large)]
        void ReplaceAll()
        {
            if (objectToSearch == null)
            {
                Debug.LogError("Parent is null");
                return;
            }
        
        
            foreach (var item in items)
            {
                // find all objects with the given name (item.search)
                var objects = objectToSearch.GetComponentsInChildren<Transform>(true);
                foreach (var obj in objects)
                {
                    if (!obj) continue;
                    // make search and object names lowercase
                    var search = item.search.ToLower();
                    var objName = obj.name.ToLower();
                
                    if (!objName.Contains(search)) continue; // skip if the name doesn't contain the search string
                
                
                    // replace the object with the given prefab (item.replace)
                    ReplacePrefabsWindow.ReplacePrefab(obj.gameObject, item.replace);
                }
            }
        
            // clear parent
            objectToSearch = null;
        
            Debug.Log("Done!");
        }
    }

    struct SearchAndReplaceItem
    {
        public string search;
        public GameObject replace;
    }
}