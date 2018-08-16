using System;
using UnityEngine;
using Zenject;

public class TestInstaller : MonoInstaller<TestInstaller>
{
    [Inject]    //We get the settings from the TestInstallerSettings as it gives us a TestInstaller.Settings bind
    Settings _settings = null;
    //[Inject]
    //EntityManager.EntityColors _entityColors = null;

    public override void InstallBindings() 
    {
        /*
         * This function is overriden by zenject and is used
         * as the IOC container used to resolve any bindings in the game
         */
         
        /* Create an interractable factory. Use the entity prefab when creating new entities. */
        Container.BindFactory<Entity, Entity.Factory>()
            .FromComponentInNewPrefab(_settings.entityPrefab)
            .WithGameObjectName("Entity")
            .UnderTransformGroup("Entities");

        /* Install an entity manager and game controller along with 
         * the ITickable interface so that the Tick() functions are called. */
        Container.BindInterfacesAndSelfTo<EntityManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();
        //Add arguments to the entitymanager creation
        Container.BindInstance(_settings.entityStickClip).WhenInjectedInto<EntityManager>();
        //Container.BindInstance(_entityColors).WhenInjectedInto<EntityManager>();


        /* Create the binding for the sound manager */
        Container.Bind<SoundManager>()
            .FromComponentInNewPrefab(_settings.audioPlayerPrefab)
            .WithGameObjectName("Sound Manager")
            .AsSingle().NonLazy();

        /* Install a signal and subscribe the sound manager to catch any signales sent */
        SignalBusInstaller.Install(Container);
		Container.DeclareSignal<EntitySoundSignal>();
        //Binding the SoundManager prevents the need of having it subscribe to this signal
        Container.BindSignal<EntitySoundSignal>().ToMethod<SoundManager>(x => x.RecievedSignal).FromResolve();
    }


    [Serializable]
    public class Settings {
        public GameObject entityPrefab;
        public GameObject audioPlayerPrefab;
        public AudioClip entityStickClip;
    }
}