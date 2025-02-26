using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace StellaRabbitStudio
{
    public class HandDrawnOnce : BaseEffectController
    {
        [Header("Hand Drawn Effect Properties")]
        [SerializeField] private bool _handDrawnEnabled = true;
        [SerializeField] private float _handDrawnSpeed = 5f;
        [SerializeField] private float _handDrawnAmount = 3f;
        [SerializeField] private bool _handDrawnRandomEnabled = false;
        [SerializeField] private float _handDrawnFixedOffset = 1f;
        [SerializeField] private float _handDrawnFramerate = 8f;
        [SerializeField] private float _handDrawnFrequency = 4f;

        public bool HandDrawnEnabled
        {
            get => _handDrawnEnabled;
            set
            {
                _handDrawnEnabled = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_HANDDRAWNEnabled", value ? 1 : 0);
            }
        }

        public float HandDrawnSpeed
        {
            get => _handDrawnSpeed;
            set
            {
                _handDrawnSpeed = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_HandDrawnSpeed", value);
            }
        }

        public float HandDrawnAmount
        {
            get => _handDrawnAmount;
            set
            {
                _handDrawnAmount = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_HandDrawnAmount", value);
            }
        }

        public bool HandDrawnRandomEnabled
        {
            get => _handDrawnRandomEnabled;
            set
            {
                _handDrawnRandomEnabled = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_HandDrawnRandomEnabled", value ? 1 : 0);
            }
        }

        public float HandDrawnFixedOffset
        {
            get => _handDrawnFixedOffset;
            set
            {
                _handDrawnFixedOffset = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_HandDrawnFixedOffset", value);
            }
        }

        public float HandDrawnFramerate
        {
            get => _handDrawnFramerate;
            set
            {
                _handDrawnFramerate = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_HandDrawnFramerate", value);
            }
        }

        public float HandDrawnFrequency
        {
            get => _handDrawnFrequency;
            set
            {
                _handDrawnFrequency = value;
                if (isPlaying)
                    instanceMaterial?.SetFloat("_HandDrawnFrequency", value);
            }
        }

        public override void LoadMaterialProperties()
        {
            base.LoadMaterialProperties();
            if (instanceMaterial == null) return;
            
            _handDrawnEnabled = instanceMaterial.GetFloat("_HANDDRAWNEnabled") > 0.5f;
            _handDrawnSpeed = instanceMaterial.GetFloat("_HandDrawnSpeed");
            _handDrawnAmount = instanceMaterial.GetFloat("_HandDrawnAmount");
            _handDrawnRandomEnabled = instanceMaterial.GetFloat("_HandDrawnRandomEnabled") > 0.5f;
            _handDrawnFixedOffset = instanceMaterial.GetFloat("_HandDrawnFixedOffset");
            _handDrawnFramerate = instanceMaterial.GetFloat("_HandDrawnFramerate");
            _handDrawnFrequency = instanceMaterial.GetFloat("_HandDrawnFrequency");
        }

        protected override void ResetEffect()
        {
            if (instanceMaterial == null) return;
            
            instanceMaterial.SetFloat("_HANDDRAWNEnabled", 0);
            instanceMaterial.SetFloat("_HandDrawnSpeed", 0);
            instanceMaterial.SetFloat("_HandDrawnAmount", 0);
            instanceMaterial.SetFloat("_HandDrawnRandomEnabled", 0);
            instanceMaterial.SetFloat("_HandDrawnFixedOffset", 0);
            instanceMaterial.SetFloat("_HandDrawnFramerate", 0);
            instanceMaterial.SetFloat("_HandDrawnFrequency", 0);
        }

        protected override void UpdateEffect()
        {
            if (instanceMaterial == null) return;

            HandDrawnEnabled = _handDrawnEnabled;
            HandDrawnSpeed = _handDrawnSpeed;
            HandDrawnAmount = _handDrawnAmount;
            HandDrawnRandomEnabled = _handDrawnRandomEnabled;
            HandDrawnFixedOffset = _handDrawnFixedOffset;
            HandDrawnFramerate = _handDrawnFramerate;
            HandDrawnFrequency = _handDrawnFrequency;
        }


    }
}
