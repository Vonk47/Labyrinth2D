using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace TestGame.UI
{
    public class MazeSettingsUI : MonoBehaviour
    {
        [Header("Sliders")]
        [SerializeField] private Slider _widthSlider;
        [SerializeField] private Slider _heightSlider;
        [SerializeField] private Slider _exitsSlider;
        [SerializeField] private Slider _distanceSlider;
        [SerializeField] private Slider _passagesSlider;
        [SerializeField] private Slider _roomChanceSlider;
        [SerializeField] private TMP_InputField _seed;

        [Header("Value Labels")]
        [SerializeField] private TextMeshProUGUI _widthLabel;
        [SerializeField] private TextMeshProUGUI _heightLabel;
        [SerializeField] private TextMeshProUGUI _exitsLabel;
        [SerializeField] private TextMeshProUGUI _distanceLabel;
        [SerializeField] private TextMeshProUGUI _passagesLabel;
        [SerializeField] private TextMeshProUGUI _roomChanceLabel;
        [SerializeField] private TextMeshProUGUI _seedLabel;

        [Header("Toggle")]
        [SerializeField] private Toggle _randomSeedToggle;

        private MazeSettings _currentSettings = new();
        private const string SavePath = "maze_settings.json";

        private void Start()
        {
            LoadSettings();
            PopulateUI();
            SetupListeners();
        }

        private void SetupListeners()
        {
            _widthSlider.onValueChanged.AddListener(v => _widthLabel.text = v.ToString("0"));
            _heightSlider.onValueChanged.AddListener(v => _heightLabel.text = v.ToString("0"));
            _exitsSlider.onValueChanged.AddListener(v => _exitsLabel.text = v.ToString("0"));
            _distanceSlider.onValueChanged.AddListener(v => _distanceLabel.text = v.ToString("0"));
            _passagesSlider.onValueChanged.AddListener(v => _passagesLabel.text = v.ToString("0"));
            _roomChanceSlider.onValueChanged.AddListener(v => _roomChanceLabel.text = v.ToString("0"));
        }

        public void ApplySettings()
        {
            _currentSettings.width = (ushort)_widthSlider.value;
            _currentSettings.height = (ushort)_heightSlider.value;
            _currentSettings.numberOfExits = (byte)_exitsSlider.value;
            _currentSettings.minDistanceBetweenExits = (byte)_distanceSlider.value;
            _currentSettings.additionalPassages = (ushort)_passagesSlider.value;
            _currentSettings.roomChance = (ushort)_roomChanceSlider.value;
            _currentSettings.useRandomSeed = _randomSeedToggle.isOn;

            SaveSettings();
        }

        private void PopulateUI()
        {
            _widthSlider.value = _currentSettings.width;
            _heightSlider.value = _currentSettings.height;
            _exitsSlider.value = _currentSettings.numberOfExits;
            _distanceSlider.value = _currentSettings.minDistanceBetweenExits;
            _passagesSlider.value = _currentSettings.additionalPassages;
            _roomChanceSlider.value = _currentSettings.roomChance;
            _randomSeedToggle.isOn = _currentSettings.useRandomSeed;
            _seed.text = _currentSettings.seed.ToString();

            // Trigger labels manually once
            _widthLabel.text = _widthSlider.value.ToString("0");
            _heightLabel.text = _heightSlider.value.ToString("0");
            _exitsLabel.text = _exitsSlider.value.ToString("0");
            _distanceLabel.text = _distanceSlider.value.ToString("0");
            _passagesLabel.text = _passagesSlider.value.ToString("0");
            _roomChanceLabel.text = _roomChanceSlider.value.ToString("0");
        }

        private void SaveSettings()
        {
            SaveSystem.Save<MazeSettings>(_currentSettings, SavePath);
        }

        private void LoadSettings()
        {
            _currentSettings = SaveSystem.Load<MazeSettings>(SavePath);
        }
    }
}