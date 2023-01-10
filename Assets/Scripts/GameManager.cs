using System;
using Data;
using Kitchen;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public OrderManager orders;

    public TextMeshPro scoring;
    public TextMeshPro gameOverText;

    public GameObject room;
    public GameObject dndManager;

    public AudioSource music;
    public AudioSource gameOverSfx;

    private int _score = 0;
    private int _money = 0;

    private Camera _raySource;

    public GameObject restartButton;
    public GameObject gameOverScreen;
    
    private void Start()
    {
        _raySource = Camera.main;
        restartButton.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        
        var ray = _raySource.ScreenPointToRay(Input.mousePosition);
        var mask = LayerMask.GetMask("Restart");

        if (!Physics.Raycast(ray, out var hit, 200, mask))
            return;

        if (hit.transform == restartButton.transform)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void UpdateScoringText()
    {
        scoring.text = $"Score: ${_score}\n" +
                       $"Orders fulfilled: {orders.OrdersFulfilled}\n" +
                       $"Mistakes: {orders.OrdersFailed}/3";

        gameOverText.text = $"You served\n" +
                            $"{orders.OrdersFulfilled} orders\n" +
                            $"and earned ${_score}";
    }

    public void OrderCompleted(Ingredient i, bool success)
    {
        var iScore = orders.recipeLibrary.GetScoreForIngredient(i) * 5;

        if (success)
        {
            _score += iScore;
            _money += iScore;
        }
        
        CheckGameOver();
    }

    public void CheckGameOver()
    {
        if (orders.QueueLength > 7)
        {
            var i = orders.QueueLength - 7;

            music.pitch = 1 + (i * 0.2f);

        }
        else
        {
            music.pitch = 1;
        }
        
        if (orders.QueueLength > 10 || orders.OrdersFailed >= 3)
        {
            orders.enabled = false;
            orders.gameObject.SetActive(false);
            
            dndManager.SetActive(false);
            
            var rb = room.AddComponent<Rigidbody>();
            rb.AddForce(new Vector3(0, 3, 0), ForceMode.VelocityChange);
            rb.AddTorque(new Vector3(420, 69, 0));
            
            music.Stop();
            gameOverScreen.SetActive(true);
            restartButton.SetActive(true);
            gameOverSfx.Play();
        }
    }
}
