using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;

public class TitleNote : MonoBehaviour
{
    public TextMeshPro noteItemName;
    public TextMeshPro recipeItemName;

    public Renderer noteProduct;
    public Renderer noteIngredient1;
    public Renderer noteIngredient2;

    public void SetRecipe(Recipe recipe)
    {
        noteItemName.text = recipe.product.name;
        recipeItemName.text = $"{recipe.product.name} (${recipe.product.Score * 5})";
        
        var pt = Resources.Load<Texture2D>($"Sprites/{recipe.product.key}");
        var i1t = Resources.Load<Texture2D>($"Sprites/{recipe.ingredients[0].key}");
        var i2t = Resources.Load<Texture2D>($"Sprites/{recipe.ingredients[1].key}");

        noteProduct.material.mainTexture = pt;
        noteIngredient1.material.mainTexture = i1t;
        noteIngredient2.material.mainTexture = i2t;
    }
}
