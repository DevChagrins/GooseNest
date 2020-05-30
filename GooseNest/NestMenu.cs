using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GooseNest
{
    class NestMenu : MonoBehaviour
    {
        private ScreenTextDisplay _screenTextDisplay;
        private List<string> _currentLoadedScenes;

        private List<GOData> _lastClickedObjects;
        private List<GameObject> _collectiveObjects;

        private Vector2 _objectsScrollPosition;
        private Vector2 _componentScrollPosition;
        GameObject focusedObject = null;

        private GameObject _saveScreenObject;
        private Component _saveMenuComponent;
        private Component _eraseSelectMenuComponent;
        private Component _eraseConfirmMenuComponent;

        private MethodInfo _eraseMethod;
        private MethodInfo _loadMethod;

        private bool _eraseCallSuccess = false;

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
            _collectiveObjects = new List<GameObject>();

            FindSaveScreen();
        }

        void FindSaveScreen()
        {
            if(!_saveScreenObject)
            {
                _saveScreenObject = GameObject.Find("SaveScreen");

                if (_saveScreenObject)
                {
                    _saveMenuComponent = _saveScreenObject.GetComponent("SaveMenu");
                    _eraseSelectMenuComponent = _saveScreenObject.GetComponent("EraseSelectMenu");
                    _eraseConfirmMenuComponent = _saveMenuComponent.GetComponent("EraseConfirmMenu");

                    if (_eraseConfirmMenuComponent && _saveMenuComponent)
                    {
                        _eraseMethod = _eraseConfirmMenuComponent.GetType().GetMethod("Erase");
                        _loadMethod = _saveMenuComponent.GetType().GetMethod("LoadSlot1");
                    }
                }
            }
        }

        public void Update()
        {
            FindSaveScreen();

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

                    UnityEngine.GameObject[] allObjects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

                    for (int index = 0; index < allObjects.Length; index++)
                    {
                        _collectiveObjects.Add(allObjects[index]);
                    }
                }
            }
            else
            {
                Scene baseScene = SceneManager.GetSceneByName("base");

                if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I))
                {
                    _collectiveObjects.Clear();
                    if (baseScene.IsValid())
                    {
                        List<GameObject> rootChildren = new List<GameObject>();
                        baseScene.GetRootGameObjects(rootChildren);

                        while(rootChildren.Count > 0)
                        {
                            GameObject rootObject = rootChildren[0];
                            rootChildren.RemoveAt(0);
                            _collectiveObjects.Add(rootObject);
                            for(int childIndex = 0; childIndex < rootObject.transform.childCount; childIndex++)
                            {
                                rootChildren.Add(rootObject.transform.GetChild(childIndex).gameObject);
                            }
                        }
                    }
                }
            }
        }

        public void OnGUI()
        {
            _screenTextDisplay.Clear();
            _screenTextDisplay.SetStepDirection(StepDirection.Reverse);

            int screenWidth = Screen.width, screenHeight = Screen.height;

            _screenTextDisplay.AddText("Goose Nest v0.1");

            _screenTextDisplay.AddTextAtPosition(string.Format("Total scenes: {0}", SceneManager.sceneCountInBuildSettings), new Vector2(screenWidth - 500, _screenTextDisplay.GetCurrentPosition().y));
            _screenTextDisplay.AddText(string.Format("Scenes loaded: {0}", SceneManager.sceneCount));
            _screenTextDisplay.AddText(string.Format("Active scene: {0}", SceneManager.GetActiveScene().name));

            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCountInBuildSettings; sceneIndex++)
            {
                _screenTextDisplay.AddText(string.Format("Built scene path: {0}", SceneUtility.GetScenePathByBuildIndex(sceneIndex)));
            }

            if (_saveScreenObject)
            {
                _screenTextDisplay.AddText(string.Format("Found Save Screen Object"));
            }
            else
            {
                _screenTextDisplay.AddText(string.Format("Did not Find Save Screen Object"));
            }

            if (_saveMenuComponent)
            {
                _screenTextDisplay.AddText(string.Format("Found Save Menu Component"));
            }
            else
            {
                _screenTextDisplay.AddText(string.Format("Did not Find Save Menu Component"));
            }

            if (_eraseSelectMenuComponent)
            {
                _screenTextDisplay.AddText(string.Format("Found Erase Menu Component"));
            }
            else
            {
                _screenTextDisplay.AddText(string.Format("Did not Find Erase Menu Component"));
            }

            if (GUI.Button(new Rect((screenWidth - 300), 110, 80, 30), "Load Game"))
            {
                GameObject mainMenu = GameObject.Find("MainMenu");
                GameObject mainMenuChild = null;
                if(mainMenu)
                {
                    if(!mainMenu.transform.parent)
                    {
                        int menuChildCount = mainMenu.transform.childCount;

                        for(int menuIndex = 0; menuIndex < menuChildCount; menuIndex++)
                        {
                            GameObject menuChild = mainMenu.transform.GetChild(menuIndex).gameObject;

                            if(menuChild.name.Equals("MainMenu", System.StringComparison.InvariantCultureIgnoreCase))
                            {
                                mainMenuChild = menuChild;
                                break;
                            }
                        }
                    }

                    mainMenu.SetActive(false);

                    if (mainMenuChild)
                    {
                        mainMenuChild.SetActive(false);
                    }
                }

                SceneManager.LoadScene("main", LoadSceneMode.Additive);
            }

            if (GUI.Button(new Rect((screenWidth - 300) + 90, 110, 80, 30), "Load Loading"))
            {
                SceneManager.LoadScene("loading");
            }

            if(_eraseMethod != null && _loadMethod != null)
            {
                if (GUI.Button(new Rect((screenWidth - 300), 140, 150, 35), "Clean Load Save 1"))
                {
                    _eraseMethod.Invoke(_eraseConfirmMenuComponent, null);
                    _loadMethod.Invoke(_saveMenuComponent, null);
                }
            }

            _screenTextDisplay.SetPosition(new Vector2(20, 100));
            _screenTextDisplay.SetStepDirection(StepDirection.Normal);

            _screenTextDisplay.AddText(string.Format("Camera position: {0}", Camera.current.gameObject.transform.position));

            try
            {
                GameObject menuParent = GameObject.Find("Menus");

                if (menuParent)
                {
                    RectTransform rectTransform = menuParent.GetComponent<RectTransform>();

                    if (rectTransform)
                    {
                        _screenTextDisplay.AddText(string.Format("Menus position: {0}", rectTransform.position));
                    }
                }
                else
                {
                    _screenTextDisplay.AddText(string.Format("Couldn't find the menus"));
                }
            }
            catch(System.Exception e)
            {
                _screenTextDisplay.AddText("Couldn't find menu: " + e.Message);
            }

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
                    if(GUI.Button(new Rect(viewPosition, viewSize), string.Format("{0} : {1} {2}", _lastClickedObjects[index].Object, _lastClickedObjects[index].Object.GetComponents<Component>().Length, _lastClickedObjects[index].Object.activeSelf ? "++" : "--")))
                    {
                        focusedObject = _lastClickedObjects[index].Object;
                        _componentScrollPosition = Vector2.zero;
                    }
                    viewPosition += viewStep;
                    //_screenTextDisplay.AddText(string.Format("{0} : {1}", _lastClickedObjects[index].Object, _lastClickedObjects[index].Object.GetComponents<Component>().Length));
                }

                for (int index = 0; index < _collectiveObjects.Count; index++)
                {
                    if(GUI.Button(new Rect(viewPosition, viewSize), string.Format("{0} {1}", _collectiveObjects[index].name, _collectiveObjects[index].activeSelf ? "++" : "--")))
                    {
                        focusedObject = _collectiveObjects[index];
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

                _componentScrollPosition = GUI.BeginScrollView(new Rect(550, screenHeight - 350, 400, 300), _componentScrollPosition, new Rect(0, 0, 420f * 2f, componentListLength * 30f));

                Vector2 viewPosition = new Vector2(0f, 0f);
                Vector2 viewStep = new Vector2(0f, 26f);
                Vector2 viewSize = new Vector2(400f, 24f);

                for(int index = 0; index < components.Length; index++)
                {
                    Component component = components[index];
                    GUI.Label(new Rect(viewPosition, viewSize), string.Format("{0} - {1}", component.name, component.GetType()));
                    viewPosition += viewStep;

                    MemberInfo[] compMembers = component.GetType().GetMembers();
                    FieldInfo[] compFields = component.GetType().GetFields();
                    MethodInfo[] compMethods = component.GetType().GetMethods();
                    if (component.GetType().Name.Equals("MouseableMenuCollider", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach(FieldInfo field in compFields)
                        {
                            GUI.Label(new Rect(viewPosition, viewSize * 2f), string.Format("{0} : {1}", field.GetValue(component).GetType(), field.GetValue(component)));
                            viewPosition += viewStep;
                        }
                    }

                    if (component.GetType().Name.Equals("EraseSelectMenu", System.StringComparison.InvariantCultureIgnoreCase) ||
                         component.GetType().Name.Equals("SaveMenu", System.StringComparison.InvariantCultureIgnoreCase) ||
                         component.GetType().Name.Equals("EraseConfirmMenu", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        //foreach (System.Reflection.MethodInfo method in compMethods)
                        //{
                        //    GUI.Label(new Rect(viewPosition, viewSize * 2f), string.Format("{0}", method.Name));
                        //    viewPosition += viewStep;
                        //}

                        foreach (FieldInfo info in compFields)
                        {
                            GUI.Label(new Rect(viewPosition, viewSize * 2f), string.Format("  {0} - {1} : {2}", info.Name, info.GetValue(component), info.GetValue(component).GetType().Name));
                            viewPosition += viewStep;
                        }
                    }
                }

                if(GUI.Button(new Rect(viewPosition, viewSize), "Activate"))
                {
                    focusedObject.SetActive(!focusedObject.activeSelf);
                }
                viewPosition += viewStep;

                GUI.Label(new Rect(viewPosition, viewSize), string.Format("Tag: {0}", focusedObject.tag));
                viewPosition += viewStep;

                if (focusedObject.transform.parent)
                {
                    if (GUI.Button(new Rect(viewPosition, viewSize), string.Format("Parent: {0}", focusedObject.transform.parent.gameObject.name)))
                    {
                        focusedObject = focusedObject.transform.parent.gameObject;
                        _componentScrollPosition = Vector2.zero;
                    }
                }
                else
                {
                    GUI.Label(new Rect(viewPosition, viewSize), string.Format("Parent: {0}", focusedObject.transform.parent ? focusedObject.transform.parent.gameObject.name : "No parent"));
                }

                viewPosition += viewStep;

                GUI.EndScrollView();
            }

            _screenTextDisplay.DrawTextToGUI();
        }
    }
}
