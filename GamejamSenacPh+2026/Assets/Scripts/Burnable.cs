using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Burnable : MonoBehaviour
{
    [SerializeField] ParticleSystem burnEmbers;
    [SerializeField] float tickInterval = 0.25f;

    Coroutine burnRoutine;

    public void StartBurn(float duration, float dps)
    {
        if (burnRoutine != null) StopCoroutine(burnRoutine);
        if (burnEmbers != null) burnEmbers.Play();
        burnRoutine = StartCoroutine(BurnRoutine(duration, dps));
    }

    public void StopBurn()
    {
        if (burnRoutine != null) StopCoroutine(burnRoutine);
        burnRoutine = null;
        if (burnEmbers != null) burnEmbers.Stop();
    }

    IEnumerator BurnRoutine(float duration, float dps)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float dmg = dps * tickInterval;
            ApplyDamage(dmg);
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }
        StopBurn();
    }

    void ApplyDamage(float dmg)
    {
        var fireable = GetComponent<IFireable>();
        if (fireable != null)
        {
            fireable.TakeDamage(dmg);
            return;
        }
        var health = GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(dmg);
        }
    }
}
