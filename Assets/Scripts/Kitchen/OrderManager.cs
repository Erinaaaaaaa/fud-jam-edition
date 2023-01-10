using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Kitchen
{

    public class OrderManager : MonoBehaviour
    {
        private int _orderCount = 0;
        private Queue<Ingredient> _orderQueue = new();
        public int QueueLength => _orderQueue.Count;

        public int ingredientLevel = 1;
        public RecipeLibrary recipeLibrary;
        public Order[] orderReceptors;
        public TextMeshPro queueText;

        public float minTimeBetweenOrders = 10;
        public float maxTimeBetweenOrders = 30;

        public int ordersToLevelUp = 5;
        
        public float endGameMaxTime = 10;
        public float endGameMinTime = 5;

        public float initialOrderDelay = 0;

        public AudioSource dingSound;
        public AudioSource levelUpSound;
        
        [NonSerialized] public int OrdersFulfilled = 0;
        [NonSerialized] public int OrdersFailed = 0;
        public int TotalOrders => OrdersFailed + OrdersFulfilled;

        public UnityEvent orderGenerated;
        public OrderCompleted orderCompleted;

        private List<Ingredient> _eligibleIngredients = new();
        private List<Ingredient> _servedIngredients = new();
        private List<Ingredient> _remainingIngredients = new();

        private double _newProbability = 0.4;
        
        private Random _random = new();

        private float _nextOrder = 0;

        private void Start()
        {
            InitEligibleIngredients();
            _nextOrder = Time.time + initialOrderDelay;
        }

        private void Update()
        {
            if (!(Time.time > _nextOrder)) return;
            
            Debug.Log("Generating new order");
            GenerateNewOrder();
        }

        private void InitEligibleIngredients()
        {
            _eligibleIngredients.Clear();
            
            _eligibleIngredients.AddRange(recipeLibrary.Ingredients.Values.Where(i => i.Score <= ingredientLevel));
            
            _remainingIngredients.Clear();
            _remainingIngredients.AddRange(_eligibleIngredients.Where(i => !_servedIngredients.Contains(i)));
            
            Debug.Log($"Prepared {_eligibleIngredients.Count} eligible ingredients including {_remainingIngredients.Count} new ingredients");
        }

        
        private void GenerateNewOrder()
        {
            var newOnly = _random.NextDouble() < _newProbability;
            Ingredient nextIngredient;

            if (newOnly && _remainingIngredients.Count != 0)
            {
                nextIngredient = _remainingIngredients[_random.Next(_remainingIngredients.Count)];
                Debug.Log($"Queued new ingredient {nextIngredient.name} from {_remainingIngredients.Count} ingredients");
            }
            else
            {
                nextIngredient = _eligibleIngredients[_random.Next(_eligibleIngredients.Count)];
                Debug.Log($"Queued random ingredient {nextIngredient.name} from {_eligibleIngredients.Count} ingredients");
            }
            
            if (_remainingIngredients.Contains(nextIngredient))
                _remainingIngredients.Remove(nextIngredient);
            _orderQueue.Enqueue(nextIngredient);
            dingSound.Play();
            
            ProvideOrder();
            _newProbability += 0.2 / ingredientLevel;
            _newProbability = Math.Min(0.8, _newProbability);
            orderGenerated.Invoke();
            _nextOrder = (float)(Time.time + (_random.NextDouble() * (maxTimeBetweenOrders - minTimeBetweenOrders) +
                                              minTimeBetweenOrders) * Math.Pow(recipeLibrary.GetScoreForIngredient(nextIngredient), 0.3333333));
        }

        public void ProvideOrder()
        {
            if (_orderQueue.Count == 0)
                return;
            
            var availableReceptor = orderReceptors.FirstOrDefault(r => r.served);

            if (availableReceptor == null)
            {
                Debug.Log("No available receptors");
            }
            else
            {
                availableReceptor.ingredient = _orderQueue.Dequeue();
                availableReceptor.served = false;
                availableReceptor.ShowOrder($"Order #{++_orderCount}");
                availableReceptor.slot.canPlace = true;

                Debug.Log($"Provided order for {availableReceptor.ingredient.name}");
            }

            queueText.text = $"Orders in queue: {_orderQueue.Count}/10";
        }
        
        public void CompleteOrder(Ingredient ingredient, bool success)
        {
            if (success)
                OrdersFulfilled++;
            else
                OrdersFailed++;

            if (!_servedIngredients.Contains(ingredient))
            {
                _servedIngredients.Add(ingredient);
            }

            if (_remainingIngredients.Contains(ingredient))
            {
                _remainingIngredients.Remove(ingredient);
                Debug.Log($"{_remainingIngredients.Count} remaining ingredients");
            }

            if (_orderQueue.Count == 0)
            {
                _nextOrder = Math.Min(Time.time + 5, _nextOrder);
            }
            else
            {
                ProvideOrder();
            }

            LevelUp();
            
            orderCompleted.Invoke(ingredient, success);
        }

        private void LevelUp()
        {
            Debug.Log("Level Up");
            if (TotalOrders % ordersToLevelUp == 0)
            {
                maxTimeBetweenOrders -= 1;
                minTimeBetweenOrders -= 0.5f;

                maxTimeBetweenOrders = Math.Max(endGameMaxTime, maxTimeBetweenOrders);
                minTimeBetweenOrders = Math.Max(endGameMinTime, minTimeBetweenOrders);
            }

            var hasLeveledUp = false;
            while (_eligibleIngredients.All(i => _servedIngredients.Contains(i)) && recipeLibrary.GetMaxScore() > ingredientLevel)
            {
                ingredientLevel++;
                ingredientLevel = Math.Min(recipeLibrary.Ingredients.Values.Max(i => i.Score), ingredientLevel);
            
                InitEligibleIngredients();

                hasLeveledUp = true;
                _newProbability = 0.4;
            }

            if (hasLeveledUp)
            {
                Debug.Log("Ingredient Level Up");
                levelUpSound.Play();
            }
        }
    }
}