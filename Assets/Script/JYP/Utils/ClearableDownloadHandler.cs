using System.Text;
using UnityEngine;

namespace Utils
{
    using UnityEngine.Networking;

    public class ClearableDownloadHandler : DownloadHandlerScript
    {
        private StringBuilder receivedData; // 수신된 데이터를 저장할 버퍼

        public ClearableDownloadHandler() : base()
        {
            receivedData = new StringBuilder();
        }

        // 수신 데이터 초기화
        public void ClearData()
        {
            receivedData.Clear();
            Debug.Log("Data cleared.");
        }

        // 수신 데이터를 반환
        public string GetData()
        {
            return receivedData.ToString();
        }

        // 데이터 수신 시 호출
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || dataLength == 0)
            {
                return false;
            }

            // 데이터를 버퍼에 추가
            receivedData.Append(Encoding.UTF8.GetString(data, 0, dataLength));
            return true;
        }

        // 모든 데이터 수신 완료 시 호출
        protected override void CompleteContent()
        {
        }
    }
}