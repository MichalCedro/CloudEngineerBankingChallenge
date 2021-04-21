using System;

namespace LoanPayment
{
    class Program
    {
        static void Main()
        {

            // get user choise whether parameters should be configured
            var configureParametersChoise = ConfigureParametersChoise();


            // get configurable parameters from user or from defaults
            LoanConfigurableParameters loanConfigurableParameters;
            if (configureParametersChoise)
            {
                loanConfigurableParameters = GetLoanConfigurableParametersFromUser();
            }
            else
            {
                loanConfigurableParameters = GetDefaultLoanConfigurableParameters();
            }


            // get loan details
            (var loanAmount, var durationOfLoan) = GetLoanDetails(loanConfigurableParameters.PeriodType);


            // calculate payment overview
            (var monthlyPayment, var totalAmountPaidInInterestRate, var totalAdministrationFee) = CalculatePaymentOverview(loanConfigurableParameters, loanAmount, durationOfLoan);


            // display payment overview
            DisplayPaymentOverview(monthlyPayment, totalAmountPaidInInterestRate, totalAdministrationFee);

        }



        private static void DisplayPaymentOverview(decimal monthlyPayment, decimal totalAmountPaidInInterestRate, decimal totalAdministrationFee)
        {
            Console.WriteLine();
            Console.WriteLine("payment overview:");
            Console.WriteLine($"monthly payment: {monthlyPayment.ToString("#.##")}");
            Console.WriteLine($"total amount paid in interest rate: {totalAmountPaidInInterestRate.ToString("#.##")}");
            Console.WriteLine($"total amount paid in administrative fees : {totalAdministrationFee}");
            Console.WriteLine("ÅOP: planned in v2");
        }

        private static (decimal MonthlyPayment, decimal TotalAmountPaidInInterestRate,decimal TotalAdministrationFee) CalculatePaymentOverview(LoanConfigurableParameters loanConfigurableParameters, decimal loanAmount, int durationOfLoan)
        {
            var rateFactor = 1 + loanConfigurableParameters.AnnualInterestRate / 12;
            var monthlyPayment = loanAmount * (decimal)Math.Pow((double)rateFactor, durationOfLoan) * (rateFactor - 1) / (decimal)(Math.Pow((double)rateFactor, durationOfLoan) - 1);

            var totalAmountPaidInInterestRate = (monthlyPayment * durationOfLoan) - loanAmount;

            var administrationFeeBasedOnPersent = loanAmount * loanConfigurableParameters.PercentageAdministrationFee;
            var totalAdministrationFee = administrationFeeBasedOnPersent < loanConfigurableParameters.QuantitativeAdministrationFee
                ? administrationFeeBasedOnPersent : loanConfigurableParameters.QuantitativeAdministrationFee;

            return (monthlyPayment, totalAmountPaidInInterestRate, totalAdministrationFee);
        }

        private static (decimal LoanAmount,  int DurationOfLoa) GetLoanDetails(PeriodType periodType)
        {
            // set loan amount
            var loanAmount = 0M;
            var parsingLoanAmountSucceed = false;
            while (!parsingLoanAmountSucceed)
            {
                Console.WriteLine("Set loan amount");
                var loanAmountFromInput = Console.ReadLine();
                parsingLoanAmountSucceed = decimal.TryParse(loanAmountFromInput, out loanAmount);

                if (!parsingLoanAmountSucceed)
                    Console.WriteLine($"{loanAmountFromInput} is not correct value for loan amount");
            }

            // set duration of loan
            var durationOfLoan = 0;
            var parsingDurationOfLoanSucceed = false;
            while (!parsingDurationOfLoanSucceed)
            {
                var messageForPeriodType = "Set duration of loan" + (periodType == PeriodType.Month ? "(months)" : "(years)");
                Console.WriteLine(messageForPeriodType);
                var durationOfLoanFromInput = Console.ReadLine();
                parsingDurationOfLoanSucceed = int.TryParse(durationOfLoanFromInput, out durationOfLoan);

                if (!parsingDurationOfLoanSucceed)
                    Console.WriteLine($"{durationOfLoanFromInput} is not correct value for duration of loan");
            }

            if (periodType == PeriodType.Year)
                durationOfLoan = durationOfLoan * 12;

            return (loanAmount, durationOfLoan);
        }

        private static bool ConfigureParametersChoise()
        {
            Console.WriteLine("Do you want to configure parameters? y/n");
            var configureParameters = false;
            var parsingconfigureParametersChoise = false;
            while (!parsingconfigureParametersChoise)
            {
                var operationFromInput = Console.ReadKey().KeyChar;

                switch (operationFromInput)
                {
                    case 'y':
                        parsingconfigureParametersChoise = true;
                        configureParameters = true;
                        break;
                    case 'n':
                        parsingconfigureParametersChoise = true;
                        configureParameters = false;
                        break;
                    default:
                        parsingconfigureParametersChoise = false;
                        break;
                }

            }
            Console.WriteLine();
            return configureParameters;
        }

        private static LoanConfigurableParameters GetDefaultLoanConfigurableParameters()
            => new LoanConfigurableParameters(0.05M, PeriodType.Year, 0.01M, 10000M);


        private static LoanConfigurableParameters GetLoanConfigurableParametersFromUser()
        {
            //annualInterestRate
            decimal annualInterestRate = 0;
            var parsingAnnualInterestRateSucceed = false;
            while (!parsingAnnualInterestRateSucceed)
            {
                Console.WriteLine("Set annual interest rate (0-100)");
                var annualInterestRateFromInput = Console.ReadLine();
                parsingAnnualInterestRateSucceed = decimal.TryParse(annualInterestRateFromInput, out annualInterestRate);


                if (!parsingAnnualInterestRateSucceed || annualInterestRate < 0 || annualInterestRate > 100)
                {
                    parsingAnnualInterestRateSucceed = false;
                    Console.WriteLine($"{annualInterestRateFromInput} is not correct value for annual interest rate");
                }
                else
                {
                    annualInterestRate = annualInterestRate / 100;
                }
            }

            //periodType
            PeriodType periodType = PeriodType.Month;
            var parsingPeriodTypeSucceed = false;
            while (!parsingPeriodTypeSucceed)
            {
                Console.WriteLine("Set perid Type (1-month, 2-year)");
                var periodTypeFromInput = Console.ReadKey().KeyChar;
                Console.WriteLine();
                int periodTypeInt;
                parsingPeriodTypeSucceed = int.TryParse(periodTypeFromInput.ToString(), out periodTypeInt);

                if (!parsingPeriodTypeSucceed || (periodTypeInt != 1 && periodTypeInt != 2))
                {
                    parsingPeriodTypeSucceed = false;
                    Console.WriteLine($"{periodTypeFromInput} is not correct value for annual period Type");
                }
                else
                {
                    periodType = (PeriodType)periodTypeInt;
                }
            }


            //percentageAdministrationFee
            decimal percentageAdministrationFee = 0;
            var parsingPercentageAdministrationFeeSucceed = false;
            while (!parsingPercentageAdministrationFeeSucceed)
            {
                Console.WriteLine("Set percentage administration fee (0-100)");
                var percentageAdministrationFeeFromInput = Console.ReadLine();
                parsingPercentageAdministrationFeeSucceed = decimal.TryParse(percentageAdministrationFeeFromInput, out percentageAdministrationFee);

                if (!parsingPercentageAdministrationFeeSucceed || percentageAdministrationFee < 0 || percentageAdministrationFee > 100)
                {
                    parsingPercentageAdministrationFeeSucceed = false;
                    Console.WriteLine($"{percentageAdministrationFeeFromInput} is not correct value for percentage administration fee");
                }
                else
                {
                    percentageAdministrationFee = percentageAdministrationFee / 100;
                }
            }


            //quantitativeAdministrationFee
            decimal quantitativeAdministrationFee = 0;
            var parsingQuantitativeAdministrationFeeSucceed = false;
            while (!parsingQuantitativeAdministrationFeeSucceed)
            {
                Console.WriteLine("Set quantitative administratio fee");
                var quantitativeAdministrationFeeFromInput = Console.ReadLine();
                parsingQuantitativeAdministrationFeeSucceed = decimal.TryParse(quantitativeAdministrationFeeFromInput, out quantitativeAdministrationFee);

                if (!parsingPercentageAdministrationFeeSucceed)
                {
                    parsingQuantitativeAdministrationFeeSucceed = false;
                    Console.WriteLine($"{quantitativeAdministrationFeeFromInput} is not correct value for quantitative administratio fee");
                }
            }

            return new LoanConfigurableParameters(annualInterestRate, periodType, percentageAdministrationFee, quantitativeAdministrationFee);
        }

    }

    public class LoanConfigurableParameters
    {
        public LoanConfigurableParameters(decimal annualInterestRate, PeriodType periodType, decimal percentageAdministrationFee, decimal quantitativeAdministrationFee)
        {
            AnnualInterestRate = annualInterestRate;
            PeriodType = periodType;
            PercentageAdministrationFee = percentageAdministrationFee;
            QuantitativeAdministrationFee = quantitativeAdministrationFee;
        }

        public Decimal AnnualInterestRate { get; }
        public PeriodType PeriodType { get; }
        public Decimal PercentageAdministrationFee { get; }
        public Decimal QuantitativeAdministrationFee { get; }


    }

    public enum PeriodType
    {
        Month = 1,
        Year = 2
    }




}
