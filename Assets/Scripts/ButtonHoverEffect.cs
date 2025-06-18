using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Material defaultMaterial;
    public Material hoverMaterial;

    private Image _image;

    void Start()
    {
        _image = GetComponent<Image>();
        if (_image != null && defaultMaterial != null)    
            _image.material = defaultMaterial;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {         
        if (_image != null && hoverMaterial != null)
        {
            _image.material = hoverMaterial;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_image != null && defaultMaterial != null)
        {
            _image.material = defaultMaterial;
        }
    }
}
