using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.VisualScripting;

// Brock Wilson
// 12/4/21
// Florida Southern College
// Senior Project

public class InitBuildings : MonoBehaviour
{
    public Transform _initContainer;

    public Transform _finalInteractableContainer;

    public Transform _interactablePrefab;
    public GameObject _Manager;

    public Material _ghost;


    // Start is called before the first frame update
    void Start()
    {
        int initialCount = _initContainer.childCount;
        for (int i = 0; i < initialCount; i++)
        {
            Transform model = _initContainer.GetChild(0);
            
            PlacePrefab(model, _finalInteractableContainer);

            Transform interactableBuilding = _finalInteractableContainer.GetChild(i);

            interactableBuilding.name = model.name;
        }

        _Manager.GetComponent<SphereTransition>().NewStart();
        
    }

    // Places instance of the prefab interactable as child of a transform
    void PlacePrefab(Transform building, Transform container)
    {
        // Instantiate prefab
        Transform interactableClone = Instantiate(_interactablePrefab);
        Transform buildingClone = Instantiate(building);

        // Reparent model into prefab instance
        building.parent = container;

        // Change meshcollider one associated with model
        if (building.childCount == 0)
            BuildPrefab(interactableClone, building, buildingClone);
        else
            for (int i = 0; i < building.childCount; i++)
                BuildPrefab(interactableClone, building.GetChild(i), buildingClone);

    }
    void BuildPrefab(Transform interactableClone, Transform building, Transform buildingClone)
    {
        ChangeMeshCollider(interactableClone, building);
        // Add prefab to building
        interactableClone.parent = building;
        ConfigurePhysics(interactableClone);
        buildingClone.parent = interactableClone.Find("BuildingVisuals/Model");
        ChangeModel(building, buildingClone);
        _Manager.GetComponent<SphereTransition>().InsertGrabListener(building);
        Transform highlightParent = interactableClone.Find("BuildingVisuals/Highlight");
        InsertHighlightModel(highlightParent, buildingClone, _ghost);

    }
    void ChangeModel(Transform building, Transform buildingClone)
    {
        Destroy(building.GetComponent<MeshFilter>());
        Destroy(building.GetComponent<MeshRenderer>());
        buildingClone.localScale = new Vector3(1, 1, 1);
        buildingClone.localPosition = new Vector3(0, 0, 0);
        buildingClone.localRotation = Quaternion.Euler(0,0,0);
    }
    void InsertHighlightModel(Transform parent, Transform model, Material material)
    {
        Mesh mesh = model.GetComponent<MeshFilter>().mesh;
        Transform Highlight = parent.GetChild(0);
        Highlight.GetComponent<MeshFilter>().mesh = mesh;
        //Highlight.GetComponent<MeshRenderer>().materials = new Material[] { material };
        Highlight.localScale = new Vector3(1,1,1);
        parent.localScale = new Vector3((float).9, (float).9, (float).9);
        Highlight.localPosition = new Vector3(0, 0, 0);
        Highlight.localRotation = Quaternion.Euler(0, 0, 0);
        //Highlight.GetComponent<Material>().color = new Color(0,0,0,0);
    }
    void ChangeMeshCollider(Transform buildingPhysics, Transform model)
    {
        buildingPhysics.gameObject.AddComponent<MeshCollider>();
        buildingPhysics.GetComponent<MeshCollider>().convex = true;
        buildingPhysics.GetComponent<MeshCollider>().sharedMesh = model.GetComponent<MeshFilter>().sharedMesh;
    }
    void ConfigurePhysics(Transform buildingPhysics)
    {

        buildingPhysics.localPosition = new Vector3(0, 0, 0);
        buildingPhysics.localScale = new Vector3(1, 1, 1);
        buildingPhysics.localRotation = Quaternion.Euler(0, 0, 0);
        buildingPhysics.gameObject.layer = 8;
        buildingPhysics.AddComponent<XRGrabInteractable>();
        buildingPhysics.GetComponent<XRGrabInteractable>().trackRotation = false;
        buildingPhysics.AddComponent<SpringJoint>();
        buildingPhysics.AddComponent<ArticulationBody>();
        buildingPhysics.GetComponent<ArticulationBody>().useGravity = false;
        buildingPhysics.GetComponent<ArticulationBody>().immovable = true;
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
