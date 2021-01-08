using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameScript : MonoBehaviour
{

    public string sceneName;
    public Button loadSceneBtn;
    // Start is called before the first frame update
    void Start()
    {
        loadSceneBtn.onClick.AddListener(ChangeScene);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            SceneManager.LoadScene(1);
        }
    }
    
     void ChangeScene() 
    {
        SceneManager.LoadSceneAsync(sceneName);
    
    }

    
}
