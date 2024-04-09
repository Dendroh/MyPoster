using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KisPayAgent
{
    class PaymentResponse
    {
        // 결제결과
        public string Result { get; set; }
        // 결제금액
        public string Price { get; set; }
        // 승인날짜
        public string ApprovalDate { get; set; }
        // 승인번호
        public string ApprovalNum { get; set; }
    }
}
