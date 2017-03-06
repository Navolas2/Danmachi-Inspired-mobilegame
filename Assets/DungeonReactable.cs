using System;

public interface DungeonReactable
{
	string object_type();

	void SpaceReact (DungeonUnit du);

	void SoundReact (DungeonReactable sound_source);

	int threat_sound {
		get;
	}

	int sound_volume {
		get;
	}
}


