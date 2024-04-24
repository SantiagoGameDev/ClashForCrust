using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using System.Xml;

public class AlterRulesMenu : MonoBehaviour
{
    private static AlterRulesMenu instance;
    public static AlterRulesMenu Instance { get { return instance; } }

    public int mapPicked;
    public bool powerupsOn = true;
    public float powerupSpawnRate = 5;
    public bool fryOn = true;
    public float frySpawnRate = 5;
    public bool crustOn = true;
    public int crustValue = 1;
    public float gameTime = 120;
    public int maxHealth = 5;
    public bool awardsOn = true;
    public int numAwards = 3;
    public bool screwMode = false;

    public AudioSource audioSource;
    public AudioClip audioClip;

    //Every thing for map picking
    //gameobject with list of map info
    [SerializeField] private GameObject mapList;
    //list of map info
    private List<MapType> maps = new List<MapType>();

    //center point that the map names orbit around
    [SerializeField] private GameObject mapNameOrigin;
    //prefab for the map name object
    [SerializeField] private GameObject mapNamePrefab;
    //all the mapName objects (made from the prefab)
    private List<GameObject> mapNames = new List<GameObject>();
    //the position angles that all of the map names go to (varies due to varying number of maps)
    private List<float> mapNameAngles = new List<float>();

    //variables for actually choosing a map
    [SerializeField] private Slider MapPickSlider;
    private float oldMapChoice;
    private bool canUpdateSlider = false;
    public bool sliderSelected;

    [SerializeField] private Button rulesButton;
    [SerializeField] private GameObject alterRulesMenu;
    public bool onRulesMenu = false;

    public delegate void CheckForToggle();
    public event CheckForToggle onChange;

    public int selectedSlider;
    public GameObject allSliders;
    private Vector3 baseSlidersPos;

    public TMP_Text rulesTitle, rulesSubtitle, rulesCC1, rulesCC2;

    public GameObject AButton, YButton;

    public bool screwModeUnlocked = false;

    [SerializeField] GameObject screwModeSlider;
    [SerializeField] AudioSource explosion;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MapType[] mapsa = mapList.GetComponentsInChildren<MapType>();

        foreach (MapType map in mapsa)
        {
            maps.Add(map);

            InitializeMapNames(map);
        }

        PlaceMapNames();

        alterRulesMenu.SetActive(false);

        oldMapChoice = 1;
        MapPickSlider.maxValue = (float)maps.Count + 1;
        MapPickSlider.value = 1;
        canUpdateSlider = true;
        MapPickSlider.interactable = true;
        MapPickSlider.Select();

        baseSlidersPos = allSliders.transform.localPosition;

        AudioManager.Instance.PlayAudio(AudioManager.AudioType.CarnivalSelect, true);
    }

    private void InitializeMapNames(MapType map)
    {
        GameObject mapName = mapNamePrefab;

        TMP_Text text = mapName.GetComponentInChildren<TMP_Text>();

        text.text = map.visibleName;

        GameObject nmp = Instantiate(mapName, mapNameOrigin.transform);

        if (map.model)
        {
            GameObject model = Instantiate(map.model, nmp.transform);

            model.transform.position = nmp.GetComponent<MapMenuOption>().model.transform.position;
            nmp.GetComponent<MapMenuOption>().model.GetComponent<MeshRenderer>().enabled = false;

            nmp.GetComponent<MapMenuOption>().model = model;
        }

        mapNames.Add(nmp);
    }

    private void PlaceMapNames()
    {
        for (int i = 0; i < mapNames.Count; i++)
        {
            float angle = 270f;

            angle += i * (360f / mapNames.Count);
            mapNameAngles.Add(angle);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * 3.5f;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * 3.5f;

            mapNames[i].transform.position = AngleToPosition(angle);
        }

        StartCoroutine(AnimatdPickedMapName());
    }

    private Vector3 AngleToPosition(float angle)
    {
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * 4.5f;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * 4.5f;

        return mapNameOrigin.transform.position + new Vector3(x, 0f, z);
    }

    //called when submit is pressed while on map selection
    public void PickedMap()
    {
        RuleChoiceInfo.Instance.gameTime = gameTime;
        RuleChoiceInfo.Instance.maxHealth = maxHealth;
        RuleChoiceInfo.Instance.crustOn = crustOn;
        RuleChoiceInfo.Instance.crustValue = crustValue;
        RuleChoiceInfo.Instance.powerupsOn = powerupsOn;
        RuleChoiceInfo.Instance.powerupSpawnRate = powerupSpawnRate;
        RuleChoiceInfo.Instance.fryOn = fryOn;
        RuleChoiceInfo.Instance.frySpawnRate = frySpawnRate;
        RuleChoiceInfo.Instance.awardsOn = awardsOn;
        RuleChoiceInfo.Instance.numAwards = numAwards;
        RuleChoiceInfo.Instance.screwMode = screwMode;

        string chosenMap = maps[mapPicked].unityName;

        SceneControl.Instance.LoadSceneFromName(chosenMap);
    }

    public void UpdateMapNameRotation(float sliderValue)
    {
        if (canUpdateSlider)
        {
            audioSource.Play();
            canUpdateSlider = false;
            bool isRight = false;

            if (oldMapChoice > sliderValue)
            {
                isRight = false;

                if (sliderValue == 2 && oldMapChoice == maps.Count + 1)
                {
                    isRight = true;
                }
            }
            if (oldMapChoice < sliderValue)
            {
                isRight = true;

                if (oldMapChoice == 0 && sliderValue != MapPickSlider.maxValue)
                {
                    isRight = false;
                }
            }

            oldMapChoice = sliderValue;

            if (sliderValue > maps.Count)
            {
                sliderValue = 1f;
                MapPickSlider.value = sliderValue;
            }
            else if (sliderValue < 1)
            {
                sliderValue = maps.Count;
                MapPickSlider.value = sliderValue;
            }

            mapPicked = (int)sliderValue - 1;

            //Debug.Log("Map Picked" + mapPicked);
            StartCoroutine(RotateNames(isRight));
        }
    }

    IEnumerator RotateNames(bool isRight)
    {
        StartCoroutine(AnimatdPickedMapName());

        MapPickSlider.interactable = false;

        float amountToRotate = 360f / mapNames.Count;

        float rotation = 0f;

        while (rotation < amountToRotate)
        {
            rotation += amountToRotate * Time.deltaTime * 0.85f;

            if (rotation > amountToRotate)
            {
                rotation = amountToRotate;
            }

            for (int i = 0; i < mapNames.Count; i++)
            {
                int pastIndex = 0;

                if (mapNames.Count > 0)
                {
                    int posDifference;

                    if (isRight)
                    {
                        posDifference = -mapPicked + 1;
                    }
                    else
                    {
                        posDifference = -mapPicked - 1;
                    }

                    pastIndex = i + posDifference;
                    if (pastIndex >= mapNames.Count)
                    {
                        pastIndex -= mapNames.Count;
                    }
                    else if (pastIndex < 0)
                    {
                        pastIndex += mapNames.Count;
                    }

                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            pastIndex = 1;
                            break;
                        case 1:
                            pastIndex = 0;
                            break;
                    }
                }
                float startAngle = mapNameAngles[pastIndex];

                float percentDone = rotation / amountToRotate;

                float rotToUse = amountToRotate * EaseOutBack(percentDone);

                float angleToUse = 0;
                if (isRight)
                {
                    angleToUse = startAngle - rotToUse;
                }
                else
                {
                    angleToUse = startAngle + rotToUse;
                }
                mapNames[i].transform.position = AngleToPosition(angleToUse);
            }

            yield return null;
        }

        MapPickSlider.interactable = true;
        MapPickSlider.Select();
        canUpdateSlider = true;
    }

    IEnumerator AnimatdPickedMapName()
    {
        float rad = 0f;

        GameObject mapObject = mapNames[mapPicked];

        if (maps[mapPicked].visibleName == "Crusty Carnival")
        {
            AudioManager.Instance.PlayAudio(AudioManager.AudioType.CarnivalSelect, true);
        }
        if (maps[mapPicked].visibleName == "Beachside Boardwalk")
        {
            AudioManager.Instance.PlayAudio(AudioManager.AudioType.BoardwalkSelect, true);
        }
        if (maps[mapPicked].visibleName == "Pirate Peak")
        {
            AudioManager.Instance.PlayAudio(AudioManager.AudioType.PirateShipSelect, true);
        }

        GameObject map = mapObject.GetComponentInChildren<Canvas>().gameObject;

        float baseScale = map.transform.localScale.x;
        foreach (GameObject mapIn in mapNames)
        {
            float c = 0;
            TMP_Text mapText = mapIn.GetComponentInChildren<TMP_Text>();
            if (mapIn == mapObject)
            {
                c = 1;
            }
            else
            {
                c = 0.6f;
            }

            StartCoroutine(BrightenMap(mapText, c));
        }

        float thisMapWasPicked = mapPicked;

        while (thisMapWasPicked == mapPicked)
        {
            rad += Time.deltaTime * 3f;

            if (rad > (2 * Mathf.PI))
            {
                rad -= (2 * Mathf.PI);
            }

            float addScale = Mathf.Sin(rad) * 0.025f + 1.25f;

            map.transform.localScale = new Vector3(baseScale * addScale, baseScale * addScale, baseScale * addScale);
            yield return null;
        }

        float scale = map.transform.localScale.x;

        while (scale > baseScale)
        {
            scale -= Time.deltaTime;

            if (scale < baseScale)
            {
                scale = baseScale;
            }

            map.transform.localScale = map.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        if (scale < baseScale)
        {
            scale = baseScale;
        }

        map.transform.localScale = map.transform.localScale = new Vector3(scale, scale, scale);
    }

    IEnumerator BrightenMap(TMP_Text text, float newC)
    {
        float baseC = text.color.r;

        while (baseC != newC)
        {
            //works for both increasing and decreasing the value
            if (baseC < newC)
            {
                baseC += Time.deltaTime * 0.85f;

                if (baseC > newC)
                {
                    baseC = newC;
                }
            }
            else if (baseC > newC)
            {
                baseC -= Time.deltaTime;

                if (baseC < newC)
                {
                    baseC = newC;
                }
            }

            text.color = new Color(baseC, baseC, baseC);
            yield return null;
        }
    }

    private float EaseOutBack(float x)
    {
        float c1 = 1;
        float c3 = c1 + 1f;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }

    public void OpenRules()
    {
        YButton.SetActive(false);
        AButton.GetComponentInChildren<TMP_Text>().text = "Toggle On / Off";
        MapPickSlider.interactable = false;
        rulesButton.interactable = false;
        alterRulesMenu.SetActive(true);
        allSliders.transform.localPosition = new Vector3(baseSlidersPos.x, baseSlidersPos.y + 160f * selectedSlider * allSliders.transform.localScale.x, baseSlidersPos.y);
        onRulesMenu = true;

        AudioManager.Instance.PlayAudio(AudioManager.AudioType.RulesMenu, true);
    }

    public void CloseRules()
    {
        YButton.SetActive(true);
        AButton.SetActive(true);
        AButton.GetComponentInChildren<TMP_Text>().text = "Select Map";
        //allSliders.transform.localPosition = new Vector3(baseSlidersPos.x, baseSlidersPos.y + 160f * selectedSlider * allSliders.transform.localScale.x, baseSlidersPos.y);
        MapPickSlider.interactable = true;
        rulesButton.interactable = true;
        alterRulesMenu.SetActive(false);
        onRulesMenu = false;

        if (maps[mapPicked].visibleName == "Crusty Carnival")
        {
            AudioManager.Instance.PlayAudio(AudioManager.AudioType.CarnivalSelect, true);
        }
        if (maps[mapPicked].visibleName == "Beachside Boardwalk")
        {
            AudioManager.Instance.PlayAudio(AudioManager.AudioType.BoardwalkSelect, true);
        }
    }

    public void GoBack()
    {
        SceneControl.Instance.LoadSceneFromName("Customization");
    }

    public void UpdateToggles()
    {
        onChange();
    }

    public void UpdateSliderPositions()
    {
        EventSystem.current.sendNavigationEvents = false;
        StartCoroutine(MoveAllSliders());
    }

    IEnumerator MoveAllSliders()
    {
        float posToGo = baseSlidersPos.y + 160f * selectedSlider * allSliders.transform.localScale.x;

        float startPos = allSliders.transform.localPosition.y;

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * 3f;

            if (time > 1f)
            {
                time = 1f;
                EventSystem.current.sendNavigationEvents = true;
            }

            float yDif = 0f;
            if (startPos < posToGo)
            {
                yDif = startPos += (posToGo - startPos) * time;
            }
            else if (startPos > posToGo)
            {
                yDif = startPos -= (startPos - posToGo) * time;
            }
            else if (startPos == posToGo)
            {
                yDif = posToGo;
            }

            allSliders.transform.localPosition = new Vector3(baseSlidersPos.x, yDif, baseSlidersPos.z);

            yield return null;
        }
    }

    public void UnlockScrewMode()
    {
        if (!screwModeUnlocked)
        {
            screwModeUnlocked = true;

            screwModeSlider.SetActive(true);

            explosion.Play();
        }
    }
}
