#pragma warning disable 0649

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Watermelon
{
    [DefaultExecutionOrder(-999)]
    [HelpURL("https://docs.google.com/document/d/1ORNWkFMZ5_Cc-BUgu9Ds1DjMjR4ozMCyr6p_GGdyCZk")]
    public class Initialiser : MonoBehaviour
    {
        // Static instance - đảm bảo chỉ có một instance tồn tại
        public static Initialiser Instance { get; private set; }

        [SerializeField] ProjectInitSettings initSettings;
        [SerializeField] Canvas systemCanvas;

        [Space]
        [SerializeField] ScreenSettings screenSettings;

        public static Canvas SystemCanvas;
        public static GameObject InitialiserGameObject;

        public static bool IsInititalized { get; private set; }
        public static bool IsStartInitialized { get; private set; }
        public static ProjectInitSettings InitSettings { get; private set; }

        public void Awake()
        {
            // Kiểm tra nếu instance đã tồn tại
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Another instance of Initialiser already exists. Destroying this one.");
                Destroy(gameObject); // Hủy gameObject này nếu đã có instance khác
                return;
            }

            // Gán instance và không hủy nó khi load scene khác
            Instance = this;

            screenSettings.Initialise();

            if (!IsInititalized)
            {
                IsInititalized = true;

                InitSettings = initSettings;
                SystemCanvas = systemCanvas;
                InitialiserGameObject = gameObject;

                DontDestroyOnLoad(gameObject);

                initSettings.Initialise(this);
            }
        }

        public void Start()
        {
            Initialise(true);
        }

        public void Initialise(bool loadingScene)
        {
            if (!IsStartInitialized)
            {
                // Initialise components
                initSettings.StartInit(this);

                // Create audio listener
                //AudioController.CreateAudioListener();

                IsStartInitialized = true;

                if (loadingScene)
                {
                    GameLoading.LoadGameScene();
                }
                else
                {
                    GameLoading.SimpleLoad();
                }
            }
        }

        public static bool IsModuleInitialised(Type moduleType)
        {
            ProjectInitSettings projectInitSettings = InitSettings;

            InitModule[] coreModules = null;
            InitModule[] initModules = null;

#if UNITY_EDITOR
            if (!IsInititalized)
            {
                projectInitSettings = RuntimeEditorUtils.GetAssetByName<ProjectInitSettings>();
            }
#endif

            if (projectInitSettings != null)
            {
                coreModules = projectInitSettings.CoreModules;
                initModules = projectInitSettings.InitModules;
            }

            for (int i = 0; i < coreModules.Length; i++)
            {
                if (coreModules[i].GetType() == moduleType)
                {
                    return true;
                }
            }

            for (int i = 0; i < initModules.Length; i++)
            {
                if (initModules[i].GetType() == moduleType)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDestroy()
        {
            IsInititalized = false;

#if UNITY_EDITOR
            SaveController.ForceSave();
#endif
        }

        private void OnApplicationFocus(bool focus)
        {
#if !UNITY_EDITOR
            if(!focus) SaveController.Save();
#endif
        }
    }
}

// -----------------
// Initialiser v 0.4.2
// -----------------

// Changelog
// v 0.4.2
// • Added loading scene logic
// v 0.4.1
// • Fixed error on module remove
// v 0.3.1
// • Added link to the documentation
// • Initializer renamed to Initialiser
// • Fixed problem with recompilation
// v 0.2
// • Added sorting feature
// • Initialiser MonoBehaviour will destroy after initialization
// v 0.1
// • Added basic version
