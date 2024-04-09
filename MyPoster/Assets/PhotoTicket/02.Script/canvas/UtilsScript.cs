using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alchera;
public class UtilsScript : MonoBehaviour, UIScript {

    void Start() {

    }

    void Update() {

    }

    public void Init() {

    }

    public void Dispose() {

    }

    // 운영음성 모드와 언어 모드를 확인하여 운영음성 사용할 때, 사용 언어를 반환해준다.
    // @return string
    public static string checkConfig() {
        if (PlayerPrefs.GetString("operateAudio") == "true") {  // 운영용 음성 사용
            if (PlayerPrefs.GetString("lang") == "kr") {    // 국문 모드
                return ConstantsScript.LANG_KR;
            } else {    // 영문 모드
                return ConstantsScript.LANG_EN;
            }
        }

        // 운영용 음성 사용X
        return "";
    }

    /**
    * 운영 모드에 따른 효과음 출력
    * @param effect
    * @return IEnumerator
    */
    public static IEnumerator playEffectAudio(AudioSource effect) {
        if (checkConfig() != null && checkConfig() != "") {
            effect.Play();
        }

        yield return null;
    }

    /**
     * 언어 모드에 따른 오디오 출력
     * @param audioKr   국문 음성
     * @param audioEn   영문 음성
     * @return IEnumerator
     */
    public static IEnumerator playAudio(AudioSource audioKr, AudioSource audioEn) {
        switch (checkConfig()) {
            case "kr": audioKr.Play(); break;
            case "en": audioEn.Play(); break;
        }

        yield return null;
    }

    /**
     * 화면 전환을 위한 오디오 중지
     * @param audio
     * @return IEnumerator
     */
    public static IEnumerator stopAudio(AudioSource audio) {
        audio.Stop();

        yield return null;
    }

    /**
     * 네트워크 연결 검사
     * @return bool
     */ 
    public static bool checkNetwork() {

        NetworkReachability reachability = Application.internetReachability;

        if (reachability == NetworkReachability.NotReachable) { // 인터넷 연결이 안된 경우
            return false;
        }

        return true;
    }
}
