using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EntitySoundSignal {
	readonly AudioClip _clip;

	public EntitySoundSignal(AudioClip clip) {
		_clip = clip;
	}

	public AudioClip GetClip() {
		return _clip;
	}
}
