namespace NRKernal.ThirdView
{
    using System.IO;
    using NRKernal.NRExamples;
    using UnityEngine;
    using UnityEngine.UI;

    public class ConfigView : MonoBehaviour
    {
        public delegate void ConfigViewCallBack(ThirdViewConfig config);
        public event ConfigViewCallBack OnConfigrationChanged;
        public GameObject m_Root;
        public UserDefineButton m_HideBtn;
        public InputField m_IPField;
        public Toggle m_UseDebug;

        private ThirdViewConfig currentConfig;

        public static string ConfigPath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, "ThirdViewConfig.json");
            }
        }

        void Awake()
        {
            m_IPField.onValueChanged.AddListener((value) =>
            {
                if (!value.Equals(currentConfig.serverIP))
                {
                    currentConfig.serverIP = value;
                    UpdateConfig();
                }

            });
            m_UseDebug.onValueChanged.AddListener((value) =>
            {
                if (value != currentConfig.useDebugUI)
                {
                    currentConfig.useDebugUI = value;
                    UpdateConfig();
                }
            });

            m_HideBtn.OnClick += OnHideBtnClick;
            m_Root.SetActive(false);

            this.LoadConfig();
        }

        private void OnHideBtnClick(string obj)
        {
            m_Root.SetActive(!m_Root.activeInHierarchy);
        }

        private void LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                Debug.Log("File is not exist :" + ConfigPath);
                var thirdView = GameObject.FindObjectOfType<NRThirdViewBehaviour>();
                currentConfig = new ThirdViewConfig(thirdView.serverIP);
            }
            else
            {
                Debug.Log("Load config from:" + ConfigPath);
                currentConfig = LitJson.JsonMapper.ToObject<ThirdViewConfig>(File.ReadAllText(ConfigPath));
            }
            m_IPField.text = currentConfig.serverIP;
            m_UseDebug.isOn = currentConfig.useDebugUI;
            OnConfigrationChanged?.Invoke(currentConfig);
        }

        // Config will Works at next run time.
        private void UpdateConfig()
        {
            var json = LitJson.JsonMapper.ToJson(currentConfig);
            File.WriteAllText(ConfigPath, json);
            Debug.Log("Save config :" + json);
            OnConfigrationChanged?.Invoke(currentConfig);
        }
    }
}
