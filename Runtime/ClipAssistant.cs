using System.Collections;
using UnityEngine;

namespace FurrFieldStudio.ClipLenghtBaker.Runtime
{
    public class ClipAssistant : MonoBehaviour
    {
        public ClipBankSO ClipBankSo;

#if UNITY_EDITOR
        
        public void Bake()
        {
            if(!TryGetComponent<Animator>(out Animator animator)) return;
            if(ClipBankSo == null) return;

            ClipBankSo.Bake(animator);
        }

#endif
    }
}