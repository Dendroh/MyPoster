using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class PaymentResponse
{
    // 결제결과
    public string Result;
    // 결제금액
    public string Price;
    // 승인날짜
    public string ApprovalDate;
    // 승인번호
    public string ApprovalNum;
}