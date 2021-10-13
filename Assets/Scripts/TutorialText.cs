using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialText : MonoBehaviour
{
    public TextMeshProUGUI main;
    public TextMeshProUGUI upper;
    public TextMeshProUGUI lower;

    public GameObject next;
    public GameObject last;

    public GameObject show;

    public GameObject menu;
    public GameObject menuButton;
    public GameObject hideButton;
    public GameObject bodyValues;
    public GameObject bodyContainer;
    public GameObject timeSlider;
    public GameObject timeText;
    public GameObject cameraToggle;
    public GameObject volumeButton;
    public GameObject autoPause;


    private int clicks;
    private bool showClicked;
    private bool timeStopped;
    private bool cameraOn;
    private bool constantDisable;
    private bool past;
    private List<GameObject> objects;
    private List<string> bodies;
    private GameObject center;


    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        if (constantDisable)
        {
            if (!past)
            {
                Disable();
            }
            if ((GameManager.Instance.getObjectCount() == 1 && clicks == 3) || (GameManager.Instance.getObjectCount() == 2 && clicks == 14))
            {
                toggleSun(false);
                toggleEarth(false);
            }
        }
        
    }

    public void init()
    {
        clicks = 0;
        show.GetComponent<Button>().interactable = false;
        showClicked = false;
        timeStopped = false;
        cameraOn = true;
        clearText();
        objects = new List<GameObject>();
        constantDisable = false;
        toggleSun(true);
        past = false;
        bodies = new List<string>();
        bodies.Add("Sun");
        bodies.Add("Earth");
        bodies.Add("Venus");
        bodies.Add("Neptune");
        bodies.Add("Saturn");
        bodies.Add("Mars");
        bodies.Add("Jupiter");
        bodies.Add("Uranus");
        bodies.Add("Mercury");
        bodies.Add("Black Hole");
        click();
    }


    public void click()
    {
        if (clicks == 0)
        {

            updateObjects();
            show.GetComponent<Button>().interactable = false;

            clearText();
            main.text = "Welcome to the Astronomical Bodies Tutorial! We are going to go into some of the features that make " +
                "this game more enjoyable! Please use the buttons in the top right to navigate through the tutorial.";
        }
        else if (clicks == 1)
        {
            updateObjects();
            clearText();
            main.text = "Let's start by pressing the 'Show' button! This will make the interface with all data and astronomical body options appear.";
            show.GetComponent<Button>().interactable = true;
            show.GetComponent<Button>().onClick.AddListener(showPressed);
        }
        else if (clicks == 2)
        {
            updateObjects();
            if (showClicked)
            {
                clearText();
                main.text = "This is the menu used to interact with what is happening on screen. Let's check out all our options!";
                Disable();
            }
            else
            {
                clicks = 1;
            }
        }
        else if (clicks == 3)
        {
            updateObjects();
            clearText();
            upper.text = "Right above is all of the astronomical bodies that you can put into the universe. Let's try putting a sun into the world by " +
                "dragging and dropping the sun icon wherever you would like into the space below.";
            Disable();
            toggleSun(true);
            constantDisable = true;

        }
        else if (clicks == 4)
        {
            updateObjects();
            constantDisable = false;
            Disable();
            Debug.Log(GameManager.Instance.getObjectCount());
            if (GameManager.Instance.getObjectCount() == 1)
            {
                autoPause.GetComponent<Toggle>().isOn = false;
                clearText();
                lower.text = "Great! Now let's set the simulation speed to 0, which will stop all movement. You can do this by moving the slider all the way to the left or clicking the 'Pause Time' button.";
                timeSlider.GetComponent<Slider>().onValueChanged.AddListener((i) => timeCompare(i));
                toggleSun(false);
                timeSlider.GetComponent<Slider>().interactable = true;
                autoPause.GetComponent<Toggle>().interactable = true;
            }
            else
            {
                clicks = 3;
                click();
            }
        }
        else if (clicks == 5)
        {
            updateObjects();
            timeCompare(0);
            if (timeStopped)
            {
                constantDisable = false;
                clearText();
                upper.text = "Now that time is essentially stopped, it is much easier to manipulate what is happening in the simulation";
                Disable();
            }
            else { clicks = 4; }
        }
        else if (clicks == 6)
        {
            cameraToggle.GetComponent<Toggle>().isOn = true;
            cameraToggle.GetComponent<Toggle>().interactable = false;
            updateObjects();
            clearText();
            lower.text = "You can click and drag on a body to move it wherever you would like, give it a try if you want!";
        }
        else if (clicks == 7)
        {
            cameraToggle.GetComponent<Toggle>().isOn = true;
            cameraToggle.GetComponent<Toggle>().interactable = true;
            updateObjects();
            clearText();
            lower.text = "Now click the toggle camera option in order to turn off the automatic camera. This will allow you to zoom in.";
            cameraToggle.GetComponent<Toggle>().onValueChanged.AddListener((b) => cameraCheck(b));


        }
        else if (clicks == 8)
        {
            updateObjects();
            if (!cameraOn)
            {
                cameraToggle.GetComponent<Toggle>().interactable = false;
                //cameraToggle.GetComponent<Toggle>().interactable = false;
                constantDisable = false;
                clearText();
                lower.text = "Try zooming in on the sun using the scroll wheel or touchpad until you can easily see the black arrow.";
                Disable();
            }
            else { clicks = 7; }
        }
        else if (clicks == 9)
        {
            updateObjects();
            clearText();
            lower.text = "This arrow shows both the direction and magnitude of the movement of the body.";
        }
        else if (clicks == 10)
        {
            updateObjects();
            clearText();
            lower.text = "Try clicking and dragging the arrow in a different direction, then zoom out, toggle the camera, and increase the simulation speed slowly.";
            cameraToggle.GetComponent<Toggle>().interactable = true;
            autoPause.GetComponent<Toggle>().interactable = true;
            timeSlider.GetComponent<Slider>().interactable = true;
        }

        else if (clicks == 11)
        {
            updateObjects();
            clearText();
            main.text = "You should see the sun move in whatever direction you dragged the arrow to be pointing in! If not feel free to press 'Back' to go back";
        }
        else if (clicks == 12)
        {
            updateObjects();
            clearText();
            upper.text = "To the left is a panel of information invloving the bodies. Here you can change their position, directional veloctiy, and mass to exact values!";
        }
        else if (clicks == 13)
        {
            updateObjects();
            clearText();
            upper.text = "You can also destroy the planet by using the trashcan button, or change the trail length.";
            toggleEarth(false);
        }
        else if (clicks == 14)
        {
            past = true;
            updateObjects();
            clearText();
            upper.text = "Let's see what happens with two bodies in the system! Drag an Earth into the system and click 'Next' if it is in a stable orbit.";
            toggleEarth(true);
            constantDisable = true;
        }
        else if (clicks == 15)
        {
            updateObjects();
            constantDisable = false;
            Debug.Log(GameManager.Instance.getObjectCount());
            if (GameManager.Instance.getObjectCount() == 2)
            {
                clearText();
                lower.text = "Great! If you click on both the sun and the earth, you will see them get an outline, along with their value tables. This means they are selected!";
                toggleEarth(false);
            }
            else
            {
                clicks = 14;
                click();
            }
        }
        else if (clicks == 16)
        {
            if (GameManager.Instance.getSOneSTwo())
            {
                updateObjects();
                clearText();
                lower.text = "Just below is the current distance of the two selected bodies!";
            }
            else
            {
                clicks = 15;
                click();
            }
        }

        //FIX VALUES
        else if (clicks == 17)
        {
            updateObjects();
            clearText();
            erase();
            main.text = "Those are some of the basics that can help you fully utilize this simulator! Feel free to stay in here and do whatever you'd like, or head back out to the main menu!";

        }
        else if (clicks == 18)
        {
            updateObjects();
            clearText();
            Enable();
        }


        //click
        // have them toggle camera
        // check and say to zoom in and manipulate arrow
        // check arrow manipulation and say woo

    }

    private void clearText()
    {
        main.text = "";
        upper.text = "";
        lower.text = "";
    }

    public void Disable()
    {
        toggleEarth(false);
        autoPause.GetComponent<Toggle>().interactable = false;
        cameraToggle.GetComponent<Toggle>().interactable = false;
        timeSlider.GetComponent<Slider>().interactable = false;
        hideButton.GetComponent<Button>().interactable = false;
        if (!constantDisable)
        {
            foreach (Transform child in bodyContainer.transform)
            {
                if (child.gameObject.GetComponent<DragAndDropCell>().getInteractable() != false)
                {
                    child.gameObject.GetComponent<DragAndDropCell>().setInteractable(false);
                    child.Find("Planet").gameObject.SetActive(false);
                }
            }
        }
        foreach (Transform child in bodyValues.transform)
        {
            foreach (Transform panel in child.gameObject.transform) 
            { 
                foreach (Transform components in panel.gameObject.transform)
                {
                    if (components.gameObject.GetComponent<Button>() != null)
                    {
                        components.gameObject.GetComponent<Button>().interactable = false;
                    }
                    if (components.gameObject.GetComponent<InputField>() != null)
                    {
                        components.gameObject.GetComponent<InputField>().interactable = false;
                    }
                    if (components.gameObject.GetComponent<Slider>() != null)
                    {
                        components.gameObject.GetComponent<Slider>().interactable = false;
                    }
                }
            }
        }
    }

    public void Enable()
    {
        autoPause.GetComponent<Toggle>().interactable = true;
        cameraToggle.GetComponent<Toggle>().interactable = true;
        timeSlider.GetComponent<Slider>().interactable = true;
        hideButton.GetComponent<Button>().interactable = true;
        if (!constantDisable)
        {
            foreach (Transform child in bodyContainer.transform)
            {
                if (child.gameObject.GetComponent<DragAndDropCell>().getInteractable() != true)
                {
                    child.gameObject.GetComponent<DragAndDropCell>().setInteractable(true);
                    child.Find("Planet").gameObject.SetActive(true);
                }
            }
        }
        foreach (Transform child in bodyValues.transform)
        {
            foreach (Transform panel in child.gameObject.transform)
            {
                foreach (Transform components in panel.gameObject.transform)
                {
                    if (components.gameObject.GetComponent<Button>() != null)
                    {
                        components.gameObject.GetComponent<Button>().interactable = true;
                    }
                    if (components.gameObject.GetComponent<InputField>() != null)
                    {
                        components.gameObject.GetComponent<InputField>().interactable = true;
                    }
                    if (components.gameObject.GetComponent<Slider>() != null)
                    {
                        components.gameObject.GetComponent<Slider>().interactable = true;
                    }
                }
            }
        }
    }

    public void toggleSun(bool value)
    {
        foreach (Transform child in bodyContainer.transform)
        {
            if (child.gameObject.tag.Equals("Sun"))
            {
                child.gameObject.GetComponent<DragAndDropCell>().setInteractable(value);
                child.Find("Planet").gameObject.SetActive(value);
            }
        }
    }

    public void toggleEarth(bool value)
    {
        foreach (Transform child in bodyContainer.transform)
        {
            if (child.gameObject.tag.Equals("Earth"))
            {
                child.gameObject.GetComponent<DragAndDropCell>().setInteractable(value);
                child.Find("Planet").gameObject.SetActive(value);
            }
        }
    }

    public void Next() //Put cap so click++ does not go too far
    {
        clicks++;
        if (clicks > 18)
        {
            clicks = 18;
        }
        click();
    }


    public void Last() // Add deletion of planets at certain point
    {
        clicks--;
        if (clicks < 0) { clicks = 0; }
        if (clicks < 2) 
        { 
            GameManager.Instance.menuHide();
            showClicked = false;
        }
        if (clicks == 2) { toggleSun(false); erase(); }
        if (clicks == 3) { erase(); }
        if (clicks >12) { clicks = 3; Disable(); past = false; erase(); }
        click();
    }

    private void showPressed()
    {
        showClicked = true;
        if (clicks != 18) //THIS NEEDS TO BE CHANGED TO MAX CLICK COUNT)
        {
            clicks = 2;
        }
        click();

    }

    public void timeCompare(float f)
    {
        timeStopped = (GameManager.Instance.getTimeValue() == 0) || GameManager.Instance.autoPause.GetComponent<Toggle>().isOn;
    }

    public void cameraCheck(bool b)
    {
        cameraOn = b;
        if (clicks == 7)
        {
            cameraToggle.GetComponent<Toggle>().interactable = false;
        }
    }

    public void updateObjects()
    {
        objects.Clear();
        object[] obj = GameObject.FindSceneObjectsOfType(typeof(GameObject));
        foreach (object o in obj)
        {
            GameObject g = (GameObject)o;
            objects.Add(g);
            
        }
    }

    public void erase()
    {
        updateObjects();
        while (GameManager.Instance.physObjects.Count != 0)
        {
            GameManager.Instance.DestroyBody(GameManager.Instance.physObjects[0]);
        }
        updateObjects();
    }

    public void setClicks(int i)
    {
        clicks = i;
    }

    public void constantDisabledToggle(bool val)
    {
        constantDisable = val;
    }
}
