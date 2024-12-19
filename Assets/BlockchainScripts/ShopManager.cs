using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thirdweb;
using Thirdweb.Unity;
using TMPro;
using UnityEngine.UI;
using System.Numerics;
using System;
using System.Data;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Unity.Collections.LowLevel.Unsafe;

public class ShopManager : MonoBehaviour
{
    public string Address { get; private set; }
    public static BigInteger ChainId = 204;

    public UnityEngine.UI.Button playButton;
    public UnityEngine.UI.Button claimTokenButton;
    public UnityEngine.UI.Button walletBalanceButton;

    public TMP_Text tokenBoughtText;
    public TMP_Text buyingStatusText;

    string customSmartContractAddress = "0xFE8305683F15f6D771D0b9126C3F4A60c7E5C1a9";
    string abiString = "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"PassGranted\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"newTotal\",\"type\":\"uint256\"}],\"name\":\"PointsAdded\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"addPoints\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"getPlayerPoints\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"grantAdventurePass\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"hasAdventurePass\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";

    int tokenAmount = 10;

    string notEnoughToken = " BNB";

    private void HideAllButtons()
    {
        playButton.interactable = false;
        claimTokenButton.interactable = false;
        walletBalanceButton.interactable = false;
    }

    private void ShowAllButtons()
    {
        playButton.interactable = true;
        claimTokenButton.interactable = true;
        walletBalanceButton.interactable = true;
    }

    private void UpdateStatus(string messageShow)
    {
        buyingStatusText.text = messageShow;
        buyingStatusText.gameObject.SetActive(true);
    }

    private void BoughtSuccessFully()
    {
        BlockchainEffect.Instance.tokens += tokenAmount;


        // Lấy Token hiện tại (mặc định là 0 nếu chưa lưu trước đó)
        int currentToken = PlayerPrefs.GetInt("Token", 0);

        // Cộng thêm tokenAmount vào Token hiện tại
        int newTokenAmount = currentToken + tokenAmount;

        // Lưu giá trị mới vào PlayerPrefs
        PlayerPrefs.SetInt("Token", newTokenAmount);
        PlayerPrefs.Save(); // Đảm bảo dữ liệu được ghi đè

        // In ra log để kiểm tra
        Debug.Log($"Token updated. Previous: {currentToken}, Added: {tokenAmount}, New: {newTokenAmount}");


        tokenBoughtText.text = "Token Owned: " + BlockchainEffect.Instance.tokens.ToString();
        UpdateStatus("Got 10 Tokens");
    }

    public async void GetTokens()
    {
        HideAllButtons();
        UpdateStatus("Getting 10 Tokens...");       
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        var balance = await wallet.GetBalance(chainId: ChainId);
        var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
        Debug.Log("balanceEth1: " + balanceEth);
        if (float.Parse(balanceEth) <= 0f)
        {
            UpdateStatus("Not Enough Token...");
            ShowAllButtons();
            return;
        }
        //Bắt đầu Coroutine
        StartCoroutine(WaitAndExecute());
        try
        {
            Claim10Tokens();
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred during the transfer: {ex.Message}");
        }
    }

    public async void Claim10Tokens()
    {
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        var contract = await ThirdwebManager.Instance.GetContract(
           customSmartContractAddress,
           ChainId,
           abiString
       );
        var address = await wallet.GetAddress();

        // Gọi hàm `submitScore` trong hợp đồng với điểm số (score)
        await ThirdwebContract.Write(wallet, contract, "addPoints", 0, address,1);

        var result = ThirdwebContract.Read<int>(contract, "getPlayerPoints", address);
        Debug.Log("result: " + result);
    }

    IEnumerator WaitAndExecute()
    {
        Debug.Log("Coroutine started, waiting for 3 seconds...");
        yield return new WaitForSeconds(3f); // Chờ 3 giây
        Debug.Log("3 seconds have passed!");
        BoughtSuccessFully();
        ShowAllButtons();
    }

    public async void GetWalletBalance()
    {
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        var balance = await wallet.GetBalance(chainId: ChainId);
        var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
        UpdateStatus("Balance: " + balanceEth + notEnoughToken);
    }

    public void ChangeToScenePlay()
    {
        SceneManager.LoadScene("Init");
    }


}
