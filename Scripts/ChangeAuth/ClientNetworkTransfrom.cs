using Unity.Netcode.Components;
using UnityEngine;

//�ı���Ȩģʽ ֧�ֿͻ����ı�Transform
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
//ԭ��unet��NetworkTransformֻ���������(����)�ı�Transform,�������˿ͻ����ڷ�����(����)��Transform�޷��ı�
//�޸�Auth�����������ͻ���Ҳ�ܸı�Transform,�Ӷ�ʵ��Transform��ͬ��
//NetworkAnimatorͬ��