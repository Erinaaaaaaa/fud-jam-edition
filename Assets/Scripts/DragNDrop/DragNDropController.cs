using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace DragNDrop
{
    public class DragNDropController : MonoBehaviour
    {
        public Camera raySource;

        public Ingredient heldIngredient = null;

        public Material itemDisplayMaterial;
        
        private bool IsHolding => heldIngredient != null && !string.IsNullOrWhiteSpace(heldIngredient.key);
        private GameObject _heldIngredientView = null;
        
        private void Update()
        {
            bool forceRecreate = false;
            
            if (Input.GetMouseButtonDown(0))
            {
                var ray = raySource.ScreenPointToRay(Input.mousePosition);
                var mask = LayerMask.GetMask("Inventory");

                if (!Physics.Raycast(ray, out var hit, 100, mask))
                    return;

                var obj = hit.transform;

                var slot = obj.GetComponent<IngredientSlot>();

                Ingredient taken = null;
                bool hasPlaced = false, hasTaken = false;

                // Hand full: placing OR swapping.
                if (IsHolding)
                {
                    // Ensure same ingredient type
                    if (slot.AcceptsSeeds == heldIngredient.IsSeed || slot.isTrash)
                    {
                        // Swapping
                        if (slot.HasIngredient() && slot.CanSwap)
                        {
                            taken = slot.TakeIngredient();
                            slot.PlaceIngredient(heldIngredient);
                            hasTaken = hasPlaced = true;
                        }
                        // Placing
                        else if (!slot.HasIngredient() && slot.canPlace)
                        {
                            slot.PlaceIngredient(heldIngredient);
                            hasPlaced = true;
                        }
                    }
                }
                // Hand empty: nothing OR taking
                else
                {
                    // Taking
                    if (slot.HasIngredient() && slot.canTake)
                    {
                        taken = slot.TakeIngredient();
                        hasTaken = true;
                    }
                }
                
                if (hasPlaced)
                    heldIngredient = null;

                if (hasTaken)
                    heldIngredient = taken;

                forceRecreate = hasPlaced && hasTaken;
            }

            UpdateHeldItem(forceRecreate);
        }

        private void UpdateHeldItem(bool forceRecreate = false)
        {
            if (!IsHolding || forceRecreate)
            {
                if (_heldIngredientView == null)
                {
                    return;
                }
                
                Destroy(_heldIngredientView);
                _heldIngredientView = null;

                if (!forceRecreate)
                    return;
            }

            var ray = raySource.ScreenPointToRay(Input.mousePosition);
            var mask = LayerMask.GetMask("DnD Preview");

            var pos = new Vector3(0, -1, 0);
            
            if (Physics.Raycast(ray, out var hit, 100, mask))
            {
                pos = hit.point;
            }

            GameObject go;

            if (_heldIngredientView == null && heldIngredient != null)
            {
                var tex = Resources.Load<Texture2D>($"Sprites/{heldIngredient.key}");                
                
                if (tex != null)
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    go.transform.SetParent(transform);
                    var renderer = go.GetComponent<Renderer>();
                    renderer.material = new Material(itemDisplayMaterial);
                    renderer.material.mainTexture = tex;
                    renderer.material.SetFloat("_Alpha", 0.5f);
                    
                    if (heldIngredient.IsSeed)
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
                    go = new GameObject("Textureless ingredient");
                    go.transform.SetParent(transform);
                    var tmpText = go.AddComponent<TextMeshPro>();
                    tmpText.text = heldIngredient.name;
                    if (heldIngredient.IsSeed)
                    {
                        tmpText.text += " Seed";
                    }
                    tmpText.fontSize = 2;
                    tmpText.alignment = TextAlignmentOptions.Center;
                    go.transform.forward = Vector3.down;
                    var rect = go.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(1, 1);
                }

                _heldIngredientView = go;
            }
            else
            {
                go = _heldIngredientView;
            }

            if (go != null) go.transform.position = pos;
        }
    }
}