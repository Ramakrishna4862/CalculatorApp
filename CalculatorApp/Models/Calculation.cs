namespace CalculatorApp.Models
{
    public class Calculation
    {
        public int Id { get; set; }
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public string Operation { get; set; } = string.Empty;
        public double Result { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
