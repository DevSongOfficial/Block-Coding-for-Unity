using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPositionTest : MonoBehaviour
{
    Canvas canvas;

    [ContextMenu("Test")]
    public void Test()
    {
        var rect = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();

        Debug.Log("transform.position: " + transform.position);
        Debug.Log("transform.localPosition: " + transform.localPosition);
        Debug.Log("rectTrasnform.position: " + rect.position);
        Debug.Log("rectTrasnform.anchoredPosition: " + rect.anchoredPosition);
        Debug.Log("rectTrasnform.rect.position: " + rect.rect.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>()
                                                            , transform.position
                                                            , Camera.main
                                                            , out Vector2 localPos);
        Debug.Log(Camera.main.WorldToScreenPoint(transform.position));
        Debug.Log(localPos);
    }

    Vector3 vector = new Vector3(421, 620, 0);
    [ContextMenu("ÁÂÇ¥º¯È¯1")]
    public void TestSet1()
    {
        transform.position = vector;
    }

    [ContextMenu("ÁÂÇ¥º¯È¯2")]
    public void TestSet2()
    {
        var rect = GetComponent<RectTransform>();
        rect.position = vector;
    }

    [ContextMenu("ÁÂÇ¥º¯È¯3")]
    public void TestSet3()
    {
        var rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector3(-218.42f, 210.59f);
    }

    [ContextMenu("ÁÂÇ¥º¯È¯4")]
    public void TestSet4()
    {
        var rect1 = GetComponent<Rect>();
        rect1.position = new Vector3(0, 0);
    }
}
