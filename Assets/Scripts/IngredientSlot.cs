using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class IngredientPlaced : UnityEvent<Ingredient>
{
}

public class IngredientSlot : MonoBehaviour
{
    public bool canPlace;
    public bool canTake;
    public bool CanSwap => canPlace && canTake;

    public bool isTrash;
    public bool isInfinite;

    public virtual bool AcceptsSeeds => false;
        
    protected Ingredient Ingredient;

    public IngredientPlaced ingredientPlaced;
    public UnityEvent ingredientTaken;

    public Material itemDisplayMaterial;
    protected GameObject ItemDisplay;

    public virtual bool PlaceIngredient(Ingredient ingredient, bool forcePlace = false)
    {
        if (!canPlace && !forcePlace) return false;

        if (ingredient == null)
        {
            TakeIngredient(forcePlace);
            return true;
        }
        
        if (ingredient.IsSeed != AcceptsSeeds && !forcePlace && !isTrash) return false;

        if (!isTrash)
        {
            Ingredient = ingredient;
            CreateItemDisplay(ingredient);    
        }
        
        if (!forcePlace)
            ingredientPlaced.Invoke(Ingredient);

        return true;
    }

    protected virtual void CreateItemDisplay(Ingredient ingredient)
    {
        var tex = Resources.Load<Texture2D>($"Sprites/{ingredient.key}");

        GameObject go;
        
        if (tex != null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.transform.SetParent(transform);
            var renderer = go.GetComponent<Renderer>();
            renderer.material = new Material(itemDisplayMaterial)
            {
                mainTexture = tex
            };
            go.transform.localPosition = new Vector3(0, 0.4f, 0);
            
            if (ingredient.IsSeed)
            {
                var plant = GameObject.CreatePrimitive(PrimitiveType.Quad);
                plant.transform.SetParent(go.transform);
                var plantRenderer = plant.GetComponent<Renderer>();
                plantRenderer.material = new Material(itemDisplayMaterial)
                {
                    mainTexture = Resources.Load<Texture2D>("Sprites/seed")
                };
                plant.transform.localPosition = new Vector3(.3f, 0.03f, -.3f);
                plant.transform.localScale = new Vector3(.5f, .5f, .5f);
            }
        }
        else
        {
            go = new GameObject("Textureless seed");
            go.transform.SetParent(transform);
            var tmpText = go.AddComponent<TextMeshPro>();
            tmpText.text = ingredient.name;
            if (ingredient.IsSeed)
            {
                tmpText.text += " Seed";
            }
            tmpText.fontSize = 2;
            tmpText.alignment = TextAlignmentOptions.Center;
            go.transform.localPosition = new Vector3(0, 0.1f, 0);
            go.transform.forward = Vector3.down;
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(1, 1);
        }

        ItemDisplay = go;
    }

    public virtual Ingredient TakeIngredient(bool forceTake = false)
    {
        if (!canTake && !forceTake) return null;

        var i = Ingredient;
        if (!isInfinite)
        {
            Ingredient = null;
            Destroy(ItemDisplay);
            ItemDisplay = null;
        }
        if (!forceTake)
            ingredientTaken.Invoke();
        return i;
    }

    public virtual bool HasIngredient()
    {
        return Ingredient != null;
    }

    public Ingredient GetIngredient()
    {
        return Ingredient;
    }
}