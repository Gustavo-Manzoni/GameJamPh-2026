using UnityEngine;

public class SoundOnStar : MonoBehaviour
{
  
    void Start()
    {
        ServiceLocator.Get<SoundManager>().Play(SFX.JogoComecar);
    }

   
}
