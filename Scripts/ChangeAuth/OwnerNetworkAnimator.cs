using Unity.Netcode.Components;
using UnityEngine;

//�ı���Ȩģʽ ֧�ֿͻ����ı�Animator
namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{
    [DisallowMultipleComponent]
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}