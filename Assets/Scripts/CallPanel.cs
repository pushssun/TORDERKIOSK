using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallPanel : MonoBehaviour
{
    [SerializeField] private Button _allClearButton;
    [SerializeField] private Button _callButton;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _popup;

    // Start is called before the first frame update
    void Start()
    {
        _allClearButton.onClick.AddListener(OnClickAllClearButton);
        _callButton.onClick.AddListener(OnClickCallButton);
    }

    private void OnClickCallButton()
    {
        if(_spawnPoint.childCount > 0)
        {
            GameManager.Instance.Step3Success = true;
        }
        OnClickAllClearButton();
        _popup.SetActive(true);
        StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        yield return new WaitForSeconds(1f);
        _popup.SetActive(false);
        gameObject.SetActive(false);
    }
    private void OnClickAllClearButton()
    {
        foreach (Transform child in _spawnPoint)
        {
            child.GetComponent<ClickCallAddItem>().OnClickDeleteButton();
            Destroy(child.gameObject);
        }
    }
}
