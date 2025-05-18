using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Element - X", menuName = "Data/Element", order = 1)]
public class ScriptableElement : ScriptableObject
{
    [Space] public int id;
    [Space] public string Name;
    [Space] public Material ColorMat;
    [Space] public List<ScriptableElement> Ingredients;

    public bool IsBasic()
    {
        return Ingredients == null || Ingredients.Count == 0;
    }
    
}