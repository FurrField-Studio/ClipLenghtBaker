using System.Collections.Generic;
using System.Linq;
using FurrFieldStudio.ClipLenghtBaker.Runtime;
using UnityEditor;
using UnityEngine;

namespace FurrFieldStudio.ClipLenghtBaker.Editor
{
    [CustomEditor(typeof(ClipBankSO))]
    public class ClipBankSOInspector : UnityEditor.Editor
    {
        private bool m_BakingFoldout;
        private bool m_DebugFoldout;

        private Animator m_Animator;
        
        public override void OnInspectorGUI()
        {
            var clipBank = target as ClipBankSO;

            RenderBakingFoldout(clipBank);
            RenderDebugFoldout(clipBank);
            
            GUILayout.Space(16);
            
            DrawDefaultInspector();
        }

        private void RenderBakingFoldout(ClipBankSO clipBank)
        {
            m_BakingFoldout = EditorGUILayout.Foldout(m_BakingFoldout, "Baking");
            if(m_BakingFoldout)
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
        }

        private void RenderDebugFoldout(ClipBankSO clipBank)
        {
            m_DebugFoldout = EditorGUILayout.Foldout(m_DebugFoldout, "Debug");
            if(m_DebugFoldout)
            {
                if (GUILayout.Button("List unassigned clips"))
                {
                    List<Clip> list = clipBank.Clips.ToList();

                    foreach (var state in clipBank.States)
                    {
                        list.Remove(list.Find(clip => clip.Name == state.Clip.Name));
                    }

                    Debug.Log("Unassigned clips:");
                    foreach (var clip in list)
                    {
                        Debug.Log(clip.Name);
                    }
                }   
            }
        }
    }
}