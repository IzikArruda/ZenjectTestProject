using UnityEngine;
using Zenject;

public class TestInstaller : MonoInstaller<TestInstaller>
{
    public GameObject entityPrefab;

    public override void InstallBindings() 
    {
        /*
         * This function is overriden by zenject and is used
         * as the IOC container used to resolve any bindings in the game
         */
         
        /* Create an interractable factory. Use the entity prefab when creating new entities. */
        Container.BindFactory<Entity, Entity.Factory>()
            .FromComponentInNewPrefab(entityPrefab)
            .WithGameObjectName("Entity")
            .UnderTransformGroup("Entities");

        /* Install an entity manager and game controller along with 
         * the ITickable interface so that the Tick() functions are called. */
        Container.BindInterfacesAndSelfTo<EntityManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();
    }
}