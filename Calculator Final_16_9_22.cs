using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator2
{
    public class Calculator
    {
        public void display()
        {
            Console.WriteLine("Enter the expression to be calculated:\n");
            string tempExpression = Console.ReadLine();
            string expression = String.Concat(tempExpression.Where(c => !Char.IsWhiteSpace(c)));

            List<string> expList = new List<string>();
            foreach (char c in expression)
            {
                expList.Add(c.ToString());
            }

            if (!validate(expression))
            {
                Console.WriteLine("Invalid expression! Expression can only contain 1234567890+-*/^%()");
                Console.WriteLine("Press e to exit. Press any other key to continue.");
                string cont = Console.ReadLine();
                if (cont == "e" || cont=="E") { }
                else { display(); }                              
            }

            else
            {    
                
                List<string> newLst = new List<string>();
                foreach (string s in expList) { newLst.Add(s); }
                newLst = joinNumbers(newLst);
                //foreach (string s in newLst) { Console.Write(s + ", "); } Console.WriteLine();
                newLst = joinDecimals(newLst);
                //foreach (string s in newLst) { Console.Write(s + ", "); }
                Console.WriteLine();
                newLst = joinNegative(newLst);
                foreach (string s in newLst) { Console.Write(s + ", "); }
                Console.WriteLine();
                newLst = solveBrackets(newLst);

                foreach (string s in newLst) { Console.Write(s + ", "); }
                double answer = calculate(newLst);
                
                Console.WriteLine("Answer: " + answer);

                Console.WriteLine("Continue? y/n");
                string cont = Console.ReadLine();
                if (cont == "y") { display(); }


            }

        }

        private Boolean validate(string expression)
        {
            string validNums = "0123456789";
            string operators = "+/*^%.";
            string validChars = validNums + operators + "-()";

            char[] tempArray = expression.ToCharArray();

            foreach (char s in operators)
            {
                if (expression.IndexOf(s) == 0 || expression.IndexOf(s) == expression.Length - 1)
                {
                    return false;
                }
            }

            if (tempArray[tempArray.Length - 1] == '-' || tempArray[tempArray.Length - 1] == '(') { return false; }

            if (tempArray[0] == ')') { return false; }

            for (int i = 0; i < expression.Length - 1; i++)
            {
                if (tempArray[i] == '+' || tempArray[i] == '-' || tempArray[i] == '*' || tempArray[i] == '/' || tempArray[i] == '^' || tempArray[i] == '%' || tempArray[i] == '.')
                {
                    if (tempArray[i + 1] == '+' || tempArray[i + 1] == '-'|| tempArray[i + 1] == '*' || tempArray[i + 1] == '/' || tempArray[i + 1] == '^' || tempArray[i + 1] == '%' || tempArray[i + 1] == '.')
                    {
                        return false;
                    }
                }
            }

            foreach (char s in tempArray)
            {
                if (validChars.IndexOf(s) == -1)
                {
                    return false;
                }
            }

            if (tempArray.Where(x => x.Equals('(')).Count() != tempArray.Where(x => x.Equals(')')).Count())
            {
                return false;
            }

            for(int i = 1; i < tempArray.Length-1; i++)
            {
                if(tempArray[i]=='.')
                {
                    if(!Double.TryParse(tempArray[i - 1].ToString(), out double a) || !Double.TryParse(tempArray[i + 1].ToString(), out double b))
                    {
                        return false;
                    }
                }
            }

            if (expression.IndexOf("---") >= 0) { return false; }

            return true;
        }

        public Boolean check(List<string> expList)
        {
            Boolean repeat = true;

            for (int i = 0; i < expList.Count - 1; i++)
            {

                string validNums = "0123456789";

                char temp = expList[i].ToCharArray()[0];
                char temp2 = expList[i + 1].ToCharArray()[0];
                if (validNums.IndexOf(temp) >= 0 && validNums.IndexOf(temp2) >= 0)
                {
                    repeat = true;
                    break;
                }
                else { repeat = false; }
            }

            return repeat;
        }

        public List<string> joinNumbers(List<string> expList)
        {
            if (check(expList))
            {
                for (int i = 0; i < expList.Count - 1; i++)
                {
                    double double1, double2;
                    if (double.TryParse(expList[i], out double1) && double.TryParse(expList[i + 1], out double2))
                    {
                        string temp = expList[i] + expList[i + 1];
                        double tempNum = double.Parse(temp);
                        expList[i] = temp;
                        expList.RemoveAt(i + 1);

                        if (check(expList)) { expList = joinNumbers(expList); }
                    }

                }

            }

            return expList;
        }

        public Boolean checkDecimals(List<string> expList)
        {
            foreach (string s in expList)
            {
                if (s == ".") { return true; }
            }

            return false;
        }

        public List<string> joinDecimals(List<string> expList)
        {
            if (checkDecimals(expList))
            {
                for (int i = 0; i < expList.Count - 1; i++)
                {
                    if (expList[i] == ".")
                    {
                        string temp = expList[i - 1] + "." + expList[i + 1];
                        expList[i - 1] = temp;
                        expList.RemoveAt(i);
                        expList.RemoveAt(i);

                        if (checkDecimals(expList)) { expList = joinDecimals(expList); }
                    }
                }
            }

            return expList;
        }

       
        public Boolean checkNegative(List<string> expList)
        {
            for (int i = 1; i < expList.Count-1; i++)
            {
                if (expList[i] == "-" && !double.TryParse(expList[i - 1], out double temp) && double.TryParse(expList[i +1], out double temp2)) {  return true; }
            }

            return false;
        }

        public List<string> joinNegative(List<string> expList)
        {
            if (expList[0] == "-" )
            {
                if(double.TryParse(expList[1], out double res))
                {
                    string temp = "-" + expList[1];
                    expList[0] = temp;
                    expList.RemoveAt(1);
                }
                else
                {
                    expList[0] = "-1";
                    expList.Add("temp");

                    for (int j = expList.Count - 1; j > 0; j--) { expList[j] = expList[j - 1]; }
                    expList[1] = "*";
                }
                
            }

            if (checkNegative(expList))
            {
                for (int i = 1; i < expList.Count - 1; i++)
                {
                    if (expList[i] == "-" && !double.TryParse(expList[i - 1], out double temp2) )
                    {
                        if(double.TryParse(expList[i + 1], out double res2))
                        {
                            expList[i] = "-" + expList[i + 1];
                            expList.RemoveAt(i + 1);
                        }
                        else
                        {
                            expList[i] = "-1";
                            expList.Add("temp");
                            
                            for(int j = expList.Count - 1; j >i; j--) { expList[j] = expList[j - 1]; }
                            expList[i + 1] = "*";
                        }

                        if (checkNegative(expList)) { expList = joinNegative(expList); }
                    }
                }

            }

            return expList;

        }


        public Boolean checkBrackets(List<string> expList)
        {
            Boolean brackets = false;

            foreach (string s in expList)
            {
                if (s == "(" || s == ")") { brackets = true; break; }
            }

            return brackets;

        }

        public void substring(List<string> expList, out List<string> subList, out int start, out int end)
        {
            subList = new List<string>();
            foreach (string s in expList) { subList.Add(s); }

            start = 0;
            end = expList.Count - 1;

            for (int i = 0; i < expList.Count - 1; i++)
            {
                if (expList[i] == ")") { end = i; break; }
            }

            for (int j = end - 1; j >= 0; j--)
            {
                if (expList[j] == "(") { start = j; break; }
            }

            subList.RemoveRange(end, expList.Count - end);
            subList.RemoveRange(0, start + 1);

        }

        public double calculate(List<string> expList)
        {
            try
            {
                if (expList.Count == 0) { return 0; }                
                else if (expList == null) { return 0; }
                else if (expList.Count == 1 ) { return double.Parse(expList[0]); }

                int i = expList.IndexOf("^");
                while (i > 0)
                {
                    double temp = Math.Pow(double.Parse(expList[i - 1]), double.Parse(expList[i + 1]));
                    expList[i - 1] = temp.ToString();
                    expList.RemoveAt(i);
                    expList.RemoveAt(i);
                    i = expList.IndexOf("^");
                }

                i = expList.IndexOf("%");
                while (i > 0)
                {
                    double temp = double.Parse(expList[i - 1]) % double.Parse(expList[i + 1]);
                    expList[i - 1] = temp.ToString();
                    expList.RemoveAt(i);
                    expList.RemoveAt(i);
                    i = expList.IndexOf("%");
                }

                i = expList.IndexOf("*");
                while (i > 0)
                {
                    double temp = double.Parse(expList[i - 1]) * double.Parse(expList[i + 1]);
                    expList[i - 1] = temp.ToString();
                    expList.RemoveAt(i);
                    expList.RemoveAt(i);
                    i = expList.IndexOf("*");
                }

                i = expList.IndexOf("/");
                while (i > 0)
                {
                    double temp = double.Parse(expList[i - 1]) / double.Parse(expList[i + 1]);
                    expList[i - 1] = temp.ToString();
                    expList.RemoveAt(i);
                    expList.RemoveAt(i);
                    i = expList.IndexOf("/");
                }

                i = expList.IndexOf("-");
                while (i > 0)
                {
                    double temp = double.Parse(expList[i - 1]) - double.Parse(expList[i + 1]);
                    expList[i - 1] = temp.ToString();
                    expList.RemoveAt(i);
                    expList.RemoveAt(i);
                    i = expList.IndexOf("-");
                }

                i = expList.IndexOf("+");
                while (i > 0)
                {
                    double temp = double.Parse(expList[i - 1]) + double.Parse(expList[i + 1]);
                    expList[i - 1] = temp.ToString(); Console.WriteLine("+: " + expList[i - 1]);
                    expList.RemoveAt(i);
                    expList.RemoveAt(i);
                    i = expList.IndexOf("+");
                }

                

                return double.Parse(expList[0]);
            }

            catch(FormatException f)
            {
                Console.WriteLine("Invalid expression! Expression can only contain 1234567890+-*/^%()");
                Console.WriteLine("Press e to exit. Press any other key to continue.");
                string cont = Console.ReadLine();
                if (cont == "e" || cont == "E") { }
                else { display(); }
                return 0;
            }
                       
        }

        public List<string> solveBrackets(List<string> expList)
        {
            List<string> tempList = new List<string>();
            foreach (string s in expList) { tempList.Add(s); }

            if (!checkBrackets(expList)) { return tempList; }
            else
            {
                substring(tempList, out tempList, out int firstInd, out int lastInd);
                double subAns = calculate(tempList);
                tempList.Clear();

                if (firstInd == 0)
                {
                    if (lastInd == expList.Count - 1)
                    {
                        tempList.Add(subAns.ToString());
                    }
                    else
                    {
                        tempList.Add(subAns.ToString());
                        if(double.TryParse(expList[lastInd+1],out double x)) 
                        {
                            if (x >= 0)
                            { tempList.Add("*"); }
                            else
                            { tempList.Add("+"); }
                        }
                        for (int i = lastInd + 1; i < expList.Count; i++) { tempList.Add(expList[i]); }
                    }
                }
                else
                {
                    if (lastInd == expList.Count - 1)
                    {
                        for (int i = 0; i < firstInd; i++) { tempList.Add(expList[i]); }
                        if (double.TryParse(expList[firstInd-1], out double y))
                        {
                            if (y >= 0)
                            { tempList.Add("*"); }
                            else
                            { tempList.Add("+"); }
                        }
                        tempList.Add(subAns.ToString());
                    }
                    else
                    {
                        for (int i = 0; i < firstInd; i++) { tempList.Add(expList[i]); }
                        if (double.TryParse(expList[firstInd - 1], out double a) )
                        {
                            if (a >= 0)
                            { tempList.Add("*"); }
                            else
                            { tempList.Add("+"); }
                        }
                        tempList.Add(subAns.ToString());
                        if (double.TryParse(expList[lastInd + 1], out double b))
                        {
                            if (b >= 0)
                            { tempList.Add("*"); }
                            else
                            { tempList.Add("+"); }
                        }
                        for (int i = lastInd + 1; i < expList.Count; i++) { tempList.Add(expList[i]); }
                    }
                }
            }

            tempList = solveBrackets(tempList);
            return tempList;
        }



    }

    class Program
    {
        static void Main(string[] args)
        {
            Calculator sample = new Calculator();
            sample.display();
            Console.Read();

        }
        
    }
}



