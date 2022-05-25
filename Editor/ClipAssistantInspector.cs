using FurrFieldStudio.ClipLenghtBaker.Runtime;
using UnityEditor;
using UnityEngine;

namespace FurrFieldStudio.ClipLenghtBaker.Editor
{
    [CustomEditor(typeof(ClipAssistant))]
    public class ClipAssistantInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var clipAssistant = target as ClipAssistant;

            if (GUILayout.Button("Bake clips"))
            {
                clipAssistant.Bake();
            }

            DrawDefaultInspector();
        }
    }
}