using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum GameStates {
    Starting,
    Playing
}

public class GameController : ITickable {
    readonly EntityManager _entityManager;
    GameStates state = GameStates.Starting;

    public GameController(EntityManager entityManager) {
        _entityManager = entityManager;
    }

    public void Tick() {
        /*
         * Update the game on every tick relative to the current state
         */

        switch(state) {
            case GameStates.Starting:
                UpdateStarting();
                break;
			case GameStates.Playing:
				UpdatePlaying();
				break;
        }
    }

    void UpdateStarting() {
        /*
         * Restart the game
         */

        _entityManager.StartNewGame();
        state = GameStates.Playing;
    }

    void UpdatePlaying() {
        /*
         * Pressing the left-mouse button will reset the game
         */

        if(Input.GetMouseButton(0)) {
            state = GameStates.Playing;
        }
    }
}
