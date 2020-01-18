using NRKernal.Record;
using UnityEngine;
using System.Collections;
using NRKernal.Persistence;
using System.IO;
using NRKernal.ThirdView.NetWork;

namespace NRKernal.ThirdView
{
    public class NRThirdViewBehaviour : SingletonBehaviour<NRThirdViewBehaviour>
    {
        private NetWorkClient netWorkClient;
        private NRThirdViewCapture viewCapture;
        private ImageTrackingAnchorTool anchorTool;
        [SerializeField]
        private Transform m_CameraRoot;
        [SerializeField]
        private GameObject m_ConfigPanelPrefab;
        [SerializeField]
        private GameObject m_DebugInfo;
        private ConfigView configView;

        public string serverIP = "192.168.0.1";
        private ThirdViewConfig currentConfig;
        private float limitWaittingTime = 5f;

        private string rtpPath
        {
            get
            {
                return string.Format("rtp://{0}:5555", serverIP);
            }
        }

        public void Start()
        {
            // if CameraRig has a parent, move to it as a child.
            if (NRSessionManager.Instance.NRSessionBehaviour != null)
            {
                var headRoot = NRSessionManager.Instance.NRSessionBehaviour.transform.parent;
                if (headRoot != null)
                {
                    transform.parent = headRoot;
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
            }

            // if config file is exist, use it.
            if (File.Exists(ConfigView.ConfigPath))
            {
                currentConfig = LitJson.JsonMapper.ToObject<ThirdViewConfig>(File.ReadAllText(ConfigView.ConfigPath));
                serverIP = currentConfig.serverIP;
            }
            else
            {
                currentConfig = new ThirdViewConfig(serverIP);
            }
            m_DebugInfo.SetActive(currentConfig.useDebugUI);

            StartCoroutine(RegistConfigPanel());

            if (gameObject.GetComponent<NetWorkClient>() == null)
            {
                netWorkClient = gameObject.AddComponent<NetWorkClient>();
            }
            netWorkClient.OnDisconnect += OnDisconnect;
            netWorkClient.OnConnect += OnConnected;
            netWorkClient.OnJoinRoomResult += OnJoinRoomResult;
            netWorkClient.OnCameraParamUpdate += OnCameraParamUpdate;
            netWorkClient.Connect(currentConfig.serverIP, currentConfig.port);

            viewCapture = gameObject.GetComponent<NRThirdViewCapture>();
            anchorTool = gameObject.GetComponent<ImageTrackingAnchorTool>();
            if (anchorTool != null)
            {
                anchorTool.OnAnchorPoseUpdate += OnTrackableImageUpdate;
                anchorTool.SwitchTrackableImage(true);
            }

            Invoke("TimeOut", limitWaittingTime);
        }

        #region Net msg
        private void OnConnected()
        {
            Debug.Log("OnConnected...");
            Invoke("CheckServerAvailable", 0.5f);
        }

        private void OnDisconnect()
        {
            Debug.Log("OnDisconnect...");
            this.Close();
        }

        private void OnCameraParamUpdate(CameraParam param)
        {
            viewCapture.UpdateCameraParam(param.fov);
        }

        private void OnJoinRoomResult(bool result)
        {
            if (result)
            {
                Debug.Log("Server is ok...");
                ReadyToStartCapture();
            }
            else
            {
                Debug.Log("Server is not ok, close myself...");
                this.Close();
            }
        }
        #endregion

        // Oprate time out.
        private void TimeOut()
        {
            if (!viewCapture.isPlaying)
            {
                this.Close();
            }
        }

        private void CheckServerAvailable()
        {
            netWorkClient.EnterRoomRequest();
        }

        private IEnumerator RegistConfigPanel()
        {
            while (GameObject.FindObjectOfType<NRVirtualDisplayer>() == null)
            {
                yield return new WaitForEndOfFrame();
            }
            var virtualdisplayer = GameObject.FindObjectOfType<NRVirtualDisplayer>();
            var root = virtualdisplayer.transform.GetComponentInChildren<Canvas>().transform;
            configView = Instantiate(m_ConfigPanelPrefab, root).GetComponent<ConfigView>();
            configView.OnConfigrationChanged += OnConfigrationChanged;
        }

        private void OnConfigrationChanged(ThirdViewConfig config)
        {
            m_DebugInfo.SetActive(config.useDebugUI);
        }

        private void OnTrackableImageUpdate(Pose pose)
        {
            //Debug.Log("Update camera pose:" + pose.ToString());
            // Update camera pos
            m_CameraRoot.localPosition = pose.position;
            m_CameraRoot.localRotation = pose.rotation;
        }

        private void ReadyToStartCapture()
        {
            var config = new NativeEncodeConfig(currentConfig.width, currentConfig.height, 4096000, currentConfig.rate, CodecType.Rtp, rtpPath, true);
            Debug.Log("ReadyToStartCapture:" + config.ToString());
            viewCapture.Init(config);
            viewCapture.StartRecord();
        }

        private void Close()
        {
            netWorkClient?.Disconect();
            viewCapture?.StopRecord();

            if (gameObject != null)
            {
                Destroy(gameObject);
            }
            if (configView != null)
            {
                Destroy(configView.gameObject);
            }
        }
    }
}
