using Unity.Netcode.Components;
using UnityEngine;

//改变授权模式 支持客户机改变Animator
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