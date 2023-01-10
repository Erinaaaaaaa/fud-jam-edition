using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class FieldSlot : IngredientSlot
{
    public override bool AcceptsSeeds => true;
    public Transform itemDisplayRoot;
    
    private bool HasCrop => HasIngredient();
    
    public InventoryManager inventoryManager;
    public Animator itemAnimator; 
    
    public override bool PlaceIngredient(Ingredient ingredient, bool forcePlace = false)
    {
        if (HasCrop && canTake && !inventoryManager.AnySlotsAvailable())
        {
            return false;
        }
        
        var b = base.PlaceIngredient(ingredient, forcePlace);

        // start growing in coroutine
        StartCoroutine(GrowIngredientCoroutine());
        
        return b;
    }

    protected override void CreateItemDisplay(Ingredient ingredient)
    {
        UpdateItemDisplay(false);
    }

    private void UpdateItemDisplay(bool ready)
    {
        if (ItemDisplay != null) 
            Destroy(ItemDisplay);
        ItemDisplay = null;
        
        if (!ready) CreateGrowingDisplay();
        else CreateReadyDisplay();
    }

    private IEnumerator GrowIngredientCoroutine()
    {
        canPlace = false;
        canTake = false;
        
        yield return new WaitForSeconds(Ingredient.seedGrowTime);
        
        UpdateItemDisplay(true);
        canTake = true;
        canPlace = true;
    }

    private void CreateReadyDisplay()
    {        
        var ingredientTex = Resources.Load<Texture2D>($"Sprites/{Ingredient.key}");
        var treeTex = Resources.Load<Texture2D>("Sprites/tree");

        var goTree = GameObject.CreatePrimitive(PrimitiveType.Quad);
        goTree.transform.SetParent(itemDisplayRoot);
        var goTreeRenderer = goTree.GetComponent<Renderer>();
        goTreeRenderer.material = new Material(itemDisplayMaterial);
        goTreeRenderer.material.mainTexture = treeTex;

        var goIngredient = GameObject.CreatePrimitive(PrimitiveType.Quad);
        goIngredient.transform.SetParent(goTree.transform);
        var goiRenderer = goIngredient.GetComponent<Renderer>();
        goiRenderer.material = new Material(itemDisplayMaterial);
        goiRenderer.material.mainTexture = ingredientTex;

        goTree.transform.localPosition = new Vector3(0, 0.4f, 0);
        goIngredient.transform.localPosition = new Vector3(0, 0.4f, 0.15f);

        ItemDisplay = goTree;
    }

    private void CreateGrowingDisplay()
    {
        var chronoTex = Resources.Load<Texture2D>("Sprites/chrono");
        var ingredientTex = Resources.Load<Texture2D>($"Sprites/{Ingredient.key}");
        var treeTex = Resources.Load<Texture2D>("Sprites/tree");

        var goTree = GameObject.CreatePrimitive(PrimitiveType.Quad);
        goTree.transform.SetParent(itemDisplayRoot);
        var goTreeRenderer = goTree.GetComponent<Renderer>();
        goTreeRenderer.material = new Material(itemDisplayMaterial);
        goTreeRenderer.material.mainTexture = treeTex;

        var goIngredient = GameObject.CreatePrimitive(PrimitiveType.Quad);
        goIngredient.transform.SetParent(goTree.transform);
        var goiRenderer = goIngredient.GetComponent<Renderer>();
        goiRenderer.material = new Material(itemDisplayMaterial);
        goiRenderer.material.mainTexture = ingredientTex;
        goiRenderer.material.SetFloat("_Alpha", 0.6f);
        
        var goChrono = GameObject.CreatePrimitive(PrimitiveType.Quad);
        goChrono.transform.SetParent(goTree.transform);
        var goChronoRenderer = goChrono.GetComponent<Renderer>();
        goChronoRenderer.material = new Material(itemDisplayMaterial);
        goChronoRenderer.material.mainTexture = chronoTex;

        goTree.transform.localPosition = new Vector3(0, 0.4f, 0);
        goIngredient.transform.localPosition = new Vector3(0, 0.4f, 0.15f);
        goChrono.transform.localPosition = new Vector3(0, 1.5f, 0);

        ItemDisplay = goTree;
    }

    public override Ingredient TakeIngredient(bool forceTake = false)
    {

        var ingredient = new Ingredient(Ingredient){IsSeed = false};
        var taken = inventoryManager.PlaceIngredient(ingredient);

        if (taken)
        {
            base.TakeIngredient(forceTake);
        }
        else
        {
            Debug.Log("Could not place ingredient. All slots full.");
            itemAnimator.ResetTrigger("Shake");
            itemAnimator.SetTrigger("Shake");
            
        }

        return null;
        /*
        var taken = base.TakeIngredient(forceTake);

        return new Ingredient(taken)
        {
            IsSeed = false
        };*/
    }
}