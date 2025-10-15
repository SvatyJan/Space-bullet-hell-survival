using UnityEngine;

public static class SelectedShip
{
    private const string Key = "selected_ship";

    public static void Save(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        PlayerPrefs.SetString(Key, id);
        PlayerPrefs.Save();
    }

    public static string LoadOrDefault(string fallbackId = "")
    {
        return PlayerPrefs.GetString(Key, fallbackId);
    }
}
