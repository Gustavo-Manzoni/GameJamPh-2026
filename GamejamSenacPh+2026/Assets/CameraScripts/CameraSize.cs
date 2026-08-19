
/*
using UnityEngine;
using System.Collections;

public class CameraSize : MonoBehaviour
{
    public Camera camera;
    public int size = 5;
    public float transitionSpeed = 5f;
    public float minSize = 3f;
    public float maxSize = 15f;

    private float targetSize;
    private float currentSize;
    private bool isTransitioning = false;
    private Map mapScript;

    [Header("Configurações de Expansão")]
    public bool expandOnlyOneSide = false;
    public enum ExpandDirection { LEFT, RIGHT, UP, DOWN, NONE }
    public ExpandDirection expandDirection = ExpandDirection.RIGHT;
    public float expandAmount = 2f;

    // Armazena a posição original ANTES do zoom
    private Vector3 preZoomPosition;

    void Start()
    {
        if (camera == null)
        {
            camera = GetComponent<Camera>();
        }

        if (camera == null)
        {
            camera = Camera.main;
        }

        mapScript = GetComponent<Map>();

        if (camera != null)
        {
            currentSize = camera.orthographicSize;
            targetSize = Mathf.Clamp(size, minSize, maxSize);
            preZoomPosition = camera.transform.position;
        }
    }

    void Update()
    {
        if (isTransitioning && camera != null)
        {
            // Transição suave do tamanho
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetSize, Time.deltaTime * transitionSpeed);

            // Se estiver expandindo apenas um lado, ajusta a posição
            if (expandOnlyOneSide && expandDirection != ExpandDirection.NONE)
            {
                UpdateCameraPosition();
            }

            // Verifica se chegou no alvo
            if (Mathf.Abs(camera.orthographicSize - targetSize) < 0.01f)
            {
                camera.orthographicSize = targetSize;
                isTransitioning = false;

                // Atualiza os bounds
                if (mapScript != null && !mapScript.IsCameraMoving)
                {
                    mapScript.updateBounds();
                }

                Debug.Log($"✅ Expansão concluída! Tamanho: {camera.orthographicSize}, Posição: {camera.transform.position}");
            }
        }
    }

    // Método principal para expandir a câmera
    public void ExpandCamera(float amount = 0f)
    {
        if (camera == null)
        {
            Debug.LogError("❌ Câmera não encontrada!");
            return;
        }

        float expandValue = amount > 0 ? amount : expandAmount;
        currentSize = camera.orthographicSize;
        targetSize = Mathf.Clamp(currentSize + expandValue, minSize, maxSize);

        // Salva a posição atual ANTES da expansão
        preZoomPosition = camera.transform.position;

        isTransitioning = true;

        Debug.Log($"🔍 Expandindo câmera: {currentSize} → {targetSize}, Direção: {expandDirection}");
    }

    // Atualiza a posição da câmera durante a expansão
    private void UpdateCameraPosition()
    {
        if (camera == null) return;

        Vector3 newPos = camera.transform.position;
        float currentSize = camera.orthographicSize;
        float sizeDifference = targetSize - currentSize;

        // A metade do tamanho da câmera em unidades mundiais
        float halfHeight = currentSize;
        float halfWidth = currentSize * camera.aspect;

        // Calcula a nova posição baseada na direção de expansão
        switch (expandDirection)
        {
            case ExpandDirection.RIGHT:
                // Mantém a borda ESQUERDA fixa
                // Borda esquerda = posX - halfWidth
                // Nova posição = borda esquerda + novoHalfWidth
                float newHalfWidth = halfWidth + (sizeDifference * camera.aspect * 0.5f);
                newPos.x = (preZoomPosition.x - halfWidth) + newHalfWidth;
                break;

            case ExpandDirection.LEFT:
                // Mantém a borda DIREITA fixa
                float newHalfWidthL = halfWidth + (sizeDifference * camera.aspect * 0.5f);
                newPos.x = (preZoomPosition.x + halfWidth) - newHalfWidthL;
                break;

            case ExpandDirection.UP:
                // Mantém a borda INFERIOR fixa
                float newHalfHeight = halfHeight + (sizeDifference * 0.5f);
                newPos.y = (preZoomPosition.y - halfHeight) + newHalfHeight;
                break;

            case ExpandDirection.DOWN:
                // Mantém a borda SUPERIOR fixa
                float newHalfHeightD = halfHeight + (sizeDifference * 0.5f);
                newPos.y = (preZoomPosition.y + halfHeight) - newHalfHeightD;
                break;

            case ExpandDirection.NONE:
                // Zoom centralizado (não mexe na posição)
                break;
        }

        camera.transform.position = newPos;
    }

    // Método para aumentar normalmente (centralizado)
    public void IncreaseCameraSize(float increaseAmount = 1f)
    {
        if (camera != null)
        {
            currentSize = camera.orthographicSize;
            targetSize = Mathf.Clamp(currentSize + increaseAmount, minSize, maxSize);
            preZoomPosition = camera.transform.position;
            isTransitioning = true;
        }
    }

    // Método UpdateCamera (chamado pelo Map)
    public void UpdateCamera()
    {
        if (expandOnlyOneSide && expandDirection != ExpandDirection.NONE)
        {
            ExpandCamera(1f);
        }
        else
        {
            IncreaseCameraSize(1f);
        }
    }

    // Configura a direção de expansão dinamicamente
    public void SetExpandDirection(ExpandDirection direction)
    {
        expandDirection = direction;
        expandOnlyOneSide = true;
        Debug.Log($"🎯 Direção de expansão definida para: {direction}");
    }

    public void SetCameraSize(float newSize)
    {
        if (camera != null)
        {
            targetSize = Mathf.Clamp(newSize, minSize, maxSize);
            preZoomPosition = camera.transform.position;
            isTransitioning = true;
        }
    }

    public void SetCameraSizeInstant(float newSize)
    {
        if (camera != null)
        {
            camera.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);
            targetSize = camera.orthographicSize;
            isTransitioning = false;

            if (mapScript != null)
            {
                mapScript.updateBounds();
            }
        }
    }

    public void DecreaseCameraSize()
    {
        if (camera != null)
        {
            currentSize = camera.orthographicSize;
            targetSize = Mathf.Clamp(currentSize - 1f, minSize, maxSize);
            preZoomPosition = camera.transform.position;
            isTransitioning = true;
        }
    }

    public void ResetCameraSize()
    {
        if (camera != null)
        {
            targetSize = Mathf.Clamp(size, minSize, maxSize);
            preZoomPosition = camera.transform.position;
            isTransitioning = true;
        }
    }

    public bool IsTransitioning
    {
        get { return isTransitioning; }
    }

    public float CurrentSize
    {
        get { return camera != null ? camera.orthographicSize : 0f; }
    }
}
*/