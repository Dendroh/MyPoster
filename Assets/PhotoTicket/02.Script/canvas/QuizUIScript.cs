using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using Alchera;
using UnityEngine.Networking;

public class QuizUIScript : MonoBehaviour, UIScript
{
	[SerializeField] GameObject categoryPrefab;
	[SerializeField] GameObject quizPrefab;
	[SerializeField] GameObject answerPrefab;
	[SerializeField] GameObject backgroundText;
	[SerializeField] Transform categoryContent;
	[SerializeField] Transform quizContent;
	[SerializeField] GameObject quizObject;
	[SerializeField] GameObject errorPopup;
	[SerializeField] GameObject advCanvas;
	[SerializeField] Text errorText;
	[SerializeField] AudioSource buttonAudio;

	RectTransform[] categorytList;
	RectTransform[] quizList;
	RectTransform[] answerList;
	RectTransform[][] answerListByQuiz;
	ReadWebcamInSequence camReader;

	int categoryHeight = 369;
	int categorySpace = 76;
	int quizContentsSpace = 78;
	int addQuizContentLine;
	bool bSetQuiz = false;

	static int count = 0;
	static int quizCount = 0;
	static int chooseCount = 0;
	static int correctCount = 0;
	public static bool quizProcess = false;
	int completeCount = 0;
	// string operateUrl = "http://192.168.1.22";
	string operateUrl = "http://myposter.kr";

	void Start()
	{
		if (PlayerPrefs.GetString("quiz") == "true")
		{
			StartCoroutine(setQuizCategory());
			StartCoroutine(setQuiz());

			camReader = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ReadWebcamInSequence>();
			camReader.changeCameraState(false);
		}
	}

	public void Init()
	{
		advCanvas.gameObject.SetActive(false);  // 광고 화면 블라인드

		// 변수 초기화
		count = 0;

		if (!bSetQuiz)
		{
			errorPopup.SetActive(true);
		}

		quizProcess = false;
	}

	public void Dispose()
	{
		StartCoroutine(removeQuiz());  // prefab에 남아있는 퀴즈, 보기 삭제 -> 퀴즈 세팅 초기화를 위해서 동작
		StartCoroutine(setQuiz());  // 퀴즈 구성하기

		backgroundText.SetActive(true);    // UI 요소 활성화
		quizObject.SetActive(false); // 퀴즈 영역 비활성화
		errorPopup.SetActive(false);
		quizCount = 0;
		completeCount = 0;
	}

	/**
	 * 카테고리 구성하기
	 */
	IEnumerator setQuizCategory()
	{
		string siteId = PlayerPrefs.GetString("site_id");

		// 서버로부터 카테고리 정보 가져오기
		string url = operateUrl + "/kiosk/quiz_category/get_list?site_id=" + siteId;

		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();

		Stream stream = response.GetResponseStream();
		StreamReader reader = new StreamReader(stream);

		string text = reader.ReadToEnd();

		JObject obj = JObject.Parse(text);

		JArray array = new JArray();

		array = JArray.Parse(obj["list"].ToString());

		categorytList = new RectTransform[array.Count];

		// 카테고리 영역 화면 구성
		if (array.Count > 4)
		{  // 카테고리가 4개보다 많은 경우
			int indexCount = array.Count % 2 > 0 ? array.Count / 2 + 1 : array.Count / 2;
			// categoryContent.GetComponent<GridLayoutGroup>().padding.top = 60;
			categoryContent.GetComponent<RectTransform>().sizeDelta = new Vector2(928, categoryHeight * indexCount + categorySpace * (indexCount - 1));
		}

		for (int i = 0; i < array.Count; i++)
		{
			// 카테고리 prefabs 설정
			categorytList[i] = Instantiate(categoryPrefab, Vector3.zero, Quaternion.identity).GetComponent<RectTransform>();
			categorytList[i].transform.SetParent(categoryContent);

			categorytList[i].GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
			categorytList[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1);
			categorytList[i].Find("Text").GetComponent<Text>().text = array[i]["name"].ToString();

			string imageTexture = array[i]["pic"].ToString();
			string imageFilePath = operateUrl + obj["file_path"].ToString() + "/" + imageTexture;

			// 이미지 추가
			List<string> fileList = new List<string>();
			fileList.Add(imageFilePath);
			StartCoroutine(addImages(fileList, categorytList[i].Find("Image").GetComponent<RectTransform>()));  // 이미지 파일 생성

			// 버튼 이벤트 설정
			categorytList[i].GetComponent<CategoryController>().id = (int)array[i]["id"];
			categorytList[i].GetComponent<CategoryController>().init();
		}

		yield break;
	}

	/**
	 * 퀴즈 구성하기
	 */
	IEnumerator setQuiz()
	{
		// 서버로부터 퀴즈 정보 가져오기
		string url = operateUrl + "/kiosk/quiz/get_list?site_id=" + PlayerPrefs.GetString("site_id");

		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();

		Stream stream = response.GetResponseStream();
		StreamReader reader = new StreamReader(stream);

		string text = reader.ReadToEnd();

		JObject obj = JObject.Parse(text);
		JArray array = new JArray();

		if (obj["done"].ToString() == null || obj["done"].ToString().Equals(""))
		{    // 완료 설정이 없는 경우
			errorPopup.SetActive(true);
			errorText.text = "완료 화면 설정을 해주세요.";
			yield break;
		}

		if (obj["file_path"].ToString() == null || obj["file_path"].ToString().Equals(""))
		{    // 파일 경로 없는 경우
			errorPopup.SetActive(true);
			errorText.text = "파일 경로를 확인해 주세요.";
			yield break;
		}

		JObject doneArray = JObject.Parse(obj["done"].ToString());

		// 완료 화면 문구 또는 이미지 설정을 하지 않은 경우
		if (!(doneArray["desc"] != null && doneArray["desc"].ToString() != "")
		    || !(doneArray["pic"] != null && doneArray["pic"].ToString() != ""))
		{
			errorPopup.SetActive(true);
			errorText.text = "완료 화면 설정을 해주세요.";
			yield break;
		}


		// Quiz 결과 정보 구성
		PlayerPrefs.SetString("doneDesc", doneArray["desc"].ToString());
		PlayerPrefs.SetString("donePic", doneArray["pic"].ToString());
		PlayerPrefs.SetString("quizFilePath", obj["file_path"].ToString());
		PlayerPrefs.SetInt("criteria", doneArray["criteria"] != null && doneArray["criteria"].ToString() != "" ? (int)doneArray["criteria"] : 0);
		PlayerPrefs.SetString("successContent", doneArray["success_content"] != null && doneArray["success_content"].ToString() != "" ? doneArray["success_content"].ToString() : "0");
		PlayerPrefs.SetString("failureContent", doneArray["failure_content"] != null && doneArray["failure_content"].ToString() != "" ? doneArray["failure_content"].ToString() : "0");

		array = JArray.Parse(obj["list"].ToString());

		quizList = new RectTransform[array.Count];
		answerListByQuiz = new RectTransform[array.Count][];

		for (int j = 0; j < array.Count; j++)
		{
			addQuizContentLine = 0;

			// Quiz prefabs 설정
			quizList[j] = Instantiate(quizPrefab, Vector3.zero, Quaternion.identity).GetComponent<RectTransform>();
			quizList[j].transform.SetParent(quizContent);

			// Quiz UI 구성
			quizList[j].GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
			quizList[j].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1);
			quizList[j].GetComponent<QuizController>().categoryId = (int)array[j]["category_id"];
			quizList[j].GetComponent<QuizController>().quizIndex = j;
			quizList[j].Find("Quiz Contents").GetComponentInChildren<Text>().text = array[j]["contents"].ToString();

			// Quiz UI RectTransform
			RectTransform quizContentsRectTransform = quizList[j].Find("Quiz Contents").Find("Text").GetComponent<RectTransform>();
			RectTransform quizContentsTextRectTransform = quizList[j].Find("Quiz Contents").GetComponent<RectTransform>();
			RectTransform quizImageRectTransform = quizList[j].Find("Quiz Image").GetComponent<RectTransform>();
			RectTransform OButtonRectTransform = quizList[j].Find("O Button").GetComponent<RectTransform>();
			RectTransform XButtonRectTransform = quizList[j].Find("X Button").GetComponent<RectTransform>();
			RectTransform OButtonClickRectTransform = quizList[j].Find("O Button Click").GetComponent<RectTransform>();
			RectTransform XButtonClickRectTransform = quizList[j].Find("X Button Click").GetComponent<RectTransform>();
			RectTransform answerItemRectTransform = quizList[j].Find("Answer Item").GetComponent<RectTransform>();

			if (array[j]["contents"].ToString().Length > 26)
			{
				addQuizContentLine = 1;
			}

			// 퀴즈 Contents 길이에 따라 UI 재설정
			quizContentsRectTransform.sizeDelta = new Vector2(quizContentsRectTransform.sizeDelta.x, quizContentsRectTransform.sizeDelta.y + quizContentsSpace * addQuizContentLine);
			quizContentsTextRectTransform.sizeDelta = new Vector2(quizContentsTextRectTransform.sizeDelta.x, quizContentsTextRectTransform.sizeDelta.y + quizContentsSpace * addQuizContentLine);
			quizImageRectTransform.anchoredPosition = new Vector2(quizImageRectTransform.anchoredPosition.x, quizImageRectTransform.anchoredPosition.y - quizContentsSpace * addQuizContentLine);
			OButtonRectTransform.anchoredPosition = new Vector2(OButtonRectTransform.anchoredPosition.x, OButtonRectTransform.anchoredPosition.y - quizContentsSpace * addQuizContentLine);
			XButtonRectTransform.anchoredPosition = new Vector2(XButtonRectTransform.anchoredPosition.x, XButtonRectTransform.anchoredPosition.y - quizContentsSpace * addQuizContentLine);
			OButtonClickRectTransform.anchoredPosition = new Vector2(OButtonClickRectTransform.anchoredPosition.x, OButtonClickRectTransform.anchoredPosition.y - quizContentsSpace * addQuizContentLine);
			XButtonClickRectTransform.anchoredPosition = new Vector2(XButtonClickRectTransform.anchoredPosition.x, XButtonClickRectTransform.anchoredPosition.y - quizContentsSpace * addQuizContentLine);
			answerItemRectTransform.anchoredPosition = new Vector2(answerItemRectTransform.anchoredPosition.x, answerItemRectTransform.anchoredPosition.y - quizContentsSpace * addQuizContentLine);

			// 정답 팝업 UI 구성
			quizList[j].Find("Answer Popup").GetChild(0).Find("Answer Desc").GetComponentInChildren<Text>().text = array[j]["answer_desc"].ToString();

			// 다음 퀴즈 버튼 이벤트 설정
			quizList[j].Find("Answer Popup").GetChild(0).Find("Exit").GetComponent<QuizController>().exit();
			quizList[j].Find("Answer Popup").GetChild(0).Find("Next Quiz").GetComponent<QuizController>().quizIndex = j;
			quizList[j].Find("Answer Popup").GetChild(0).Find("Next Quiz").GetComponent<QuizController>().init();
			quizList[j].Find("Answer Popup").gameObject.SetActive(false);

			if (array[j]["file_list"] != null && array[j]["file_list"].Count() > 0)
			{    // 설정된 이미지 파일이 있으면 추가
				List<string> fileList = new List<string>();
				for (int i = 0; i < array[j]["file_list"].Count(); i++)
				{   // 파일 목록이 존재하는 경우
					string imageTexture = array[j]["file_list"][i]["name"].ToString();
					string imageFilePath = operateUrl + PlayerPrefs.GetString("quizFilePath") + "/" + imageTexture;
					fileList.Add(imageFilePath);
				}

				RectTransform rectTransform = quizList[j].Find("Quiz Image").GetComponent<RectTransform>();
				StartCoroutine(addImages(fileList, rectTransform));  // 이미지 파일 생성
			}

			if (array[j]["type"].ToString().Equals("0"))
			{  // O,X 퀴즈
				quizList[j].Find("Answer Item").gameObject.SetActive(false);
				quizList[j].Find("O Button").gameObject.SetActive(true);
				quizList[j].Find("X Button").gameObject.SetActive(true);

				// OX 버튼 이벤트 설정
				quizList[j].Find("O Button").GetComponent<AnswerController>().quizIndex = j;
				quizList[j].Find("O Button").GetComponent<AnswerController>().type = ConstantsScript.OX;
				quizList[j].Find("O Button").GetComponent<AnswerController>().ox = ConstantsScript.O;
				quizList[j].Find("X Button").GetComponent<AnswerController>().quizIndex = j;
				quizList[j].Find("X Button").GetComponent<AnswerController>().type = ConstantsScript.OX;
				quizList[j].Find("X Button").GetComponent<AnswerController>().ox = ConstantsScript.X;

				if (array[j]["answer"].ToString().Equals(ConstantsScript.WRONG_ANSWER))
				{   // 정답이 X인 경우
					quizList[j].Find("O Button").GetComponent<AnswerController>().isAnswer = ConstantsScript.WRONG_ANSWER;
					quizList[j].Find("X Button").GetComponent<AnswerController>().isAnswer = ConstantsScript.CORRECT_ANSWER;
				} else
				{    // 정답이 O인 경우
					quizList[j].Find("O Button").GetComponent<AnswerController>().isAnswer = ConstantsScript.CORRECT_ANSWER;
					quizList[j].Find("X Button").GetComponent<AnswerController>().isAnswer = ConstantsScript.WRONG_ANSWER;
				}

				quizList[j].Find("O Button").GetComponent<AnswerController>().init();
				quizList[j].Find("X Button").GetComponent<AnswerController>().init();
			} else
			{    // 객관식 퀴즈
				quizList[j].Find("Answer Item").gameObject.SetActive(true);
				quizList[j].Find("O Button").gameObject.SetActive(false);
				quizList[j].Find("X Button").gameObject.SetActive(false);

				if (array[j]["item_list"].Count() > 0)
				{    // 보기 목록이 있는 경우
					answerList = new RectTransform[array[j]["item_list"].Count()];
					answerListByQuiz[j] = new RectTransform[array[j]["item_list"].Count()];

					for (int k = 0; k < array[j]["item_list"].Count(); k++)
					{
						// 보기 prefabs 설정
						answerList[k] = Instantiate(answerPrefab, Vector3.zero, Quaternion.identity).GetComponent<RectTransform>();
						answerList[k].transform.SetParent(quizList[j].Find("Answer Item").GetChild(0));
						answerList[k].GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
						answerList[k].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1);

						answerList[k].Find("Circle Image").Find("Number").GetComponent<Text>().text = k + 1 + "";
						answerList[k].Find("Content").GetComponent<Text>().text = array[j]["item_list"][k]["contents"].ToString();

						// 보기 버튼 이벤트 설정
						answerList[k].GetComponent<AnswerController>().quizIndex = j;
						answerList[k].GetComponent<AnswerController>().answerIndex = k;
						answerList[k].GetComponent<AnswerController>().isAnswer = array[j]["item_list"][k]["is_answer"].ToString();
						if (array[j]["item_list"][k]["is_answer"].ToString().Equals(ConstantsScript.CORRECT_ANSWER))
						{
							quizList[j].GetComponent<QuizController>().answerNumber.Add(k + 1);
						}
						answerList[k].GetComponent<AnswerController>().type = ConstantsScript.MULTIPLE_CHOICE;
						answerList[k].GetComponent<AnswerController>().answerCount = array[j]["answer_count"].ToString();
						answerList[k].GetComponent<AnswerController>().init();

						answerListByQuiz[j][k] = new RectTransform();
						answerListByQuiz[j][k] = answerList[k];
					}
				} else
				{

				}
			}
		}

		bSetQuiz = true;

		yield break;
	}

	/**
	 * 카테고리 선택 후 문제 구성하기
	 * @param id        카테고리 id
	 */
	public void clickCategory(int id)
	{
		// 버튼 효과음 출력
		StartCoroutine(playEffectAudio(buttonAudio));

		backgroundText.SetActive(false);    // 불필요한 UI 요소 비활성화
		quizObject.SetActive(true); // 퀴즈 영역 활성화

		for (int i = 0; i < quizList.Length; i++)
		{
			if (quizList[i].GetComponent<QuizController>().categoryId == id)
			{  // 퀴즈의 카테고리 id와 같은 경우
				quizList[i].gameObject.SetActive(true); // 오브젝트 활성화
									// 퀴즈 순서 번호
				if (quizCount < 10)
				{
					quizList[i].Find("Quiz Number").GetComponent<Text>().text = "0" + (quizCount + 1);
				} else
				{
					quizList[i].Find("Quiz Number").GetComponent<Text>().text = "" + (quizCount + 1);
				}

				quizCount++;
			} else
			{
				quizList[i].gameObject.SetActive(false);
			}
		}

		for (int i = 0; i < quizList.Length; i++)
		{
			if (quizList[i].GetComponent<QuizController>().categoryId == id)
			{
				quizList[i].Find("Quiz Total Count").GetComponent<Text>().text = "/" + quizCount;   // UI 설정
			}
		}

		if (quizCount == 0)
		{   // 퀴즈가 없는 경우
			errorPopup.SetActive(true);
		}

		PlayerPrefs.SetInt("quizCount", quizCount);  // Quiz 개수 정보 저장

		quizProcess = true;
	}

	/**
	 * 보기 선택하기
	 * @param quizIndex
	 * @param isAnswer          정답 여부 0 오답, 1 정답
	 * @param answerIndex
	 * @param type              퀴즈 유형 0 OX퀴즈, 1 객관식
	 * @param answerCount       정답 개수
	 * @param ox                o, x
	 */
	public void clickAnswer(int quizIndex, string isAnswer, int answerIndex, string type, string answerCount, string ox)
	{
		// 버튼 효과음 출력
		StartCoroutine(playEffectAudio(buttonAudio));

		List<int> answerNumber = quizList[quizIndex].GetComponent<QuizController>().answerNumber;
		string correctText = "딩동댕!";
		string wrongText = "땡!";
		string middleText = " <size=45>정답</size> ";


		if (type.Equals(ConstantsScript.OX))
		{ // OX퀴즈
			if (ox.Equals(ConstantsScript.O))
			{
				quizList[quizIndex].Find("O Button Click").gameObject.SetActive(true);
			} else
			{
				quizList[quizIndex].Find("X Button Click").gameObject.SetActive(true);
			}

			// 정답 팝업 활성화
			quizList[quizIndex].Find("Answer Popup").gameObject.SetActive(true);

			if (isAnswer.Equals(ConstantsScript.CORRECT_ANSWER))
			{ // 정답인 경우
				quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Text").GetComponent<Text>().text = correctText + middleText + "<size=52><color=#00B050>" + ox.ToUpper() + "</color></size>";
				quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Image").gameObject.SetActive(true);
				quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Wrong Image").gameObject.SetActive(false);
				count++;    //  맞힌 개수 증가
			} else
			{
				if (ox.Equals(ConstantsScript.O))
				{
					quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Text").GetComponent<Text>().text = wrongText + middleText + "<size=52><color=#00B050>X</color></size>";
				} else
				{
					quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Text").GetComponent<Text>().text = wrongText + middleText + "<size=52><color=#00B050>O</color></size>";
				}
				quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Image").gameObject.SetActive(false);
				quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Wrong Image").gameObject.SetActive(true);
			}
		} else
		{    // 객관식
		     // 버튼 UI 구성
			answerListByQuiz[quizIndex][answerIndex].GetComponent<Button>().image.color = new Color32(0x00, 0xB0, 0x50, 0xFF);

			ColorBlock colors = answerListByQuiz[quizIndex][answerIndex].GetComponent<Button>().colors;
			answerListByQuiz[quizIndex][answerIndex].GetComponent<Button>().colors = colors;
			answerListByQuiz[quizIndex][answerIndex].Find("Content").GetComponent<Text>().color = Color.white;
			answerListByQuiz[quizIndex][answerIndex].Find("Circle Image").GetComponent<Image>().color = Color.white;
			answerListByQuiz[quizIndex][answerIndex].Find("Circle Image").Find("Number").GetComponent<Text>().color = Color.white;

			if (Convert.ToInt32(answerCount) == 1)
			{    // 정답이 1개인 경우
			     // 정답 팝업 활성화
				quizList[quizIndex].Find("Answer Popup").gameObject.SetActive(true);

				if (isAnswer.Equals(ConstantsScript.CORRECT_ANSWER))
				{ // 정답인 경우
				  // O, X 정답 표시
					quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Text").GetComponent<Text>().text = correctText;
					quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Image").gameObject.SetActive(true);
					quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Wrong Image").gameObject.SetActive(false);
					count++;    //  맞힌 개수 증가
				} else
				{    // 오답인 경우
				     // O, X 정답 표시
					quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Text").GetComponent<Text>().text = wrongText;
					quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Image").gameObject.SetActive(false);
					quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Wrong Image").gameObject.SetActive(true);
				}

				quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Text").GetComponent<Text>().text += middleText + "<color=#00B050>" + answerNumber[0] + "번</color>";
			} else if (Convert.ToInt32(answerCount) > 1)
			{   // 중복 정답인 경우
				chooseCount++;

				answerListByQuiz[quizIndex][answerIndex].GetComponent<Button>().interactable = false; // 선택한 보기 비활성화

				if (isAnswer.Equals(ConstantsScript.CORRECT_ANSWER))
				{ // 정답인 경우
					correctCount++;
				}

				if (chooseCount == Convert.ToInt32(answerCount))
				{
					// 정답 팝업 활성화
					quizList[quizIndex].Find("Answer Popup").gameObject.SetActive(true);

					if (correctCount == Convert.ToInt32(answerCount))
					{ // 정답인 경우
					  // O, X 정답 표시
						quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Text").GetComponent<Text>().text = correctText + middleText + "<color=#00B050>" + answerNumber[0] + "번</color>";
						quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Image").gameObject.SetActive(true);
						quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Wrong Image").gameObject.SetActive(false);
						count++;    //  맞힌 개수 증가
					} else
					{    // 오답인 경우
					     // O, X 정답 표시
						quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Text").GetComponent<Text>().text = wrongText + middleText + "<color=#00B050>" + answerNumber[0] + "번</color>";
						quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Image").gameObject.SetActive(false);
						quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Wrong Image").gameObject.SetActive(true);
					}

					for (int i = 1; i < quizList[quizIndex].GetComponent<QuizController>().answerNumber.Count(); i++)
					{
						quizList[quizIndex].Find("Answer Popup").Find("ConfirmPanel").Find("Correct Text").GetComponent<Text>().text += "<color=#00B050>, " + answerNumber[i] + "번</color>";
					}

					correctCount = 0;
					chooseCount = 0;
				}
			}
		}
	}

	/**
	 * 다음 퀴즈 선택하기
	 * @parma quizIndex
	 */
	public void clickNext(int quizIndex)
	{
		// 버튼 효과음 출력
		StartCoroutine(playEffectAudio(buttonAudio));

		if (completeCount < quizCount - 1)
		{ // 다음 문제가 있는 경우
		  // 다음 퀴즈 활성화
			quizList[quizIndex].gameObject.SetActive(false);
			completeCount++;
		} else
		{    // 다음 문제가 없을 때
			PlayerPrefs.SetInt("correctCount", count); // 맞힌 개수 정보 저장

			FlowController.instance.ChangeFlow(FlowController.instance.quizResultCanvas);   // 결과창으로 이동
		}
	}

	/**
	 * 서버로부터 이미지 정보 가져와 이미지 생성하기
	 * @param filePathList
	 * @param rectTransform  이미지 영역 크기
	 */
	IEnumerator addImages(List<string> filePathList, RectTransform rectTransform)
	{
		// 이미지 크기 설정
		float imageWidth = rectTransform.rect.width;
		float imageHeight = rectTransform.rect.height;

		for (int i = 0; i < filePathList.Count(); i++)
		{
			using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(filePathList[i]))
			{    // 서버로부터 이미지 정보 가져오기
				yield return unityWebRequest.SendWebRequest();
				if (unityWebRequest.result != UnityWebRequest.Result.ConnectionError)
				{  // 성공
					Texture2D texture = DownloadHandlerTexture.GetContent(unityWebRequest);

					float cropWidth;
					float cropHeight;

					float imageRatio = (float)texture.width / (float)texture.height; // 이미지 비율
													 // center crop and resize
					if (imageRatio > 1.0)
					{
						cropHeight = imageHeight;
						cropWidth = cropHeight * imageRatio;
					} else
					{
						cropWidth = imageWidth;
						cropHeight = cropWidth / imageRatio;
					}

					if (cropWidth < imageWidth)
					{
						float widthRatio = (float)imageWidth / (float)cropWidth;
						cropWidth = imageWidth;
						cropHeight = cropHeight * widthRatio;
					}

					if (cropHeight < imageHeight)
					{
						float heightRatio = (float)imageHeight / (float)cropHeight;
						cropHeight = imageHeight;
						cropWidth = cropWidth * heightRatio;
					}

					Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

					// 이미지 영역에 추가
					GameObject imageObject = new GameObject("Image");
					imageObject.transform.SetParent(rectTransform.transform, false);
					Image image = imageObject.AddComponent<Image>();
					image.rectTransform.sizeDelta = new Vector2(cropWidth, cropHeight);
					image.sprite = sprite;
				} else
				{
					Debug.LogError("Failed to load image from server: " + unityWebRequest.error);
				}
			}
		}
	}

	/*
	 * 팝업 닫기
	 * @parma popup
	 */
	public void cancelPopup(GameObject popup)
	{
		// 버튼 효과음 출력
		StartCoroutine(playEffectAudio(buttonAudio));

		popup.SetActive(false);
	}

	/**
	 * 종료하기. 처음으로 돌아가기
	 */
	public void exit()
	{
		// 인트로 화면 전환
		FlowController.instance.ChangeFlow(FlowController.instance.introCanvas);
	}

	IEnumerator removeQuiz()
	{
		if (quizList != null)
		{
			for (int i = 0; i < quizList.Count(); i++)
			{
				Destroy(quizList[i].gameObject);
			}

			quizList = null;
		}
		if (answerList != null && answerList.Count() > 0)
		{
			for (int i = 0; i < answerList.Count(); i++)
			{
				if (answerList[i].gameObject != null)
				{
					Destroy(answerList[i].gameObject);
				}
			}

			answerList = null;
		}

		yield break;
	}

	/**
	 * 운영 모드에 따른 효과음 출력
	 * @param effect
	 * @return IEnumerator
	 */
	IEnumerator playEffectAudio(AudioSource effect)
	{
		if (UtilsScript.checkConfig() != null && UtilsScript.checkConfig() != "")
		{
			effect.Play();
		}

		yield return null;
	}
}
