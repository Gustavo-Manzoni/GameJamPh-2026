using UnityEngine;
using DG.Tweening;
public class CutsceneImagesManager : MonoBehaviour
{
    CanvasGroup[] cutsceneImages;
    [SerializeField] float fadeInTime;
    [SerializeField] float fadeOutTime;
    int currentIndex;
    public void Next()
    {
        cutsceneImages[currentIndex].DOFade(0f, fadeOutTime).OnComplete(() => {
          
        });
          currentIndex++;
            if (currentIndex >= cutsceneImages.Length)
            {
                gameObject.SetActive(false);
                return;
            }
            cutsceneImages[currentIndex].DOFade(1f, fadeInTime);
    }
}
