using NUnit.Framework;
using calculatorService;

namespace Tests
{

    [TestFixture]
    public class Tests
    {
       

        [TestCase(0,0)]
        [TestCase(-1,5)]
        [TestCase(9,12)]
        public void Test1(int firstValue, int secondValue)
        {
           Calculator calculator = CreateCalculatorService();
           var result = calculator.add(firstValue, secondValue);
           Assert.AreEqual(result, firstValue+secondValue, $"{result} does not match the expected {firstValue + secondValue}");
        }
    
      private Calculator CreateCalculatorService(){
          return new Calculator();
      }
    }
}
