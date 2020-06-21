using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.ProceduralGeneration;
using System;
using Assets.Scripts.MiniMap;
using UnityEngine.AI;
using Assets.Scripts.Resources;

public class GameCreator : MonoBehaviour
{
    public static GameCreator Instance;
    public bool Persistent;

    public int SceneToLoadIndex;

    [SerializeField] private List<MapPreset> MapPresets;

    public Dropdown mapDropdown;
    public InputField seedInput;

    public GameObject playerStartCamp;

    public GameObject GoldPrefab;
    public int GoldAmount;
    public GameObject StonePrefab;
    public int StoneAmount;
    public GameObject FoodPrefab;
    public int FoodAmount;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        if (Persistent)
            DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetMapPresets();

        seedInput.characterValidation = InputField.CharacterValidation.Integer;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            CreatePlayer(FindObjectOfType<MeshGenerator>());
        }
#endif
    }

    public void SetMapPresets()
    {
        List<Dropdown.OptionData> dropdownData = new List<Dropdown.OptionData>();

        foreach (MapPreset preset in MapPresets)
        {
            dropdownData.Add(new Dropdown.OptionData(preset.name));
        }

        mapDropdown.AddOptions(dropdownData);
    }

    public void LoadGame()
    {
        Fade.Instance.AddEventOnEndFade(LoadScene);
        Fade.Instance.FadeIn();
    }

    private void LoadScene()
    {
        SceneManager.sceneLoaded += OnGameLoaded;
        SceneManager.LoadSceneAsync(SceneToLoadIndex);
    }

    private void OnGameLoaded(Scene scene, LoadSceneMode mode)
    {
		SceneManager.sceneLoaded-=OnGameLoaded;

        CreateMap();

        Fade.Instance.AddEventOnEndFade(CreateMiniMap);
        Fade.Instance.FadeOut();
    }

    private void CreateMap()
    {
        MapPreset _CurrentPreset;

        if (mapDropdown.value == 0)
            _CurrentPreset = MapPresets[UnityEngine.Random.Range(0, MapPresets.Count - 1)];
        else
            _CurrentPreset = MapPresets[mapDropdown.value - 1];

        MeshGenerator _MeshGenerator = FindObjectOfType<MeshGenerator>();

        _MeshGenerator.Preset = _CurrentPreset;
        _MeshGenerator.UsePreset = true;
        _MeshGenerator.Seed = int.Parse(seedInput.text);
        _MeshGenerator.GenerateMesh();

        CreateResources(_MeshGenerator);
        CreateMissions();
        CreatePlayer(_MeshGenerator);

        _MeshGenerator.BuildNavigationMesh();
    }

    private void CreateResources(MeshGenerator _MeshGenerator)
    {
        _MeshGenerator.GenerateTreeMap();

        CreateResource(GoldPrefab, GoldAmount, _MeshGenerator);
        CreateResource(StonePrefab, StoneAmount, _MeshGenerator);
    }

    private void CreateResource(GameObject Prefab, int amount, MeshGenerator _MeshGenerator)
    {
        Vector3 _Size = (Prefab.GetComponent<BoxCollider>().size * Prefab.transform.localScale.x).With(y: 1);
        int _SkippedInstances = 0;

        for (int i = 0; i < amount; i++)
        {
            Vector3 _Position = _MeshGenerator.GetValidMapPosition(_Size, 1000);

            if (_Position != Vector3.zero)
            {
                _MeshGenerator.FlatMapInRadius(_Position, _Size.x);

                Collider[] _Colliders = Physics.OverlapSphere(_Position, 150);
                bool _Colliding = false;


                for (int x = 0; x < _Colliders.Length; x++)
                {
                    if (_Colliders[x].GetComponent<Resource>() != null)
                    {
                        if (_Colliders[x].GetComponent<Resource>().resourceType == Prefab.GetComponent<Resource>().resourceType)
                        {
                            _Colliding = true;
                            _SkippedInstances++;
                        }
                    }
                }

                if (!_Colliding)
                    Instantiate(Prefab, _Position.With(y: 0), Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0));
            }
        }

        Debug.Log("Created " + (amount - _SkippedInstances) + " / " + amount + " of " + Prefab.GetComponent<Resource>().resourceType);
    }

    private void CreateMissions()
    {

    }

    private void CreatePlayer(MeshGenerator _MeshGenerator)
    {
        Vector3 _Position = _MeshGenerator.GetValidMapPosition(new Vector3(25, 1, 25), 10000);

        if (_Position != Vector3.zero)
            _MeshGenerator.FlatMapInRadius(_Position, 35);

        GameObject _InitialCamp = Instantiate(playerStartCamp, _Position.With(y: 0), Quaternion.identity);

        CameraControl.CenterCamera(_InitialCamp.transform.position);
    }


    private void CreateMiniMap()
    {
        MiniMap _MiniMap = FindObjectOfType<MiniMap>();

        _MiniMap.SetUp();
    }
}
