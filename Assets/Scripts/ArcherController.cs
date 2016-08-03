using UnityEngine;
using System.Collections;
using Spine.Unity;

public class ArcherController : BaseController {
	protected override void InitModel(){
		Model = ConfigManager.share().getCharacterConfig().Archer;
		// Debug.Log(model.Life);
	}}

