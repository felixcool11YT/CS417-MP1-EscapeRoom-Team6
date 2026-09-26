using UnityEngine;
using UnityEngine.UI;

public static class HUDStabilizer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StabilizeHUD()
    {
        HeadLock("GameplayHUD", 0.7f, 0.0007f);
        HeadLock("NewspaperReadUI", 0.35f, 0.00045f);
    }

    private static void HeadLock(string objectName, float distance, float worldScale)
    {
        GameObject go = GameObject.Find(objectName);
        if (go == null)
            return;

        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null || canvas.renderMode == RenderMode.WorldSpace)
            return;

        Camera cam = canvas.worldCamera;
        if (cam == null)
            cam = Camera.main;
        if (cam == null)
            return;

        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        if (scaler != null)
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(cam.transform, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = Vector2.zero;
        rt.sizeDelta = new Vector2(1920f, 1080f);
        rt.localScale = new Vector3(worldScale, worldScale, worldScale);
        rt.localPosition = new Vector3(-960f * worldScale, -540f * worldScale, distance);
        rt.localRotation = Quaternion.identity;
    }
}
