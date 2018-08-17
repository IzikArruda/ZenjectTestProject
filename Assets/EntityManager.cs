using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class EntityManager : ITickable, IFixedTickable {
    Entity.Factory _entityFactory;
    List<Entity> createdEntitties = new List<Entity>();
    readonly SignalBus _signalBus;
    readonly EntityColors _entityColors;
    readonly AudioClip _entityStickClip;
    readonly WindowManager.GameSize _gameSize;


    #region Startup Functions

    public EntityManager(
            Entity.Factory entityFactory,
            SignalBus signalBus,
            AudioClip entityStickClip,
            EntityColors entityColors,
            WindowManager.GameSize gameSize) {
        _entityFactory = entityFactory;
        _signalBus = signalBus;
        _entityStickClip = entityStickClip;
        _entityColors = entityColors;
        _gameSize = gameSize;
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

    List<Entity> GetAllControlled() {
        /*
         * Return a list of all player-controlled entities
         */
        List<Entity> controlled = new List<Entity>();

        foreach(Entity entitiy in createdEntitties) {
            if(entitiy.IsControlled()) {
                controlled.Add(entitiy);
            }
        }

        return controlled;
    }

    Entity GetPlayer(List<Entity> entities) {
        /*
         * Return the first encountered player entity from the given list of entities
         */
        Entity player = null;

        for(int i = 0; i < entities.Count && player == null; i++) {
            if(entities[i].currentType == EntityType.Player) {
                player = entities[i];
            }
        }

        return player;
    }

    List<Entity> GetAllStrays() {
        /*
         * Return a list of all non-controlled entities
         */
        List<Entity> strays = new List<Entity>();

        foreach(Entity entitiy in createdEntitties) {
            if(!entitiy.IsControlled()) {
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
        EntityCollisions();

        if(StrayCount() < 3) {
            NewStray();
        }
    }

    public void FixedTick() {
        /*
         * Update the player's position from user inputs. 
         * 
         * First move the player entity then clamp their position within the game area.
         */
        List<Entity> controllables = GetAllControlled();
        Entity player = GetPlayer(controllables);



        if(player != null) {
            Vector3 previousPosition = player.center;

            /* Move the player using user input */
            player.UpdateFromInput();
            float curX = player.center.x;
            float curZ = player.center.z;
            //Clamp the player within the game bounderies
            if(curX < 0) {
                curX = 0;
            }
            else if(curX > _gameSize.width) {
                curX = _gameSize.width;
            }
            if(curZ < 0) {
                curZ = 0;
            }
            else if(curZ > _gameSize.height) {
                curZ = _gameSize.height;
            }
            Vector3 currentPositon = new Vector3(curX, player.center.y, curZ);
            player.NewPosition(currentPositon);
            //Get the actual movement
            Vector3 actualMovement = currentPositon - previousPosition;
            Debug.Log(actualMovement);


            /* Apply the movement to the controllable entities (except for the player) */
            foreach(Entity controllable in controllables) {
                if(controllable.currentType != EntityType.Player) {
                    controllable.NewPosition(controllable.center + actualMovement);
                }
            }
        }

    }

    public void EntityCollisions() {
        /*
         * Check if any controlled entities collided with any strays. The check is done by having
         * the strays check if they have collided with any controllables.
         * 
         * When a stray collides with a controllable, have the stray become a controllable
         * and fire a signal to play a sound clip of the entity connecting.
         * 
         * A problem that will occur is that when a stray becomes a controllable, it won't 
         * check to see if it's connected to another stray on this tick.
         */
        List<Entity> players = GetAllControlled();
        List<Entity> strays = GetAllStrays();

        foreach(Entity stray in strays) {
            if(stray.CollidedWithEntities(players)) {
                ChangeEntityState(stray, EntityType.Attached);
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
        newEntity.NewPosition(new Vector3(0, 0, 0));

        return newEntity;
    }

    public void ChangeEntityState(Entity entity, EntityType newType) {
        /*
         * Change the EntityType of the given entity to the given newType
         */

        switch(newType) {
            case EntityType.Player:
                entity.BecomePlayer(_entityColors.player);
                break;
            case EntityType.Stray:
                entity.BecomeStray(_entityColors.stray);
                break;
            case EntityType.Attached:
                entity.BecomeAttached(_entityColors.attached);
                break;
            default:
                Debug.Log("WARNING: Changing an entity into an unhandeled state");
                break;
        }
    }

    public void NewStray() {
        /*
         * Create a new stray interractable entity in a random position
         */
        Entity newStray = NewEntity();
        float randX = Random.Range(0, _gameSize.width);
        float randZ = Random.Range(0, _gameSize.height);

        ChangeEntityState(newStray, EntityType.Stray);
        newStray.NewPosition(new Vector3(randX, 0, randZ));
    }

    public void NewPlayer() {
        /*
         * Create a new player interractable entity at the center of the world
         */
        Entity newPlayer = NewEntity();

        ChangeEntityState(newPlayer, EntityType.Player);
        newPlayer.NewPosition(new Vector3(_gameSize.width/2f, 0, _gameSize.height / 2f));
    }

    #endregion

    [Serializable]
    public class EntityColors {
        public Color player;
        public Color stray;
        public Color attached;
        public Color background;
    }
}
