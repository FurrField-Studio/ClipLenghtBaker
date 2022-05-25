using FurrFieldStudio.ClipLenghtBaker.Runtime;
using UnityEditor;
using UnityEngine;

namespace FurrFieldStudio.ClipLenghtBaker.Editor
{
    [CustomEditor(typeof(ClipBankSO))]
    public class ClipBankSOInspector : UnityEditor.Editor
    {
        private bool m_Foldout;

        private Animator m_Animator;
        
        public override void OnInspectorGUI()
        {
            var clipBank = target as ClipBankSO;

            m_Foldout = EditorGUILayout.Foldout(m_Foldout, "Baking");
            if(m_Foldout)
            {
                m_Animator = EditorGUILayout.ObjectField("Animator", m_Animator, typeof(Animator), allowSceneObjects: true) as Animator;
                
                if (GUILayout.Button("Bake"))
                {
                    if (m_Animator != null)
                    {
                        clipBank.Bake(m_Animator);
                    
                        EditorUtility.SetDirty(clipBank);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }   
            }

            GUILayout.Space(16);
            
            DrawDefaultInspector();
        }
    }
}