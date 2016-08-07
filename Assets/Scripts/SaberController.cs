using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SaberController : BaseController {
	protected override void InitModel(){
		Model = ConfigManager.share().CharacterConfig.GetModel(TroopType.TROOP_SABER);;
		// Debug.Log(model.Life);
	}

    protected override void InitTrickController(){
		TrickController = new TrickController (Model.Tricks);
    }
}
