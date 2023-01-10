using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    
    public IngredientSlot[] slots;

    public bool AnySlotsAvailable()
    {
        return slots.Any(s => !s.HasIngredient());
    }

    public bool PlaceIngredient(Ingredient ingredient)
    {
        foreach (var slot in slots)
        {
            // If the slot is empty, use the slot.
            if (!slot.HasIngredient())
            {
                slot.PlaceIngredient(ingredient, true);
                // Placed Ingredient successfully, return true.
                return true;
            }
        }

        // Couldnt place ingredient since all slots were full. Return false.
        return false;
    }
}
