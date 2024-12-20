using AC.GameTool.Core;
using AC.GameTool.SaveData;
using AC.GameTool.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class LoadSceneController : MonoBehaviour
{
    [SerializeField] float _timeLoad;
    [SerializeField] AssetReference _gameplaySceneRef;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadGame()
    {
        yield return null;

        LoadingUI loadUI = UIManager.Instance.TryShowUI(UITypeName.Loading) as LoadingUI;                        
        loadUI.StartLoading(_timeLoad, 0f, 1f, () =>
        {

            AsyncOperationHandle<SceneInstance> loadSceneHandle = Addressables.LoadSceneAsync(_gameplaySceneRef, UnityEngine.SceneManagement.LoadSceneMode.Single, true);
            loadSceneHandle.Completed += handle =>
            {
                loadUI.OnCloseUI();
            };       
        }, UIManager.Instance.CheckLoadCompleted, SaveManager.Instance.CheckLoadCompleted);

    }
}
