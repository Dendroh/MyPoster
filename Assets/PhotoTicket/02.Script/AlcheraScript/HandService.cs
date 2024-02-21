// ---------------------------------------------------------------------------
//
// Copyright (c) 2018 Alchera, Inc. - All rights reserved.
//
// This example script is under BSD-3-Clause licence.
//
//  Author      sg.ju@alcherainc.com
//
// ---------------------------------------------------------------------------
using System;
using UnityEngine;
using UnityEngine.Profiling;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Alchera
{
	/// <summary>
	/// 손 인식을 위한 클래스
	/// - 기본적으로 Init, Detect, Release 구조를 띄고 있습니다.
	/// - 초기에 Init 을 하기 위해 HandLib.Context에 각종 설정값을 설정한 뒤 초기화합니다.
	///       need3D가 항상 false이므로, IFit3D에 대한 연산은 하지 않습니다.
	/// - Detection은 TextureToImageData에서 얻은 ImageData를 이용하여 Detection을 수행한 뒤, 결과를 Translater에 전달합니다.
	/// - Task 방식을 이용하여, 비동기로 처리합니다. 즉 매 프레임마다 항상 handData를 반환한다고 보장할 수 없습니다.
	/// - Translator 에  handData[] 값들이 들어가있으며, 메모리 관리를 위하여 배열을 미리 할당하고 있으니,
	/// 적절한 값이 들어있는 count만큼 Fetch하여 사용합니다.
	/// </summary>
	public class HandService : MonoBehaviour, IDetectService
	{
		private struct Translator : ITranslator
		{
			internal UInt16 maxCount;
			internal UInt16 count;
			internal HandData[] storage;
			internal IFit3D fit3d;
			internal ProcParams fitParam;
			internal ProcCache[] cache;
			internal bool need3D;
			unsafe IEnumerable<T> ITranslator.Fetch<T>(IEnumerable<T> result)
			{
				// Only `HandData` can be fetched
				if (typeof(T) != typeof(HandData))
					throw new NotSupportedException(String.Format("Available types: `{0}`", typeof(HandData).FullName));

				Profiler.BeginSample("HandTranslator.Fetch");
				// apply 3D fitting for each hands if needed
				if (need3D)
				{
					fixed (HandData* hands = storage)
					{
						for (var i = 0; i < count; ++i)
							fit3d.Process((IntPtr)(hands + i), ref fitParam, ref cache[i]);
						for (var p = 0; p < HandData.NumPoints; ++p)
							hands[0].Points[p] *= 10;
					}
				}
				Profiler.EndSample();

				// segment of reserved storage
				return result = new ArraySegment<HandData>(storage, 0, count)
				    as IEnumerable<HandData>    // double casting. 
				    as IEnumerable<T>           // usually this is bad 
				    ;
			}

			void IDisposable.Dispose() { }
		}

		public int maxCount = 10; ///< 손을 인식할 수 있는 최대 갯수. 갯수에 제한은 없지만, 많은 얼굴을 인식할 시 성능이 저하되므로 한번에 촬영하는 인원수를 고려하여 세팅이 필요합니다. 사람은 손을 두개 가지고 있습니다. (inspector에서 변경)
		public bool need3D; ///< 마이포스터는 2D hand만 사용합니다. (inspector에서 false로 고정시킵니다.)

		TaskCompletionSource<ITranslator> promise;
		IFit3D fit3d;
		Translator translator;
		HandLib.Context context;

		public unsafe void Awake()
		{
			try
			{
				// 인터넷 연결이 안된 경우 알체라 내부 기능에서 어플리케이션 다운되는 현상 발생
				// 알체라 내부 기능에서 예외처리(Exception)가 안되기에 스킵 후 다음 로직에서 에러 처리
				if (!UtilsScript.checkNetwork())
				{  // 인터넷 연결이 안된 경우
					return;
				}

				context = default(HandLib.Context);
				context.maxCount = (UInt16)maxCount;
				context.resolution = HandInputResolution.RESOLUTION_1280X720_P;

				//currently, you should download model folder first  and paste it in that path;
				string str = Application.persistentDataPath + "/model";
				fixed (byte* path = context.modelPath.folderPath)
				{
					for (int i = 0; i < str.Length; i++)
					{
						path[i] = (byte)str[i];
					}
				}

				HandLib.Init(ref context);  // 인터넷 연결이 안된 경우 어플리케이션 다운 발생

				translator = default(Translator);
				translator.maxCount = (UInt16)maxCount;
				translator.count = 0;
				translator.storage = new HandData[translator.maxCount];
				translator.need3D = need3D;

				if (need3D)
				{
					translator.cache = new ProcCache[translator.maxCount];

					fit3d = translator.fit3d = HandLib.Fit3DProcessor(ref context);
					// set fitting parameters here
					HandLib.Set(ref translator.fitParam, 1280, 720, (UInt16)Screen.width, (UInt16)Screen.height);

					fit3d.Init(ref translator.fitParam);
				}
			} catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
			}
		}

		unsafe Task<ITranslator> IDetectService.DetectAsync(ref ImageData image)
		{
			Profiler.BeginSample("HandService.DetectAsync");

			// too small image
			if (image.WebcamWidth < 40)
			{
				Debug.LogWarning("FaceService.DetectAsync: image is too small");
				goto DetectDone;
			}
			if (promise != null)
				promise.TrySetCanceled();
			// the number of detected hands
			fixed (HandData* handPtr = translator.storage)
			{
				translator.count = (UInt16)HandLib.Detect(ref context, ref image, handPtr);
			}
		DetectDone:
			// return result in translator form
			promise = new TaskCompletionSource<ITranslator>();
			promise.SetResult(translator);

			Profiler.EndSample();
			return promise.Task;
		}

		void IDisposable.Dispose()
		{
			Debug.LogWarning("HandService.Dispose");

			HandLib.Release(ref context);
			if (need3D)
			{
				fit3d.Dispose();
			}
			if (promise == null)
				return;
			promise.TrySetCanceled();
			promise = null;
		}
	}

}