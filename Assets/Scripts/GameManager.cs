 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{

    // Camera Controls
    float cameraShiftTolerance = .09f;
    float zoomInTolerance = .45f;
    float zoomOutTolerance = .1f;
    int zoomTicks = 1;
    const int zoomDelay = 0;
    public Camera camera;

    // Changes the time steps of the simulation? Can't tell
    const float SIM_TIME_STEP = 1 / 100;

    public static GameManager Instance { get; private set; }

    // Gravity
    static float G = 0.667408f;
    public List<Gravity> physObjects;
    public Dictionary<Gravity, GameObject> planetControllers = new Dictionary<Gravity, GameObject>();
    

    public GameObject startButton, creditsButton, howToButton, volumeButton, volumeButtontemp, backButton, timeSlider, menuButton, showButton, hideButton, pauseMenu, autoCameraToggle, autoPause;
    public GameObject titleText, creditsText, howToText, distanceText;
    public GameObject volumeSlider;
    public GameObject canvas;
    public GameObject events;
    EventSystem eventSystem;
    public GameObject background, howToBackground, creditsBackground;
    public GameObject cellContainer;
    public GameObject dropdown;
    public List<string> presetLevels;
    private int selectedLevel;
    public AudioMixer mixer;
    public GameObject backgroundAudio;
    public float distanceModifier;

    public GameObject CenterOfMassPrefab;
    GameObject centerOfMassIndicator;

    public GameObject planetPrefab;
    public GameObject sunPrefab;

    public GameObject scrollView;
    public GameObject Content;
    public GameObject PlanetControllerPrefab;

    public Color colortemp;
    public string planettemp;

    Vector2 mouseDragPos;

    private int ObjectCounter = 0;
    private int MAXOBJECTCOUNT = 12;
    private bool AutoCamera = true;
    private Vector3 startCameraPos;
    private bool draggingNothing;
    private float savedTimeValue = 1;

    public bool select1 = false;
    public bool select2 = false;

    public Gravity s1;
    public Gravity s2;
    public Material notSelect;
    public Material select;

    public GameObject tutorialInfo;

    private bool setInputFields = false;
    private int counter = 0;

    public Vector2 centerOfSystem;

    private System.DateTime updateTime = System.DateTime.Now.AddSeconds(.02);

    private void Awake()
    {
        // Frame Rate Limiter (30 fps)
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 30;
        AutoCamera = false;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(canvas);
            DontDestroyOnLoad(events);
            DontDestroyOnLoad(backgroundAudio);
            dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener((i) => changeSelectedLevel(i));
            timeSlider.GetComponent<Slider>().onValueChanged.AddListener((i) => setTimeScale(i));
            autoCameraToggle.GetComponent<Toggle>().onValueChanged.AddListener(b => setAutoCamera(b));
            Time.fixedDeltaTime = SIM_TIME_STEP;
            eventSystem = events.GetComponent<EventSystem>();
        }
        else
        {
            Destroy(gameObject);
            Destroy(canvas);
            Destroy(events);
            Destroy(backgroundAudio);
        }
    }

    public void Update()
    {
        // ui updates
        if (updateTime < System.DateTime.Now)
        {
            foreach (KeyValuePair<Gravity, GameObject> keyValue in planetControllers)
            {
                Gravity g = keyValue.Key;
                GameObject pc = keyValue.Value;

                UIControllerSetup ui = pc.GetComponent<UIControllerSetup>();

                if (Time.timeScale != 0)
                {
                    setInputFields = false;
                    counter = 0;
                    ui.massTextField.SetActive(true);
                    ui.posXTextField.SetActive(true);
                    ui.posYTextField.SetActive(true);
                    ui.velocityXTextField.SetActive(true);
                    ui.velocityYTextField.SetActive(true);
                    ui.massInputField.SetActive(false);
                    ui.posXInputField.SetActive(false);
                    ui.posYInputField.SetActive(false);
                    ui.velocityXInputField.SetActive(false);
                    ui.velocityYInputField.SetActive(false);
                    ui.massTextField.GetComponent<Text>().text = "" + g.GetComponent<Rigidbody2D>().mass;
                    ui.posXTextField.GetComponent<Text>().text = "" + Mathf.Round(g.GetComponent<Rigidbody2D>().position.x * 10000) / 100;
                    ui.posYTextField.GetComponent<Text>().text = "" + Mathf.Round(g.GetComponent<Rigidbody2D>().position.y * 10000) / 100;
                    ui.velocityXTextField.GetComponent<Text>().text = "" + Mathf.Round(g.GetComponent<Rigidbody2D>().velocity.x * 10000) / 100;
                    ui.velocityYTextField.GetComponent<Text>().text = "" + Mathf.Round(g.GetComponent<Rigidbody2D>().velocity.y * 10000) / 100;
                    distanceText.GetComponent<TextMeshProUGUI>().text = "Distance: " + distance();
                }
                else
                {

                    ui.massInputField.SetActive(true);
                    ui.posXInputField.SetActive(true);
                    ui.posYInputField.SetActive(true);
                    ui.velocityXInputField.SetActive(true);
                    ui.velocityYInputField.SetActive(true);
                    if (!setInputFields)
                    {
                        ui.massInputField.GetComponent<InputField>().text = "" + ui.massTextField.GetComponent<Text>().text;
                        ui.posXInputField.GetComponent<InputField>().text = "" + ui.posXTextField.GetComponent<Text>().text;
                        ui.posYInputField.GetComponent<InputField>().text = "" + ui.posYTextField.GetComponent<Text>().text;
                        ui.velocityXInputField.GetComponent<InputField>().text = "" + ui.velocityXTextField.GetComponent<Text>().text;
                        ui.velocityYInputField.GetComponent<InputField>().text = "" + ui.velocityYTextField.GetComponent<Text>().text;
                        counter +=1;
                        if (counter.Equals(physObjects.Count))
                        {
                            setInputFields = true; 
                        }
                    }
                    ui.massTextField.SetActive(false);
                    ui.posXTextField.SetActive(false);
                    ui.posYTextField.SetActive(false);
                    ui.velocityXTextField.SetActive(false);
                    ui.velocityYTextField.SetActive(false);
                }
            }
            updateTime = System.DateTime.Now.AddSeconds(.1);
        }
        // Camera Repositioning
        if (physObjects.Count > 0)
        {
            CameraReposition();
        }
    }
    void FixedUpdate()
    {
            if (!tutorialInfo.activeSelf)
            {
                Enable();
            }
        
        
        if (!camera)
        {
            LoadScene();
        }
        // Increase frames by one; every 30 frames update these functions and set frames to 0
        if (updateTime < System.DateTime.Now)
        {
            CenterOfSystem();
            CullFaroffObjects();
            // centerOfSystem = CenterOfSystem();
            // centerOfMassIndicator.transform.position = centerOfSystem;
            if (centerOfSystem.Equals(Vector2.zero)) { centerOfMassIndicator.transform.position = camera.transform.position;}
        }
    }
    void LoadScene()
    {
        physObjects = new List<Gravity>();
        camera = FindObjectOfType<Camera>();
        foreach (Gravity g in FindObjectsOfType<Gravity>())
        {
            physObjects.Add(g);
            NewPlanetController(g, g.getTag());
            ++ObjectCounter;
            //Debug.Log("Objects: " + ObjectCounter);
        }

        SetUpCoM();
        

    }

    void SetUpCoM() {
        centerOfMassIndicator = Instantiate(CenterOfMassPrefab, centerOfSystem, Quaternion.identity);
        SpriteRenderer sr = centerOfMassIndicator.gameObject.GetComponent<SpriteRenderer>();
        //CoM is slightly translucent
        sr.color = new Color(1, 1, 1, .5f);
        //and it is drawn in front of other objects
        sr.sortingOrder = 5;        
    }

    //Get rid of anything too far from the center of the system
    // Called once every 30 frames (frame cap is at 30 fps)
    void CullFaroffObjects()
    {
        //Vector2 c = CenterOfSystem();
        HashSet<Gravity> toRemove = new HashSet<Gravity>();
        foreach (Gravity go in physObjects) {
            if ((go.GetComponent<Rigidbody2D>().position - centerOfSystem).magnitude > 100) 
            {
                DestroyBody(go);
                break;
                //toRemove.Add(go);
            }
        }

        //foreach (Gravity go in toRemove) {
        //    DestroyBody(go);
        //}
    }

    public void NewPlanetController(Gravity g, string name) {
        GameObject pc = Instantiate(PlanetControllerPrefab);
        pc.GetComponent<UIControllerSetup>().setup(g,name);
        pc.transform.SetParent(Content.gameObject.transform, false);
        planetControllers.Add(g, pc);
    }

    public void CreateBody(Vector2 pos, GameObject planet)
    {
        //if(ObjectCounter < MAXOBJECTCOUNT)
        Gravity g = SpawnScript.SpawnNewPlanet(pos, planet);
        colortemp = SpawnScript.getColor();
        planettemp = SpawnScript.getPrefab();
        physObjects.Add(g);
        ++ObjectCounter;
        //Debug.Log("Objects: " + ObjectCounter);
        NewPlanetController(g, planettemp);
    }

    public void DestroyBody(Gravity g) {
        if(g.Equals(s1))
		{
            
            s1 = null;
            select1 = false;
		}
        if (g.Equals(s2))
        {
            s2 = null;
            select2 = false;
        }
        physObjects.Remove(g);
        Destroy(g.gameObject.transform.parent.gameObject);
        Destroy(planetControllers[g]);
        planetControllers.Remove(g);
        --ObjectCounter;
        //Debug.Log("Objects: " + ObjectCounter);
    }

    public void CenterOfSystem()
    {
        Vector2 v = Vector2.zero;
        float totalMass = 0.0f;
        foreach (Gravity go in physObjects)
        {
            Vector3 p = go.gameObject.transform.position;
            v += go.GetComponent<Rigidbody2D>().mass * new Vector2(p.x, p.y);
            totalMass += go.GetComponent<Rigidbody2D>().mass;
            //Debug.Log("CenterOfSystem");
        }
        if (physObjects.Count > 0) v /= totalMass;

        centerOfMassIndicator.transform.position = v;
    }

    public void CameraReposition()
    {
        if (AutoCamera)
        {

            Vector3 center = centerOfSystem;
            center.z = camera.transform.position.z;
            if ((camera.transform.position - center).magnitude > cameraShiftTolerance)
                camera.transform.position = Vector3.Lerp(camera.transform.position, center, .005f);

            bool needToZoomOut = false;
            bool needToZoomIn = true;

            foreach (Gravity g in physObjects)
            {
                Vector3 v = camera.WorldToViewportPoint(g.transform.position);
                needToZoomOut |= v.x < zoomOutTolerance
                              || v.x > (1 - zoomOutTolerance)
                              || v.y < zoomOutTolerance
                              || v.y > (1 - zoomOutTolerance);

                needToZoomIn &= v.x > zoomInTolerance
                             && v.x < (1 - zoomInTolerance)
                             && v.y > zoomInTolerance
                             && v.y < (1 - zoomInTolerance);
            }

            if (needToZoomOut)
            {
                zoomTicks++;
            }
            else if (needToZoomIn)
            {
                zoomTicks--;
            }
            else { zoomTicks = 0; }

            if (zoomTicks > zoomDelay || zoomTicks < -zoomDelay)
            {
                camera.orthographicSize += .025f * Mathf.Sign(zoomTicks);
            }
        }
        else {

            if (camera.orthographicSize > .01) 
                camera.orthographicSize -= .2f * Input.mouseScrollDelta.y;

            Vector3 dragLoc = camera.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                mouseDragPos = camera.ScreenToWorldPoint(Input.mousePosition);
                startCameraPos = camera.transform.position;
                draggingNothing = !eventSystem.IsPointerOverGameObject() && !Physics2D.OverlapPoint(dragLoc);
            }

            if (Input.GetMouseButton(0) && draggingNothing)
            {
                Vector2 move = new Vector2(dragLoc.x, dragLoc.y) - mouseDragPos;
                camera.transform.position = startCameraPos - .85f * (Vector3) move;
            }
        }
    }

    // Change in velocity
    //Seems to be focus on gravity
    public Vector2 deltaV(Gravity obj)
    {
        Vector2 c = Vector2.zero;
        // Definitly uses a lot of RAM, is there a way to only call the data once? 
        // Collect each position once and then compare?
        foreach (Gravity go in physObjects)
        {
            if (!go.Equals(obj))
            {
                Rigidbody2D GO_RigidBody = go.gameObject.GetComponent<Rigidbody2D>();
                Rigidbody2D objToAttract = obj.gameObject.GetComponent<Rigidbody2D>();
                Vector2 direction = GO_RigidBody.position - objToAttract.position;
                float distance = direction.magnitude * distanceModifier;

                // This is the actual equation for gravitational interaction of two celestial bodies
                // Fn = G * ((m1*m2)/distance^2)
                Vector2 force = direction.normalized * ((GO_RigidBody.mass * objToAttract.mass) / (distance * distance)) * G;
                c += force;
            }
        }
        return c;
    }

    public int distance()
	{
        if(!(s1 is null) && !(s2 is null))
		{            
            // Distance is (x2 − x1)^2 + (y2 − y1)^2
            float xDiff = s2.rbody.position.x - s1.rbody.position.x;
            float yDiff = s2.rbody.position.y - s1.rbody.position.y;
            return (int)(Mathf.Pow(xDiff, 2) + Mathf.Pow(yDiff, 2));
        }
        return 0;
	}

    public void changeSelectedLevel(int level) 
    {
        this.selectedLevel = level;
    }

    public void setAutoCamera(bool c)
    {
        AutoCamera = c;
    }

    public void startOnClick()
    {

        resetValues();
        volumeSlider.SetActive(false);
        titleText.SetActive(false);
        startButton.SetActive(false);
        howToButton.SetActive(false);
        creditsButton.SetActive(false);
        background.SetActive(false);
        dropdown.SetActive(false);
        showButton.SetActive(true);
        scrollView.SetActive(true);
        cellContainer.SetActive(true);
        tutorialInfo.SetActive(false);
        volumeButtontemp.SetActive(false);

        StartCoroutine(LoadYourAsyncScene(presetLevels[selectedLevel]));
    }

    public void menuOnClick()
    {
        volumeSlider.SetActive(false);
        volumeButtontemp.SetActive(true);
        titleText.SetActive(true);
        startButton.SetActive(true);
        howToButton.SetActive(true);
        creditsButton.SetActive(true);
        background.SetActive(true);
        dropdown.SetActive(true);
        pauseMenu.SetActive(false);
        showButton.SetActive(false);
        scrollView.SetActive(false);
        cellContainer.SetActive(false);
        tutorialInfo.SetActive(false);
        while (physObjects.Count != 0)
        {
            DestroyBody(physObjects[0]);
        }
        StartCoroutine(LoadYourAsyncScene("Main Menu"));
    }

    public void creditsOnClick()
    {
        volumeSlider.SetActive(false);
        volumeButtontemp.SetActive(false);
        titleText.SetActive(false);
        startButton.SetActive(false);
        howToButton.SetActive(false);
        creditsButton.SetActive(false);
        dropdown.SetActive(false);
        backButton.SetActive(true);
        creditsText.SetActive(true);
        creditsBackground.SetActive(true);
    }

    public void tutorial()
    {
        volumeSlider.SetActive(false);
        titleText.SetActive(false);
        startButton.SetActive(false);
        howToButton.SetActive(false);
        creditsButton.SetActive(false);
        background.SetActive(false);
        dropdown.SetActive(false);
        showButton.SetActive(true);
        scrollView.SetActive(true);
        cellContainer.SetActive(true);
        volumeButtontemp.SetActive(false);
        tutorialInfo.SetActive(true);
        StartCoroutine(LoadYourAsyncScene("Tutorial"));
    }

    public void backOnClick()
    {
        volumeSlider.SetActive(false);
        volumeButtontemp.SetActive(true);
        titleText.SetActive(true);
        startButton.SetActive(true);
        howToButton.SetActive(true);
        creditsButton.SetActive(true);
        pauseMenu.SetActive(false);
        dropdown.SetActive(true);
        backButton.SetActive(false);
        creditsText.SetActive(false);
        howToText.SetActive(false);
        howToBackground.SetActive(false);
        creditsBackground.SetActive(false);
        
    }

    public void pauseOnClick()
    {
        setTimeScale(1 - Time.timeScale);
        
    }

    public void deleteOnClick(GameObject container)
    {
        int check1 = planetControllers.Count;
        //Debug.Log("planetControllers.Count(Before):" + check1);
        //Debug.Log(container.ToString());
        foreach (KeyValuePair<Gravity, GameObject> item in planetControllers)
        {
            //Debug.Log(item.Value.ToString());
            if (container == item.Value)
            {
                DestroyBody(item.Key);
                break;
                //planetControllers.Remove(item.Key);
                //physObjects.Remove(item.Key);
                //item.Key.delete();
                //Destroy(item.Key);
                //Destroy(item.Value);
                //--ObjectCounter;
                //Debug.Log("Objects: " + ObjectCounter);
                //break;
            }
        }
        int check2 = planetControllers.Count;
        //Debug.Log("planetControllers.Count(After):" + check2);
    }

    // TIME
    public void setTimeScale(float t)
    {
        if (t != 0)
        {
            autoPause.GetComponent<Toggle>().isOn = false;
        }
        Time.timeScale = t;
        Time.fixedDeltaTime = SIM_TIME_STEP * Time.timeScale;
    }

    public void pauseTime()
    {
        setTimeScale(0);
    }

    public void startTime()
    {
        setTimeScale(1);
    }

    public bool isPaused()
    {
        if (Time.timeScale == 0)
        {
            return true;
        }
        return false;
    }

    public void menuHide()
    {
            pauseMenu.SetActive(false);
            distanceText.SetActive(false);
            showButton.SetActive(true);
        
    }

    public void menuShow()
    {
        tutorialInfo.GetComponent<TutorialText>().constantDisabledToggle(false);
        pauseMenu.SetActive(true);
        distanceText.SetActive(true);
        showButton.SetActive(false);

    }


    IEnumerator LoadYourAsyncScene(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    // Volume Controls
    // Activates the volume slider by clicking the icon
    public void volumeOnClick()
    {
        if (volumeSlider.activeSelf == true)
        {
            volumeSlider.SetActive(false);
        }
        else
        {
            volumeSlider.SetActive(true);
        }
    }
    // Sets the volume using the slider
    public void setVolume(float sliderValue)
    {
        mixer.SetFloat("backgroundVolume", (Mathf.Log10(sliderValue) * 20));
    }

    // Returns number of objects in space
    public int getObjectCount()
    {
        return ObjectCounter;
    }

    public float getTimeValue()
    {
        return timeSlider.GetComponent<Slider>().value;

    }



    public void resetValues()
    {
        ObjectCounter = 0;
        timeSlider.GetComponent<Slider>().value = 1;
        autoCameraToggle.GetComponent<Toggle>().isOn = false;
        autoPause.GetComponent<Toggle>().isOn = false;
        Enable();
        //Debug.Log("J");
    }


    public void Enable()
    {
        autoPause.GetComponent<Toggle>().interactable = true;
        autoCameraToggle.GetComponent<Toggle>().interactable = true;
        timeSlider.GetComponent<Slider>().interactable = true;
        hideButton.GetComponent<Button>().interactable = true;
        foreach (Transform child in cellContainer.transform)
            {
                if (child.gameObject.GetComponent<DragAndDropCell>().getInteractable() != true)
                {
                    child.gameObject.GetComponent<DragAndDropCell>().setInteractable(true);
                    child.Find("Planet").gameObject.SetActive(true);
                }
            }
    }

    public bool getSOneSTwo()
    {
        return (select2 && select1);
    }

    public void highlight(bool yellow, bool one, bool two)
    {
        if (yellow)
        {
            if (one)
            {
                planetControllers[s1].transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.yellow;
            }
            if (two)
            {
                planetControllers[s2].transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.yellow;
            }
        }
        else
        {
            if (one)
            {
                Color c = planetControllers[s1].transform.GetChild(0).gameObject.GetComponent<Image>().color;
                c = Color.white;
                c.a = 0.3921f;
                planetControllers[s1].transform.GetChild(0).gameObject.GetComponent<Image>().color = c;
            }
            if (two)
            {
                Color c = planetControllers[s2].transform.GetChild(0).gameObject.GetComponent<Image>().color;
                c = Color.white;
                c.a = 0.3921f;
                planetControllers[s2].transform.GetChild(0).gameObject.GetComponent<Image>().color = c;
            }
        }
    }


    public void timeToggle()
    {
        if (autoPause.GetComponent<Toggle>().isOn)
        {
            savedTimeValue = getTimeValue();
            setTimeScale(0);
        }
        else 
        { 
            setTimeScale(savedTimeValue);
        }
    }
}