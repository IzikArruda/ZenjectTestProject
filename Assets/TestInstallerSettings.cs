using System;
using UnityEngine;
using Zenject;

/*
 * Create a scriptable object installer that tracks all the values for the game.
 * 
 * Enable the commented line bellow to be able to re-create the script
 */
//[CreateAssetMenu(fileName = "TestInstallerSettings", menuName = "Installers/TestInstallerSettings")]
public class TestInstallerSettings : ScriptableObjectInstaller<TestInstallerSettings>
{
    //Include the settings used in the TestInstaller
    public TestInstaller.Settings testInstallerSettings;
    
    [Serializable]
    public class Prefabs {
        public GameObject entityPrefab;
        public GameObject audioPlayerPrefab;
    }

    [Serializable]
    public class SoundEffects {
        public AudioClip entityStickClip;
    }

    public override void InstallBindings() {
        //Include the testInstaller's settings. The [Inject] call in the testInstaller 
        //is what we are binding this to.
        Container.BindInstance(testInstallerSettings);
    }
}