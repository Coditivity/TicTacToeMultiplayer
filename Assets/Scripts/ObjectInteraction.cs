
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{

    
    /// <summary>
    /// A class that filters OnMouseUp unity event by checking if there is an object in front of this gameobject. Could be a UI object or a normal gameObject
    /// </summary>
    public class ObjectInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
    {
        
        [SerializeField]
        private List<GameObject> _relatedUIPanels = null;


        public void AddRelatedObject(GameObject relatedObject)
        {
            _relatedUIPanels.Add(relatedObject);
        }

        void Start()
        {
        }

        
        public void OnPointerUp(PointerEventData eventData)
        {


        }

        
        public void OnPointerDown(PointerEventData eventData)
        {

        }

        
        private void OnMouseDown()
        {

        }

        public delegate void OnMouseEnterListener();
        public event OnMouseEnterListener OnMouseEnterEvent;

        void OnMouseEnter()
        {
            
            bool hitListEmpty = false;
            bool relUIHit = CheckRelatedUIHit(out hitListEmpty);
            if (!relUIHit && !hitListEmpty)
            {
                return;
            }
            
            if(OnMouseEnterEvent!=null)
            {
                OnMouseEnterEvent.Invoke();
            }

        }

        public delegate void OnMouseOverListener(bool bAnotherObjectIsInFront);
        public event OnMouseOverListener OnMouseOverEvent;
        void OnMouseOver()
        {
            bool isHitEmpty = true;
            bool relUIHit = CheckRelatedUIHit(out isHitEmpty);
            if (!isHitEmpty && !relUIHit) //There is a ui in front of this object
            {
                if(OnMouseOverEvent!=null)
                {
                    OnMouseOverEvent.Invoke(true);
                }
            }
            else 
            {
                if (OnMouseOverEvent != null)
                {
                    OnMouseOverEvent.Invoke(false);
                }
            }

        }

        public delegate void OnMouseUpListener();
        public event OnMouseUpListener OnMouseUpEvent;
        
        private void OnMouseUp()
        {
            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = Input.mousePosition;

            bool brelatedUIHit = false;
            Transform transform;

            GameObject raycastHitGameObject = ((CustomStandaloneInputModule)EventSystem.current.currentInputModule)
                .PointerEventDataLeft().pointerEnter;


            if (raycastHitGameObject != null)
            {


                for (int i = 0; i < _relatedUIPanels.Count; i++)
                {
                    GameObject g = _relatedUIPanels[i];

                    if (raycastHitGameObject == g)
                    {
                        brelatedUIHit = true;
                    }
                    else
                    {
                        transform = raycastHitGameObject.transform.parent;
                        int count = 0;
                        while (transform != null && count < 1000)
                        {
                            count++;

                            if (transform.gameObject == g)
                            {
                                brelatedUIHit = true;
                                break;
                            }
                            else
                            {
                                transform = transform.parent;
                            }

                        }

                    }
                }
            }

            if (!brelatedUIHit && raycastHitGameObject != null)
            {
                return;
            }

            if(OnMouseUpEvent!=null)
            {
                OnMouseUpEvent.Invoke();
            }
            


        }

        /// <summary>
        /// Checks the related user interface hit.
        /// </summary>
        /// <returns><c>true</c>, if related user interface hit was checked, <c>false</c> otherwise.</returns>
        /// <param name="isHitListEmpty">O is hit list empty.</param>
        public bool CheckRelatedUIHit(out bool isHitListEmpty)
        {
            isHitListEmpty = true;
            PointerEventData ped = new PointerEventData(EventSystem.current);

            ped.position = Input.mousePosition;
            

            bool brelatedUIHit = false;
            Transform transform;
          

            GameObject raycastHitGameObject = null;
            if (EventSystem.current.currentInputModule != null)
            {
                raycastHitGameObject = ((CustomStandaloneInputModule)EventSystem.current.currentInputModule)
                    .PointerEventDataLeft().pointerEnter;
            }
            

            if (raycastHitGameObject != null)
            {
            

                foreach (GameObject g in _relatedUIPanels)
                {
                 
                    if (raycastHitGameObject == g)
                    {
                        brelatedUIHit = true;
                    }
                    else
                    {
                        transform = raycastHitGameObject.transform.parent;
                        int count = 0;
                        while (transform != null && count < 1000)
                        {
                            count++;

                            if (transform.gameObject == g)
                            {
                                brelatedUIHit = true;
                                break;
                            }
                            else
                            {
                                transform = transform.parent;
                            }

                        }

                    }
                }
            }
            isHitListEmpty = raycastHitGameObject == null ? true : false;                        
            return brelatedUIHit;
        }

      
        public void OnPointerEnter(PointerEventData eventData)
        {

        }

        
        public void OnPointerExit(PointerEventData eventData)
        {


        }

       
        public void OnPointerClick(PointerEventData eventData)
        {

        }

        
        
        void OnMouseExit()
        {

        }


    }
}