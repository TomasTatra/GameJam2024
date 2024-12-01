using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialMenu : MonoBehaviour
{
    [SerializeField]
    private string _text;

    [SerializeField]
    private TextMeshProUGUI _textField;

    private Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
       _textField.text = _text;
        originalPosition = _textField.rectTransform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _textField.DOFade(1, 0.5f);
        Vector3 temp = originalPosition;
        temp.y -= 20;
        //_textField.rectTransform.DOMove(temp, 1);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //_textField.rectTransform.DOMove(originalPosition, 1);
        _textField.DOFade(0, 0.5f);
    }
}
