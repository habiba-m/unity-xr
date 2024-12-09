using UnityEngine;
using UnityEngine.UI;

public class AlphaController : MonoBehaviour
{
    public Material targetMaterial; 
    public Slider alphaSlider;      

    void Start()
    {
        if (targetMaterial != null && alphaSlider != null)
        {

            alphaSlider.value = targetMaterial.GetFloat("_Alpha");
            alphaSlider.onValueChanged.AddListener(SetAlpha);
        }
    }

    public void SetAlpha(float value)
    {
        if (targetMaterial != null)
        {
            targetMaterial.SetFloat("_Alpha", value);
        }
    }
}
