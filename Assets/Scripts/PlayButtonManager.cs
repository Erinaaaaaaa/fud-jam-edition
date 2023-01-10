using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonManager : MonoBehaviour
{
    private Camera _raySource;
    
    private void Start()
    {
        _raySource = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        
        var ray = _raySource.ScreenPointToRay(Input.mousePosition);
        var mask = LayerMask.GetMask("Restart");

        if (!Physics.Raycast(ray, out var hit, 100, mask))
            return;

        if (hit.transform == transform)
        {
            SceneManager.LoadScene("Scenes/Game", LoadSceneMode.Single);
        }
    }
}
