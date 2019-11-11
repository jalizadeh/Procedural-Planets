using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    Planet planet;

    public Slider resolutionSlider;
    public Text resolutionCount;

    // Start is called before the first frame update
    void Start()
    {
        planet = FindObjectOfType<Planet>();
        resolutionSlider.value = planet.resolution;
    }

    
    public void SetResolution(float res)
    {
        planet.resolution = Mathf.RoundToInt(res);
        planet.GeneratePlanet();
        resolutionCount.text = "Resolution: " + Mathf.RoundToInt(res);
    }
}
