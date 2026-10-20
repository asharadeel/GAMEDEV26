// ============================================================
// Title:       PlayerStats
// Version:     1.3
// Written by:  Ashar
// Description: Survival: hunger, thirst, stamina, health and death.
// ============================================================

using UnityEngine;
using UnityEngine.SceneManagement;

// Goes on the Player. All stats go 0-100.
// Hunger and thirst drop by 1 every few seconds. While either is at or below
// the low level, health drops by 1 every few seconds. Sprinting drains stamina,
// not sprinting refills it. Health 0 = death.
public class PlayerStats : MonoBehaviour
{
    [Header("Hunger / Thirst")]
    public float hungerDropEvery = 5f;   // seconds per -1 hunger
    public float thirstDropEvery = 5f;   // seconds per -1 thirst
    public float lowLevel = 25f;         // at or below this, health starts dropping

    [Header("Health")]
    public float healthDropEvery = 8f;   // seconds per -1 health while hungry/thirsty

    [Header("Stamina (per second)")]
    public float sprintCost = 20f;
    public float staminaRegen = 15f;

    [Header("Live profile (read/edit while playing)")]
    [SerializeField] private PlayerProfile profile;

    public PlayerProfile Profile
    {
        get { return profile; }
    }

    public bool IsSprinting { get; set; }

    private float hungerTimer;
    private float thirstTimer;
    private float healthTimer;

    void Awake()
    {
        // Same object as ProfileManager.Profile, shown here so you can watch it in the Inspector
        profile = ProfileManager.Profile;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (Tick(ref hungerTimer, hungerDropEvery))
            profile.Hunger -= 1f;
        if (Tick(ref thirstTimer, thirstDropEvery))
            profile.Thirst -= 1f;

        bool low = profile.Hunger <= lowLevel || profile.Thirst <= lowLevel;
        if (!low)
            healthTimer = 0f; // restart the countdown once fed/watered again
        else if (Tick(ref healthTimer, healthDropEvery))
            profile.Health -= 1f;

        if (IsSprinting)
            profile.Stamina -= sprintCost * dt;
        else
            profile.Stamina += staminaRegen * dt;

        if (profile.Health <= 0f)
            Die();
    }

    // Adds time to a timer. True (and resets) each time it passes the interval.
    bool Tick(ref float timer, float interval)
    {
        timer += Time.deltaTime;
        if (timer < interval)
            return false;

        timer -= interval;
        return true;
    }

    // Called by mobs and enemy projectiles (via Damage.Hit / SendMessage)
    public void TakeDamage(float amount)
    {
        profile.Health -= amount;
    }

    public void Consume(ConsumableData item)
    {
        profile.Hunger += item.hunger;
        profile.Thirst += item.thirst;
        profile.Stamina += item.stamina;
        profile.Health += item.health;
    }

    void Die()
    {
        Debug.Log("Player died");
        profile.RefillAll();
        ProfileManager.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Save when leaving the scene too, not just when the game closes
    void OnDestroy()
    {
        ProfileManager.Save();
    }

    // Right-click the component header in the Inspector to use these
    [ContextMenu("Save Now")]
    void SaveNow()
    {
        ProfileManager.Save();
    }

    [ContextMenu("Delete Save")]
    void DeleteSave()
    {
        ProfileManager.Delete();
        profile = ProfileManager.Profile;
    }
}
