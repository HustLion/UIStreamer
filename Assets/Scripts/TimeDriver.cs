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

                    var min = bounds.min; // LeftTop
                    var xyMax = Math.Max(bounds.size.x, bounds.size.y);
                    var worldLeftBottom = new Vector3(min.x, min.y + xyMax, min.z);
                    var worldRightTop = new Vector3(min.x + xyMax, min.y, min.z);
                    var toPosition = new Vector4(worldRightTop.x, worldRightTop.y, worldRightTop.z, 1.0f);
                    var fromPosition = new Vector4(worldLeftBottom.x, worldLeftBottom.y, worldLeftBottom.z, 1.0f);
                    if (canvas.renderMode == RenderMode.WorldSpace)
                    {
                        var toPositionSp = Camera.main.WorldToScreenPoint(toPosition);
                        var fromPositionSp = Camera.main.WorldToScreenPoint(fromPosition);

                        mat.SetVector("_ToPosition", toPositionSp);
                        mat.SetVector("_FromPosition", fromPositionSp);
                        Debug.Log($"from: {fromPosition}, to: {toPosition}; sp: {fromPositionSp} {toPositionSp}");
                        
                    } else if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                        var toPositionSp = Camera.main.WorldToScreenPoint(toPosition);
                        var fromPositionSp = Camera.main.WorldToScreenPoint(fromPosition);
                        mat.SetVector("_ToPosition", toPosition);
                        mat.SetVector("_FromPosition", toPosition);
                        Debug.Log($"from: {fromPosition}, to: {toPosition}; sp: {fromPositionSp} {toPositionSp}");
                    } else if (canvas.renderMode == RenderMode.ScreenSpaceCamera) {
                        var toPositionSp = Camera.main.WorldToScreenPoint(toPosition);
                        var fromPositionSp = Camera.main.WorldToScreenPoint(fromPosition);
                        mat.SetVector("_ToPosition", toPosition);
                        mat.SetVector("_FromPosition", toPosition);
                        Debug.Log($"from: {fromPosition}, to: {toPosition}; sp: {fromPositionSp} {toPositionSp}");
                    }
                }

            }
        }
    }
}