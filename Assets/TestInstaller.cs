using System;
using UnityEngine;
using Zenject;

public class TestInstaller : MonoInstaller<TestInstaller> {
    /* We get the following variables from the TestInstallerSettings's bindings */
    [Inject]
    TestInstallerSettings.Prefabs _prefabs = null;
    [Inject]
    TestInstallerSettings.SoundEffects _soundEffects = null;
    [Inject]
    WindowManager.GameSize _gameSize = null;
    [Inject]
    EntityManager.EntityColors _entityColors = null;

    public override void InstallBindings() 
    {
        /*
         * This function is overriden by zenject and is used
         * as the IOC container used to resolve any bindings in the game
         */
         
        /* Create an interractable factory. Use the entity prefab when creating new entities. */
        Container.BindFactory<Entity, Entity.Factory>()
            .FromComponentInNewPrefab(_prefabs.entityPrefab)
            .WithGameObjectName("Entity")
            .UnderTransformGroup("Entities");

        /* Install an entity manager and game controller along with 
         * the ITickable interface so that the Tick() functions are called. */
        Container.BindInterfacesAndSelfTo<EntityManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<WindowManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();
        //Add arguments to the manager creation
        Container.BindInstance(_soundEffects.entityStickClip).WhenInjectedInto<EntityManager>();

        /* Create the binding for the sound manager */
        Container.Bind<SoundManager>()
            .FromComponentInNewPrefab(_prefabs.audioPlayerPrefab)
            .WithGameObjectName("Sound Manager")
            .AsSingle().NonLazy();
        

        //Create a new background prefab and pass it into the window manager
        GameObject backgroundObject = Instantiate(_prefabs.backgroundPrefab);
        Container.BindInstance(backgroundObject).WhenInjectedInto<WindowManager>();
        Container.BindInstance(_entityColors.background).WhenInjectedInto<WindowManager>();


        /* Install a signal and subscribe the sound manager to catch any signales sent */
        SignalBusInstaller.Install(Container);
		Container.DeclareSignal<EntitySoundSignal>();
        //Binding the SoundManager prevents the need of having it subscribe to this signal
        Container.BindSignal<EntitySoundSignal>().ToMethod<SoundManager>(x => x.RecievedSignal).FromResolve();
    }
}