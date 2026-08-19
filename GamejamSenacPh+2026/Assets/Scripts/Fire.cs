using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Fire : MonoBehaviour, IFireable
{
    [SerializeField] Health health;
    [SerializeField] Image lifebarImage;

    [SerializeField] Transform basePivot;
  
    [SerializeField] Transform upperPivot;
    
    [SerializeField] Transform smokePivot;
    [SerializeField] Transform fireVisual;

   
    [SerializeField] float baseSwayAngle = 1.5f;
    [SerializeField] float upperSwayAngle = 4f;
    [SerializeField] float smokeSwayAngle = 9f;
    [SerializeField] float smokeFloatHeight = 0.05f;
    [SerializeField] float baseStiffness = 150f;
    [SerializeField] float baseDamping = 14f;
    [SerializeField] float upperStiffness = 85f;
    [SerializeField] float upperDamping = 8f;
    [SerializeField] float smokeStiffness = 45f;
    [SerializeField] float smokeDamping = 5f;

    
    [SerializeField] float idleSwayAngle = 7f;
    [SerializeField] float idleSwaySpeed = 2.2f;
    [SerializeField] float idleBreathAmount = 0.08f;
    [SerializeField] float fireSwayStiffness = 75f;
    [SerializeField] float fireSwayDamping = 7f;
    [SerializeField] float fireScaleStiffness = 115f;
    [SerializeField] float fireScaleDamping = 10f;
    [SerializeField] Vector2 hitFlareKick = new Vector2(0.16f, 0.34f);

    [SerializeField] float cameraShakeIntensity = 0.08f;
    [SerializeField] float cameraShakeDuration = 0.12f;
    [SerializeField] Color lifebarHitTint = new Color(1f, 0.45f, 0.05f);
    [SerializeField] float lifebarTintTime = 0.12f;
    [SerializeField] GameObject fireGameObject;
    Image lifebarTargetImage;
    SpringFloat fireSwaySpring;
    SpringVector2 fireScaleSpring;
    SpringFloat baseSpring;
    SpringFloat upperSpring;
    SpringFloat smokeSpring;
    Vector3 fireBaseScale;
    Vector3 baseBaseScale;
    Vector3 upperBaseScale;
    Vector3 smokeBaseLocalPosition;
    Quaternion baseBaseRotation;
    Quaternion upperBaseRotation;
    Quaternion smokeBaseRotation;
    bool isDying;
    FeedbackManager _feedbackManager;
    [SerializeField] Ease fireDieEase;
    [SerializeField] float fireDieDuration = 1f;
 
    [SerializeField] PositionRecorder recorder;

    public PositionRecorder Recorder { get => recorder; set => recorder = value; }
    GameManager _gameManager;

    public event System.Action OnChimneyDestroyed;

    void Awake()
    {
        health.OnDie += Die;
        lifebarTargetImage = lifebarImage;
       fireVisual = transform;
        basePivot = transform.parent;

        fireBaseScale = fireVisual.localScale;
        CachePivotTransforms();
        fireSwaySpring = new SpringFloat(0f) { stiffness = fireSwayStiffness, damping = fireSwayDamping };
        fireScaleSpring = new SpringVector2(Vector2.one) { stiffness = fireScaleStiffness, damping = fireScaleDamping };
        baseSpring = new SpringFloat(0f) { stiffness = baseStiffness, damping = baseDamping };
        upperSpring = new SpringFloat(0f) { stiffness = upperStiffness, damping = upperDamping };
        smokeSpring = new SpringFloat(0f) { stiffness = smokeStiffness, damping = smokeDamping };
       
    }
    void Start()
    {
        _feedbackManager = ServiceLocator.Get<FeedbackManager>();
        _gameManager = ServiceLocator.Get<GameManager>();
        _gameManager.IncreasePollution(_gameManager.NormalFurnacePollution);
    }
    void Update() => AnimateFire();

    void OnDestroy()
    {
        if (health != null) health.OnDie -= Die;
    }

    void CachePivotTransforms()
    {
        if (basePivot != null)
        {
            baseBaseRotation = basePivot.localRotation;
            baseBaseScale = basePivot.localScale;
        }
        if (upperPivot != null)
        {
            upperBaseRotation = upperPivot.localRotation;
            upperBaseScale = upperPivot.localScale;
        }
        if (smokePivot != null)
        {
            smokeBaseRotation = smokePivot.localRotation;
            smokeBaseLocalPosition = smokePivot.localPosition;
        }
    }

    public void TakeDamage(float ammount)
    {
        if (isDying || health == null) return;
        health.TakeDamage(ammount);
        ServiceLocator.Get<SoundManager>().Play(SFX.HitChamine);
        _feedbackManager.FeedbackPlayerAreaOnChimneyHit();
        lifebarImage.fillAmount = health.CurrentHealth / health.MaxHealth;
        _feedbackManager.ShakeCamera(cameraShakeIntensity, cameraShakeDuration);

        fireScaleSpring.velocity += hitFlareKick;
        fireSwaySpring.velocity += Random.Range(-35f, 35f);
        float hitDirection = Random.value < 0.5f ? -1f : 1f;
        baseSpring.velocity += hitDirection * 22f;
        upperSpring.velocity += hitDirection * 48f;
        smokeSpring.velocity += hitDirection * 65f;

        StartCoroutine(FlashLifebarRoutine());
    }

    void AnimateFire()
    {
        if (fireVisual == null) return;
        float phase = Time.time * idleSwaySpeed + GetInstanceID() * 0.17f;
        float sway = fireSwaySpring.Update(Mathf.Sin(phase) * idleSwayAngle, Time.deltaTime);
        fireVisual.localRotation = Quaternion.Euler(0f, 0f, sway);

        float breath = Mathf.Sin(phase * 1.37f) * idleBreathAmount;
        Vector2 scale = fireScaleSpring.Update(new Vector2(1f - breath * 0.45f, 1f + breath), Time.deltaTime);
        fireVisual.localScale = new Vector3(fireBaseScale.x * scale.x, fireBaseScale.y * scale.y, fireBaseScale.z);
        AnimateChimney(phase);
    }

    void AnimateChimney(float phase)
    {
       
        float baseAngle = baseSpring.Update(Mathf.Sin(phase * 0.48f) * baseSwayAngle, Time.deltaTime);
        float upperAngle = upperSpring.Update(Mathf.Sin(phase * 0.72f + 0.8f) * upperSwayAngle, Time.deltaTime);
        float smokeAngle = smokeSpring.Update(Mathf.Sin(phase * 1.18f + 1.6f) * smokeSwayAngle, Time.deltaTime);

        if (basePivot != null)
        {
            basePivot.localRotation = baseBaseRotation * Quaternion.Euler(0f, 0f, baseAngle);
            float squash = Mathf.Abs(baseAngle) * 0.003f;
            basePivot.localScale = new Vector3(baseBaseScale.x * (1f + squash), baseBaseScale.y * (1f - squash), baseBaseScale.z);
        }
        if (upperPivot != null)
        {
            upperPivot.localRotation = upperBaseRotation * Quaternion.Euler(0f, 0f, upperAngle);
            float stretch = Mathf.Abs(upperAngle) * 0.006f;
            upperPivot.localScale = new Vector3(upperBaseScale.x * (1f - stretch), upperBaseScale.y * (1f + stretch), upperBaseScale.z);
        }
        if (smokePivot != null)
        {
            smokePivot.localRotation = smokeBaseRotation * Quaternion.Euler(0f, 0f, smokeAngle);
            float floatY = Mathf.Sin(phase * 1.64f) * smokeFloatHeight;
            float driftX = Mathf.Sin(phase * 0.93f + 0.4f) * smokeFloatHeight * 0.35f;
            smokePivot.localPosition = smokeBaseLocalPosition + new Vector3(driftX, floatY, 0f);
        }
    }

    IEnumerator FlashLifebarRoutine()
    {
        if (lifebarTargetImage == null) yield break;
        Color original = lifebarTargetImage.color;
        lifebarTargetImage.color = lifebarHitTint;
        yield return new WaitForSeconds(lifebarTintTime);
        float t = 0f;
        while (t < 0.12f)
        {
            t += Time.deltaTime;
            lifebarTargetImage.color = Color.Lerp(lifebarHitTint, original, t / 0.12f);
            yield return null;
        }
        lifebarTargetImage.color = original;
    }

    void Die()
    {
        if (isDying) return;
        isDying = true;
        _gameManager.IncreasePollution(-_gameManager.NormalFurnacePollution);
        fireGameObject.transform.DOScale(0, fireDieDuration).SetEase(fireDieEase).OnComplete(() => Destroy(fireGameObject));
        OnChimneyDestroyed?.Invoke();
    }
   
}
