

namespace TransferService.Infrastructure.Response
{

    public class FraudResponse
    {
        public bool Success { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        public string TransactionId { get; set; }
        public string RiskLevel { get; set; }
        public int RiskScore { get; set; }
        public string[] RiskFactors { get; set; }
        public bool ShouldBlock { get; set; }
        public string[] Recommendations { get; set; }
        public object[] RequiredActions { get; set; }
        public int ProcessingTime { get; set; }
    }
}

