using UnityEngine;
[System.Serializable]
public class HeadAndHands
{
    [SerializeField] GameObject head, handR, handL;
    public GameObject Head => head;
    public GameObject HandR => handR;
    public GameObject HandL => handL;

}
public class PersonClothesRandomizer : MonoBehaviour
{
    [SerializeField] Transform happyFacesCollection, angryFaceCollection, torsoCollection;
    [SerializeField] HeadAndHands[] headAndHands;

    void Awake()
    {
      
        for(int i = 0; i < happyFacesCollection.childCount; i++)
        {
            happyFacesCollection.GetChild(i).gameObject.SetActive(false);
        }
        happyFacesCollection.GetChild(Random.Range(0, happyFacesCollection.childCount -1)).gameObject.SetActive(true);


        for(int i = 0; i < angryFaceCollection.childCount; i++)
        {
            angryFaceCollection.GetChild(i).gameObject.SetActive(false);
        }
        angryFaceCollection.GetChild(Random.Range(0, angryFaceCollection.childCount -1)).gameObject.SetActive(true);
        
        for(int i = 0; i < torsoCollection.childCount; i++)
        {
            torsoCollection.GetChild(i).gameObject.SetActive(false);
        }
        torsoCollection.GetChild(Random.Range(0, torsoCollection.childCount -1)).gameObject.SetActive(true);
   
        foreach(var item in headAndHands)
        {
            item.Head.SetActive(false);
            item.HandR.SetActive(false);
            item.HandL.SetActive(false);
        }
        var randomIndex = Random.Range(0, headAndHands.Length);
        headAndHands[randomIndex].Head.SetActive(true);
        headAndHands[randomIndex].HandR.SetActive(true);
        headAndHands[randomIndex].HandL.SetActive(true);
   
   }
}
