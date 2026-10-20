// ============================================================
// Title:       PlayerStats
// Version:     1.1
// Written by:  Ashar
// Description: Survival: hunger, thirst, stamina, health and death.
// ============================================================

using UnityEngine;
using UnityEngine.SceneManagement;

// Goes on the Player. Runs survival: hunger/thirst drain, damage when empty,
// stamina refill when not sprinting, death.
public class PlayerStats : MonoBehaviour
{
    [Header("Survival (per second)")]
    public float hungerDrain = 0.5f;
    public float thirstDrain = 0.8f;
    public float starveDamage = 2f;   // health lost while hunger is 0 (and again while thirst is 0)
    public float staminaRegen = 15f;
    public float sprintCost = 20f;

    [Header("Live profile (read/edit while playing)")]
    [SerializeField] private PlayerProfile profile;

    public PlayerProfile Profile
    {
        get { return profile; }
    }

    public bool IsSprinting { get; set; }

    void Awake()
    {
        // Same object as ProfileManager.Profile, shown here so you can watch it in the Inspector
        profile = ProfileManager.Profile;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        profile.Hunger -= hungerDrain * dt;
        profile.Thirst -= thirstDrain * dt;

        if (profile.Hunger <= 0f)
            profile.Health -= starveDamage * dt;
        if (profile.Thirst <= 0f)
            profile.Health -= starveDamage * dt;

        if (IsSprinting)
            profile.Stamina -= sprintCost * dt;
        else
            profile.Stamina += staminaRegen * dt;

        if (profile.Health <= 0f)
            Die();
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
