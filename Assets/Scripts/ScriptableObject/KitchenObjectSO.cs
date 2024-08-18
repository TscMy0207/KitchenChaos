using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject
{
    public Transform prefeb;
    public Sprite sprite;
    [SerializeField] public string objectName;
}