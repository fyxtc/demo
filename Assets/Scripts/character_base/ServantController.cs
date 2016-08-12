using UnityEngine;
using System.Collections;

public class ServantController : BaseController {
	// 这里好玩的一点是：是在start里面重写SummonHandler=null比较好还是这里重写animdeadcomplete为空比较好
	// 都不好，直接在PlayerTroopController里面SetController直接忽略最好
}
