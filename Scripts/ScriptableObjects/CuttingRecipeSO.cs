using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�в�
[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject {
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax;
}