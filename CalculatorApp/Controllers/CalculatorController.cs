using CalculatorApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CalculatorApp.Controllers
{
    public class CalculatorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private Calculator CreateCalculator(int? number1, int? number2)
        {
            return new Calculator
            {
                Number1 = number1,
                Number2 = number2
            };
        }

        



        //[HttpPost]
        public IActionResult Add(int? number1, int? number2)
        {
            //int result = number1 + number2;
            //return Content(result.ToString());
            //Calculator calculator = new Calculator();
            //calculator.Number1 = number1;
            //calculator.Number2 = number2;
            Calculator calculator = CreateCalculator(number1, number2);

            //if (number1 == null || number2 == null)
            if(!ValidateNumbers(calculator, number1, number2))
            {
                //calculator.ErrorMessage = "Number cannot be empty";
                return View("Index", calculator);
            }
            //else
            //{

                calculator.Result = number1.Value + number2.Value;
                calculator.Operation = $"{number1} + {number2} = {calculator.Result}";
                SaveHistory(calculator.Operation);
                calculator.IsCalculated = true;
                return View("Index", calculator);
            //}


        }
        //[HttpPost]
        public IActionResult Subtract(int? number1,int? number2)
        {
            //Calculator calculator = new Calculator();

            //calculator.Number1 = number1;
            //calculator.Number2 = number2;
            Calculator calculator = CreateCalculator(number1, number2);
            //if (number1 == null || number2 == null)
            if(!ValidateNumbers(calculator,number1,number2))
            {
                //calculator.ErrorMessage = "Number cannot be empty";
                return View("Index", calculator);
            }
            calculator.Result = number1.Value - number2.Value;
            calculator.Operation = $"{number1} - {number2} = {calculator.Result}";
            SaveHistory(calculator.Operation);
            calculator.IsCalculated = true;
            return View("Index", calculator);
        }
        public IActionResult Multiply(int? number1,int? number2)
        {
            //Calculator calculator = new Calculator();
            //calculator.Number1 = number1;
            //calculator.Number2 = number2;
            Calculator calculator = CreateCalculator(number1, number2);

            //if (number1 == null || number2 == null)
            if(!ValidateNumbers(calculator, number1, number2))
            {
                //calculator.ErrorMessage = "Number cannot be empty";
                return View("Index", calculator);
            }
            calculator.Result = number1.Value * number2.Value;
            calculator.Operation = $"{number1} * {number2} = {calculator.Result}";
            SaveHistory(calculator.Operation);
            calculator.IsCalculated = true;
            return View("Index", calculator);
        }

        public IActionResult Divide(int? number1,int? number2)
        {
            //Calculator calculator = new Calculator();
            //calculator.Number1 = number1;
            //calculator.Number2 = number2;
            Calculator calculator = CreateCalculator(number1, number2);

            //calculator.Result = number1 / number2;
            //if (number1 == null || number2 == null)
            if(!ValidateNumbers(calculator,number1,number2))
            {
                //calculator.ErrorMessage = "Number cannot be empty";
                return View("Index", calculator);
            }

            if(number2 == 0)
            {
                calculator.ErrorMessage = "Cannot divide by Zero";
                return View("Index", calculator);
            }

            calculator.Result = (double)number1.Value / number2.Value;
            calculator.Operation = $"{number1} / {number2} = {calculator.Result}";
            SaveHistory(calculator.Operation);
            calculator.IsCalculated = true;
            return View("Index", calculator);
            
        }
        public IActionResult Clear()
        {
            //Calculator calculator = new Calculator();
            //calculator.IsCalculated = false;
            //return View("Index", calculator);
            return View("Index", new Calculator());
        }

        private void SaveHistory(string operation)
        {
            List<string> history;
            string? historyJson = HttpContext.Session.GetString("CalculationHistory");
            if(historyJson == null)
            {
                history = new List<string>();
            }
            else
            {
                history = JsonSerializer.Deserialize<List<string>>(historyJson) ?? new List<string>();
            }

            history.Add(operation);

            if(history.Count > 10)
            {
                history.RemoveAt(0);
            }

            HttpContext.Session.SetString("CalculationHistory", JsonSerializer.Serialize(history));
        }

        private List<string> GetHistory()
        {
            string? historyJson = HttpContext.Session.GetString("CalculationHistory");
            if(string.IsNullOrEmpty(historyJson))
            {
                return new List<string>();
            }
            return JsonSerializer.Deserialize<List<string>>(historyJson) ?? new List<string>();
        }


        public IActionResult History()
        { 
            CalculationHistory model = new CalculationHistory();
            model.Operations = GetHistory();
            return View(model);
        }

        public IActionResult ClearHistory()
        {
            HttpContext.Session.Remove("CalculationHistory");
            return RedirectToAction("History");
        }

        private bool ValidateNumbers(Calculator calculator, int? number1,int? number2)
        {
            if(number1 == null || number2 == null)
            {
                calculator.ErrorMessage = "Number cannot be empty";
                return false;
            }
            return true;
        }


    }
}
