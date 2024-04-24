using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatsAndSkins : MonoBehaviour
{
    private static HatsAndSkins instance;
    public static HatsAndSkins Instance { get { return instance; } }

    [SerializeField] private List<GameObject> hats;
    public List<GameObject> Hats { get { return hats; } }

    [SerializeField] private List<Material> skins;
    public List<Material> Skins { get {  return skins; } }

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
