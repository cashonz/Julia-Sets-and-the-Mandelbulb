using UnityEngine;

public class RayMarchController : MonoBehaviour
{
    [Header("RayMarch Settings")]
    public int maxRaySteps;
    public int maxIterations;
    public float NormalApproxStep;
    public float escapeRadius;
    public float boundingRadius;
    [Header("Fractal Settings")]
    public FractalType fractalType;
    public enum FractalType
    {
        Julia,
        Mandelbulb,
        Intersect
    }
    public Vector4 JuliaSet;
    public float mandelPowerValue;
    public float rotation;
    public Vector3 rotationAxis;
     public bool sliceFractal;
    public float slicePosition;
    public Vector3 slicePlaneNormal;
    [Header("Light Parameters")]
    public Color backgroundColor;
    public Color fractalColorInner;
    public Color fractalColorOuter;
    public Color lightColor;
    public Vector3 lightPosition;
    public float specularExponent;
    public float diffuseStrength;
    public float lightStepScale;
    public int lightSteps;
    public int penumbra;
    public float colorLerpFactor;
    [Header("Misc Necessary")]
    [SerializeField] private ComputeShader compute;
    [SerializeField] private Camera cam;
    public RenderTexture target;
    private int screenHeight;
    private int screenWidth;
    private float fov;
    public Material blitMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        fov = Camera.main.fieldOfView;
        InitRenderTexture();
    }

    // Update is called once per frame
    void Update()
    {
        int kernelHandle = compute.FindKernel("CSMain");
        setComputeParams(kernelHandle);
        int groupsX = Mathf.CeilToInt(screenWidth / 8.0f);
        int groupsY = Mathf.CeilToInt(screenHeight / 8.0f);
        compute.Dispatch(kernelHandle, groupsX, groupsY, 1);

        //blitMaterial.SetTexture("_MainTex", target);
        blitMaterial.SetTexture("_InputTexture", target);
    }

    void setComputeParams(int kernelHandle)
    {
        compute.SetTexture(kernelHandle, "Result", target);
        compute.SetFloat("screenWidth", screenWidth);
        compute.SetFloat("screenHeight", screenHeight);
        compute.SetFloat("fov", fov);
        compute.SetInt("maxRaySteps", maxRaySteps);
        compute.SetInt("maxIterations", maxIterations);
        compute.SetFloat("_EscapeRadius", escapeRadius);
        compute.SetFloat("delta", NormalApproxStep);
        compute.SetFloat("theta", rotation);
        compute.SetFloat("_BoundingRadius", boundingRadius);
        compute.SetVector("c", JuliaSet);
        compute.SetVector("rotationAxis", rotationAxis);
        compute.SetVector("_SlicePlaneNormal", slicePlaneNormal);
        compute.SetVector("_BackgroundColor", backgroundColor);
        compute.SetVector("_FractalColorInner", fractalColorInner);
        compute.SetVector("_FractalColorOuter", fractalColorOuter);
        compute.SetVector("_LightPos", lightPosition);
        compute.SetVector("_LightColor", lightColor);
        compute.SetFloat("_Specular", specularExponent);
        compute.SetFloat("_ColorLerpFactor", colorLerpFactor);
        compute.SetVector("_CamPos", cam.transform.position);
        compute.SetMatrix("_CameraToWorld", cam.cameraToWorldMatrix);
        compute.SetBool("slice", sliceFractal);
        compute.SetFloat("sliceVal", slicePosition);
        compute.SetFloat("_Diffuse", diffuseStrength);
        compute.SetFloat("_LightStepScale", lightStepScale);
        compute.SetInt("_LightSteps", lightSteps);
        compute.SetInt("_Penumbra", penumbra);
        compute.SetFloat("_PowVal", mandelPowerValue);

        switch(fractalType)
        {
            case FractalType.Julia:
                compute.SetInt("fractalType", 0);
                break;
            case FractalType.Mandelbulb:
                compute.SetInt("fractalType", 1);
                break;
            case FractalType.Intersect:
                compute.SetInt("fractalType", 2);
                break;
        }
    }

    void InitRenderTexture()
    {
        if (target == null || target.width != Screen.width || target.height != Screen.height)
        {
            // Release old texture if it exists
            if (target != null) target.Release();

            target = new RenderTexture(Screen.width, Screen.height, 0);
            target.enableRandomWrite = true;
            target.Create();
        }
    }
}
