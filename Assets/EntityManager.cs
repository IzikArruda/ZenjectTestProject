using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EntityManager : ITickable, IFixedTickable {
    Entity.Factory _entityFactory;
    List<Entity> createdEntitties = new List<Entity>();
	readonly SignalBus _signalBus;
    
    public readonly AudioClip _entityStickClip;


    #region Startup Functions

    public EntityManager(
			Entity.Factory entityFactory,
			SignalBus signalBus,
            AudioClip entityStickClip) {
        _entityFactory = entityFactory;
		_signalBus = signalBus;
        _entityStickClip = entityStickClip;
    }
    
    void ResetEntities() {
        /*
         * Remove all the entities from the game
         */
        foreach(var entity in createdEntitties) {
            GameObject.Destroy(entity.gameObject);
        }

        createdEntitties.Clear();
    }

    public void StartNewGame() {
        /*
         * Start a new game by spawning one player and three strays
         */

        ResetEntities();
        NewPlayer();
        NewStray();
        NewStray();
        NewStray();
    }

    #endregion


    #region Helper Functions

    List<Entity> GetAllPlayers() {
        /*
         * Return a list of all player entities
         */
        List<Entity> players = new List<Entity>();

        foreach(Entity entitiy in createdEntitties) {
            if(entitiy.player) {
                players.Add(entitiy);
            }
        }

        return players;
    }

    List<Entity> GetAllStrays() {
        /*
         * Return a list of all non-player entities
         */
        List<Entity> strays = new List<Entity>();

        foreach(Entity entitiy in createdEntitties) {
            if(!entitiy.player) {
                strays.Add(entitiy);
            }
        }

        return strays;
    }

    int StrayCount() {
        return GetAllStrays().Count;
    }
    
    #endregion


    #region Tick Updates

    public void Tick() {
        StrayPlayerCollisions();
        
        if(StrayCount() < 3) {
            NewStray();
        }
    }

    public void FixedTick() {
        /*
         * Update the player's position from user inputs
         */
        List<Entity> players = GetAllPlayers();
        
        foreach(Entity player in players) {
            player.UpdateFromInput();
        }
    }

    public void StrayPlayerCollisions() {
        /*
         * Check if any players collided with any strays. The check is done by having
         * the strays check if they have collided with any players.
         * 
         * When a stray collides with a player, have the stray become a player
         * and fire a signal to play a sound clip of the entity connecting.
         * 
         * A problem that will occur is that when a stray becomes a player, it won't 
         * check to see if it's connected to another stray on this tick.
         */
        List<Entity> players = GetAllPlayers();
        List<Entity> strays = GetAllStrays();

        foreach(Entity stray in strays) {
            if(stray.CollidedWithEntities(players)) {
                stray.BecomePlayer();
                _signalBus.Fire(new EntitySoundSignal(_entityStickClip));
            }
        }
    }

    #endregion


    #region Entity Creation

    public Entity NewEntity() {
        /*
         * Create a new entity using the factory, add it to the list, then return it.
         * All new entities are not players by default and start at (0, 0, 0)
         */
        Entity newEntity = _entityFactory.Create();
        createdEntitties.Add(newEntity);
        newEntity.player = false;
        newEntity.NewPosition(new Vector3(0, 0, 0));
        
        return newEntity;
    }

    public void NewStray() {
        /*
         * Create a new stray interractable entity in a random position
         */
        Entity newStray = NewEntity();
        float randX = Random.Range(-10.0f, 10.0f);
        float randZ = Random.Range(-10.0f, 10.0f);

        newStray.BecomeStray();
        newStray.NewPosition(new Vector3(randX, 0, randZ));
    }

    public void NewPlayer() {
        /*
         * Create a new player interractable entity at the center of the world
         */
        Entity newPlayer = NewEntity();
        newPlayer.player = true;

        newPlayer.BecomePlayer();
        newPlayer.NewPosition(new Vector3(0, 0, 0));
    }

    #endregion
}
