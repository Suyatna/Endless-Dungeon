﻿using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int health;
    public int sceneIndexLoad;
    public float[] position;

    public PlayerData(Player player)
    {
        health = player.mHealth;
        sceneIndexLoad = player.saveSceneIndex;
        
        position = new float[3];
        
        var transform = player.transform;
        var position1 = transform.position;
        
        position[0] = position1.x;
        position[1] = position1.y;
        position[2] = position1.z;
    }
}
