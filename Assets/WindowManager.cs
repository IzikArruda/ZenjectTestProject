using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/*
 * Manage the playing field by creating a square sprite for the play area
 */
public class WindowManager : ITickable {

    public SpriteRenderer background;
    public GameSize _gameSize;

    public WindowManager(GameSize gameSize, GameObject backgroundPrefab, Color backgroundColor) {
        _gameSize = gameSize;
        backgroundPrefab.name = "Background";
        background = backgroundPrefab.GetComponent<SpriteRenderer>();
        background.color = backgroundColor;
        background.transform.localScale = new Vector3(gameSize.width, gameSize.height, 1);
        background.transform.position = new Vector3(gameSize.width/2f, -1, gameSize.height/2f);
    }
    
    public void Tick() {

    }

    [Serializable]
    public class GameSize {
        public float width;
        public float height;
    }
}
