using Data;
using Kitchen;
using UnityEngine;
using UnityEngine.Serialization;

public class IngredientCrafting : MonoBehaviour
{
    public RecipeLibrary recipeLibrary;

    public IngredientSlot inSlot1;
    public IngredientSlot inSlot2;

    public IngredientSlot outSlot;

    public void UpdateRecipe()
    {
        if (!inSlot1.HasIngredient() || !inSlot2.HasIngredient())
        {
            outSlot.PlaceIngredient(null, true);
            return;
        }

        var i = recipeLibrary.GetRecipeResult(inSlot1.GetIngredient(), inSlot2.GetIngredient());
        outSlot.PlaceIngredient(i, true);
    }

    public void ConfirmCraft()
    {
        inSlot1.TakeIngredient();
        inSlot2.TakeIngredient();
    }
}