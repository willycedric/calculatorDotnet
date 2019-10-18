using System;

namespace calculatorService
{
    public class Calculator
    {
	public int add(int a, int b)
	{
		if(a == 0)
		  return 1 +b;
		else
		  return a +b;
	}
    }
}
