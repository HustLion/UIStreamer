using System;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    public class TimeDriver : MonoBehaviour
    {
        private Material mat;
        public bool shouldUpdate = true;

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
                    mat.SetVector("_ToPosition", new Vector4(worldRightTop.x, worldRightTop.y, worldRightTop.z, 1.0f));
                    mat.SetVector("_FromPosition", new Vector4(worldLeftBottom.x, worldLeftBottom.y, worldLeftBottom.z, 1.0f));
                }

            }
        }
    }
}