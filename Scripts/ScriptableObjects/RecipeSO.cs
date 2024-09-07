using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//¶©µ¥
[CreateAssetMenu()]
public class RecipeSO : ScriptableObject {
    public List<KitchenObjectSO> kitchenObjectSOList;
    public string recipeName;
}