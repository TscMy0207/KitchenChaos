using Unity.Netcode.Components;
using UnityEngine;

//改变授权模式 支持客户机改变Transform
namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
//原版unet的NetworkTransform只允许服务器(主机)改变Transform,这就造成了客户机在服务器(主机)上Transform无法改变
//修改Auth可以让其他客户机也能改变Transform,从而实现Transform的同步
//NetworkAnimator同理