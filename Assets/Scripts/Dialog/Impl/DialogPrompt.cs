using System;
using System.Linq;
using System.Xml.Schema;
//using Rinput;
using UnityEngine;

    public class DialogPrompt : MonoBehaviour
    {
        //TODO:
        public DialogSequence dialog;
        [SerializeField] GameObject promptObject;
        //[SerializeField] GameInput input = GameInput.A;
        [SerializeField] float activationDistance = 1.5f;
        [SerializeField] Transform target;
        static float closestDistance = float.MaxValue;
        static DialogPrompt closestDialog = null;
        private bool wasClosest;
#if UNITY_EDITOR
        private void OnValidate()
        {
            target = GameObject.FindGameObjectsWithTag("Player").First(x=>x.name.Contains("Player")).transform;
        }
        #endif

        private void Update()
        {
            float distance = Vector3.Distance(target.position,transform.position);


            if (closestDialog == this)
            {
                closestDistance = distance;
                //if(InputManager.Instance.P1IsPressed(input))
                //    Prompt();
            }else if (wasClosest)
            {
                promptObject.SetActive(false);
                wasClosest = false;
            }

            if (distance > activationDistance && closestDialog == this)
            {
                closestDialog = null;
                closestDistance = float.MaxValue;
            }
            if (distance >= activationDistance) return;
            
            if (closestDialog && distance >= closestDistance) return;
            
            closestDistance = distance;
            closestDialog = this;
            wasClosest = true;
            promptObject.SetActive(true);
        }

        public void Prompt()
        {
            DialogManager.Instance
                .CurrentDialogSequence = dialog;
            DialogManager.Instance.Open();
        }
    }
