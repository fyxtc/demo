using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrickController{
    // 因为models其实很少的，没必要再区分什么type的trick了，直接遍历找即可
    protected int[] ids;
    protected List<int> skillIds = new List<int>();
    protected List<int> statusIds = new List<int>();
    protected List<int> naturalIds = new List<int>();
    protected TrickConfig trickConfig = ConfigManager.share().TrickConfig;
    public TrickController(int[] ids){
        this.ids = ids;
        SplitModels();
    }

    protected void SplitModels(){
        foreach(int id in ids){
            TrickType type = trickConfig.GetModel(id).Type;
            switch(type){
            case TrickType.TRICK_SKILL:
                skillIds.Add(id);
                break;
            case TrickType.TRICK_STATUS:
                statusIds.Add(id);
                break;
            case TrickType.TRICK_NATURAL:
                naturalIds.Add(id);
                break;
            default:
                Debug.Assert(false, "error trick model type");
                break;
            }
        }
    }

    // protected abstract List<int> GetCurrentTrick();

    public List<int> GetSkillTrick(SkillType skilling){
        List<int> res = new List<int>();
        TrickModel m;
        foreach(int id in skillIds){
            m = trickConfig.GetModel(id);
            if(m.Skill == skilling && canTrigger(m)){
                res.Add(id);
            }
        }
        // else 就返回空了
        return res;
    }

    public List<int> OnStatusTrick(TrickStatusType statusing, bool isStart){
        List<int> res = new List<int>();
        if(isStart){
            TrickModel m;
            foreach(int id in statusIds){
                m = trickConfig.GetModel(id);
                if(m.Status == statusing && canTrigger(m)){
                    res.Add(id);
                }
            }
        }
        return res;
    }

    public List<int> OnNaturalTrick(bool isStart){
        List<int> res = new List<int>();
        if(isStart){
            TrickModel m;
            foreach(int id in naturalIds){
                m = trickConfig.GetModel(id);
                if(canTrigger(m)){
                    res.Add(id);
                }
            }
        }
        return res;
    }

    protected bool canTrigger(TrickModel model){
        return Random.value <= model.Rate;
    }
}
