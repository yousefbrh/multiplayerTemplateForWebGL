using TMPro;
using UnityEngine;

namespace UI
{ 
    [ExecuteAlways]
    public class VersionNumber : MonoBehaviour
    {
        private void Awake() => GetComponent<TMP_Text>().text = $"version {Application.version}";
    }
}