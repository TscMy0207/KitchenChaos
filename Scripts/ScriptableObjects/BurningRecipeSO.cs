using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����Ⱥ�
[CreateAssetMenu()]
public class BurningRecipeSO : ScriptableObject {
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burningTimerMax;
}