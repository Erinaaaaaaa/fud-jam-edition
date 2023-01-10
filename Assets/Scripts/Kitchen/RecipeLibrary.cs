using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Unity.VisualScripting;
using UnityEngine;

namespace Kitchen
{
    public class RecipeLibrary : MonoBehaviour
    {
        [NonSerialized] public Dictionary<string, Ingredient> Ingredients;
        [NonSerialized] public List<Recipe> Recipes;

        private int _maxScore = 0;
        
        void Start()
        {
            var json = Resources.Load<TextAsset>("ingredients");
            var obj = JsonUtility.FromJson<IngredientsJson>(json.text);


            Ingredients = obj.ingredients.ToDictionary(i => i.key);
            Recipes = obj.recipes.Select(r => new Recipe
            {
                ingredients = r.ingredients.Select(i => Ingredients[i]).ToArray(),
                product = Ingredients[r.product]
            }).ToList();

            foreach (var (_, ingredient) in Ingredients)
            {
                _maxScore = Math.Max(_maxScore, GetScoreForIngredient(ingredient));
            }

            Debug.Log($"{obj.ingredients.Length} ingredients and {Recipes.Count} recipes loaded");
        }

        // Methods

        public int GetScoreForIngredient(Ingredient ingredient)
        {
            var libraryIngredient = Ingredients[ingredient.key];
            
            if (libraryIngredient.Score > 0)
                return libraryIngredient.Score;
            
            var r = GetRecipesForProduct(ingredient).FirstOrDefault();
            if (r == null)
            {
                return libraryIngredient.Score = 1;
            }

            return libraryIngredient.Score = GetScoreForIngredient(r.ingredients[0]) + GetScoreForIngredient(r.ingredients[1]) + 1;
        }

        public int GetMaxScore()
        {
            return _maxScore;
        }

        public Ingredient GetIngredient(string key)
        { 
            return Ingredients.TryGetValue(key, out var i) ? i : null;
        }

        public Ingredient GetSeedForIngredient(string key)
        {
            if (Ingredients.TryGetValue(key, out var baseIngredient))
            {
                var i = new Ingredient(baseIngredient)
                {
                    IsSeed = true,
                };

                return i;
            }

            return null;
        }
        
        public IEnumerable<Recipe> GetRecipesForIngredient(Ingredient ingredient)
        {
            return Recipes.Where(r => r.ingredients.Contains(ingredient));
        }

        public IEnumerable<Recipe> GetRecipesForProduct(Ingredient product)
        {
            return Recipes.Where(r => Equals(r.product, product));
        }
        
        

        public Ingredient GetRecipeResult(Ingredient left, Ingredient right)
        {
            return Recipes
                .Where(recipe => (Equals(left, recipe.ingredients[0]) && Equals(right, recipe.ingredients[1])) ||
                                 (Equals(left, recipe.ingredients[1]) && Equals(right, recipe.ingredients[0])))
                .Select(recipe => recipe.product)
                .FirstOrDefault();
        }

        private List<Ingredient> _baseIngredientsCache = null;
        private List<Ingredient> _tipIngredientsCache = null;

        public IEnumerable<Ingredient> GetBaseIngredients()
        {
            if (_baseIngredientsCache != null)
                return _baseIngredientsCache;
        
            var l = new List<Ingredient>();
        
            foreach (var (_, ingredient) in Ingredients)
            {
                if (!GetRecipesForProduct(ingredient).Any())
                    l.Add(ingredient);
            }

            _baseIngredientsCache = l;

            return l;
        }
        
        public IEnumerable<Ingredient> GetTipIngredients()
        {
            if (_tipIngredientsCache != null)
                return _tipIngredientsCache;
        
            var l = new List<Ingredient>();
        
            foreach (var (_, ingredient) in Ingredients)
            {
                if (!GetRecipesForIngredient(ingredient).Any())
                    l.Add(ingredient);
            }

            _tipIngredientsCache = l;

            return l;
        }
        
        // Private classes for JSON
        
        [Serializable]
        private class IngredientsJson
        {
            public Ingredient[] ingredients;
            public RecipeJson[] recipes;
        }

        [Serializable]
        private class RecipeJson
        {
            public string key;
            public string[] ingredients;
            public string product;
        }
    }
}