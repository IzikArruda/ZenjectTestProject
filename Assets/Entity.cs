﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/*
 * A entity is anything that collides in the game.
 * 
 * Although this script is a monobehaviour, I am ignoring the build-in
 * unity scripts so i can use the tick interface.
 */
public class Entity : MonoBehaviour {
    public Vector3 center { get; set; }
    public bool player;

    public void UpdateFromInput() {
        /*
         * Update this entity's current position relative to the user input
         */
        float vert = Input.GetAxis("Vertical");
        float hori = Input.GetAxis("Horizontal");
        float movementSpeed = 0.1f;
        Vector3 movement = new Vector3(hori, 0, vert)*movementSpeed;

        NewPosition(center + movement);
    }

    public bool CollidedWithEntities(List<Entity> entities) {
        /*
         * Given a list of entities, run a collision check with 
         * this entitiy and each of the ones in the list
         */
        bool collided = false;
        
        for(int i = 0; i < entities.Count && !collided; i++) {
            if(CollidedWithEntity(entities[i])) {
                collided = true;
            }
        }

        return collided;
    }

    public bool CollidedWithEntity(Entity entity) {
        /*
         * Check if the given entity has collided with this entity
         *
         * All entities are circle and have a radius of 1.
         */
        bool collided = false;
        float entRad = 1;

        if((center - entity.center).magnitude <= (2*(entRad/2f))) {
            collided = true;
        }

        return collided;
    }

    public void BecomePlayer() {
        /*
         * Change this entity into a player
         */

        gameObject.name = "Stray";
        player = true;
    }

    public void BecomeStray() {
        /*
         * Change this entity into a stray
         */

        gameObject.name = "Player";
        player = false;
    }

    public void NewPosition(Vector3 pos) {
        /*
         * Move the entity to the given position
         */

        center = pos;
        gameObject.transform.position = center;
    }

    public class Factory : PlaceholderFactory<Entity> {
    }
}
