using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;

[System.Serializable]
public class PlayerData
{
    public int coins { get; set; }

    public int medals { get; set; }

    public int[] levels { get; set; }

    public System.DateTime timeStamp { get; set; }

    public string hash { get; set; }
}

/// <summary>
/// Class for saving player data in order to keep the player progress. Data is loaded and saved
/// </summary>
/// 
public class Serializer
{
    static PlayerData playerData;
    /// <summary>
    /// Saves the max level of each category, coins, medals, and challenge button timestamp into a txt
    /// in a certain path
    /// </summary>
    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.txt");
        playerData = new PlayerData();
        playerData.coins = GameManager.Instance.GetCoins();
        playerData.medals = GameManager.Instance.GetMedals();
        playerData.levels = new int[GameManager.Instance.GetCategories() - 1];
        playerData.timeStamp = new System.DateTime();
        for (int i = 0; i < GameManager.Instance.GetCategories() - 1; i++)
        {
            playerData.levels[i] = GameManager.Instance.GetMaxLevelFromCategory((GameManager.LEVEL_CATEGORY)i + 1);
        }
        if(GameManager.Instance.GetPressTimeStamp() != null)
        {
            playerData.timeStamp = GameManager.Instance.GetPressTimeStamp();
        }
        
        //Serialize with default hash
        playerData.hash = "";

        bf.Serialize(file, playerData);

        //Creates a hash for the player
        playerData.hash = GetStringSha256Hash(file.ToString());

        //For security, save again with the hash generated
        bf.Serialize(file, playerData);


        file.Close();
    }

    /// <summary>
    /// If exists a savedata file, get saved data from it and tells GameManager to update its values
    /// with the read ones.
    /// </summary>
    public static void Load()
    {
        //File.Delete(Application.persistentDataPath + "/savedGames.txt");
        if (File.Exists(Application.persistentDataPath + "/savedGames.txt"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.txt", FileMode.Open);
            playerData = (PlayerData)bf.Deserialize(file);
            //Creates a security playerdata, that will save the player data
            PlayerData securityPlayerData = playerData;

            //Creates the default hash
            securityPlayerData.hash = "";

            //Serialize all data again for the security player and sets its new hash.
            bf.Serialize(file, securityPlayerData);
            securityPlayerData.hash = GetStringSha256Hash(file.ToString());


            file.Close();

            //if data has been changed (player hash and security player hash are not the same),
            //we restart all the player progress
            if(playerData.hash != securityPlayerData.hash)
            {
                playerData.coins = 0;
                playerData.medals = 0;
                playerData.levels = new int[GameManager.Instance.GetCategories() - 1];
                playerData.timeStamp = new System.DateTime();
            }
            
            for (int i = 0; i < GameManager.Instance.GetCategories() - 1; i++)
            {
                int max = (int)playerData.levels[i];
                GameManager.Instance.SetMaxLevelFromCategory((GameManager.LEVEL_CATEGORY)i+1, max);
            }

            int coins = playerData.coins;
            int medals = playerData.medals;
            System.DateTime timeStamp = playerData.timeStamp;


            GameManager.Instance.AddCoins(coins);
            GameManager.Instance.AddMedals(medals);
            GameManager.Instance.SetPressTimeStamp(timeStamp);
        }
    }

    //https://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
    internal static string GetStringSha256Hash(string text)
    {
        if (System.String.IsNullOrEmpty(text))
            return System.String.Empty;

        using (var sha = new System.Security.Cryptography.SHA256Managed())
        {
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            return System.BitConverter.ToString(hash).Replace("-", System.String.Empty);
        }
    }
}
