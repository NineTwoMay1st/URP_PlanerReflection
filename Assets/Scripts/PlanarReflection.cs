using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class PlanarReflection : MonoBehaviour
{
    [SerializeField]
    public bool _isReflect = true;
    [SerializeField]
    private Vector2Int _screenSize = new Vector2Int(1920, 1080);
    private Camera _mainCamera;
    private Camera _reflectionCamera;
    private Transform _reflectionPlane;
    [SerializeField]
    private Material _material;
    private RenderTexture outputTexture;
    [Header("高度偏移")]
    [SerializeField]
    private float _verticalOffset;
    private Transform _mainCamTransform;
    private Transform _reflectionCamTransform;
    [SerializeField]
    private LayerMask _ignoreLayer;
    private float _currentTime;
    private float _lastTime;
    private float _time = 0;

    private void Start()
    {
        Init();
        RenderPipelineManager.endCameraRendering += EndCameraRendering;
    }

    private void Init()
    {
        _mainCamera = Camera.main;
        _mainCamTransform = _mainCamera.transform;
        GameObject cam = GameObject.Find("reflectCamera");
        if (cam != null)
            _reflectionCamera = cam.GetComponent<Camera>();
        else
            _reflectionCamera = new GameObject("reflectCamera").AddComponent<Camera>();
        _reflectionCamTransform = _reflectionCamera.transform;
        _reflectionCamera.CopyFrom(_mainCamera);
        _reflectionCamera.cullingMask = ~_ignoreLayer;
        _reflectionPlane = transform;
        if(outputTexture == null || outputTexture.texelSize.x != _screenSize.x || outputTexture.texelSize.y != _screenSize.y)
            outputTexture = new RenderTexture(_screenSize.x, _screenSize.y, 24);
        _reflectionCamera.targetTexture = outputTexture;
        if(_material)
            _material.SetTexture("_ReflectTexture", outputTexture);
    }

    private void EndCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        if (!_isReflect || _mainCamTransform == null || _reflectionCamTransform == null)
            return;
        Vector3 cameraDirectionWorldSpace = _mainCamTransform.forward;
        Vector3 cameraUpWorldSpace = _mainCamTransform.up;
        Vector3 cameraPositionWorldSpace = _mainCamTransform.position;

        cameraPositionWorldSpace.y += _verticalOffset;


        Vector3 cameraDirectionPlaneSpace = _reflectionPlane.InverseTransformDirection(cameraDirectionWorldSpace);
        Vector3 cameraUpPlaneSpace = _reflectionPlane.InverseTransformDirection(cameraUpWorldSpace);
        Vector3 cameraPositionPlaneSpace = _reflectionPlane.InverseTransformPoint(cameraPositionWorldSpace);


        cameraDirectionPlaneSpace.y *= -1;
        cameraUpPlaneSpace.y *= -1;
        cameraPositionPlaneSpace.y *= -1;

        cameraDirectionWorldSpace = _reflectionPlane.TransformDirection(cameraDirectionPlaneSpace);
        cameraUpWorldSpace = _reflectionPlane.TransformDirection(cameraUpPlaneSpace);
        cameraPositionWorldSpace = _reflectionPlane.TransformPoint(cameraPositionPlaneSpace);

        _reflectionCamTransform.position = cameraPositionWorldSpace;
        _reflectionCamTransform.LookAt(cameraPositionWorldSpace + cameraDirectionWorldSpace, cameraUpWorldSpace);
    }


    private void Update()
    {
#if UNITY_EDITOR
        if (_isReflect && _material != null)
            _material.SetTexture("_ReflectTexture", outputTexture);
#endif
        CheckVisible();
    }


    private void OnWillRenderObject()
    {
        _currentTime = Time.time;
    }

    private void OnValidate()
    {
        if (_isReflect)
        {
            Init();
            if (_reflectionCamera)
                _reflectionCamera.enabled = true;
            if (_material)
                _material.SetFloat("_ReflectPower", 1);
            EndCameraRendering(new ScriptableRenderContext(), null);
        }
        else
        {
            if (_reflectionCamera)
                _reflectionCamera.enabled = false;
            if (_material)
                _material.SetFloat("_ReflectPower", 0);
            _material.SetTexture("_ReflectTexture", null);
        }
    }

    private void CheckVisible()
    {
        _time += Time.deltaTime;
        if (_time > 0.5f)
        {
            if (_reflectionCamera)
                _reflectionCamera.enabled = _currentTime != _lastTime;
            _lastTime = _currentTime;
            _time = 0;
        }
    }
}
