using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Zenisoft.UI
{
    public class PlayerListItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        
        public void SetUp(Player player)
        {
            if (string.IsNullOrEmpty(player.NickName)) 
                playerNameText.text = "Player " + player.ActorNumber;
            else 
                playerNameText.text = player.NickName;
        }
    }
}