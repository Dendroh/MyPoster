using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KisPayAgent
{
	class AgentSendData
	{
		// 명령어
		public string Command { get; set; }
		// 결제 금액
		public string Amount { get; set; }
		// 결제 날짜(결제 취소시 사용)
		public string PaymentDate { get; set; }
		// 결제 승인 번호(결제 취소시 사용)
		public string PaymentAuthNum { get; set; }
		// 프린트 파일 경로
		public string FilePath { get; set; }
		// 프린트 인쇄 수량
		public string PrintCount { get; set; }
		// 프린트 타입 선택
		public string PrintType { get; set; }

	}
}