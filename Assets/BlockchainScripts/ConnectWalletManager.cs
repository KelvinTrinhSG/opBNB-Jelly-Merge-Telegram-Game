using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectWalletManager : MonoBehaviour
{
    public void SwitchToShopScene()
    {
        SceneManager.LoadScene("ShopScene");
    }
}
