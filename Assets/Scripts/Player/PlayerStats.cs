// ============================================================
// Title:       PlayerStats
// Version:     1.5
// Written by:  Ashar
// Updated by:  Davud Jabiyev (v1.4: counts eating and drinking for achievements)
//              Ashar (v1.5: armour takes damage before health)
// Description: Survival: hunger, thirst, stamina, health and death.
// ============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//goes on the Player. all stats go 0-100
public class PlayerStats : MonoBehaviour
{
    [Header("Hunger / Thirst")]
    public float hungerDropEvery = 5f;   //seconds per -1
    public float thirstDropEvery = 5f;   //seconds per -1
    public float lowLevel = 25f;         //at or below this, health drops

    [Header("Health")]
    public float healthDropEvery = 8f;   //seconds per -1 while hungry/thirsty

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

    //timers
    private float hungerTimer;
    private float thirstTimer;
    private float healthTimer;
    private PlayerArmor armor;

    //STARTING PROCEDURES
    void Awake()
    {
        profile = ProfileManager.Profile;   //same object, shown for the Inspector

        armor = GetComponent<PlayerArmor>();
        if (armor == null)
            armor = gameObject.AddComponent<PlayerArmor>();
    }

    //SURVIVAL
    void Update()
    {
        float dt = Time.deltaTime;

        if (Tick(ref hungerTimer, hungerDropEvery))
            profile.Hunger -= 1f;
        if (Tick(ref thirstTimer, thirstDropEvery))
            profile.Thirst -= 1f;

        bool low = profile.Hunger <= lowLevel || profile.Thirst <= lowLevel;
        if (!low)
            healthTimer = 0f; //restart once fed/watered
        else if (Tick(ref healthTimer, healthDropEvery))
            profile.Health -= 1f;

        if (IsSprinting)
            profile.Stamina -= sprintCost * dt;
        else
            profile.Stamina += staminaRegen * dt;

        if (profile.Health <= 0f)
            Die();
    }

    //true (and resets) each time it passes the interval
    bool Tick(ref float timer, float interval)
    {
        timer += Time.deltaTime;
        if (timer < interval)
            return false;

        timer -= interval;
        return true;
    }

    //DAMAGE AND ITEMS
    //called via Damage.Hit / SendMessage
    public void TakeDamage(float amount)
    {
        amount = armor.Absorb(amount);   //shield first, the rest hits health
        profile.Health -= amount;
    }

    public void Consume(ConsumableData item)
    {
        profile.Hunger += item.hunger;
        profile.Thirst += item.thirst;
        profile.Stamina += item.stamina;
        profile.Health += item.health;

        //achievements
        AchievementManager.Add(AchievementStat.ItemsConsumed, 1);
        if (item.thirst > 0)
            AchievementManager.Add(AchievementStat.DrinksConsumed, 1);
        if (item.hunger > 0)
            AchievementManager.Add(AchievementStat.FoodEaten, 1);
    }

    //DEATH AND SAVING
    void Die()
    {
        Debug.Log("Player died");
        profile.RefillAll();
        ProfileManager.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //save when leaving the scene too
    void OnDestroy()
    {
        ProfileManager.Save();
    }

    //TESTING
    [ContextMenu("Save Now")]
    void SaveNow()
    {
        ProfileManager.Save();
        Debug.Log("Saved the profile.");
    }

    [ContextMenu("Delete Save")]
    void DeleteSave()
    {
        ProfileManager.Delete();
        profile = ProfileManager.Profile;
        Debug.Log("Deleted the save.");
    }
}
