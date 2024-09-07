using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ÖóÈâ
[CreateAssetMenu()]
public class FryingRecipeSO : ScriptableObject {
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float fryingTimerMax;
}