using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
class AgentSendData
{
    // 명령어
    public string Command;
    // 결제 금액
    public string Amount;
    // 결제 날짜(결제 취소시 사용)
    public string PaymentDate;
    // 결제 승인 번호(결제 취소시 사용)
    public string PaymentAuthNum;
    // 프린트 파일 경로
    public string FilePath;
    // 프린트 인쇄 수량
    public string PrintCount;
    // 결제결과
    public string Result;
}
