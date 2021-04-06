using System;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    [AddComponentMenu("UI/UIEffects/StreamerWithRamp", 2)]
    public class StreamerWithRamp : BaseMaterialEffect
    {
        private const uint k_ShaderId = 1 << 4;
        private Material mat;
        
        [Tooltip("Effect Progress")] [SerializeField] [Range(0, 1)]
        float m_Progress = 0f;

        [Tooltip("Streamer Texture")] [SerializeField]
        private Texture2D m_StreamerTexture;

        [SerializeField]
        private Color m_StreamerColor = Color.white;

        [SerializeField][Range(0, 10)]
        private float m_MoveSpeed = 1f;

        [SerializeField][Range(0,10)]
        private float m_Power = 1f;
        
        [SerializeField] EffectPlayer m_Player;
        /// <summary>
        /// Effect factor between 0(start) and 1(end).
        /// </summary>
        public float progress
        {
            get { return m_Progress; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (Mathf.Approximately(m_Progress, value)) return;
                m_Progress = value;
                SetEffectParamsDirty();
            }
        }

        public Texture2D streamerTexture
        {
            get { return m_StreamerTexture; }
            set
            {
                if (value == m_StreamerTexture) return;
                m_StreamerTexture = value;
                SetEffectParamsDirty();
            }
        }

        public Color streamerColor
        {
            get => m_StreamerColor;
            set
            {
                if (m_StreamerColor == value) return;
                m_StreamerColor = value;
                SetEffectParamsDirty();
            }
        }

        public float moveSpeed
        {
            get => m_MoveSpeed;
            set
            {
                if (Mathf.Approximately(m_MoveSpeed, value)) return;
                m_MoveSpeed = value;
                SetEffectParamsDirty();
            }
        }

        public float power
        {
            get => m_Power;
            set
            {
                if (Mathf.Approximately(m_Power, value)) return;
                m_Power = value;
                SetEffectParamsDirty();
            }
        }

        protected override void SetEffectParamsDirty()
        {
            base.SetEffectParamsDirty();
            if (mat)
            {
                mat.SetFloat("_Progress", progress);
                mat.SetTexture("_StreamerTexture", streamerTexture);
                mat.SetColor("_StreamerColor", streamerColor);
                mat.SetFloat("_MoveSpeed", moveSpeed);
                mat.SetFloat("_Power", power);
            }
        }

        public EffectPlayer effectPlayer
        {
            get { return m_Player ?? (m_Player = new EffectPlayer()); }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            effectPlayer.OnEnable(f => progress = f);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            effectPlayer.OnDisable();
        }
        public override Hash128 GetMaterialHash(Material material)
        {
            if (!isActiveAndEnabled || !material || !material.shader)
                return k_InvalidHash;

            return new Hash128(
                (uint) material.GetInstanceID(),
                k_ShaderId,
                0,
                0
            );
        }
        public override void ModifyMaterial(Material newMaterial, Graphic graphic)
        {
            var connector = GraphicConnector.FindConnector(graphic);

            newMaterial.shader = Shader.Find(string.Format("Hidden/{0} (StreamerWithRamp)", newMaterial.shader.name));
            mat = newMaterial;
            // paramTex.RegisterMaterial(newMaterial);
        }

        
        public void Play(bool reset = true)
        {
            effectPlayer.Play(reset);
        }
        public void Stop(bool reset = true)
        {
            effectPlayer.Stop(reset);
        }

        private void Update()
        {
            if (mat)
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
                    var canvas = GetComponentInParent<Canvas>();
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