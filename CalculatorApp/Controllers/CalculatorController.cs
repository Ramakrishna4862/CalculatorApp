using CalculatorApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CalculatorApp.Data;

namespace CalculatorApp.Controllers
{

    public class CalculatorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalculatorController(ApplicationDbContext context)
        {
            _context = context;
        }


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
            //SaveHistory(calculator.Operation);
            Calculation calculation = new Calculation
            {
                Number1 = number1.Value,
                Number2 = number2.Value,
                Operation = "+",
                Result = calculator.Result,
                CreatedDate = DateTime.Now
            };
            _context.Calculations.Add(calculation);
            _context.SaveChanges();

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
            //SaveHistory(clculator.Operation);
            Calculation calculation = new Calculation
            {
                Number1 = number1.Value,
                Number2 = number2.Value,
                Operation = "-",
                Result = calculator.Result,
                CreatedDate = DateTime.Now
            };
            _context.Calculations.Add(calculation);
            _context.SaveChanges();

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
            //SaveHistory(calculator.Operation);
            Calculation calculation = new Calculation
            {
                Number1 = number1.Value,
                Number2 = number2.Value,
                Operation = "*",
                Result = calculator.Result,
                CreatedDate = DateTime.Now
            };
            _context.Calculations.Add(calculation);
            _context.SaveChanges();

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
            //SaveHistory(calculator.Operation);
            Calculation calculation = new Calculation
            { 
                Number1 = number1.Value,
                Number2 = number2.Value,
                Operation = "/",
                Result = calculator.Result,
                CreatedDate = DateTime.Now
            };
            _context.Calculations.Add(calculation);
            _context.SaveChanges();

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


        //public IActionResult History()
        //{ 
        //    CalculationHistory model = new CalculationHistory();
        //    model.Operations = GetHistory();
        //    return View(model);
        //}

        public IActionResult History()
        {
            List<Calculation> calculations = _context.Calculations.OrderByDescending(c => c.CreatedDate).ToList();
            return View(calculations); 
        }

        public IActionResult ClearHistory()
        {
            //HttpContext.Session.Remove("CalculationHistory");
            //return RedirectToAction("History");
            _context.Calculations.RemoveRange(_context.Calculations);
            _context.SaveChanges();
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

        public IActionResult Delete(int id)
        {
            Calculation? calculation = _context.Calculations.Find(id);
            if(calculation == null)
            {
                return NotFound();
            }
            _context.Calculations.Remove(calculation);
            _context.SaveChanges();
            return RedirectToAction("History");
        }

        //public IActionResult Edit(int id)
        //{
        //    Calculation? calculation = _context.Calculations.Find(id);
        //    if (calculation == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(calculation);
        //}

        [HttpPost]

        public IActionResult Edit(Calculation calculation)
        {
            Calculation? existingCalculation = _context.Calculations.Find(calculation.Id);
            if (existingCalculation == null)
            {
                return NotFound();
            }

            if (calculation.Operation == "/" && calculation.Number2 == 0)
            {
                ModelState.AddModelError("Number2", "Cannot divided by zero.");
                return View(calculation);
            }

            existingCalculation.Number1 = calculation.Number1;
            existingCalculation.Number2 = calculation.Number2;


            existingCalculation.Operation = calculation.Operation;

            if (calculation.Operation == "+")
            {
                existingCalculation.Result = calculation.Number1 + calculation.Number2;
            }
            else if (calculation.Operation == "-")
            {
                existingCalculation.Result = calculation.Number1 - calculation.Number2;
            }
            else if (calculation.Operation == "*")
            {
                existingCalculation.Result = calculation.Number1 * calculation.Number2;
            }
            else if(calculation.Operation == "/")
            {
                //if(calculation.Number2 == 0)
                //{
                //    return Content("Cannot divide by zero");
                //}
                existingCalculation.Result = (double)calculation.Number1 / calculation.Number2;
            }
                _context.SaveChanges();
            return RedirectToAction("History");
        }

        public IActionResult Edit(int id)
        {
            Calculation? calculation = _context.Calculations.Find(id);

            if (calculation == null)
            {
                return NotFound();
            }

            return View(calculation);
        }



    }
}
