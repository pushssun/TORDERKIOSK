using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollButton : MonoBehaviour
{
    private ScrollRect _scrollRect;

    [SerializeField] private Toggle[] _buttons;
    [SerializeField] private RectTransform[] _transforms;
    [SerializeField] private TextMeshProUGUI _typeText;
    
    // Start is called before the first frame update
    void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
        for(int i=0;i<_buttons.Length;i++)
        {
            int index = i;
            _buttons[i].onValueChanged.AddListener((isOn)=>OnClickTypeButton(index));
        }    
    }

    private void OnClickTypeButton(int index)
    {
        _typeText.text = _transforms[index].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;  
        Canvas.ForceUpdateCanvases();
        _scrollRect.content.anchoredPosition =
            (Vector2)_scrollRect.transform.InverseTransformPoint(_scrollRect.content.position)
            - (Vector2)_scrollRect.transform.InverseTransformPoint(_transforms[index].position);

    }
}
