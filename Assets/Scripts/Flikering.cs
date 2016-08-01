using UnityEngine;
using System.Collections;

public class Flikering : MonoBehaviour {

    // Your light gameObject here.
    public Light _light;

    public float MaxIntensity;
    public float MinIntensity;
    public float FlikeAmount;

    void Start() {

    }

    void Update() {
        float currentIntensity = _light.intensity;
        currentIntensity += Random.Range(-FlikeAmount, FlikeAmount);
        if (currentIntensity > MaxIntensity) currentIntensity = MaxIntensity;
        if (currentIntensity < MinIntensity) currentIntensity = MinIntensity;
        _light.intensity = currentIntensity;
    }
}
