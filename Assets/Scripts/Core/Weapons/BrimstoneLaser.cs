using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrimstoneLaser : MonoBehaviour, IWeapon
{
    [Header("Refs")]
    [SerializeField] private Transform firePoint;

    [Header("Tiled Beam")]
    [SerializeField] private SpriteRenderer segmentPrefab;
    [SerializeField] private Transform segmentParent;
    [SerializeField] private SpriteRenderer endCapPrefab;

    [Header("FX Prefabs")]
    [SerializeField] private ParticleSystem chargeSparkPrefab;
    [SerializeField] private ParticleSystem chargeGlowPrefab;
    [SerializeField] private ParticleSystem chargeCooldownPrefab;

    [Header("Timings")]
    [SerializeField] private float chargeTime = 1.5f;
    [SerializeField] private float fireDuration = 1.0f;
    [SerializeField] private float cooldownTime = 1.0f;
    [SerializeField] private float tickInterval = 0.1f;

    [Header("Damage & Range")]
    [SerializeField] private float damagePerSecond = 100f;
    [SerializeField] private int baseRange = 2;

    [Header("Layers")]
    [SerializeField] private LayerMask environmentMask;
    [SerializeField] private LayerMask enemyMask;

    [Header("Audio")]
    [SerializeField] private AudioClip chargeSfx;
    [SerializeField] private AudioClip fireSfx;

    private readonly List<SpriteRenderer> pool = new List<SpriteRenderer>();
    private ParticleSystem chargeSpark;
    private ParticleSystem chargeGlow;
    private ParticleSystem chargeCooldown;
    private AudioSource audioSrc;
    private Coroutine cycleCo;
    private SpaceEntity owner;
    private ShipStats stats;
    private SpriteRenderer endCapInstance;

    private const float SEGMENT_UNIT = 1f;

    private enum State { Idle, Charging, Firing, Cooldown }
    private State state = State.Idle;

    private void Awake()
    {
        if (!firePoint) firePoint = transform;
        if (!segmentParent) segmentParent = transform;

        owner = GetComponentInParent<SpaceEntity>();
        stats = owner ? owner.GetComponent<ShipStats>() : null;

        audioSrc = GetComponent<AudioSource>();
        if (!audioSrc) audioSrc = gameObject.AddComponent<AudioSource>();

        if (endCapPrefab)
        {
            endCapInstance = Instantiate(endCapPrefab, segmentParent);
            endCapInstance.enabled = false;
        }

        if (chargeSparkPrefab)
        {
            chargeSpark = Instantiate(chargeSparkPrefab, firePoint);
            chargeSpark.gameObject.SetActive(false);
        }
        if (chargeGlowPrefab)
        {
            chargeGlow = Instantiate(chargeGlowPrefab, segmentParent);
            chargeGlow.gameObject.SetActive(false);
        }
        if (chargeCooldownPrefab)
        {
            chargeCooldown = Instantiate(chargeCooldownPrefab, firePoint);
            chargeCooldown.gameObject.SetActive(false);
        }

        HideSegments();
        if (endCapInstance) endCapInstance.enabled = false;
    }

    public void Fire()
    {
        if (cycleCo == null && state == State.Idle)
            cycleCo = StartCoroutine(Cycle());
    }

    public void Upgrade()
    {
        baseRange += 1;
        damagePerSecond += 20f;
    }

    public void Evolve()
    {
        baseRange += 2;
        damagePerSecond += 60f;
    }

    private IEnumerator Cycle()
    {
        state = State.Charging;
        FXPlay(chargeSpark);
        FXStop(chargeCooldown);
        if (audioSrc && chargeSfx) audioSrc.PlayOneShot(chargeSfx);

        float t = 0f;
        while (t < chargeTime) { t += Time.deltaTime; yield return null; }

        state = State.Firing;
        FXStop(chargeSpark);
        FXPlay(chargeGlow);
        if (audioSrc && fireSfx) audioSrc.PlayOneShot(fireSfx);

        float fireT = 0f;
        float tickT = 0f;
        while (fireT < fireDuration)
        {
            fireT += Time.deltaTime;
            tickT += Time.deltaTime;

            int maxByEnv = ComputeMaxSegmentsByEnvironment();
            int segmentCount = Mathf.Clamp(baseRange, 0, maxByEnv);

            LayoutSegments(segmentCount);

            while (tickT >= tickInterval)
            {
                TickDamage(segmentCount);
                tickT -= tickInterval;
            }

            yield return null;
        }

        HideSegments();
        if (endCapInstance) endCapInstance.enabled = false;
        FXStop(chargeGlow);

        state = State.Cooldown;
        FXPlay(chargeCooldown);
        yield return new WaitForSeconds(cooldownTime);
        FXStop(chargeCooldown);

        state = State.Idle;
        cycleCo = null;
    }

    private int ComputeMaxSegmentsByEnvironment()
    {
        Vector2 origin = firePoint.position;
        Vector2 dir = firePoint.up;
        float angle = firePoint.eulerAngles.z;

        RaycastHit2D hit = Physics2D.BoxCast(origin, new Vector2(0.9f, 0.1f), angle, dir, baseRange * SEGMENT_UNIT, environmentMask);
        if (hit.collider)
        {
            float d = Mathf.Max(0f, hit.distance);
            return Mathf.FloorToInt(d / SEGMENT_UNIT);
        }
        return baseRange;
    }

    private void EnsurePool(int count)
    {
        while (pool.Count < count)
        {
            var seg = Instantiate(segmentPrefab, segmentParent);
            seg.enabled = false;
            seg.transform.localScale = Vector3.one;
            pool.Add(seg);
        }
    }

    private void LayoutSegments(int count)
    {
        EnsurePool(count);

        segmentParent.position = firePoint.position;
        segmentParent.rotation = firePoint.rotation;

        for (int i = 0; i < pool.Count; i++)
            pool[i].enabled = i < count;

        for (int i = 0; i < count; i++)
        {
            float y = (i + 0.5f) * SEGMENT_UNIT;
            var seg = pool[i];
            seg.transform.localPosition = new Vector3(0f, y, 0f);
            seg.transform.localRotation = Quaternion.identity;
        }

        if (endCapInstance)
        {
            endCapInstance.enabled = count > 0;
            if (count > 0)
            {
                float endY = (count * SEGMENT_UNIT) - (0.5f * SEGMENT_UNIT);
                endCapInstance.transform.localPosition = new Vector3(0f, endY, 0f);
                endCapInstance.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void HideSegments()
    {
        for (int i = 0; i < pool.Count; i++)
            pool[i].enabled = false;
    }

    private void TickDamage(int segmentCount)
    {
        if (segmentCount <= 0) return;

        Vector2 origin = firePoint.position;
        Vector2 dir = firePoint.up;
        float angle = firePoint.eulerAngles.z;

        float length = segmentCount * SEGMENT_UNIT;
        Vector2 center = origin + dir * (length * 0.5f);
        Vector2 size = new Vector2(0.9f, length);

        Collider2D[] hits = new Collider2D[32];
        int n = Physics2D.OverlapBoxNonAlloc(center, size, angle, hits, enemyMask);

        float dmg = (damagePerSecond + (stats ? stats.BaseDamage : 0f)) * tickInterval;

        for (int i = 0; i < n; i++)
        {
            var se = hits[i].GetComponent<SpaceEntity>();
            if (se != null) se.TakeDamage(dmg);
        }
    }

    private static void FXPlay(ParticleSystem ps)
    {
        if (!ps) return;
        ps.gameObject.SetActive(true);
        ps.Clear(true);
        ps.Play(true);
    }

    private static void FXStop(ParticleSystem ps)
    {
        if (!ps) return;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.gameObject.SetActive(false);
    }
}
