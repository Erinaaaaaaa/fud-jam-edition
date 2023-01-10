using System;
using System.Linq;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Kitchen
{
    [Serializable]
    public class OrderCompleted : UnityEvent<Ingredient, bool>
    {
    }

    public class Order : MonoBehaviour
    {
        public RecipeLibrary recipeLibrary;
        
        public Ingredient ingredient;
        public IngredientSlot slot;
        
        public GameObject notePrefab;
        private Note _currentNote;

        public OrderCompleted orderCompleted;

        public AudioSource successSound;
        public AudioSource failureSound;

        public bool served;

        public void ShowOrder(string title)
        {
            _currentNote = Instantiate(notePrefab, transform).GetComponent<Note>();
            _currentNote.recipe = recipeLibrary.GetRecipesForProduct(ingredient).FirstOrDefault();
            if (_currentNote.recipe != null)
                _currentNote.recipe.product.Score = recipeLibrary.GetScoreForIngredient(ingredient);
            _currentNote.ShowOrder(ingredient, title);
        }

        public void HideOrder(Ingredient i)
        {
            _currentNote.HideNote(i);
        }

        public void CompleteOrder(Ingredient i)
        {
            var success = false;
            
            if (Equals(i, ingredient))
            {
                Debug.Log("Success");
                success = true;
                successSound.Play();
            }
            else
            {
                Debug.Log("Incorrect order");
                failureSound.Play();
            }

            served = true;
            slot.canPlace = false;
            slot.PlaceIngredient(null, true);
            HideOrder(i);
            orderCompleted.Invoke(i, success);
        }
    }
}