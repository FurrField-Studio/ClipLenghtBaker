﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace FurrFieldStudio.ClipLenghtBaker.Runtime
{
    [CreateAssetMenu(fileName = "ClipBank", menuName = "ClipLenghtBaker/ClipBank", order = 0)]
    public class ClipBankSO : ScriptableObject
    {
        [SerializeField]
        private RuntimeAnimatorController RuntimeAnimatorController;

        public Clip[] Clips;
        
        public State[] States;

        [SerializeField][HideInInspector]
        private string[] m_StateNameToClip;
        
        [SerializeField][HideInInspector]
        private int[] m_StateHashToClip;

        public void Bake(Animator animator)
        {
            RuntimeAnimatorController = animator.runtimeAnimatorController;
            var animatorController = animator.runtimeAnimatorController as AnimatorController;
            Clips = new Clip[RuntimeAnimatorController.animationClips.Length];
            
            for (int index = 0; index < RuntimeAnimatorController.animationClips.Length; index++)
            {
                Clips[index] = new Clip(RuntimeAnimatorController.animationClips[index]);
            }

            List<State> animatorStates = new List<State>();
            
            foreach(AnimatorControllerLayer i in animatorController.layers) //for each layer
            {
                foreach (ChildAnimatorState j in i.stateMachine.states) //for each state
                {
                    animatorStates.Add(new State(j.state, Clips));
                }
            }

            States = animatorStates.ToArray();

            m_StateNameToClip = new string[States.Length];
            for (int i = 0; i < States.Length; i++)
            {
                m_StateNameToClip[i] = States[i].Name;
            }
            
            m_StateHashToClip = new int[States.Length];
            for (int i = 0; i < States.Length; i++)
            {
                m_StateHashToClip[i] = States[i].Hash;
            }
        }

        public Clip GetClipFromStateName(string stateName) => Array.Find(States, state => state.Name == stateName).Clip;

        public Clip GetClipFromStateHash(int stateHash) => Array.Find(States, state => state.Hash == stateHash).Clip;

    }

    [Serializable]
    public class State
    {
        public string Name;
        public int Hash;
        public Clip Clip;

        public State(AnimatorState animatorState, Clip[] clips)
        {
            Name = animatorState.name;
            Hash = animatorState.nameHash;
            var animationClip = animatorState.motion as AnimationClip;

            Clip = animationClip != null ? Array.Find(clips, clip => clip.Name == animationClip.name) : null;
        }
    }
    
    [Serializable]
    public class Clip
    {
        public string Name;
        public float Lenght;

        public Clip(AnimationClip animationClip)
        {
            Name = animationClip.name;
            Lenght = animationClip.length;
        }
    }
}