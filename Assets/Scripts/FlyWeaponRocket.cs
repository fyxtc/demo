using UnityEngine;
using System.Collections;

public class FlyWeaponRocket : BaseFlyWeapon 
{
	private new float speed = 10.0f;
	public override float GetSpeed(){
		return this.speed;
	}
}
