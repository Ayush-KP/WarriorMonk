using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.Mathematics;

[System.Serializable]
public struct SaveData
{
    public static SaveData Instance;

    //map stuff
    public HashSet<string> sceneNames;
    
    //Bench stuff
    public string benchSceneName;
    public Vector2 benchPos;

    //player stuff
    public int playerHealth;
    public int playerHeartShards;
    public float playerMana;
    public int playerManaOrb;
    public int playerOrbShard;
    public float playerOrb0Fill, playerOrb1Fill, playerOrb2Fill;
    public bool playerHalfMana;
    public Vector2 playerPosition;
    public string lastScene;
    public bool playerUnlockedWallJump;
    public bool playerUnlockedDash;
    public bool playerUnlockedSideCast;
    public bool playerUnlockedUpCast;

    //enemies stuff
    //shade
    public Vector2 shadePos;
    public string sceneWithShade;
    public Quaternion shadeRot;

    public void Initialize()
    {
        if(!File.Exists(Application.persistentDataPath + "/save.bench.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.bench.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.shade.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.shade.data"));
        }
        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }

    public void SaveBench()
    {
        using(BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.bench.data")))
        {
            writer.Write(benchSceneName);
            writer.Write(benchPos.x);
            writer.Write(benchPos.y);
            Debug.Log("Bench data saved: " + benchSceneName + " at position " + benchPos);
        }
    }
    public void LoadBench()
    {
        string path = Application.persistentDataPath + "/save.bench.data";
        if (File.Exists(path))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                try
                {
                    benchSceneName = reader.ReadString();
                    benchPos.x = reader.ReadSingle();
                    benchPos.y = reader.ReadSingle();
                }
                catch (EndOfStreamException e)
                {
                    Debug.LogError("Error reading save file: " + e.Message);
                }
            }
            Debug.Log("Bench data loaded: " + benchSceneName + " at position " + benchPos);
        }
        else
        {
            Debug.LogWarning("Save file does not exist.");
        }
    }
    
    public void SavePlayerData()
    {
        string path = Application.persistentDataPath + "/save.player.data";
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(path)))
        {
            playerHealth = PlayerController.Instance.Health;
            writer.Write(playerHealth);
            playerHeartShards = PlayerController.Instance.heartShards;
            writer.Write(playerHeartShards);
            playerMana = PlayerController.Instance.Mana;
            writer.Write(playerMana);
            playerHalfMana = PlayerController.Instance.halfMana;
            writer.Write(playerHalfMana);
            playerManaOrb = PlayerController.Instance.manaOrbs;
            writer.Write(playerManaOrb);
            playerOrbShard = PlayerController.Instance.orbShards;
            writer.Write(playerOrbShard);
            playerOrb0Fill = PlayerController.Instance.manaOrbHandler.orbFills[0].fillAmount;
            writer.Write(playerOrb0Fill);
            playerOrb1Fill = PlayerController.Instance.manaOrbHandler.orbFills[1].fillAmount;
            writer.Write(playerOrb1Fill);
            playerOrb2Fill = PlayerController.Instance.manaOrbHandler.orbFills[2].fillAmount;
            writer.Write(playerOrb2Fill);
            playerUnlockedWallJump = PlayerController.Instance.unlockedWallJump;
            writer.Write(playerUnlockedWallJump);
            playerUnlockedDash = PlayerController.Instance.unlockedDash;
            writer.Write(playerUnlockedDash);
            playerUnlockedSideCast = PlayerController.Instance.unlockedSideCast;
            writer.Write(playerUnlockedSideCast); 
            playerUnlockedUpCast = PlayerController.Instance.unlockedUpCast;
            writer.Write(playerUnlockedUpCast);

            playerPosition = PlayerController.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);
            Debug.Log("position: " + playerPosition);

            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);
        }
       // Debug.Log("Player data saved");
       // Debug.Log("y position:"+ )
    }

    public void LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/save.player.data";
        if (File.Exists(path))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                playerHealth = reader.ReadInt32();
                playerHeartShards = reader.ReadInt32();
                playerMana = reader.ReadSingle();
                playerHalfMana = reader.ReadBoolean();
                playerManaOrb = reader.ReadInt32();
                playerOrbShard = reader.ReadInt32();
                playerOrb0Fill = reader.ReadSingle();
                playerOrb1Fill = reader.ReadSingle();
                playerOrb2Fill = reader.ReadSingle();
                playerUnlockedWallJump = reader.ReadBoolean();
                playerUnlockedDash = reader.ReadBoolean();
                playerUnlockedSideCast = reader.ReadBoolean();
                playerUnlockedUpCast = reader.ReadBoolean();
                playerPosition.x = reader.ReadSingle();
                playerPosition.y = reader.ReadSingle();
                lastScene = reader.ReadString();

                SceneManager.LoadScene(lastScene);
                PlayerController.Instance.transform.position = playerPosition;
                PlayerController.Instance.Health = playerHealth;
                PlayerController.Instance.heartShards = playerHeartShards;
                PlayerController.Instance.Mana = playerMana;
                PlayerController.Instance.halfMana = playerHalfMana;
                PlayerController.Instance.manaOrbs = playerManaOrb;
                PlayerController.Instance.orbShards = playerOrbShard;
                PlayerController.Instance.manaOrbHandler.orbFills[0].fillAmount = playerOrb0Fill;
                PlayerController.Instance.manaOrbHandler.orbFills[1].fillAmount = playerOrb1Fill;
                PlayerController.Instance.manaOrbHandler.orbFills[2].fillAmount = playerOrb2Fill;
                PlayerController.Instance.unlockedWallJump = playerUnlockedWallJump;
                PlayerController.Instance.unlockedDash = playerUnlockedDash;
                PlayerController.Instance.unlockedSideCast = playerUnlockedSideCast;
                PlayerController.Instance.unlockedUpCast = playerUnlockedUpCast;
            }
        }
        else
        {
            Debug.Log("File deosn't exist");
            PlayerController.Instance.halfMana = false;
            PlayerController.Instance.health = PlayerController.Instance.maxHealth;
            PlayerController.Instance.heartShards = 0;
            PlayerController.Instance.Mana = 0.5f;
            PlayerController.Instance.unlockedWallJump = false;
            PlayerController.Instance.unlockedDash = false;
        }
    }
    
    public void SaveShadeData()
    {
        string path = Application.persistentDataPath + "/save.shade.data";
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(path)))
        {
            sceneWithShade = SceneManager.GetActiveScene().name;
            shadePos = Shade.Instance.transform.position;
            shadeRot = Shade.Instance.transform.rotation;

            writer.Write(sceneWithShade);
            writer.Write(shadePos.x);
            writer.Write(shadePos.y);
            writer.Write(shadeRot.x);
            writer.Write(shadeRot.y);
            writer.Write(shadeRot.z);
            writer.Write(shadeRot.w);
        }
    }

    public void LoadShadeData()
    {
        string path = Application.persistentDataPath + "/save.shade.data";
        if (File.Exists(path))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                sceneWithShade = reader.ReadString();
                shadePos.x = reader.ReadSingle();
                shadePos.y = reader.ReadSingle();

                float rotationX = reader.ReadSingle();
                float rotationY = reader.ReadSingle();
                float rotationZ = reader.ReadSingle();
                float rotationW = reader.ReadSingle();

                shadeRot = new Quaternion(rotationX, rotationY, rotationZ, rotationW);
            }
        }
        else
        {
            Debug.Log("File deosn't exist");
        }
    }
}
//public void LoadBench()
//{
//    if(File.Exists(Application.persistentDataPath + "/save.bench.data"))
//    {
//        using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.bench.data")))
//        {
//            benchSceneName = reader.ReadString();
//            benchPos.x = reader.ReadSingle();
//            benchPos.y = reader.ReadSingle();
//        }
//    }
//}