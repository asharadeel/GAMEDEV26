// ============================================================
// Title:       ProfileSelectUI
// Version:     1.0
// Written by:  Ashar
// Description: The Main Menu's profile select. Shows the 3 profile
//              cards ("Empty slot" or name + creation date). Empty
//              card: opens New Profile (name, optional passkey,
//              ball colour). Filled card: plays, asking for the
//              passkey first if it has one. Each filled card has
//              Edit (name + colour) and Delete (confirm pop-up, then
//              the passkey too if it has one).
//              Goes on the ProfileSelect container.
// ============================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// One profile card in the list
[System.Serializable]
public class ProfileCard
{
    public Button button;         // the card itself
    public Text label;
    public Button editButton;     // only shown when the slot has a profile
    public Button deleteButton;
}

public class ProfileSelectUI : MonoBehaviour
{
    public string playScene = "Hub";
    public ProfileCard[] cards = new ProfileCard[ProfileManager.MaxProfiles];

    [Header("New Profile window")]
    public GameObject newProfileWindow;
    public InputField nameInput;
    public Toggle passkeyToggle;
    public GameObject[] passkeyFields;   // shown only while the toggle is on
    public InputField passkeyInput;
    public Button[] newColours;          // swatches, the colour is the swatch's own colour
    public Button createButton;
    public Text newError;

    [Header("Passkey pop-up (log in, or confirm a delete)")]
    public GameObject passkeyPopup;
    public Text passkeyTitle;
    public InputField passkeyPopupInput;
    public Button passkeyEnterButton;
    public Text passkeyEnterLabel;
    public Text passkeyMessage;

    [Header("Delete pop-up")]
    public GameObject deletePopup;
    public Text deleteTitle;
    public Button deleteConfirmButton;

    [Header("Edit pop-up")]
    public GameObject editPopup;
    public InputField editNameInput;
    public Button[] editColours;
    public Button editSaveButton;
    public Text editMessage;

    private int selectedSlot;     // the card being created/played/edited/deleted
    private bool deleting;        // passkey pop-up: true = deleting, false = logging in
    private Color newColour;
    private Color editColour;

    void Start()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            int slot = i;   // copy, so each button remembers its own slot
            cards[i].button.onClick.AddListener(() => ClickCard(slot));
            cards[i].editButton.onClick.AddListener(() => OpenEdit(slot));
            cards[i].deleteButton.onClick.AddListener(() => OpenDelete(slot));
        }

        foreach (Button swatch in newColours)
        {
            Button s = swatch;
            s.onClick.AddListener(() => newColour = PickColour(newColours, s));
        }
        foreach (Button swatch in editColours)
        {
            Button s = swatch;
            s.onClick.AddListener(() => editColour = PickColour(editColours, s));
        }

        passkeyToggle.onValueChanged.AddListener(ShowPasskeyFields);
        createButton.onClick.AddListener(Create);
        passkeyEnterButton.onClick.AddListener(EnterPasskey);
        deleteConfirmButton.onClick.AddListener(ConfirmDelete);
        editSaveButton.onClick.AddListener(SaveEdit);

        Refresh();
    }

    // Updates every card from the save files
    void Refresh()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            PlayerProfile p = ProfileManager.PeekSlot(i);
            bool filled = p != null;

            if (filled)
            {
                string locked = p.PasswordProtected ? " (passkey)" : "";
                cards[i].label.text = p.PlayerName + locked + "\nCreated " + p.CreatedAt;
            }
            else
            {
                cards[i].label.text = "Empty slot";
            }
            cards[i].editButton.gameObject.SetActive(filled);
            cards[i].deleteButton.gameObject.SetActive(filled);
        }
    }

    void ClickCard(int slot)
    {
        selectedSlot = slot;
        PlayerProfile p = ProfileManager.PeekSlot(slot);

        if (p == null)
            OpenNewProfile();
        else if (p.PasswordProtected)
            OpenPasskey(false, p);
        else
            Play(slot);
    }

    void Play(int slot)
    {
        ProfileManager.LoadSlot(slot);
        SceneManager.LoadScene(playScene);
    }

    // ---- New profile ----

    void OpenNewProfile()
    {
        nameInput.text = "";
        passkeyInput.text = "";
        passkeyToggle.isOn = false;
        ShowPasskeyFields(false);
        newColour = PickColour(newColours, newColours[0]);
        newError.text = "";
        newProfileWindow.SetActive(true);
    }

    void ShowPasskeyFields(bool on)
    {
        foreach (GameObject field in passkeyFields)
            field.SetActive(on);
    }

    void Create()
    {
        string playerName = nameInput.text.Trim();
        if (playerName == "")
        {
            newError.text = "Enter a name.";
            return;
        }
        if (passkeyToggle.isOn && !PlayerProfile.IsValidPasskey(passkeyInput.text))
        {
            newError.text = "Passkey must be 3-30 characters.";
            return;
        }

        ProfileManager.CreateProfile(selectedSlot, playerName, passkeyToggle.isOn, passkeyInput.text, newColour);
        newProfileWindow.SetActive(false);
        Refresh();
    }

    // ---- Passkey ----

    void OpenPasskey(bool forDelete, PlayerProfile p)
    {
        deleting = forDelete;
        passkeyTitle.text = deleting ? "Passkey to delete " + p.PlayerName : "Username: " + p.PlayerName;
        passkeyEnterLabel.text = deleting ? "Delete" : "Enter";
        passkeyPopupInput.text = "";
        passkeyMessage.text = "";
        passkeyPopup.SetActive(true);
    }

    void EnterPasskey()
    {
        PlayerProfile p = ProfileManager.PeekSlot(selectedSlot);
        if (p == null)
            return;

        if (!p.CheckPasskey(passkeyPopupInput.text))
        {
            passkeyMessage.text = "Wrong passkey.";
            return;
        }

        passkeyPopup.SetActive(false);
        if (deleting)
            DeleteSelected();
        else
            Play(selectedSlot);
    }

    // ---- Delete ----

    void OpenDelete(int slot)
    {
        selectedSlot = slot;
        PlayerProfile p = ProfileManager.PeekSlot(slot);
        if (p == null)
            return;

        deleteTitle.text = "Delete " + p.PlayerName + "?";
        deletePopup.SetActive(true);
    }

    // "Delete" on the confirm pop-up: asks for the passkey too if there is one
    void ConfirmDelete()
    {
        deletePopup.SetActive(false);
        PlayerProfile p = ProfileManager.PeekSlot(selectedSlot);
        if (p == null)
            return;

        if (p.PasswordProtected)
            OpenPasskey(true, p);
        else
            DeleteSelected();
    }

    void DeleteSelected()
    {
        ProfileManager.DeleteSlot(selectedSlot);
        Refresh();
    }

    // ---- Edit ----

    void OpenEdit(int slot)
    {
        selectedSlot = slot;
        PlayerProfile p = ProfileManager.PeekSlot(slot);
        if (p == null)
            return;

        editNameInput.text = p.PlayerName;
        editColour = p.BallColour;
        foreach (Button swatch in editColours)
            swatch.transform.localScale = swatch.image.color == editColour ? Vector3.one * 1.2f : Vector3.one;
        editMessage.text = "";
        editPopup.SetActive(true);
    }

    void SaveEdit()
    {
        string playerName = editNameInput.text.Trim();
        if (playerName == "")
        {
            editMessage.text = "Enter a name.";
            return;
        }

        ProfileManager.EditProfile(selectedSlot, playerName, editColour);
        editPopup.SetActive(false);
        Refresh();
    }

    // Makes the picked swatch bigger than the rest and returns its colour
    Color PickColour(Button[] swatches, Button picked)
    {
        foreach (Button swatch in swatches)
            swatch.transform.localScale = swatch == picked ? Vector3.one * 1.2f : Vector3.one;
        return picked.image.color;
    }
}
