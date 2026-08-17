using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Code
{
    [SerializeField] public List<KeyCode> sequence;
    public UnityEngine.Events.UnityEvent onComplete;
}

public class KonamiCode : MonoBehaviour
{
    [SerializeField] private Code[] allCodes;
    [SerializeField] float timeToResetSequence;
   [SerializeField] float timer;
    int currentIndex = 0;
    List<int> allIndexes = new List<int>();
    void Start()
    {
        timer = timeToResetSequence;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            allIndexes.Clear();    
            currentIndex = 0;
            timer = timeToResetSequence;
        }
        foreach (KeyCode a in System.Enum.GetValues(typeof(KeyCode)))
        {
           
            if (Input.GetKeyDown(a))
              {
               
                 timer = timeToResetSequence;
                if (currentIndex == 0)
                {
                    foreach (Code c in allCodes)
                    {

                        if (c.sequence[currentIndex] == a)
                        {
                           
                            int pos = System.Array.IndexOf(allCodes, c);
                            allIndexes.Add(pos);
                         
                        }


                    }
                       currentIndex++;
                    if (allIndexes.Count == 0)
                    {

                        allIndexes.Clear();
                        currentIndex = 0;
                        timer = timeToResetSequence;
                    }
                }
                else
                {

                    for (int i = 0; i < allCodes.Length; i++)
                    {
                        
                        if (allIndexes.Contains(i))
                        {

                            if (allCodes[i].sequence[currentIndex] != a)
                            {
                                allIndexes.Remove(i);

                            }
                            else
                            {

                                if (currentIndex == allCodes[i].sequence.Count - 1)
                                {
                                
                                    currentIndex = 0;
                                    allIndexes.Clear();
                                    allCodes[i].onComplete?.Invoke();
                                    return;
                                }
                                 currentIndex++;

                            }


                        }





                    }


                }

             }




        }



    }


}
