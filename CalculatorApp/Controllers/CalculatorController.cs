using CalculatorApp.Models;
using Microsoft.AspNetCore.Mvc;
using CalculatorApp.Data;

namespace CalculatorApp.Controllers
{

    public class CalculatorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

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

        private bool ValidateNumbers(Calculator calculator, int? number1, int? number2)
        {
            if (number1 == null || number2 == null)
            {
                calculator.ErrorMessage = "Number cannot be empty";
                return false;
            }

            return true;
        }



        [HttpPost]
        public IActionResult Add(int? number1, int? number2)
        {
            Calculator calculator = CreateCalculator(number1, number2);

            if (!ValidateNumbers(calculator, number1, number2))
            {
                return View("Index", calculator);
            }

            calculator.Result = CalculateResult(number1.Value, number2.Value, "+");
            calculator.Operation = $"{number1} + {number2} = {calculator.Result}";

            SaveCalculation(number1.Value, number2.Value, "+", calculator.Result);

            calculator.IsCalculated = true;
            return View("Index", calculator);



        }

        [HttpPost]
        public IActionResult Subtract(int? number1, int? number2)
        {
            Calculator calculator = CreateCalculator(number1, number2);
            if (!ValidateNumbers(calculator, number1, number2))
            {
                return View("Index", calculator);
            }
            calculator.Result = CalculateResult(number1.Value, number2.Value, "-"); 
            calculator.Operation = $"{number1} - {number2} = {calculator.Result}";
            SaveCalculation(number1.Value, number2.Value, "-", calculator.Result);

            calculator.IsCalculated = true;
            return View("Index", calculator);
        }

        [HttpPost]
        public IActionResult Multiply(int? number1, int? number2)
        {
            Calculator calculator = CreateCalculator(number1, number2);

            if (!ValidateNumbers(calculator, number1, number2))
            {
                return View("Index", calculator);
            }
            calculator.Result = CalculateResult(number1.Value, number2.Value, "*");
            calculator.Operation = $"{number1} * {number2} = {calculator.Result}";

            SaveCalculation(number1.Value, number2.Value, "*", calculator.Result);

            calculator.IsCalculated = true;
            return View("Index", calculator);
        }


        [HttpPost]
        public IActionResult Divide(int? number1, int? number2)
        {

            Calculator calculator = CreateCalculator(number1, number2);


            if (!ValidateNumbers(calculator, number1, number2))
            {
                return View("Index", calculator);
            }

            if (number2 == 0)
            {
                calculator.ErrorMessage = "Cannot divide by Zero";
                return View("Index", calculator);
            }

            calculator.Result = CalculateResult(number1.Value, number2.Value, "/");
            calculator.Operation = $"{number1} / {number2} = {calculator.Result}";

            SaveCalculation(number1.Value, number2.Value, "/", calculator.Result);

            calculator.IsCalculated = true;
            return View("Index", calculator);

        }
        public IActionResult Clear()
        {
            return View("Index", new Calculator());
        }






        public IActionResult History(
    string? searchText,
    string? operation,
    //string? sortColumn,
    //string? sortOrder,
    int page = 1)
        {
            if(page < 1)
            {
                page = 1;
            }

            var query = _context.Calculations.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c =>
                    c.Number1.ToString().Contains(searchText) ||
                    c.Number2.ToString().Contains(searchText));
            }

            if (!string.IsNullOrEmpty(operation))
            {
                query = query.Where(c => c.Operation == operation);
            }

            // Count filtered records
            int totalRecords = query.Count();

            // Pagination
            List<Calculation> calculations = query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(calculations);
        }



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
            existingCalculation.Result = CalculateResult(
    calculation.Number1,
    calculation.Number2,
    calculation.Operation
);


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

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Calculation? calculation = _context.Calculations.Find(id);
            if (calculation == null)
            {
                return NotFound();
            }
            _context.Calculations.Remove(calculation);
            _context.SaveChanges();
            return RedirectToAction("History");



        }
        private void SaveCalculation(int number1, int number2, string operation, double result)
        {
            Calculation calculation = new Calculation
            {
                Number1 = number1,
                Number2 = number2,
                Operation = operation,
                Result = result,
                CreatedDate = DateTime.Now
            };

            _context.Calculations.Add(calculation);
            _context.SaveChanges();
        }

        private double CalculateResult(int number1, int number2, string operation)
        {
            if (operation == "+")
                return number1 + number2;
            if (operation == "-")
                return number1 - number2;
            if (operation == "*")
                return number1 * number2;
            if (operation == "/")
                return (double)number1 / number2;
            throw new ArgumentException("Invalid operation.");
        }
    }
}
