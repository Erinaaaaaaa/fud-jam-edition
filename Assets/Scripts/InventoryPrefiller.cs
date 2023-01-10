using System.Collections;
using System.Collections.Generic;
using Kitchen;
using UnityEngine;

public class InventoryPrefiller : MonoBehaviour
{
    public IngredientSlot[] slots;
    public RecipeLibrary recipeLibrary;
    public string[] ingredientKeys;

    public bool isSeedFiller;
    
    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < ingredientKeys.Length; i++)
        {
            if (isSeedFiller)
            {
                slots[i].PlaceIngredient(recipeLibrary.GetSeedForIngredient(ingredientKeys[i]), true);
            }
            else
            {
                slots[i].PlaceIngredient(recipeLibrary.GetIngredient(ingredientKeys[i]), true);
            }
        }
    }
}
