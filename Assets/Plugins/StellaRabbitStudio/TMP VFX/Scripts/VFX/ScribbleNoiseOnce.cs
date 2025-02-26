using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace StellaRabbitStudio
{
    public class ScribbleNoiseOnce : BaseEffectController
    {
        [Header("Scribble Effect Properties")]
        [SerializeField] private bool _scribbleEnabled = true;
        [SerializeField] private float _scribbleSpeed = 1f;
        [SerializeField] private float _scribbleAmplitude = 0.005f;
        [SerializeField] private float _scribbleFrequency = 10f;
        [SerializeField] private float _scribbleThickness = 0.5f;
        [SerializeField] private float _scribbleFramerate = 8f;

        public bool ScribbleEnabled
        {
            get => _scribbleEnabled;
            set
            {
                _scribbleEnabled = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_SCRIBBLENOISEEnabled", value ? 1 : 0);
            }
        }

        public float ScribbleSpeed
        {
            get => _scribbleSpeed;
            set
            {
                _scribbleSpeed = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_ScribbleSpeed", value);
            }
        }

        public float ScribbleAmplitude
        {
            get => _scribbleAmplitude;
            set
            {
                _scribbleAmplitude = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_ScribbleAmplitude", value);
            }
        }

        public float ScribbleFrequency
        {
            get => _scribbleFrequency;
            set
            {
                _scribbleFrequency = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_ScribbleFrequency", value);
            }
        }

        public float ScribbleThickness
        {
            get => _scribbleThickness;
            set
            {
                _scribbleThickness = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_ScribbleThickness", value);
            }
        }

        public float ScribbleFramerate
        {
            get => _scribbleFramerate;
            set
            {
                _scribbleFramerate = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_ScribbleFramerate", value);
            }
        }

        public override void LoadMaterialProperties()
        {
            base.LoadMaterialProperties();
            if (instanceMaterial == null) return;
            
            _scribbleEnabled = instanceMaterial.GetFloat("_SCRIBBLENOISEEnabled") > 0.5f;
            _scribbleSpeed = instanceMaterial.GetFloat("_ScribbleSpeed");
            _scribbleAmplitude = instanceMaterial.GetFloat("_ScribbleAmplitude");
            _scribbleFrequency = instanceMaterial.GetFloat("_ScribbleFrequency");
            _scribbleThickness = instanceMaterial.GetFloat("_ScribbleThickness");
            _scribbleFramerate = instanceMaterial.GetFloat("_ScribbleFramerate");
        }

        protected override void ResetEffect()
        {
            if (instanceMaterial == null) return;
            instanceMaterial.SetFloat("_SCRIBBLENOISEEnabled", 0);
            instanceMaterial.SetFloat("_ScribbleSpeed", 0);
            instanceMaterial.SetFloat("_ScribbleAmplitude", 0);
            instanceMaterial.SetFloat("_ScribbleFrequency", 0);
            instanceMaterial.SetFloat("_ScribbleThickness", 0);
            instanceMaterial.SetFloat("_ScribbleFramerate", 0);
        }
        protected override void UpdateEffect()
        {
            if (instanceMaterial == null) return;

            ScribbleEnabled = _scribbleEnabled;
            ScribbleSpeed = _scribbleSpeed;
            ScribbleAmplitude = _scribbleAmplitude;
            ScribbleFrequency = _scribbleFrequency;
            ScribbleThickness = _scribbleThickness;
            ScribbleFramerate = _scribbleFramerate;
        }
    }
}
