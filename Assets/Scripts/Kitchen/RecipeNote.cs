using System;
using Data;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kitchen
{
    public class RecipeNote : MonoBehaviour
    {
        public Animator animator;

        public Transform ingredient1;
        public Transform ingredient2;

        public TextMeshPro title;

        public Transform noteBase;

        private bool _visible;

        public void InitializeNote(Recipe recipe)
        {
            if (recipe == null)
            {
                noteBase.gameObject.SetActive(false);
            }
            else
            {
                noteBase.gameObject.SetActive(true);
            
                noteBase.Rotate(Vector3.forward, Random.Range(-5, 5));
                title.text = $"{recipe.product.name} (${recipe.product.Score*5})";

                var i1m = ingredient1.GetComponent<Renderer>().material;
                var i2m = ingredient2.GetComponent<Renderer>().material;

                i1m.mainTexture = Resources.Load<Texture2D>($"Sprites/{recipe.ingredients[0].key}");
                i2m.mainTexture = Resources.Load<Texture2D>($"Sprites/{recipe.ingredients[1].key}");
            }
        }
        
        public void ShowNote()
        {
            if (_visible) return;
            Debug.Log("Showing recipe note");
            animator.SetBool("Visible", true);
            _visible = true;
        }
        
        public void HideNote()
        {
            if (!_visible) return;
            Debug.Log("Hiding recipe note");
            animator.SetBool("Visible", false);
            _visible = false;
        }
    }
}
