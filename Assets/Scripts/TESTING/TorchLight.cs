using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TorchLight : MonoBehaviour
{
    [SerializeField] private Light light;

    [SerializeField] private float minRange = 20;
    [SerializeField] private float maxRange = 120f;
    [SerializeField] private float minIntensity = 1;
    [SerializeField] private float maxIntensity = 3;

    private float range = 100f;
    private float intensity = 3;

    private float rangeSpeed;
    private float intensitySpeed;

    [SerializeField] private float changeTime = 0.5f;

    private void Start() => StartCoroutine(LightRealism());

    private void Update()
    {
        light.range = Mathf.SmoothDamp(light.range, range, ref rangeSpeed, changeTime);
        light.intensity = Mathf.SmoothDamp(light.intensity, intensity, ref intensitySpeed, changeTime);
    }

    public IEnumerator LightRealism()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeTime);

            range = Random.Range(minRange, maxRange);

            intensity = range / (maxRange - minRange) * (maxIntensity - minIntensity);

            changeTime = Random.Range(0.5f, 1f);
        }
    }
}