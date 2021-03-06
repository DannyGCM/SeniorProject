using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.VisualScripting;
using System.Threading.Tasks;
using TMPro;

// Brock Wilson
// 12/4/21
// Florida Southern College
// Senior Project

public class ImageCycle : MonoBehaviour
{

    Renderer rend;

    public Transform skySphere;

    public Transform rotationPlane;

    public Transform rotationPlaneContainer;

    public string resourcesSkysphereImageDirectory = "Textures/Sky";

    public string resourcesAudioDirectory = "Audio/mp3Sound";

    public TextAsset mapAsset;

    int mapAssetImageIndex = 0;

    int mapAssetOffsetIndex = 1;

    int mapAssetAudioIndex = 2;

    int mapAssetDescriptionIndex = 3;

    int mapAssetTitleIndex = 4;

    int mapAssetButtonsStartIndex = 5;

    public InputActionReference rClick = null;

    public InputActionReference lClick = null;

    public Transform TourInteractables;

    public Transform DebuggerPanel;

    public AudioClip tourVoice;

    public AudioSource speaker;

    public AudioListener earpiece;

    public Camera maincam;


    // Contains all text from mapAsset
    string[][] lines;

    private Object[] texSphere;

    private Object[] audioArray;

    int rotPlaneIteration = 0;

    XRSimpleInteractable globalButton = null;

    string globalImageName;

    int globalClicks = 0;

    int descriptionBoxIteration = 1;

    // Start is called before the first frame update
    public void Start()
    {

        // Read txt file to build our button map
        lines = BuildLinesFromText(mapAsset.text);

        // Get the renderer component of Transform we want
        rend = skySphere.GetComponent<Renderer>();
        rend.material.color = new Color(1, 1, 1, 0);

        // Load all the skysphere textures
        texSphere = Resources.LoadAll(resourcesSkysphereImageDirectory, typeof(Texture2D));

        audioArray = Resources.LoadAll(resourcesAudioDirectory, typeof(AudioClip));

        rClick.action.started += delegate { AnnoyingButtonFunction(globalButton); };
        lClick.action.started += delegate { AnnoyingButtonFunction(globalButton); };

        Transform canvas = TourInteractables.Find("TopBar");
        canvas.Find("Close").GetComponent<Button>().onClick.AddListener(delegate { CloseDescBox(TourInteractables); });
        canvas.Find("Expand").GetComponent<Button>().onClick.AddListener(delegate { ResizeDescBox(TourInteractables); });
        canvas.Find("Minimize").GetComponent<Button>().onClick.AddListener(delegate { MinimizeDescBox(TourInteractables); });

    }
    void CloseDescBox(Transform TourInteractables)
    {
        TourInteractables.GetComponent<Animator>().SetBool("NewImage", false);
    }
    void ResizeDescBox(Transform TourInteractables)
    {
        descriptionBoxIteration = (descriptionBoxIteration + 1) % 3;
        TourInteractables.GetComponent<Animator>().SetInteger("Expand", descriptionBoxIteration);
    }
    void MinimizeDescBox(Transform TourInteractables)
    {
        TourInteractables.GetComponent<Animator>().SetTrigger("Minimize");
    }

    // Removes all whitespace from lines
    string[][] BuildLinesFromText(string text)
    {
        string[] rows = text.Split("\n"[0]);
        string[][] final = new string[rows.Length][];
        for (int i = 0; i < rows.Length; i++)
        {
            //rows[i] = rows[i].Replace(" ", string.Empty);
            string[] result = rows[i].Trim().Split(new string[] { ", " }, System.StringSplitOptions.None);
            //var temp = rows[i].Trim().Split(", "[]);
            /*if (temp[0] == "") temp[0] = "/";
            final[i] = temp;*/
            if (result[0] == "") result[0] = "/";
            final[i] = result;
        }
        /*for (int i = 0; i < final.Length; i++)
        {
            for (int j = 0; j < final[i].Length; j++)
            {
                Debug.Log(final[i][j]);
            }
        }*/
        return final;
    }

    // Returns array of strings representing the line found in text file
    // Function cleaned
    public string[] FindImageInTxt(string imgToFind)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i][mapAssetImageIndex].Trim().ToUpper() == imgToFind.Trim().ToUpper())
            {
                return lines[i]; // return line that contains information to build new environment
            }
        }
        string error = "No entry found in file: " + mapAsset.name + " searching by name: " + imgToFind;
        PanelDebug(error, true);
        Debug.LogError(error);

        return null;
    }


    // Returns index of the picture image that is found in our files
    // Function cleaned
    private int FindImgIndex(string imgName, Object[] texSphere)
    {
        imgName = imgName.Trim();
        for (int i = 0; i < texSphere.Length; i++)
        {
            string texName = texSphere[i].name.Trim();

            if (imgName.ToUpper() == texName.ToUpper())
                return i;
        }
        string error = "No texture found in files with name: " + imgName;
        PanelDebug(error, true);
        Debug.LogError(error);
        return -1;
    }

    // Deletes all of the buttons in the environment
    // Function cleaned
    public void ClearButtons()
    {
        for (int i = 0; i < rotationPlaneContainer.childCount; i++)
            Destroy(rotationPlaneContainer.GetChild(i).gameObject);
    }
    public void ResetAudio()
    {

    }
    public void DisableDescription()
    {
        TourInteractables.gameObject.SetActive(false);
    }

    public void PanelDebug(string message, bool isError = false)
    {

        Text description = DebuggerPanel.Find("Canvas").Find("Description").GetComponent<Text>();
        if (isError)
        {
            message = "Error: " + message;
        }
        description.text = "\n:{" + Time.frameCount + "|: " + message + " [CurrentImage:" + globalImageName + "] }:" + description.text;

    }

    // Takes in string arrays from mapAsset that contains angles/imgnames and spawns the buttons associated with the angles.
    // Function cleaned
    void SpawnButtons(string[] buttonsLine)
    {
        Debug.Log("Spawning buttons" + buttonsLine[0]);
        int numberOfButtons = buttonsLine.Length / 2;
        XRSimpleInteractable[] interactables = new XRSimpleInteractable[numberOfButtons];

        // Extract angles
        for (int i = 0; i < numberOfButtons; i++)
        {
            // Get the angle at even indexes of line and assign it to roty
            float rotationy = float.Parse(buttonsLine[i * 2]);

            // Create instance of our rotationPlane prefab
            Transform rotationPlaneClone = Instantiate(rotationPlane);

            // Set this instance's parent to be in a container for organization
            rotationPlaneClone.parent = rotationPlaneContainer;
            // Set position of rotation plane to be bound to parent
            rotationPlaneClone.position = rotationPlaneContainer.position;
            // With this instance, rotate it to degree specified from txt file
            rotationPlaneClone.transform.Rotate(0, rotationy, 0);
            // Set this instance's name to be 'rotationPlane[integer]'.
            rotationPlaneClone.name = rotationPlane.name + (i + rotPlaneIteration); // rotPlaneIteration is used to ensure that no old names can be reused once a button is deleted. This is primarily used for debugging

            // Set this index of the interactables array to the rotation plane's XRinteractable host gameobject
            interactables[i] = rotationPlaneClone.Find("Interaction").GetComponent<XRSimpleInteractable>();

        }
        // Add event listeners to all buttons in the canvas
        for (int i = 0; i < numberOfButtons; i++)
        {
            XRSimpleInteractable identifier = interactables[i];

            //interactables[i].selectEntered.AddListener(delegate { ButtonClicked(identifier); });

            Animator ArrowAnimator = interactables[i].gameObject.GetComponent<Animator>();
            interactables[i].hoverEntered.AddListener(delegate { RunArrow(ArrowAnimator, true, identifier); });
            interactables[i].hoverExited.AddListener(delegate { RunArrow(ArrowAnimator, false, identifier); });
        }
        rotPlaneIteration += numberOfButtons;
    }

    // Takes an input rotation (presumably of the button pressed) and tries to find that rotation in the line provided
    // Function cleaned
    private string FindButtonClicked(string[] buttonsLine, Vector3 rotation)
    {
        for (int i = 0; i < buttonsLine.Length; i += 2)
        {
            // Convert our text file angle to quaternion and compare
            float number;
            bool success = float.TryParse(buttonsLine[i], out number);
            if (success)
            {
                if (rotation == new Vector3(0, number, 0))
                {
                    // Return the imgname that follows the rotation angle
                    return buttonsLine[i + 1];
                }
            }
        }
        string error = "Could not find the angle: EulerAngles(" + rotation[0] + "," + rotation[1] + "," + rotation[2] + ") in the line provided.";
        PanelDebug(error, true);
        Debug.LogError(error);
        return null;
    }

    public string[] StringArraySlice(string[] list, int fromIndex, int toIndex)
    {
        int lengthOfSlicedList = toIndex - fromIndex;

        string[] slicedList = new string[lengthOfSlicedList + 1];

        for (int i = 0; i < lengthOfSlicedList; i++)
        {

            slicedList[i] = list[fromIndex + i];

        }
        slicedList[lengthOfSlicedList] = "null";

        return slicedList;
    }

    // Called when a listener function is triggered by a button being clicked. Functions similar to OnClick()
    public void ButtonClicked(XRSimpleInteractable button)
    {
        // Get the parent rotation plane of the button that was pressed
        Transform rotationPlane = button.transform.parent.transform;

        // Get name of image that is currently applied to our material
        string imgName = rend.material.GetTexture("_BaseMap").name;

        // Eliminate whitespace from imgName
        imgName = imgName.Trim();

        // Find the line in the text file that has this imgName as the host image we are currently in
        string[] line = FindImageInTxt(imgName);

        // From the line found, find an angle in the text that matches the one of the button that was clicked and return the image associated
        string[] buttonsLine = StringArraySlice(line, mapAssetButtonsStartIndex, line.Length); // Buttonsline only contains the button information
        string nextImg = FindButtonClicked(buttonsLine, rotationPlane.localRotation.eulerAngles);
        Debug.Log("Image: " + nextImg);
        if (nextImg != null)
        {
            // Eliminate whitespace from the string nextImg
            nextImg = nextImg.Trim().Replace(" ", "");

            // Load the skysphere buttons and texture
            LoadSkysphere(nextImg);
        }

    }

    public void LoadSkysphere(string imgName)
    {

        imgName = imgName.Trim();

        ClearButtons();
        string[] line = FindImageInTxt(imgName);


        string tempOffset = StringArraySlice(line, mapAssetOffsetIndex, mapAssetOffsetIndex + 1)[0];
        if (tempOffset == "" || tempOffset == " ") tempOffset = "0";
        float offset = float.Parse(tempOffset.Trim().Replace(" ", ""));

        string audio = StringArraySlice(line, mapAssetAudioIndex, mapAssetAudioIndex + 1)[0];

        string[] descriptionAndTitle = new string[] { StringArraySlice(line, mapAssetDescriptionIndex, mapAssetDescriptionIndex + 1)[0], StringArraySlice(line, mapAssetTitleIndex, mapAssetTitleIndex + 1)[0] };

        string[] buttonsLine = StringArraySlice(line, mapAssetButtonsStartIndex, line.Length);


        audio = audio.Trim();

        FindAndSetTextureOfSkySphere(imgName);
        SpawnButtons(buttonsLine);

        OffsetSkysphere(offset);
        Debug.Log("a");
        FindAndSpawnAudio(audio);
        Debug.Log("b");
        SpawnDescriptionAndTitle(descriptionAndTitle);
        Debug.Log("done");

    }

    public void FindAndSpawnAudio(string audio)
    {

        int audioIndex = FindAudioIndex(audio, audioArray);
        if (audioIndex != -1)
        {
            if (speaker.clip != (AudioClip)audioArray[audioIndex])
            {
                speaker.clip = (AudioClip)audioArray[audioIndex];
                speaker.Play();
            }

        }
        else
        {
            speaker.Stop();
        }
        // Play audio
        /*speaker.clip = tourVoice;*/



    }
    private int FindAudioIndex(string audio, Object[] audioArray)
    {
        audio = audio.Trim();
        for (int i = 0; i < audioArray.Length; i++)
        {
            string audioName = audioArray[i].name.Trim();
            //Debug.Log(audio + " " + audioName);
            if (audio.ToUpper() == audioName.ToUpper())
                return i;
        }
        string error = "No audio found in files with name: " + audio;
        if (audio != "")
        {
            PanelDebug(error, true);
        }
        return -1;
    }
    public void SpawnDescriptionAndTitle(string[] descriptionAndTitle)
    {
        Debug.Log(descriptionAndTitle[0] + " " + descriptionAndTitle[1]);
        string newDescription = descriptionAndTitle[0].Replace("_", " ");
        string newTitle = descriptionAndTitle[1].Replace("_", " ");
        if (newDescription != "" && newDescription != " ")
        {
            TextMeshProUGUI title = TourInteractables.Find("DescriptionPanel").Find("Canvas").Find("Title").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI description = TourInteractables.Find("DescriptionPanel").Find("Canvas").Find("Description").GetComponent<TextMeshProUGUI>();

            title.text = newTitle;
            description.text = newDescription;

            TourInteractables.GetComponent<Animator>().SetBool("NewImage", true);

        }
        else
        {
            TourInteractables.GetComponent<Animator>().SetBool("NewImage", false);
        }

    }
    public void OffsetSkysphere(float offset)
    {
        //Debug.Log(offset);
        //skySphere.parent.eulerAngles = new Vector3(0, offset, 0);
    }

    public void FindAndSetTextureOfSkySphere(string imgName)
    {

        //Debug.Log(imgName + " idk ");
        imgName = imgName.Trim();
        // Find the index of the image in file
        int imgIndex = FindImgIndex(imgName, texSphere);
        // Set skysphere to texture found with index
        rend.material.SetTexture("_BaseMap", (Texture2D)texSphere[imgIndex]);

        globalImageName = imgName;
        PanelDebug("");
    }

    void RunArrow(Animator ani, bool animating, XRSimpleInteractable button)
    {
        if (ani)
        {
            ani.SetBool("Hover", animating);
            if (animating == true)
            {
                globalButton = button;
            }

        }
    }
    void AnnoyingButtonFunction(XRSimpleInteractable button)
    {
        globalClicks += 1;
        //Debug.Log("clicked: " + globalClicks);
        button = globalButton;
        if (button)
        {
            button.transform.Find("Collider").GetComponent<MeshCollider>().enabled = false;
            //Debug.Log(button.transform.parent.name + " yeet ");
            ButtonClicked(button);
        }

    }


    // Update is called once per frame
    void Update()
    {

    }
}

