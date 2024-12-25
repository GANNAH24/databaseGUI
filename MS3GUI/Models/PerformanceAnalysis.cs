//NEW PERFORMANCE ANALYSIS MODEL




namespace MS3GUI.Models
{
    public class PerformanceAnalysis
    {
        public string AssessmentTitle { get; set; }
        public int TotalMarks { get; set; }
        public int MarksScored { get; set; }
        public int Weightage { get; set; }
        public int PassingMarks { get; set; }
        public string Status { get; set; }
        public decimal Percentage { get; set; }
    }
}
