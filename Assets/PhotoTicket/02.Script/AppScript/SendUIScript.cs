using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alchera;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System;

public class SendUIScript : MonoBehaviour, UIScript
{
    ReadWebcamInSequence camReader;
    [SerializeField] WritePhoneNumber Numberpanel;
    [SerializeField] WriteEmailKeyboard Keyboardpanel;
    [SerializeField] RectTransform cancelButtonPhone;
    [SerializeField] RectTransform cancelButtonEmail;
    [SerializeField] RectTransform cancelButtonQR;
    [SerializeField] RectTransform sendButtonPhone;
    [SerializeField] RectTransform sendButtonQR;
    [SerializeField] RectTransform sendButtonEmail;
    [SerializeField] GameObject ConfirmPopupPhone;
    [SerializeField] GameObject ConfirmPopupEmail;
    [SerializeField] GameObject ConfirmPopupQR;
    [SerializeField] GameObject printErrorPopup;
    [SerializeField] Text ConfirmNumber;
    [SerializeField] Text ConfirmEmail;
    [SerializeField] MovieDownManager downManager;
    [SerializeField] Button btnPhone;
    [SerializeField] Button btnEmail;
    [SerializeField] Button btnQR;
    [SerializeField] GameObject phoneTab;
    [SerializeField] GameObject emailTab;
    [SerializeField] GameObject qrTab;
    [SerializeField] AudioSource sendDataAudioKr;
    [SerializeField] AudioSource sendDataAudioEn;
    [SerializeField] AudioSource checkAudioKr;
    [SerializeField] AudioSource checkAudioEn;
    [SerializeField] AudioSource buttonAudio;
    [SerializeField] SelectUIScript selectUIScript;
    [SerializeField] GameObject netClientErrorCanvas;
    [SerializeField] GameObject isReconnectPopup;
    [SerializeField] GameObject failReconnectPopup;
    [SerializeField] GameObject reconnectPopup;
    [SerializeField] GameObject networkErrorPopup;

    // SelectUIScript selectUIScript;

    Text phoneSendText;
    Text emailSendText;
    Text qrSendText;
    Boolean sendButtonClickFlag = false;
    public bool bCheckPrint;
    bool bSendScene = false;   // 다른 화면에서 사운드 제어위해 사용

    [Serializable]
    public class JSONResult
    {
        public int myposterid;
        public string email_send;
        public string mobile_send;
        public int success;
        public int error;
        public string message;
        public string url;
    }

    void Start()
    {
        camReader = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ReadWebcamInSequence>();
        phoneSendText = btnPhone.GetComponentInChildren<Text>();
        emailSendText = btnEmail.GetComponentInChildren<Text>();
        qrSendText = btnQR.GetComponentInChildren<Text>();

        StartCoroutine(SendSetting());
    }

    void Update()
    {

    }

    public void Init()
    {
        // 전송 안내 멘트 출력
        StartCoroutine(UtilsScript.playAudio(sendDataAudioKr, sendDataAudioEn));

        print("InitSend");

        sendSetting();  // 전송 설정 및 UI 설정

        // paymode t인 경우 프린터 출력 가능 체크
        //if (PlayerPrefs.GetString("pay_mode").Equals("t")) {
        //    checkPrint();
        //    Debug.Log("프린터 상태 체크 시작");
        //}

        sendButtonClickFlag = false;
        ConfirmPopupPhone.SetActive(false);
        ConfirmPopupEmail.SetActive(false);
        ConfirmPopupQR.SetActive(false);
        printErrorPopup.SetActive(false);
        bSendScene = true;

        cancelButtonPhone.sizeDelta = new Vector2(324, 170);
        sendButtonPhone.anchoredPosition = new Vector2(324, -1090);
        sendButtonPhone.sizeDelta = new Vector2(756, 170);

        cancelButtonEmail.sizeDelta = new Vector2(324, 170);
        sendButtonEmail.anchoredPosition = new Vector2(324, -1090);
        sendButtonEmail.sizeDelta = new Vector2(756, 170);

        cancelButtonQR.sizeDelta = new Vector2(324, 170);
        sendButtonQR.anchoredPosition = new Vector2(324, -1090);
        sendButtonQR.sizeDelta = new Vector2(756, 170);

        //cancelButtonPhone.sizeDelta = new Vector2(0, 170);
        //sendButtonPhone.anchoredPosition = new Vector2(0, -1090);
        //sendButtonPhone.sizeDelta = new Vector2(1080, 170);

        //cancelButtonEmail.sizeDelta = new Vector2(0, 170);
        //sendButtonEmail.anchoredPosition = new Vector2(0, -1090);
        //sendButtonEmail.sizeDelta = new Vector2(1080, 170);

        camReader.changeCameraState(false);
    }

    private IEnumerator sendSetting() {
        yield return StartCoroutine(SendSetting());
    }

    public void Dispose()
    {
        ConfirmPopupPhone.SetActive(false);
        ConfirmPopupEmail.SetActive(false);
        bSendScene = false;
        Numberpanel.phoneNumberText.text = "010 - ";
        Numberpanel.phoneNumber = "010 - ";
        Keyboardpanel.emailAddressText.text = "";
    }

    public void Cancel() {
        // 버튼 효과음 출력
        StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

        sendDataAudioKr.Stop();
        sendDataAudioEn.Stop();

        FlowController.instance.ChangeFlow(FlowController.instance.AgreementCanvas);
    }

    public void checkPrint() {
        selectUIScript.canvas = "send";
        selectUIScript.sendType = "check_print";

        Debug.Log("Call select Coroutine");

        //큐 탐색 시작
        StartCoroutine(selectUIScript.CheckQueue());

        AgentSendData sendData = new AgentSendData();
        sendData.Command = "print_status";
        sendData.PrintCount = "1";

        string checkMessage = JsonUtility.ToJson(sendData);

        Debug.Log("SENDMESSAGE:" + checkMessage);
        SelectUIScript._netClient.SendMessage(checkMessage);
    }

    public void Btn_Tab(GameObject tab) {
        if (tab == phoneTab) {
            emailTab.SetActive(false);
            phoneTab.SetActive(true);
            qrTab.SetActive(false);
            ReType("phone");
        }

        if (tab == emailTab) {
            emailTab.SetActive(true);
            phoneTab.SetActive(false);
            qrTab.SetActive(false);
            ReType("email");
        }

        if (tab == qrTab) {
            emailTab.SetActive(false);
            phoneTab.SetActive(false);
            qrTab.SetActive(true);
            ReType("email");
        }

        tab.SetActive(true);
    }

    public void ChageColor(Button button) {
        ColorBlock btnPhonecolorBlock = btnPhone.colors;
        ColorBlock btnEmailcolorBlock = btnEmail.colors;
        ColorBlock btnQRcolorBlock = btnEmail.colors;

        if (button == btnPhone) {
            btnPhonecolorBlock.normalColor = new Color(0.682f, 0f, 0f, 1f);
            btnEmailcolorBlock.normalColor = new Color(0.682f, 0f, 0f, 0.588f);
            btnQRcolorBlock.normalColor = new Color(0.682f, 0f, 0f, 0.588f);

            btnPhone.colors = btnPhonecolorBlock;
            btnEmail.colors = btnEmailcolorBlock;
            btnQR.colors = btnQRcolorBlock;
        }

        if (button == btnEmail) {
            btnPhonecolorBlock.normalColor = new Color(0.682f, 0f, 0f, 0.588f);
            btnEmailcolorBlock.normalColor = new Color(0.682f, 0f, 0f, 1f);
            btnQRcolorBlock.normalColor = new Color(0.682f, 0f, 0f, 0.588f);

            btnPhone.colors = btnPhonecolorBlock;
            btnEmail.colors = btnEmailcolorBlock;
            btnQR.colors = btnQRcolorBlock;
        }

        if (button == btnQR) {
            btnPhonecolorBlock.normalColor = new Color(0.682f, 0f, 0f, 0.588f);
            btnEmailcolorBlock.normalColor = new Color(0.682f, 0f, 0f, 0.588f);
            btnQRcolorBlock.normalColor = new Color(0.682f, 0f, 0f, 1f);

            btnPhone.colors = btnPhonecolorBlock;
            btnEmail.colors = btnEmailcolorBlock;
            btnQR.colors = btnQRcolorBlock;
        }

        button.GetComponentInChildren<Text>().color = Color.white;
    }

    public void SendPoster(string type) {
        // 화면 전환을 위한 오디오 중지
        StartCoroutine(UtilsScript.stopAudio(sendDataAudioKr));
        StartCoroutine(UtilsScript.stopAudio(sendDataAudioEn));

        // 버튼 효과음 출력
        StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

        // 전송 정보 확인 안내 멘트 출력
        StartCoroutine(UtilsScript.playAudio(checkAudioKr, checkAudioEn));

        if (type == "phone") {
            //PlayerPrefs.SetString("PHONE_NUMBER", Numberpanel.phoneNumberText.text);
            PlayerPrefs.SetString("PHONE_NUMBER", Numberpanel.phoneNumber);
            PlayerPrefs.SetString("EMAIL_ADDRESS", "");

            ConfirmPopupPhone.SetActive(true);

            ConfirmNumber.text = PlayerPrefs.GetString("PHONE_NUMBER");
        }

        if (type == "email") {
            PlayerPrefs.SetString("EMAIL_ADDRESS", Keyboardpanel.emailAddressText.text);
            PlayerPrefs.SetString("PHONE_NUMBER", "");

            ConfirmPopupEmail.SetActive(true);

            ConfirmEmail.text = PlayerPrefs.GetString("EMAIL_ADDRESS");
        }

        if (type == "qr") {
            ConfirmPopupQR.SetActive(true);
        }

        PlayerPrefs.SetString("sendType", type);
    }

    public void ReType(string type) {
        if (bSendScene) { // 인트로에서 사운드 제어위해 사용
            // 버튼 효과음 출력
            StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));
        }

        // 화면 전환을 위한 오디오 중지
        checkAudioKr.Stop();
        checkAudioEn.Stop();

        if (type == "phone" || type == "both") {
            ConfirmPopupPhone.SetActive(false);

            Numberpanel.phoneNumberText.text = "010 - ";
            Numberpanel.phoneNumber = "010 - ";
            Numberpanel.SetSendButton(false);
        }

        if (type == "email" || type == "both") {
            ConfirmPopupEmail.SetActive(false);

            Keyboardpanel.emailAddressText.text = "";
            Keyboardpanel.SetSendButton(false);
        }

        if (type == "qr" || type == "both") {
            ConfirmPopupQR.SetActive(false);
        }
    }

    public void Confirm() {
        // 버튼 효과음 출력
        StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

        // 프린터 출력 모드인 경우 에이전트 연결 여부 체크
        if (PlayerPrefs.GetString("pay_mode").Equals("t") || PlayerPrefs.GetString("pay_mode").Equals("r")) {
            if (SelectUIScript._netClient.status == false && SelectUIScript._netClient.isRetry == true) {  // 재연결 시도중
                reconnectPopup.SetActive(true);
                return;
            } else if (SelectUIScript._netClient.status == false && SelectUIScript._netClient.isRetry == false) { // 재연결 실패
                netClientErrorCanvas.SetActive(true);
                isReconnectPopup.SetActive(false);
                failReconnectPopup.SetActive(true);
                return;
            }
        }

        if (!sendButtonClickFlag) {
            FlowController.instance.Loading(true);

            Debug.Log("문자전송 함수 실행 : Confirm");

            StartCoroutine(Upload());
        }
    }

    public void confirmSendData() {
        // 버튼 효과음 출력
        StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

        if (!sendButtonClickFlag) {
            FlowController.instance.Loading(true);

            Debug.Log("문자전송 함수 실행 : Confirm");

            StartCoroutine(Upload());
        }
    }

    void SendPosterPrint() {
        string filepath = "";

        if (Application.isEditor) {
            filepath = Application.dataPath + "/1.jpg";
        } else {
            filepath = Path.GetFullPath(".") + "/photo/1.jpg";
        }

        AgentSendData sendData = new AgentSendData();
        sendData.Command = "print";
        sendData.FilePath = filepath;
        sendData.PrintCount = PlayerPrefs.GetString("count");
        if (PlayerPrefs.GetString("pay_mode").Equals("t")) {
            sendData.PrintCount = "1";
        }

        PlayerPrefs.SetString("count", "0");

        string payMessage = JsonUtility.ToJson(sendData);

        Debug.Log("SENDMESSAGE:" + payMessage);

        //출력 명령 전송
        SelectUIScript._netClient.SendMessage(payMessage);
    }

    void GotoEnd() {
        checkAudioKr.Stop();
        checkAudioEn.Stop();

        string paymode = PlayerPrefs.GetString("pay_mode");
        string price = PlayerPrefs.GetString("price");
        string count = PlayerPrefs.GetString("count");

        // 결제모드가 t인경우 포스터 무조건 출력
        if (paymode.Equals("t")) {
            SendPosterPrint();
        } else if (paymode.Equals("r")) {
            // 결제모드가 r인경우 유료 포스터만 출력
            if (price != string.Empty && int.Parse(count) > 0) {
                SendPosterPrint();
            }
        }

        PlayerPrefs.SetString("price", "");
        FlowController.instance.ChangeFlow(FlowController.instance.endCanvas);
    }

    IEnumerator Upload()
    {
        // 업로드 시작되면 플래그값 변경
        sendButtonClickFlag = true;

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        // 기본 정보
        string siteId = PlayerPrefs.GetString("site_id");
        string kioskId = PlayerPrefs.GetString("kiosk_id");
        string phone = Numberpanel.phoneNumber.Length > 16 ? Numberpanel.phoneNumber : "";
        string email = Keyboardpanel.emailAddressText.text;

        string productId = FlowController.instance.currentMovieId;

        // 약관 동의 내역 정보
        string agreeMarketing = PlayerPrefs.GetString("agree_marketing");

        phone = phone.Replace(" ", string.Empty);
        phone = phone.Replace("-", string.Empty);

        // 결제 정보
        string approvalNum = PlayerPrefs.GetString("approval_num");
        string approvalTime = PlayerPrefs.GetString("approval_time");
        string price = PlayerPrefs.GetString("price");
        int paymentType = PlayerPrefs.GetInt("payment_type");
        string printCount = PlayerPrefs.GetString("count");
        string bSmsSend = PlayerPrefs.GetString("bSmsSend");

        // 약관 동의 정보 초기화
        PlayerPrefs.SetString("agree_marketing", "");

        // 저장되어있는 사용자 정보 초기화
        PlayerPrefs.SetString("b_payment", "false");
        PlayerPrefs.SetString("approval_num", "");
        PlayerPrefs.SetString("approval_time", "");
        PlayerPrefs.SetString("payment_time", "");
        PlayerPrefs.SetInt("payment_type", 0);
        PlayerPrefs.SetString("bSmsSend", "");

        if (phone.Length != 0)
        {
            formData.Add(new MultipartFormDataSection("phone", phone));
        }

        if (email.Length != 0)
        {
            formData.Add(new MultipartFormDataSection("email", email));
        }
        
        formData.Add(new MultipartFormDataSection("kiosk_id", kioskId));
        formData.Add(new MultipartFormDataSection("site_id", siteId));
        formData.Add(new MultipartFormDataSection("product_id", productId));
        formData.Add(new MultipartFormDataSection("payment_type", paymentType.ToString()));

        if (agreeMarketing != string.Empty) // 선택 약관 동의 내역 있을 시 데이터 추가
        {
            formData.Add(new MultipartFormDataSection("agree_marketing", agreeMarketing));
            Debug.Log("agreeMarketing:" + agreeMarketing);
        }

        if (approvalNum != string.Empty)
        {
            formData.Add(new MultipartFormDataSection("approval_num", approvalNum));
            Debug.Log("approvalNum:" + approvalNum);
        }

        if (approvalTime != string.Empty)
        {
            formData.Add(new MultipartFormDataSection("approval_time", approvalTime));
            Debug.Log("approvalTime:" + approvalTime);
        }

        if (price != string.Empty)
        {
            formData.Add(new MultipartFormDataSection("price", price));
            Debug.Log("price:" + price);
        }

        if (printCount != string.Empty)
        {
            formData.Add(new MultipartFormDataSection("print_count", printCount));
            Debug.Log("printCount:" + printCount);
        }

        if (bSmsSend != string.Empty && bSmsSend == "true") {
            formData.Add(new MultipartFormDataSection("sms_send", bSmsSend));
            Debug.Log("printCount:" + printCount);
        }

        string filepath1 = Application.dataPath + "/1.mp4";
        string filepath2 = Application.dataPath + "/1.jpg";

        if (Application.isEditor)
        {
            filepath1 = Application.dataPath + "/1.mp4";
            filepath2 = Application.dataPath + "/1.jpg";
        }
        else
        {
            filepath1 = Path.GetFullPath(".") + "/photo/1.mp4";
            filepath2 = Path.GetFullPath(".") + "/photo/1.jpg";
        }

        byte[] img1 = File.ReadAllBytes(filepath1);
        byte[] img2 = File.ReadAllBytes(filepath2);

        formData.Add(new MultipartFormFileSection("mp4", img1, "1.mp4", "video/mp4"));
        formData.Add(new MultipartFormFileSection("jpg", img2, "1.jpg", "image/jpg"));

        // UnityWebRequest www = UnityWebRequest.Post(ConstantsScript.OPERATE_URL + "/file/upload/", formData);
        UnityWebRequest www = UnityWebRequest.Post(ConstantsScript.OPERATE_URL + "/file/upload/", formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);

            // 전송 실패시 재전송을 위해 플래그 변경
            FlowController.instance.Loading(false);
            sendButtonClickFlag = false;
            
            ReType("both");

            // 네트워크 에러 팝업 출력
            networkErrorPopup.SetActive(true);
        } else {
            Debug.Log("Form upload complete!");

            byte[] results = www.downloadHandler.data;

            string strJson = Encoding.Default.GetString(results);

            Debug.Log(strJson);

            JSONResult result = JsonUtility.FromJson<JSONResult>(strJson);
            Debug.Log("ID" + result.myposterid);
            Debug.Log("success" + result.success);
            Debug.Log("error" + result.error);
            Debug.Log("message" + result.message);
            Debug.Log("down_url" + result.url);

            // QR Code 생성을 위해 사용
            PlayerPrefs.SetString("down_url", result.url);

            sendButtonClickFlag = true;
            
            //문자 전송 완료되면 아래 함수 실행
            GotoEnd();
        }
    }

    IEnumerator SendSetting() {
        string siteId = PlayerPrefs.GetString("site_id");
 
        UnityWebRequest www = UnityWebRequest.Get(ConstantsScript.OPERATE_URL + "/site/get_send_setting/" + siteId);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            if (bSendScene) {   // 전송 화면인 경우 에러 팝업 출력
                // 네트워크 에러 팝업 출력
                networkErrorPopup.SetActive(true);
            } else {    // Config에서 실행한 경우 네트워크 체크 씬으로 전환
                FlowController.instance.ChangeFlow(FlowController.instance.networkCheckCanvas);
            }
        } else {
            Debug.Log("Get send setting complete!");

            byte[] results = www.downloadHandler.data;

            string strJson = Encoding.Default.GetString(results);

            Debug.Log(strJson);

            JSONResult result = JsonUtility.FromJson<JSONResult>(strJson);

            Debug.Log("email_send" + result.email_send);
            Debug.Log("mobile_send" + result.mobile_send);
            Debug.Log("success" + result.success);
            Debug.Log("error" + result.error);
            Debug.Log("message" + result.message);

            // 탭 좌표
            RectTransform emailTransform = btnEmail.gameObject.GetComponent<RectTransform>();
            RectTransform phoneTransform = btnPhone.gameObject.GetComponent<RectTransform>();
            RectTransform qrTransform = btnQR.gameObject.GetComponent<RectTransform>();
            Vector3 position = new Vector3();
            Vector2 size = new Vector2();

            // QR 사용 여부
            string isQRCode = PlayerPrefs.GetString("qrCode");

            // 탭 비활성화
            btnEmail.gameObject.SetActive(false);
            btnPhone.gameObject.SetActive(false);
            btnQR.gameObject.SetActive(false);

            if (result.email_send.Equals("Y") && result.mobile_send.Equals("Y")) {
                // 탭 전체 활성화
                btnEmail.gameObject.SetActive(true);
                btnPhone.gameObject.SetActive(true);

                if (isQRCode.Equals("true")) {  // QR Code 사용
                    btnQR.gameObject.SetActive(true);

                    // 이메일 탭 배치
                    position = emailTransform.anchoredPosition;
                    size = emailTransform.sizeDelta;
                    position.x = 0;
                    size.x = 360;
                    emailTransform.anchoredPosition = position;
                    emailTransform.sizeDelta = size;

                    // 휴대전화 탭 배치
                    position = phoneTransform.anchoredPosition;
                    size = phoneTransform.sizeDelta;
                    position.x = 720;
                    size.x = 360;
                    phoneTransform.anchoredPosition = position;
                    phoneTransform.sizeDelta = size;

                    // QR 탭 배치
                    position = phoneTransform.anchoredPosition;
                    size = phoneTransform.sizeDelta;
                    position.x = 360;
                    size.x = 360;
                    qrTransform.anchoredPosition = position;
                    qrTransform.sizeDelta = size;
                } else {
                    // 이메일 탭 배치
                    position = emailTransform.anchoredPosition;
                    size = emailTransform.sizeDelta;
                    position.x = 0;
                    size.x = 550;
                    emailTransform.anchoredPosition = position;
                    emailTransform.sizeDelta = size;

                    // 휴대전화 탭 배치
                    position = phoneTransform.anchoredPosition;
                    size = phoneTransform.sizeDelta;
                    position.x = 550;
                    size.x = 550;
                    phoneTransform.anchoredPosition = position;
                    phoneTransform.sizeDelta = size;
                }

                // 휴대폰 탭 클릭 - 기본값
                Btn_Tab(phoneTab);
                ChageColor(btnPhone);
            } else if (result.mobile_send.Equals("Y")) {
                //휴대전화 탭 활성화
                btnPhone.gameObject.SetActive(true);

                if (isQRCode.Equals("true")) {  // QR Code 사용
                    btnQR.gameObject.SetActive(true);

                    //휴대전화 탭 배치
                    position = phoneTransform.anchoredPosition;
                    size = phoneTransform.sizeDelta;
                    position.x = 0;
                    size.x = 550;
                    phoneTransform.anchoredPosition = position;
                    phoneTransform.sizeDelta = size;

                    // QR 탭 배치
                    position = phoneTransform.anchoredPosition;
                    size = phoneTransform.sizeDelta;
                    position.x = 550;
                    size.x = 550;
                    qrTransform.anchoredPosition = position;
                    qrTransform.sizeDelta = size;
                } else {
                    //휴대전화 탭 배치
                    position = phoneTransform.anchoredPosition;
                    size = phoneTransform.sizeDelta;
                    position.x = 0;
                    size.x = 1100;
                    phoneTransform.anchoredPosition = position;
                    phoneTransform.sizeDelta = size;
                }

                //휴대전화 탭 클릭
                Btn_Tab(phoneTab);
                ChageColor(btnPhone);
            } else if (result.email_send.Equals("Y")) {
                //이메일 탭 활성화
                btnEmail.gameObject.SetActive(true);

                if (isQRCode.Equals("true")) {  // QR Code 사용
                    //이메일 탭 배치
                    position = emailTransform.anchoredPosition;
                    size = emailTransform.sizeDelta;
                    position.x = 0;
                    size.x = 550;
                    emailTransform.anchoredPosition = position;
                    emailTransform.sizeDelta = size;

                    // QR 탭 배치
                    position = phoneTransform.anchoredPosition;
                    size = phoneTransform.sizeDelta;
                    position.x = 550;
                    size.x = 550;
                    qrTransform.anchoredPosition = position;
                    qrTransform.sizeDelta = size;
                } else { 
                    //이메일 탭 배치
                    position = emailTransform.anchoredPosition;
                    size = emailTransform.sizeDelta;
                    position.x = 0;
                    size.x = 1100;
                    emailTransform.anchoredPosition = position;
                    emailTransform.sizeDelta = size;
                }

                //이메일 탭 클릭
                Btn_Tab(emailTab);
                ChageColor(btnEmail);
            }
        }
    }

    public void cancelPopup(GameObject popup) {
        // 버튼 효과음 출력
        StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

        popup.SetActive(false);
    }

    public void SuccessCheckPrint() {
        Debug.Log("success check");
        bCheckPrint = true;
    }

    public void FailCheckPrint() {
        Debug.Log("fail check");
        bCheckPrint = false;
    }
}
