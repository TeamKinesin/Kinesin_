using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace StellaRabbitStudio
{
    public abstract class BaseEffectController : MonoBehaviour
    {
        // 공유 material 인스턴스를 위한 static 변수
        protected static Material sharedInstanceMaterial;
        protected static TMP_Text sharedTmpText;
        
        [Header("Base Settings")]
        public bool useInstanceMaterial = true;
        public bool playOnEnable = true;
        public float playTime = 1f;
        public int rePlay = -1; // -1: infinite, 0: no replay, >0: replay count
        public float rePlayCoolTime = 0f;

        [Header("Events")]
        public UnityEvent OnEffectStarted;
        public UnityEvent OnEffectCompleted;
        public UnityEvent OnEffectStopped;

        protected TMP_Text tmpText;
        protected Material originalMaterial;
        protected Material instanceMaterial => sharedInstanceMaterial;
        protected bool isPlaying;
        protected float currentTime;
        protected int currentReplayCount;
        protected float cooldownTimer = 0f;
        protected bool isInCooldown = false;

        public virtual void Awake()
        {
            tmpText = GetComponent<TMP_Text>();
            if (tmpText == null)
            {
                Debug.LogError($"TextMeshProUGUI component not found on {gameObject.name}");
                return;
            }

            // 첫 번째 효과가 초기화될 때만 material 인스턴스화
            if (sharedInstanceMaterial == null)
            {
                if (useInstanceMaterial)
                {
                    if (tmpText.fontMaterial != tmpText.fontSharedMaterial)
                    {
                        sharedInstanceMaterial = tmpText.fontMaterial;
                    }
                    else
                    {
                        originalMaterial = tmpText.fontSharedMaterial;
                        sharedInstanceMaterial = new Material(originalMaterial);
                        tmpText.fontMaterial = sharedInstanceMaterial;
                    }
                }
                else
                {
                    sharedInstanceMaterial = tmpText.fontSharedMaterial;
                }
                sharedTmpText = tmpText;
            }
            else if (tmpText != sharedTmpText)
            {
                Debug.LogError("Multiple TMP_Text components with effects are not supported!");
            }
        }

        public virtual void LoadMaterialProperties() {
        }

        protected virtual void OnEnable()
        {
            if (playOnEnable)
                Play();
        }

        protected virtual void OnDisable()
        {
            Stop();
        }

        protected virtual void OnDestroy()
        {
            // 마지막 효과가 제거될 때 공유 material 정리
            if (sharedInstanceMaterial != null && gameObject.scene.isLoaded)
            {
                var remainingEffects = GetComponents<BaseEffectController>();
                if (remainingEffects.Length <= 1) // 자기 자신만 남았을 때
                {
                    if (useInstanceMaterial)
                    {
                        Destroy(sharedInstanceMaterial);
                    }
                    sharedInstanceMaterial = null;
                    sharedTmpText = null;
                }
            }
        }

        public virtual void Play()
        {
            isPlaying = true;
            currentTime = 0f;
            currentReplayCount = 0;
            OnEffectStarted?.Invoke();
        }

        public virtual void Stop()
        {
            isPlaying = false;
            OnEffectStopped?.Invoke();
            ResetEffect();
        }

        protected abstract void ResetEffect();
        protected abstract void UpdateEffect();

        protected virtual void Update()
        {
            if (!isPlaying && isInCooldown)
            {
                cooldownTimer += Time.deltaTime;
                if (cooldownTimer >= rePlayCoolTime)
                {
                    isInCooldown = false;
                    isPlaying = true;
                    currentTime = 0f;
                    UpdateEffect();
                }
                return;
            }

            if (!isPlaying) return;

            currentTime += Time.deltaTime;
            UpdateEffect();

            if (currentTime >= playTime)
            {
                OnEffectCompleted?.Invoke();
                
                bool shouldContinue = rePlay == -1 || currentReplayCount < rePlay;
                if (shouldContinue)
                {
                    if (rePlay > 0)
                        currentReplayCount++;

                    if (rePlayCoolTime > 0)
                    {
                        isPlaying = false;
                        isInCooldown = true;
                        cooldownTimer = 0f;
                        ResetEffect();
                    }
                    else
                    {
                        currentTime = 0f;
                        UpdateEffect();
                    }
                }
                else
                {
                    Stop();
                }
            }
        }
    }
}
