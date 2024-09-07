using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ÇÐ²Ë
[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject {
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax;
}