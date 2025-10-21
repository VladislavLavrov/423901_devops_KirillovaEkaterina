using System.ComponentModel.DataAnnotations;

namespace WebApplicationCalculator.Models
{
    public class CalculatorModel
    {
        [Display(Name = "Первое число")]
        public double FirstNumber { get; set; }

        [Display(Name = "Второе число")]
        public double SecondNumber { get; set; }

        [Display(Name = "Операция")]
        public string Operation { get; set; } = "+";

        [Display(Name = "Результат")]
        public double Result { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public List<string> AvailableOperations => new List<string>
        {
            "+", "-", "*", "/"
        };
    }
}
