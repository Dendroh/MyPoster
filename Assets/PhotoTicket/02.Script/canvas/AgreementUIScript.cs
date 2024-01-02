using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

public class AgreementUIScript : MonoBehaviour, UIScript
{
    [SerializeField] GameObject ConfirmPopup;
    [SerializeField] Text details;
    [SerializeField] GameObject scrollView;
    [SerializeField] GameObject cancelButton;
    [SerializeField] GameObject cancelPaymentButton;
    [SerializeField] Toggle terms;
    [SerializeField] Toggle marketing;
    [SerializeField] Toggle agreeAll;
    [SerializeField] AudioSource agreementAudioKr;
    [SerializeField] AudioSource agreementAudioEn;
    [SerializeField] AudioSource cancelPaymentAudioKr;
    [SerializeField] AudioSource cancelPaymentAudioEn;
    [SerializeField] AudioSource buttonAudio;
    [SerializeField] AudioSource toggleAudio;
    [SerializeField] Image loadingGuide;
    [SerializeField] GameObject loadingProgress;
    [SerializeField] Sprite[] payGuideSprites;
    [SerializeField] SelectUIScript selectUIScript;


    // Start is called before the first frame update
    void Start()
    {
        terms.onValueChanged.AddListener(delegate {
            agreeValueChanged(terms);
        });
        marketing.onValueChanged.AddListener(delegate {
            agreeValueChanged(marketing);
        });
        agreeAll.onValueChanged.AddListener(delegate {
            agreeAllValueChanged(agreeAll);
        });

        PlayerPrefs.SetString("b_payment", "false");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void agreeValueChanged(Toggle toggle)
    {
        // 토글 클릭 효과음 출력
        StartCoroutine(UtilsScript.playEffectAudio(toggleAudio));

        // 약관 동의 해제 시 모두 동의 체크 해제
        if (!toggle.isOn) {
            agreeAll.isOn = false;
        }

        if (terms.isOn && marketing.isOn) {
            agreeAll.isOn = true;
        }
    }

    public void agreeAllValueChanged(Toggle toggle)
    {
        // 토글 클릭 효과음 출력
        StartCoroutine(UtilsScript.playEffectAudio(toggleAudio));

        if (toggle.isOn) {  // 모두 동의 체크
            terms.isOn = true;
            marketing.isOn = true;;
        } else {
            // 모두 동의 해제 - 약관 동의 일괄 해제
            // 약관 동의 해제 시 agreeValueChanged에 의해 모두 동의 체크 또한 해제됨 이때 불필요한 약관의 체크 해제 방지를 위해 조건 설정
            if (terms.isOn && marketing.isOn)
            {
                terms.isOn = false;
                marketing.isOn = false;
            }
        }
    }

    public void viewAgreement(int type)
    {
        if (type == 0) {    // 사진 전송 필수 동의
            if (scrollView.activeSelf == true && details.text ==  agreement(0))
            {
                scrollView.SetActive(false);
            }
            else
            {
                scrollView.SetActive(true);
                details.text = agreement(0);
            }
        } else if (type == 1) { // 마켓팅 활용 동의
            if (scrollView.activeSelf == true && details.text == agreement(1))
            {
                scrollView.SetActive(false);
            }
            else
            {
                scrollView.SetActive(true);
                details.text = agreement(1);
            }
        }
    }

    // public void viewDetails(GameObject scrollView)
    // {
    //     bool check = false;

    //     for(int i=0; i<groups.Length; i++)
    //     {
    //         {
    //             check = true;
    //             continue;
    //         }
    //     }

    //     scrollView.SetActive(!scrollView.activeSelf);
    // }

    public void agree()
    {
        if (!terms.isOn)
        {
            ConfirmPopup.SetActive(true);
            return;
        }

        if (marketing.isOn)
        {
            PlayerPrefs.SetString("agree_marketing", "Y");
        }

        // 퀴즈 모드이며, 홍보 화면을 사용하는 경우, 프로모션 화면으로 이동
        if (PlayerPrefs.GetString("quiz") == "true" && PlayerPrefs.GetString("promotion") == "true") {
            FlowController.instance.ChangeFlow(FlowController.instance.promotionCanvas);
        } else {
            FlowController.instance.ChangeFlow(FlowController.instance.sendCanvas);
        }
    }

    public string agreement(int type) {
        string siteId = PlayerPrefs.GetString("site_id");
        string url = ConstantsScript.OPERATE_URL + "/site/get_terms?id=" + siteId + "&type=" + type;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        Stream stream = response.GetResponseStream();
        StreamReader reader = new StreamReader(stream);

        string text = reader.ReadToEnd();
      
        JObject obj = JObject.Parse(text);

        string terms = obj["terms"].ToString();

        return terms;
    }

    public void Cancel()
    {
        // 버튼 효과음 출력
        StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

        FlowController.instance.ChangeFlow(FlowController.instance.resultCanvas);
    }

    public void CancelPayment() {
        // 버튼 효과음 출력
        StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

        // 화면 전환을 위한 오디오 중지
        StartCoroutine(UtilsScript.stopAudio(agreementAudioKr));
        StartCoroutine(UtilsScript.stopAudio(agreementAudioEn));

        string price = PlayerPrefs.GetString("price");

        Debug.Log("결제 취소! 취소할 금액 : " + price);

        // 결제 요청 시작
        if (int.Parse(price) > 0) { // 결제 내역 있을 때만 취소 가능
            FlowController.instance.Loading(true);
            loadingProgress.SetActive(true);
            SetLoadingGuide(0);  // 카드 삽입 UI 구성

            //큐 탐색 시작
            StartCoroutine(selectUIScript.CheckQueue());

            selectUIScript.sendType = "payment";
            selectUIScript.canvas = "agreement";

            AgentSendData sendData = new AgentSendData();

            sendData.Command = "payment_cancel";
            sendData.Amount = price.ToString();
            sendData.PaymentDate = PlayerPrefs.GetString("payment_time");
            sendData.PaymentAuthNum = PlayerPrefs.GetString("approval_num");

            string payMessage = JsonUtility.ToJson(sendData);

            Debug.Log("SENDMESSAGE:" + payMessage);

            //출력 명령 전송
            SelectUIScript._netClient.SendMessage(payMessage);
        }
    }

    public void Init() {
        // 화면 UI 설정
        ConfirmPopup.SetActive(false);
        scrollView.SetActive(false);
        terms.isOn = false;
        marketing.isOn = false;
        agreeAll.isOn = false;
        toggleAudio.enabled = true;
        cancelButton.SetActive(true);
        loadingGuide.gameObject.SetActive(true);
        FlowController.instance.Loading(false);

        if (PlayerPrefs.GetString("b_payment").Equals("true")) {    // 결제 처리가 된 경우
            cancelButton.SetActive(false);
            cancelPaymentButton.SetActive(true);
        }
        
        if (PlayerPrefs.GetString("marketingAgree").Equals("false")) { // 마케팅 동의 비할성
            // 화면 UI 재구성
            marketing.gameObject.SetActive(false);
            agreeAll.gameObject.SetActive(false);

            RectTransform scrollViewRect = scrollView.GetComponent<RectTransform>();
            scrollViewRect.sizeDelta = new Vector2(scrollViewRect.sizeDelta.x, scrollViewRect.sizeDelta.y + 200);
            scrollViewRect.anchoredPosition = new Vector2(scrollViewRect.anchoredPosition.x, scrollViewRect.anchoredPosition.y - 100);

            RectTransform termsRect = terms.GetComponent<RectTransform>();
            termsRect.anchoredPosition = new Vector2(termsRect.anchoredPosition.x, termsRect.anchoredPosition.y - 105);

        }

        // 약관 동의 안내 멘트 출력
        StartCoroutine(UtilsScript.playAudio(agreementAudioKr, agreementAudioEn));
    }

    public void Dispose()
    {
        // 화면 전환을 위한 오디오 중지
        StartCoroutine(UtilsScript.stopAudio(agreementAudioKr));
        StartCoroutine(UtilsScript.stopAudio(agreementAudioEn));
        StartCoroutine(UtilsScript.stopAudio(cancelPaymentAudioKr));
        StartCoroutine(UtilsScript.stopAudio(cancelPaymentAudioEn));

        if (PlayerPrefs.GetString("marketingAgree").Equals("false")) { // 마케팅 동의 비할성
            RectTransform scrollViewRect = scrollView.GetComponent<RectTransform>();
            scrollViewRect.sizeDelta = new Vector2(scrollViewRect.sizeDelta.x, scrollViewRect.sizeDelta.y - 200);
            scrollViewRect.anchoredPosition = new Vector2(scrollViewRect.anchoredPosition.x, scrollViewRect.anchoredPosition.y + 100);

            RectTransform termsRect = terms.GetComponent<RectTransform>();
            termsRect.anchoredPosition = new Vector2(termsRect.anchoredPosition.x, termsRect.anchoredPosition.y + 105);
        }

        toggleAudio.enabled = false;
    }

    public void SetLoadingProgress(bool active) {
        loadingProgress.SetActive(active);
    }

    public void SetLoadingGuide(int i) {
        loadingGuide.sprite = payGuideSprites[i];
    }

    public void SuccessCancel() {
        StartCoroutine(UtilsScript.playAudio(cancelPaymentAudioKr, cancelPaymentAudioEn));
        SetLoadingGuide(1);
        StartCoroutine(SuccessCoroutineCancel());
    }

    public void FailCancel() {
        SetLoadingGuide(2);
        StartCoroutine(FailCoroutineCancel());
    }

    public void cancelPopup(GameObject popup) {
        // 버튼 효과음 출력
        StartCoroutine(UtilsScript.playEffectAudio(buttonAudio));

        popup.SetActive(false);
    }

    IEnumerator SuccessCoroutineCancel() {
        //로딩바숨김
        Debug.Log("SuccessCoroutine:CancelPayment");

        PlayerPrefs.SetString("price", "");
        PlayerPrefs.SetString("count", "");
        PlayerPrefs.SetString("approval_num", "");
        PlayerPrefs.SetString("approval_time", "");
        PlayerPrefs.SetString("payment_time", "");
        PlayerPrefs.SetString("b_payment", "false");

        loadingProgress.SetActive(false);
        yield return new WaitForSeconds(2.2f);

        FlowController.instance.ChangeFlow(FlowController.instance.selectCanvas);

        yield break;
    }

    IEnumerator FailCoroutineCancel() {
        //로딩바숨김
        Debug.Log("FailCoroutine:");

        loadingProgress.SetActive(false);
        yield return new WaitForSeconds(2.3f);

        FlowController.instance.Loading(false);
        SetLoadingGuide(3);

        yield break;
    }
}