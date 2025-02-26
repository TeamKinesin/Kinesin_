using UnityEngine;

namespace StellaRabbitStudio
{
    public class DoodleOnce : BaseEffectController
    {
        [Header("Doodle Effect Properties")]
        [SerializeField] private bool _doodleEnabled = true;
        [SerializeField] private DoodleTypeEnum _doodleType = DoodleTypeEnum.Line;
        [SerializeField] private float _doodleSpeed = 1f;
        [SerializeField] private float _doodleIntensity = 1f;
        [SerializeField] private float _doodleScale = 1f;
        [SerializeField] private int _doodleFramerate = 8;
        [SerializeField] private Color _doodleColor = Color.white;

        public enum DoodleTypeEnum
        {
            Line = 0,
            Circle = 1
        }

        public bool DoodleEnabled
        {
            get => _doodleEnabled;
            set
            {
                _doodleEnabled = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_DOODLEEnabled", value ? 1 : 0);
            }
        }

        public DoodleTypeEnum DoodleType
        {
            get => _doodleType;
            set
            {
                _doodleType = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_DoodleType", (float)value);
            }
        }

        public float DoodleSpeed
        {
            get => _doodleSpeed;
            set
            {
                _doodleSpeed = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_DoodleSpeed", value);
            }
        }

        public float DoodleIntensity
        {
            get => _doodleIntensity;
            set
            {
                _doodleIntensity = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_DoodleIntensity", value);
            }
        }

        public float DoodleScale
        {
            get => _doodleScale;
            set
            {
                _doodleScale = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_DoodleScale", value);
            }
        }

        public int DoodleFramerate
        {
            get => _doodleFramerate;
            set
            {
                _doodleFramerate = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_DoodleFramerate", value);
            }
        }

        public Color DoodleColor
        {
            get => _doodleColor;
            set
            {
                _doodleColor = value;
                if (isPlaying)
                    instanceMaterial?.SetColor("_DoodleColor", value);
            }
        }

        public override void LoadMaterialProperties()
        {
            base.LoadMaterialProperties();
            if (instanceMaterial == null) return;

            _doodleEnabled = instanceMaterial.GetFloat("_DOODLEEnabled") > 0.5f;
            _doodleType = (DoodleTypeEnum)instanceMaterial.GetFloat("_DoodleType");
            _doodleSpeed = instanceMaterial.GetFloat("_DoodleSpeed");
            _doodleIntensity = instanceMaterial.GetFloat("_DoodleIntensity");
            _doodleScale = instanceMaterial.GetFloat("_DoodleScale");
            _doodleFramerate = (int)instanceMaterial.GetFloat("_DoodleFramerate");
            _doodleColor = instanceMaterial.GetColor("_DoodleColor");
        }

        protected override void ResetEffect()
        {
            if (instanceMaterial == null) return;

            instanceMaterial.SetFloat("_DOODLEEnabled", 0);
            instanceMaterial.SetFloat("_DoodleType", 0);
            instanceMaterial.SetFloat("_DoodleSpeed", 0);
            instanceMaterial.SetFloat("_DoodleIntensity", 0);
            instanceMaterial.SetFloat("_DoodleScale", 1);
            instanceMaterial.SetFloat("_DoodleFramerate", 8);
            instanceMaterial.SetColor("_DoodleColor", Color.white);
        }

        protected override void UpdateEffect()
        {
            if (instanceMaterial == null) return;

            DoodleEnabled = _doodleEnabled;
            DoodleType = _doodleType;
            DoodleSpeed = _doodleSpeed;
            DoodleIntensity = _doodleIntensity;
            DoodleScale = _doodleScale;
            DoodleFramerate = _doodleFramerate;
            DoodleColor = _doodleColor;
        }
    }
}
