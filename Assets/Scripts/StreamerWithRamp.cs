using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    [AddComponentMenu("UI/UIEffects/StreamerWithRamp", 2)]
    public class StreamerWithRamp : BaseMaterialEffect
    {
        private const uint k_ShaderId = 1 << 4;
        [Tooltip("Effect Progress")] [SerializeField] [Range(0, 1)]
        float m_Progress = 0f;

        [Tooltip("Streamer Texture")] [SerializeField]
        private Texture2D m_StreamerTexture;
        
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

        protected override void SetEffectParamsDirty()
        {
            base.SetEffectParamsDirty();
            if (mat)
            {
                mat.SetFloat("_Progress", progress);
                mat.SetTexture("_StreamerTexture", streamerTexture);
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

        private Material mat;
        
        public void Play(bool reset = true)
        {
            effectPlayer.Play(reset);
        }
        public void Stop(bool reset = true)
        {
            effectPlayer.Stop(reset);
        }

    }
}