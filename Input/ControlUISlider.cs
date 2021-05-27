using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlUISlider : MonoBehaviour
{
    [SerializeField]
    private OVRInput.RawButton sliderControlInc = OVRInput.RawButton.RThumbstickUp;
    [SerializeField]
    private OVRInput.RawButton sliderControlDec = OVRInput.RawButton.RThumbstickDown;
    [SerializeField]
    private float slideSpeed = 1f;
    public bool isActive;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            MoveSlider(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y * Time.unscaledDeltaTime * slideSpeed);
        }
        //MoveSlider(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y * slideSpeed);
        //if(OVRInput.Get(sliderControlInc))
        //{
        //    MoveSlider(stepValue);
        //}
        //else if (OVRInput.Get(sliderControlDec))
        //{
        //    MoveSlider(-stepValue);
        //}

    }

    private void FixedUpdate()
    {
    }

    public void SetUIActive(bool b)
    {
        isActive = b;
    }

    public bool GetUIActive()
    {
        return isActive;
    }

    private void MoveSlider(float val)
    {
        gameObject.GetComponent<Slider>().value = Mathf.Clamp(gameObject.GetComponent<Slider>().value + val, 0, 1);
    }
}
