using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GooseNest
{
    class NestMenu : MonoBehaviour
    {
        private ScreenTextDisplay _screenTextDisplay;
        private List<string> _currentLoadedScenes;

        private List<GOData> _lastClickedObjects;
        private List<Object> _collectiveObjects;

        private Vector2 _objectsScrollPosition;
        private Vector2 _componentScrollPosition;
        GameObject focusedObject = null;

        class GOData : System.Object
        {
            public GameObject Object;
            public float DistanceFromOrigin;
            public int ObjectHash;

            public GOData(GameObject gameObject, float distance)
            {
                Object = gameObject;
                DistanceFromOrigin = distance;
                ObjectHash = Object.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                
                return ObjectHash == obj.GetHashCode();
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                return ObjectHash;
            }
        }

        public void Start()
        {
            _screenTextDisplay = new ScreenTextDisplay();

            int screenWidth = Screen.width, screenHeight = Screen.height;
            _screenTextDisplay.SetStartPosition(new Vector2(screenWidth - 300, screenHeight - 100));
            _screenTextDisplay.SetStepDirection(StepDirection.Reverse);
            _currentLoadedScenes = new List<string>();
            _lastClickedObjects = new List<GOData>();
            _collectiveObjects = new List<Object>();
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftShift))
            {
                _lastClickedObjects.Clear();
                _collectiveObjects.Clear();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit[] raycastHits = Physics.RaycastAll(ray, 50f);
                if (raycastHits.Length > 0)
                {
                    for (int index = 0; index < raycastHits.Length; index++)
                    {
                        GameObject hitObject = raycastHits[index].collider.gameObject;
                        if (!_lastClickedObjects.Contains(new GOData(hitObject, 0f)))
                        {
                            _lastClickedObjects.Add(new GOData(hitObject, Vector3.Distance(ray.origin, hitObject.transform.position)));
                        }
                    }
                }
            }

            if (SceneManager.sceneCount <= 1)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
                {
                    _lastClickedObjects.Clear();
                    _collectiveObjects.Clear();

                    UnityEngine.Object[] allObjects = GameObject.FindObjectsOfType(typeof(GameObject));

                    for (int index = 0; index < allObjects.Length; index++)
                    {
                        _collectiveObjects.Add(allObjects[index]);
                    }
                }
            }
        }

        public void OnGUI()
        {
            _screenTextDisplay.Clear();
            int screenWidth = Screen.width, screenHeight = Screen.height;

            _screenTextDisplay.AddText("Goose Nest v0.1");

            _screenTextDisplay.AddTextAtPosition(string.Format("Total scenes: {0}", SceneManager.sceneCountInBuildSettings), new Vector2(screenWidth - 500, _screenTextDisplay.GetCurrentPosition().y));
            _screenTextDisplay.AddText(string.Format("Scenes loaded: {0}", SceneManager.sceneCount));
            _screenTextDisplay.AddText(string.Format("Active scene: {0}", SceneManager.GetActiveScene().name));

            //for(int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            //{
            //    _screenTextDisplay.AddText(string.Format("Loaded scene name: {0}", SceneManager.GetSceneAt(sceneIndex).name));
            //}

            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCountInBuildSettings; sceneIndex++)
            {
                _screenTextDisplay.AddText(string.Format("Built scene path: {0}", SceneUtility.GetScenePathByBuildIndex(sceneIndex)));
            }

            if (GUI.Button(new Rect((screenWidth - 300), 110, 80, 30), "Load Game"))
            {
                SceneManager.LoadScene("main", LoadSceneMode.Additive);
            }

            if (GUI.Button(new Rect((screenWidth - 300) + 90, 110, 80, 30), "Load Loading"))
            {
                SceneManager.LoadScene("loading");
            }

            //_screenTextDisplay.SetPosition(new Vector2(20, screenHeight - 100));

            float listLength = 0;
            if (_lastClickedObjects.Count > 0)
                listLength = _lastClickedObjects.Count;

            if (_collectiveObjects.Count > 0)
                listLength = _collectiveObjects.Count;

            if (listLength <= 0f)
                focusedObject = null;

            listLength *= 24f;

            if (listLength > 0f)
            {
                _objectsScrollPosition = GUI.BeginScrollView(new Rect(20, screenHeight - 350, 500, 300), _objectsScrollPosition, new Rect(0, 0, 520, listLength));

                Vector2 viewPosition = new Vector2(0f, 0f);
                Vector2 viewStep = new Vector2(0f, 24f);
                Vector2 viewSize = new Vector2(500f, 24f);

                for (int index = 0; index < _lastClickedObjects.Count; index++)
                {
                    if(GUI.Button(new Rect(viewPosition, viewSize), string.Format("{0} : {1}", _lastClickedObjects[index].Object, _lastClickedObjects[index].Object.GetComponents<Component>().Length)))
                    {
                        focusedObject = _lastClickedObjects[index].Object;
                        _componentScrollPosition = Vector2.zero;
                    }
                    viewPosition += viewStep;
                    //_screenTextDisplay.AddText(string.Format("{0} : {1}", _lastClickedObjects[index].Object, _lastClickedObjects[index].Object.GetComponents<Component>().Length));
                }

                for (int index = 0; index < _collectiveObjects.Count; index++)
                {
                    if(GUI.Button(new Rect(viewPosition, viewSize), string.Format("{0}", _collectiveObjects[index].name)))
                    {
                        focusedObject = _collectiveObjects[index] as GameObject;
                        _componentScrollPosition = Vector2.zero;
                    }
                    viewPosition += viewStep;
                }

                GUI.EndScrollView();
            }


            if (focusedObject)
            {
                float componentListLength = 0;

                Component[] components = focusedObject.GetComponents<Component>();

                if(components.Length > 0)
                {
                    componentListLength = components.Length * 24;
                }

                _componentScrollPosition = GUI.BeginScrollView(new Rect(550, screenHeight - 350, 400, 300), _componentScrollPosition, new Rect(0, 0, 420, componentListLength));

                Vector2 viewPosition = new Vector2(0f, 0f);
                Vector2 viewStep = new Vector2(0f, 24f);
                Vector2 viewSize = new Vector2(400f, 24f);

                for(int index = 0; index < components.Length; index++)
                {
                    GUI.Label(new Rect(viewPosition, viewSize), string.Format("{0} - {1}", components[index].name, components[index].GetType()));
                    viewPosition += viewStep;
                }
            }

            _screenTextDisplay.DrawTextToGUI();
        }
    }
}
