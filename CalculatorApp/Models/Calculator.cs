namespace CalculatorApp.Models
{
    public class Calculator
    {
        public int? Number1 { get; set; }
        public int? Number2 { get; set; }
        public double Result { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsCalculated { get; set; }
        public string? Operation { get; set; }
    }
}
