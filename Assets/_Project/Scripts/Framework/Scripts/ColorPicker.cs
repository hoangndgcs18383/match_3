using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utils
{
    public enum ColorPickerType
    {
        RGB,
        RGBA
    }

    public class ColorPicker : MonoBehaviour, IDragHandler
    {
        public delegate void ColorHandler(Color color);

        public static ColorPicker Current;

        private bool _isInit;

        #region Properties

        [Title("Settings")] [SerializeField] private ColorPickerType colorPickerType = ColorPickerType.RGB;
        [SerializeField] private bool useSharp = false;
        [SerializeField] private bool useSaveLocal = false;

        [EnableIf("useSaveLocal")] [SerializeField]
        private List<string> listColor = new List<string>();

        [Title("UI")] [SerializeField] private RectTransform pickColorIndicator;
        [SerializeField] private RawImage pickColorRawImage;
        [SerializeField] private RawImage previewRawImage;

        [SerializeField] private TMP_InputField hexInputField;

        [Tooltip("Slider 360 degree")] [SerializeField]
        private Slider mainColorSlider;

        [FoldoutGroup("RGB")] [SerializeField] private Slider rSlider;
        [FoldoutGroup("RGB")] [SerializeField] private RawImage rRawImage;
        [FoldoutGroup("RGB")] [SerializeField] private Slider gSlider;
        [FoldoutGroup("RGB")] [SerializeField] private RawImage gRawImage;
        [FoldoutGroup("RGB")] [SerializeField] private Slider bSlider;
        [FoldoutGroup("RGB")] [SerializeField] private RawImage bRawImage;
        [FoldoutGroup("RGB")] [SerializeField] private Slider aSlider;
        [FoldoutGroup("RGB")] [SerializeField] private RawImage aRawImage;

        #endregion

        #region Varibles

        private Canvas _rootCanvas;
        private Camera _worldCamera;

        private RectTransform _pickColorIndicatorParent;

        private static HSV _modifiedHsv;

        private ColorHandler _onColorChanged;
        private ColorHandler _onColorSelected;

        private Color32 _originalColor;
        private Color32 _modifiedColor;
        private bool _completed = true;

        #endregion

        private void Awake()
        {
            if (Current == null) Current = this;
            _rootCanvas = GetComponentInParent<Canvas>();
            _worldCamera = _rootCanvas.worldCamera;
            hexInputField.onEndEdit.AddListener(SetHex);
            aSlider.gameObject.SetActive(colorPickerType == ColorPickerType.RGBA);
        }

        private void Start()
        {
            Load();
            SetUp(Color.white, null, null);
        }

        public bool SetUp(Color originColor, ColorHandler onColorChanged, ColorHandler onColorSelected)
        {
            if (_isInit) return false;

            if (_completed)
            {
                _completed = false;
                _originalColor = originColor;
                _modifiedColor = originColor;

                _onColorChanged = onColorChanged;
                _onColorSelected = onColorSelected;
                UpdateColor(true);
                _isInit = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdateColor(bool isHSV)
        {
            if (isHSV)
            {
                _modifiedHsv = new HSV(_modifiedColor);
            }
            else
            {
                _modifiedColor = _modifiedHsv.ToColor();
            }

            UpdateSlider();
            mainColorSlider.value = (float)_modifiedHsv.H;
            UpdateRawImageRgba();

            pickColorRawImage.color = new HSV(_modifiedHsv.H, 1, 1).ToColor();
            pickColorIndicator.anchorMin = new Vector2((float)_modifiedHsv.S, (float)_modifiedHsv.V);
            pickColorIndicator.anchorMax = pickColorIndicator.anchorMin;
            previewRawImage.color = _modifiedColor;
            _onColorChanged?.Invoke(_modifiedColor);
        }

        private void UpdateSlider()
        {
            rSlider.value = _modifiedColor.r;
            gSlider.value = _modifiedColor.g;
            bSlider.value = _modifiedColor.b;
            if (colorPickerType == ColorPickerType.RGBA) aSlider.value = _modifiedColor.a;
        }

        private void UpdateRawImageRgba()
        {
            rRawImage.color = new Color32(255, _modifiedColor.g, _modifiedColor.b, 255);
            gRawImage.color = new Color32(_modifiedColor.r, 0, _modifiedColor.b, 255);
            bRawImage.color = new Color32(_modifiedColor.r, _modifiedColor.g, 0, 255);
            if (colorPickerType == ColorPickerType.RGBA)
                aRawImage.color = new Color32(_modifiedColor.r, _modifiedColor.g, _modifiedColor.b, 255);
        }


        private void SetChooseColor(Vector3 pos)
        {
            if (_pickColorIndicatorParent == null)
                _pickColorIndicatorParent = (RectTransform)pickColorIndicator.parent;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_pickColorIndicatorParent, pos, _worldCamera,
                out Vector2 localPoint);
            localPoint = Rect.PointToNormalized((_pickColorIndicatorParent).rect, localPoint);

            if (pickColorIndicator.anchorMin != localPoint)
            {
                pickColorIndicator.anchorMin = localPoint;
                pickColorIndicator.anchorMax = localPoint;
                _modifiedHsv.S = localPoint.x;
                _modifiedHsv.V = localPoint.y;
                UpdateColor(false);
            }
        }


        #region Funtions

        [Button]
        public void Complete()
        {
            _completed = true;
            if (useSaveLocal) Save();
            _onColorSelected?.Invoke(_modifiedColor);
        }

        private void Save()
        {
            listColor.Add(ColorUtility.ToHtmlStringRGBA(_modifiedColor));
            Debug.Log($"{string.Join(",", listColor.ToArray())}");
            PlayerPrefs.SetString("~~listColor~~", string.Join(",", listColor.ToArray()));
        }

        private void Load()
        {
            if (PlayerPrefs.HasKey("~~listColor~~"))
            {
                string[] list = PlayerPrefs.GetString("~~listColor~~").Split(',');
                foreach (var item in list)
                {
                    listColor.Add(item);
                }
            }
        }

        private void Edit()
        {
        }
        
        private void ResetToDefault()
        {
        }
        

        #endregion

        #region UI Events

        public void OnDrag(PointerEventData eventData)
        {
            SetChooseColor(eventData.position);
        }

        public void SetMainColor(float value)
        {
            _modifiedHsv.H = value;
            UpdateColor(false);
        }

        private void SetHex(string hex)
        {
            if (ColorUtility.TryParseHtmlString(useSharp ? "#" + hex : hex, out Color c))
            {
                _modifiedColor = c;
                UpdateColor(true);
            }
            else
            {
                hexInputField.text = ColorUtility.ToHtmlStringRGB(_modifiedColor);
            }
        }

        #endregion
    }
}