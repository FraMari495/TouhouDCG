using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Connecting
{
    public static class Client
    {

        public static IEnumerator GetTime(Action<string> onCompleted)
        {
            UnityWebRequest request = UnityWebRequest.Get("http://framariunity.php.xdomain.jp/Time/GetTime.php");
            yield return Send(onCompleted, request);
        }

        private static IEnumerator Send(Action<string> onCompleted, UnityWebRequest request)
        {
            request.timeout = 5;

            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    onCompleted(request.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("�C���^�[�l�b�g�ɐڑ��ł��܂���");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("�f�[�^�������G���[");
                    break;
                default:
                    Debug.LogError("�s���ȃG���[");
                    break;
            }

        }
    }


}