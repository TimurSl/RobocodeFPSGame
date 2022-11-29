using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenisoft.UI;

namespace Zenisoft.Network
{
    public class MasterClient : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Ссылка на класс сетевого менеджера
        /// </summary>
        public static MasterClient Instance;
        
        // Обьекты интерфейса для создания комнаты
        [SerializeField] private InputField roomNameInputField;
        [SerializeField] private InputField roomPasswordInputField;
        [SerializeField] private TextMeshProUGUI roomNameText;

        // Обьекты интерфейса для игроков в комнате
        [SerializeField] private Transform playerListContent;
        [SerializeField] private GameObject playerListItemPrefab;
        
        // Обьекты интерфейса для комнат
        [SerializeField] private Transform roomListContent;
        [SerializeField] private GameObject roomListItemPrefab;

        // Обьекты интерфейса для подключения к кастомной комнате
        [SerializeField] private InputField customRoomNameInputField;
        [SerializeField] private InputField customRoomPasswordInputField;
        
        
        /// <summary>
        /// Вызывается при создании объекта
        /// </summary>
        private void Awake()
        {
            PhotonNetwork.ConnectUsingSettings();

            Instance = this;
        }
        

        /// <summary>
        /// Вызывается при подключении к мастер-серверу
        /// </summary>
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        /// <summary>
        /// Вызывается при успешном подключении к лобби
        /// </summary>
        public override void OnJoinedLobby()
        {
            Debug.Log("<color=green>Joined Lobby</color>");
            WindowsManager.Layout.OpenWindow("MainMenu");
        }

        /// <summary>
        /// Создать новую комнату
        /// </summary>
        public void CreateNewRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
                return;
            
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            if (string.IsNullOrEmpty(roomPasswordInputField.text))
                roomOptions.IsVisible = true;
            else
            {
                roomOptions.IsVisible = false;
                roomOptions.CustomRoomProperties = new Hashtable() {{ "Password", roomPasswordInputField.text }};
                roomOptions.CustomRoomPropertiesForLobby = new string[] {"Password"};
            }
            
            PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        }
        
        /// <summary>
        /// Выйти из комнаты
        /// </summary>
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            WindowsManager.Layout.OpenWindow("MainMenu");
        }

        /// <summary>
        /// Вызывается при подключении игрока к комнате
        /// </summary>
        /// <param name="newPlayer">Игрок, который подключился</param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            UpdatePlayerList();
        }

        /// <summary>
        /// Подключение к скрытой комнате с паролем
        /// </summary>
        public void ConnectToHiddenRoom()
        {
            string roomName = customRoomNameInputField.text;
            string roomPassword = customRoomPasswordInputField.text;
            
            if (string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(roomPassword))
                return;
            
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = false;
            roomOptions.CustomRoomProperties = new Hashtable() {{ "Password", roomPassword }};
            
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
        
        /// <summary>
        /// Вызывается, когда игрок выходит из комнаты
        /// </summary>
        /// <param name="otherPlayer">Игрок который вышел из комнаты</param>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            UpdatePlayerList();
        }
        
        /// <summary>
        /// Обновляет список игроков в комнате
        /// </summary>
        private void UpdatePlayerList()
        {
            foreach (Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }
            
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
            }
        }
        
        /// <summary>
        /// Подключение к комнате 
        /// </summary>
        /// <param name="roomInfo">Комната</param>
        public void JoinRoom(RoomInfo roomInfo)
        {
            PhotonNetwork.JoinRoom(roomInfo.Name);
        }
        
        
        /// <summary>
        /// Вызывается при обновлении списка комнат (автоматически)
        /// </summary>
        /// <param name="roomList">Список комнат, получаеться после обновления</param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            // if (WindowsManager.layout.isWindowOpen("FindRoom"))
            //     return;
            
            for (int i = 0; i < roomListContent.childCount; i++)
            {
                Destroy(roomListContent.GetChild(i).gameObject);
            }
            foreach (var room in roomList)
            {
                if (!room.RemovedFromList)
                {
                    RoomListItem roomItem = Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>();
                    roomItem.SetUp(room);
                    roomItem.SetOnJoinRoomCallback(JoinRoom);
                }
            }
        }

        /// <summary>
        /// Колбэк при подключении к комнате
        /// </summary>
        public override void OnJoinedRoom()
        {
            WindowsManager.Layout.OpenWindow("GameRoom");
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;

            for (int i = 0; i < playerListContent.childCount; i++)
            {
                Destroy(playerListContent.GetChild(i).gameObject);
            }
            foreach (var player in PhotonNetwork.PlayerList)
            {
                Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
            }
        }

        /// <summary>
        /// Панель разработчика
        /// </summary>
        private void OnGUI()
        {
#if DEVELOPER_EDITION
            GUILayout.Box(
                "State: " + PhotonNetwork.NetworkClientState + "\n" +
                "Ping: " + PhotonNetwork.GetPing() + "\n" +
                "Server: " + PhotonNetwork.CloudRegion + "\n" +
                "Online: " + PhotonNetwork.CountOfPlayers + "\n" +
                "Rooms: " + PhotonNetwork.CountOfRooms + "\n" +
                "IP: " + PhotonNetwork.ServerAddress + "\n" +
                "Nick: " + PhotonNetwork.NickName + "\n"

                ,new GUIStyle(GUI.skin.box) {alignment = TextAnchor.MiddleLeft}); 
            
            PhotonNetwork.NickName = GUILayout.TextField(PhotonNetwork.NickName);
            
            if (PhotonNetwork.InRoom)
            {
                GUILayout.Box(
                    "Room: " + PhotonNetwork.CurrentRoom.Name + "\n" +
                    "Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "\n" +
                    "MaxPlayers: " + PhotonNetwork.CurrentRoom.MaxPlayers + "\n" +
                    "IsOpen: " + PhotonNetwork.CurrentRoom.IsOpen + "\n" +
                    "IsVisible: " + PhotonNetwork.CurrentRoom.IsVisible + "\n"
                    
                    , new GUIStyle(GUI.skin.box) {alignment = TextAnchor.MiddleLeft});
                
                if (GUILayout.Button("Leave Room"))
                {
                    PhotonNetwork.LeaveRoom();
                }
                
            }
            
            
            if (GUILayout.Button("Connect to Random Room"))
                PhotonNetwork.JoinRandomOrCreateRoom();

            if (GUILayout.Button("Create Test Room"))
                PhotonNetwork.CreateRoom("TestRoom");
#endif
            
        }
    }
}