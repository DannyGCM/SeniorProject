using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Interactions;

public class ClickDetection : MonoBehaviour
{
    public InputActionReference rClick = null;
    public InputActionReference lClick = null;
    public MeshCollider collider = null;
    // Start is called before the first frame update
    void Start()
    {
        //rClick.action.performed += delegate { ClickEnabled(collider); };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
