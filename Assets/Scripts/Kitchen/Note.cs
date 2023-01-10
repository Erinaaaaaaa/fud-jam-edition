using System;
using System.Linq;
using Data;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kitchen
{
    public class Note : MonoBehaviour
    {
        public Recipe recipe;
        
        public TextMeshPro noteTitle;

        public Animator noteAnimator;
        public TextMeshPro noteBody;
        public Transform ingredientIllustration;

        public IngredientSlot servedSlot;
        public RecipeNote recipeNote;

        public AudioSource slideSound;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var mask = LayerMask.GetMask("Recipe Notes");

            if (!Physics.Raycast(ray, out var hit, 100, mask)
                || hit.transform != transform)
            {
                HideRecipe();
                return;
            }

            ShowRecipe();
        }

        public void ShowOrder(Ingredient ingredient, string title)
        {
            var tex = Resources.Load<Texture2D>($"Sprites/{ingredient.key}");

            if (tex != null)
            {
                ingredientIllustration.gameObject.SetActive(true);
                var renderer = ingredientIllustration.GetComponent<Renderer>();
                renderer.material.mainTexture = tex;
                ingredientIllustration.Rotate(Vector3.forward, Random.Range(-15, 15));
            }
            else
            {
                ingredientIllustration.gameObject.SetActive(false);
            }

            noteBody.text = ingredient.name;
            noteAnimator.SetBool("Note Visible", true);
            noteTitle.text = title;
            
            recipeNote.InitializeNote(recipe);
        }

        public void HideNote(Ingredient i)
        {
            noteAnimator.SetBool("Note Visible", false);
            servedSlot.PlaceIngredient(i, true);
        }

        public void ShowRecipe()
        {
            recipeNote.ShowNote();
        }

        public void HideRecipe()
        {
            recipeNote.HideNote();
        }

        public void DestroyNote()
        {
            Destroy(gameObject);
        }

        public void PlaySlideSound()
        {
            slideSound.Play();
        }
    }
}