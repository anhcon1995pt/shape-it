
using AC.GameTool.Attribute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Base
{
    public class AutoFitCameraInScreen : MonoBehaviour
    {
        [SerializeField] Camera _mainCam;
        [SerializeField] Camera[] _camOverlays;
        [ReadOnlly]
        [SerializeField] Vector3 _camPosition;
        [SerializeField] Vector2 _contentSize;
        [SerializeField] CameraPivot _pivot;      
        [ReadOnlly]
        [SerializeField] Vector2 _screenSize;
        [ReadOnlly]
        [SerializeField] float _contentRatio;
        [ReadOnlly]
        [SerializeField] float _screenRatio;
        [ReadOnlly]
        [SerializeField] float _cameraSize;
        private void Awake()
        {
            _screenSize = new Vector2(Screen.width, Screen.height);
            _contentRatio = _contentSize.x / _contentSize.y;
            _screenRatio = _screenSize.x / _screenSize.y;
            _camPosition = _mainCam.transform.position;
        }
        // Start is called before the first frame update
        void Start()
        {
            ChangeCameraSize();
            ScreenEventManager.Instance.OnScreenChange += ScreenResolutionChange;
        }

        private void OnDestroy()
        {
            ScreenEventManager.Instance.OnScreenChange -= ScreenResolutionChange;
        }
        // Update is called once per frame
        void Update()
        {

        }

        public void ChangeCameraSize()
        {
            float height = _contentSize.x / _screenRatio;
            _cameraSize = Mathf.Clamp(height / 2f, 0.5f, 20f);
            _mainCam.orthographicSize = _cameraSize;
            for(int i=0; i< _camOverlays.Length; i++)
            {
                _camOverlays[i].orthographicSize = _cameraSize;
            }
            //_mainCam.transform.position = _camPosition + pivotPos;
        }

        void ScreenResolutionChange(Vector2Int screenSize)
        {
            _screenSize = screenSize;
            _screenRatio = _screenSize.x / _screenSize.y;
            ChangeCameraSize();
        }
        enum CameraPivot
        {
            Top,
            Center,
            Bottom
        }
    }

}