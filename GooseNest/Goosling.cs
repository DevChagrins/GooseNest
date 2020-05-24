using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace GooseNest
{
    class Goosling : MonoBehaviour
    {

        public static ArrayList _UDO = new ArrayList();

        public GameObject _goose;

        public bool _disableHumans;

        public Vector3 _goosePos;

        public float _gooseMass;

        float _deltaTime = 0.0f;

        public bool _menuEnabled;

        public GUIStyle _guiStyle = null;

        private GameObject _cloneGoose;

        private GameObject _sphereTracker;

        private ScreenTextDisplay _screenTextDisplay;

        public void Start()
        {
            _goose = GameObject.Find("Goose");
            _goose.AddComponent<ShowCollisions>();
            _gooseMass = _goose.GetComponent<Rigidbody>().mass;

            // Slightly larger GUI style for labels
            _guiStyle = new GUIStyle();
            _guiStyle.fontSize = Screen.height * 2 / 100;
            _guiStyle.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            // Screen Text Display
            _screenTextDisplay = new ScreenTextDisplay();
            _screenTextDisplay.Clear();
        }

        void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.B))
            {
                _disableHumans = !_disableHumans;
            }

            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.P))
            {
                if (!_cloneGoose)
                {
                    _cloneGoose = GameObject.Instantiate(_goose.gameObject, 
                        _goose.gameObject.transform.position + (_goose.gameObject.transform.forward * 1f), 
                        _goose.gameObject.transform.rotation, _goose.gameObject.transform.parent);
                }
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            {
                if (!_sphereTracker)
                {
                    _sphereTracker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Collider sphereCollider = _sphereTracker.GetComponent<Collider>();
                    sphereCollider.isTrigger = true;
                    _sphereTracker.transform.localScale = new Vector3(.5f, .5f, .5f);
                }
                else
                {
                    GameObject.Destroy(_sphereTracker);
                    _sphereTracker = null;
                }
            }

            if(_sphereTracker)
            {
                _sphereTracker.transform.position = _goose.gameObject.transform.position + (_goose.gameObject.transform.forward * 1f);
            }

            if(_disableHumans)
            {
                GameObject[] @objs = GameObject.FindGameObjectsWithTag("Human");
                foreach(GameObject g in @objs)
                {
                    g.transform.position = new Vector3(100000, 100000, 10000);
                }
            }

            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                _menuEnabled = !_menuEnabled;
            }
        }

        public void OnGUI()
        {
            _screenTextDisplay.Clear();
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            float msec = _deltaTime * 1000.0f;
            float fps = 1.0f / _deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);

            if (!_menuEnabled)
            {
                Vector3 _gs_position = _goose.gameObject.transform.position;
                Quaternion _gs_rotation = _goose.gameObject.transform.rotation;
                Vector3 _gs_velocity = _goose.GetComponent<Rigidbody>().velocity;

                string _position_string = string.Format("Position: ({0}, {1}, {2})", _gs_position.x.ToString("F4"), _gs_position.y.ToString("F4"), _gs_position.z.ToString("F4"));
                string _rotation_string = string.Format("Rotation: ({0}, {1}, {2}, {3})", _gs_rotation.x.ToString("F4"), _gs_rotation.y.ToString("F4"), _gs_rotation.z.ToString("F4"), _gs_rotation.w.ToString("F4"));
                string _velocity_string = string.Format("Velocity: ({0}, {1}, {2})", _gs_velocity.x.ToString("F4"), _gs_velocity.y.ToString("F4"), _gs_velocity.z.ToString("F4"));

                _screenTextDisplay.AddText(_position_string);
                _screenTextDisplay.AddText(_rotation_string);
                _screenTextDisplay.AddText(_velocity_string);

                if (_cloneGoose)
                {
                    _screenTextDisplay.AddText("Clone Position: (" + _cloneGoose.gameObject.transform.position + ")");
                    _screenTextDisplay.AddText("Clone Rotation: (" + _cloneGoose.gameObject.transform.rotation + ")");
                    _screenTextDisplay.AddText("Clone Velocity: " + _cloneGoose.GetComponent<Rigidbody>().velocity);
                }

                _screenTextDisplay.AddText("GooseNest 0.1");
            }

            if (_menuEnabled)
            {
                GUI.Box(new Rect(20, 20, Screen.width / 2, Screen.height / 2), "Goose Nest");
                _screenTextDisplay.AddText("Slowmotion: CTRL+1");
                _screenTextDisplay.AddText("Save Location: CTRL+S");
                _screenTextDisplay.AddText("Load Location: CTRL+L");
                _screenTextDisplay.AddText("Wireframe View: CTRL+W");
                _screenTextDisplay.AddText("Delete All Humans: CTRL+B");
                _screenTextDisplay.AddText("Advanced Output: CTRL+O");
            }

            _screenTextDisplay.DrawTextToGUI();
        }
    }
}
