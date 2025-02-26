using UnityEngine;

namespace StellaRabbitStudio
{
    public class BlackHoleOnce : BaseEffectController
    {
        [Header("Black Hole Effect Properties")]
        [SerializeField] private bool _blackHoleEnabled = true;
        [SerializeField] private float _blackHoleAmount = 10f;
        [SerializeField] private float _blackHoleSpeed = 5f;
        [SerializeField] private float _blackHoleThickness = 1f;
        [SerializeField] private Color _blackHoleColor = Color.black;

        public bool BlackHoleEnabled
        {
            get => _blackHoleEnabled;
            set
            {
                _blackHoleEnabled = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_BLACKHOLEEnabled", value ? 1 : 0);
            }
        }

        public float BlackHoleAmount
        {
            get => _blackHoleAmount;
            set
            {
                _blackHoleAmount = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_BlackHoleAmount", value);
            }
        }

        public float BlackHoleSpeed
        {
            get => _blackHoleSpeed;
            set
            {
                _blackHoleSpeed = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_BlackHoleSpeed", value);
            }
        }

        public float BlackHoleThickness
        {
            get => _blackHoleThickness;
            set
            {
                _blackHoleThickness = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_BlackHoleThickness", value);
            }
        }

        public Color BlackHoleColor
        {
            get => _blackHoleColor;
            set
            {
                _blackHoleColor = value;
                if (isPlaying)
                    instanceMaterial?.SetColor("_BlackHoleColor", value);
            }
        }

        public override void LoadMaterialProperties()
        {
            base.LoadMaterialProperties();
            if (instanceMaterial == null) return;
            
            _blackHoleEnabled = instanceMaterial.GetFloat("_BLACKHOLEEnabled") > 0.5f;
            _blackHoleAmount = instanceMaterial.GetFloat("_BlackHoleAmount");
            _blackHoleSpeed = instanceMaterial.GetFloat("_BlackHoleSpeed");
            _blackHoleThickness = instanceMaterial.GetFloat("_BlackHoleThickness");
            _blackHoleColor = instanceMaterial.GetColor("_BlackHoleColor");
        }

        protected override void ResetEffect()
        {
            if (instanceMaterial == null) return;
            
            instanceMaterial.SetFloat("_BLACKHOLEEnabled", 0);
            instanceMaterial.SetFloat("_BlackHoleAmount", 0);
            instanceMaterial.SetFloat("_BlackHoleSpeed", 0);
            instanceMaterial.SetFloat("_BlackHoleThickness", 0);
            instanceMaterial.SetColor("_BlackHoleColor", Color.black);
        }

        protected override void UpdateEffect()
        {
            if (instanceMaterial == null) return;

            BlackHoleEnabled = _blackHoleEnabled;
            BlackHoleAmount = _blackHoleAmount;
            BlackHoleSpeed = _blackHoleSpeed;
            BlackHoleThickness = _blackHoleThickness;
            BlackHoleColor = _blackHoleColor;
        }
    }
}
