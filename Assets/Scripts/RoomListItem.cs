using System;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zenisoft.UI
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomNameText;
        [SerializeField] private TextMeshProUGUI playersRoomText;

        private RoomInfo roomInfo;
        public void SetUp(RoomInfo room)
        {
            roomInfo = room;
            roomNameText.text = room.Name;
            playersRoomText.text = room.PlayerCount + "/" + room.MaxPlayers;
        }

        public void SetOnJoinRoomCallback(Action<RoomInfo> callback)
        {
            GetComponent<Button>().onClick.AddListener(() => callback(roomInfo));
        }
    }
}