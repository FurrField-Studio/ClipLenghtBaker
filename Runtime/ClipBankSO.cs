using System;
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

            AnimatorController animatorController;
            
            if (animator.runtimeAnimatorController is AnimatorOverrideController animatorOverrideController)
            {
                animatorController = animatorOverrideController.runtimeAnimatorController as AnimatorController;
                BakeClipsDataForOverrideController(animatorOverrideController);
            }
            else
            { 
                animatorController = animator.runtimeAnimatorController as AnimatorController;
                BakeClipsDataForController(animator.runtimeAnimatorController as AnimatorController);
            }

            List<State> animatorStates = new List<State>();
            
            foreach(AnimatorControllerLayer layer in animatorController.layers) //for each layer
            {
                foreach (ChildAnimatorState childAnimatorState in layer.stateMachine.states) //for each state
                {
                    animatorStates.Add(new State(childAnimatorState.state, Clips));
                }

                AddAnimatorStatesRecursive(ref animatorStates, layer.stateMachine);
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

        private void AddAnimatorStatesRecursive(ref List<State> animatorStates, AnimatorStateMachine animatorStateMachine)
        {
            if(animatorStateMachine.stateMachines.Length == 0) return;

            foreach (ChildAnimatorStateMachine childAnimatorStateMachine in animatorStateMachine.stateMachines) //for each substate
            {
                foreach (var childAnimatorState in childAnimatorStateMachine.stateMachine.states)
                {
                    animatorStates.Add(new State(childAnimatorState.state, Clips));
                }

                AddAnimatorStatesRecursive(ref animatorStates,  childAnimatorStateMachine.stateMachine);
            }
        }

        private void BakeClipsDataForOverrideController(AnimatorOverrideController animatorOverrideController)
        {
            List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            
            animatorOverrideController.GetOverrides(overrides);

            List<Clip> clips = new List<Clip>(RuntimeAnimatorController.animationClips.Length);

            for (int index = 0; index < RuntimeAnimatorController.animationClips.Length; index++)
            {
                if (overrides[index].Value != null)
                {
                    clips.Add(new Clip(overrides[index].Value));
                }
            }

            Clips = clips.ToArray();
        }
        
        private void BakeClipsDataForController(AnimatorController animatorController)
        {
            Clips = new Clip[RuntimeAnimatorController.animationClips.Length];
            
            for (int index = 0; index < RuntimeAnimatorController.animationClips.Length; index++)
            {
                Clips[index] = new Clip(RuntimeAnimatorController.animationClips[index]);
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
        public float StateTime;
        public Clip Clip;

        public State(AnimatorState animatorState, Clip[] clips)
        {
            Name = animatorState.name;
            Hash = animatorState.nameHash;
            var animationClip = animatorState.motion as AnimationClip;

            Clip = animationClip != null ? Array.Find(clips, clip => clip.Name == animationClip.name) : null;
            StateTime = Clip != null ? animatorState.speed * Clip.Lenght : 0;
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