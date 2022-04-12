using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    float value;
    [Range(0.5f,2.0f)]
    public float fadeOutTime = 1.0f;
    public Image myImage;
    Color imgColor;

    //public IEnumerator Fadein;
    public IEnumerator Fadeout;

    void Start()
    {
        imgColor = myImage.color;
        value = myImage.color.a;
        SetCoroutine();
    }

    public void SetCoroutine()
    {
        //Fadein = FadeIn();
        Fadeout = FadeOut();
    }
      
    //public void FadeOut()
    //{
    //    value = 0;
    //    myImage.raycastTarget = false; imgColor.a = value;
    //    myImage.color = imgColor;
    //}

    public bool IsRunFadeOut;
    IEnumerator FadeOut()
    {
        IsRunFadeOut = true;
        while (value > 0f)
        {
            value -= Time.deltaTime / fadeOutTime;

            if (value <= 0f)
            {
                value = 0f;
                myImage.raycastTarget = false;
                IsRunFadeOut = false;
            }
            imgColor.a = value;
            myImage.color = imgColor;
            yield return null;
        }
    }

    public void FadeIn()
    {
        value = 1.0f;
        myImage.raycastTarget = false;
        imgColor.a = value;
        myImage.color = imgColor;
    }

    //public bool IsRunFadeIn;
    //IEnumerator FadeIn()
    //{
    //    IsRunFadeIn = true;
    //    while (value < 1.0f)
    //    {
    //        value += Time.deltaTime / fadeOutTime;
    //        if (value >= 1.0f)
    //        {
    //            value = 1.0f;
    //            myImage.raycastTarget = true;
    //            IsRunFadeIn = false;
    //        }
    //        imgColor.a = value;
    //        myImage.color = imgColor;
    //        yield return null;
    //    }
    //}
}
