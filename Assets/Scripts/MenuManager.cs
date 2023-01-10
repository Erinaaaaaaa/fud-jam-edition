using Kitchen;
using UnityEngine;
using Random = System.Random;

public class MenuManager : MonoBehaviour
{
    public RecipeLibrary recipeLibrary;
    public IngredientCrafting craftingTable;

    public TitleNote titleNote;

    private Random _random = new();
    
    // Start is called before the first frame update
    void Start()
    {
        var r = recipeLibrary.Recipes[_random.Next(recipeLibrary.Recipes.Count)];
        
        craftingTable.inSlot1.PlaceIngredient(r.ingredients[0]);
        craftingTable.inSlot2.PlaceIngredient(r.ingredients[1]);
        
        titleNote.SetRecipe(r);
    }
}
