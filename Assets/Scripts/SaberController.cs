using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SaberController : BaseController {
	protected override void InitModel(){
		Model = ConfigManager.share().CharacterConfig.Saber;;
		// Debug.Log(model.Life);
	}
}
