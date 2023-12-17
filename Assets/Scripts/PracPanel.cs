using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PracPanelRoutine());
    }

    private IEnumerator PracPanelRoutine()
    {
        yield return new WaitForSeconds(3f);
        _panel.SetActive(false);
    }
}
