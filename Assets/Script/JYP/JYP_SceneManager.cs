using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JYP_SceneManager : MonoBehaviour
{
    public Button createButton;
    
    // Start is called before the first frame update
    void Start()
    {
        
        createButton.onClick.AddListener(
            () =>
            {
                SceneManager.LoadScene("JYP_TotalTestScene");
            }
        );    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
