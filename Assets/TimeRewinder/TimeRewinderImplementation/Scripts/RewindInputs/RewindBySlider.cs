using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Example how to rewind time with slider Input
/// </summary>
public class RewindBySlider : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    //Cause TimeManager timestep is set to Time.FixedDeltaTime, animator of the slider is also se to same timestep

    [SerializeField] Slider slider;
    [SerializeField] RewindManager rewindManager;
    [SerializeField] AudioSource rewindSound;
    Animator sliderAnimator;
    private int howManyFingersTouching = 0;


    void Start()
    {
        sliderAnimator = slider.GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        howManyFingersTouching++;

        if (howManyFingersTouching == 1)
            OnSliderDown();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        howManyFingersTouching--;

        if (howManyFingersTouching == 0)
            OnSliderUp();
    }
    public void OnSliderUp()
    {
        if (slider.interactable)
        {
            rewindManager.StopRewindTimeBySeconds();                    //After rewind is done, correctly stop it
            RestoreSliderAnimation();
            rewindSound.Stop();
        }
    }
    public void OnSliderDown()
    {
        if (slider.interactable)
        {
            rewindManager.StartRewindTimeBySeconds(-slider.value);       //Start rewind preview
            SliderAnimationPause();
            rewindSound.Play();
        }
    }
    public void OnSliderUpdate(Single value)
    {
        rewindManager.SetTimeSecondsInRewind(-value);                    //If slider value changes, change rewind preview state
    }
    public void SliderAnimationPause()
    {
        sliderAnimator.SetFloat("TimeRewindSpeed", 0);
    }
    public void RestoreSliderAnimation()
    {
        float animationTimeStartFrom = (slider.value - slider.minValue) / rewindManager.howManySecondsToTrack;
        sliderAnimator.Play("AutoResizeAnim", 0, animationTimeStartFrom);
        sliderAnimator.SetFloat("TimeRewindSpeed", 1);
        StartCoroutine(ResetSliderValue());
    }
    //Cause slider animator is in fixed update
    IEnumerator ResetSliderValue()
    {
        yield return new WaitForFixedUpdate();
        slider.value = 0;
    }
}