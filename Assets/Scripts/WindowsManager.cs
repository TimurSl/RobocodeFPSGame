using System.Linq;
using UnityEngine;

namespace Zenisoft
{
    public class WindowsManager : MonoBehaviour
    {
        public static WindowsManager Layout;
        [SerializeField] private GameObject[] windows;

        private void Awake()
        {
            Layout = this;
            foreach (GameObject window in windows)
            {
                window.SetActive(false);
            }
        }

        private void Start()
        {
            OpenWindow("Login");
        }

        // ReSharper disable once InvalidXmlDocComment
        public void OpenWindow(string windowName)
        {
            foreach (GameObject window in windows)
            {
                window.SetActive(window.name == windowName);     
            }
            
            // foreach (GameObject window in windows)
            // {
            //     if (window.name == name)
            //     {
            //          window.SetActive(true);
            //     }
            //     else
            //     {
            //          window.SetActive(false);
            //     }
            // }
            // ЭТО ЧЕ ТАКОЕ? ПОЧЕМУ НЕ ПРОСТО СДЕЛАТЬ ТАК?
        }
        
        public bool IsWindowOpen(string windowName)
        {
            return windows.Any(window => window.name == windowName && window.activeSelf);
        }
    }
}