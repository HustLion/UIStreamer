using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    public class TimeDriver : MonoBehaviour
    {
        private Material mat;
        public bool shouldUpdate = true;
        public Canvas canvas;

        private static float _Time
        {
            get
            {
                return Time.time;
            }
        }

        private void Start()
        {
            var image = GetComponent<Image>();
            if (image)
            {
                mat = image.material;
            }
        }

        private void Update()
        {
            if (shouldUpdate && mat != null)
            {
                var rt = GetComponent<RectTransform>();
                if (rt)
                {
                    var worldCorners = new Vector3[4];
                    rt.GetWorldCorners(worldCorners);
                    var bounds = new Bounds(worldCorners[0], Vector3.zero);
                    for (int i = 0; i < 4; i++)
                    {
                        bounds.Encapsulate(worldCorners[i]);
                    }

                    var min = bounds.min; // LeftBottom
                    var xyMax = Math.Max(bounds.size.x, bounds.size.y);
                    var worldLeftTop = new Vector3(min.x, min.y + xyMax, min.z);
                    var worldRightBottom = new Vector3(min.x + xyMax, min.y, min.z);
                    var toPosition = new Vector4(worldRightBottom.x, worldRightBottom.y, worldRightBottom.z, 1.0f);
                    var fromPosition = new Vector4(worldLeftTop.x, worldLeftTop.y, worldLeftTop.z, 1.0f);
                    // Debug.DrawLine(fromPosition, toPosition, Color.green);
                    if (canvas.renderMode == RenderMode.WorldSpace)
                    {
                        var toPositionSp = Camera.main.WorldToScreenPoint(toPosition);
                        var fromPositionSp = Camera.main.WorldToScreenPoint(fromPosition);

                        mat.SetVector("_ToPosition", toPositionSp);
                        mat.SetVector("_FromPosition", fromPositionSp);
                        mat.SetFloat("_ScreenHeight", 0);
                        // Debug.Log($"from: {fromPosition}, to: {toPosition}; sp: {fromPositionSp} {toPositionSp}");
                        
                    } else if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    {
                        var toPositionSp = toPosition;
                        var fromPositionSp = fromPosition;
                        mat.SetVector("_ToPosition", toPositionSp);
                        mat.SetVector("_FromPosition", fromPositionSp);
                        mat.SetFloat("_ScreenHeight", Screen.height);
                        // Debug.Log($"from: {fromPosition}, to: {toPosition}; sp: {fromPositionSp} {toPositionSp}");
                    } else if (canvas.renderMode == RenderMode.ScreenSpaceCamera) {
                        var toPositionSp = Camera.main.WorldToScreenPoint(toPosition);
                        var fromPositionSp = Camera.main.WorldToScreenPoint(fromPosition);
                        mat.SetVector("_ToPosition", toPositionSp);
                        mat.SetVector("_FromPosition", fromPositionSp);
                        mat.SetFloat("_ScreenHeight", 0);
                        // Debug.Log($"from: {fromPosition}, to: {toPosition}; sp: {fromPositionSp} {toPositionSp}");
                    }
                }

            }
        }
    }
}