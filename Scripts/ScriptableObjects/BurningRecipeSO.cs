using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Öó¹ý¶Èºó
[CreateAssetMenu()]
public class BurningRecipeSO : ScriptableObject {
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burningTimerMax;
}