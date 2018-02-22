using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3
{
    /// <summary>
    /// Provides a cache of the Units table and methods for converting Units
    /// </summary>
    public class UnitManager : DependencyObject
    {
        private UnitsAdapter unitsAdapter = new UnitsAdapter();

        /// <summary>Dictionary of Units in the database keyed by their ID</summary>
        public Dictionary<int, Unit> UnitList
        {
            get { return (Dictionary<int, Unit>)GetValue(UnitListProperty); }
            set { SetValue(UnitListProperty, value); }
        }

        /// <summary>Backing for <see cref="UnitList"/></summary>
        public static readonly DependencyProperty UnitListProperty =
            DependencyProperty.Register("UnitList", typeof(Dictionary<int, Unit>), typeof(UnitManager), new PropertyMetadata(null));


        /// <summary>Dictionary of units keyed by name</summary>
        public Dictionary<string, Unit> UnitNameMap;

        /// <summary>Dictionary of units keyed by abbreviation</summary>
        public Dictionary<string, Unit> UnitAbbrevMap;

        /// <summary>Create a new instance of the manager</summary>
        public UnitManager()
        {
            
        }

        /// <summary>Convert an amount in one unit to its equivalent in another unit</summary>
        /// <param name="amount">Amount in the source unit</param>
        /// <param name="sourceID">ID of the source unit</param>
        /// <param name="targetID">ID of the target unit</param>
        /// <exception cref="UnitNotFoundException">If the source or target units do not exist</exception>
        /// <exception cref="InvalidUnitException">
        /// If source or target unit are malformed or incompatible
        /// </exception>
        /// <returns>Equivalent amount in the target unit</returns>
        public Fraction Convert(Fraction amount, int sourceID, int targetID)
        {
            if (!UnitList.TryGetValue(sourceID, out Unit source))
                throw new UnitNotFoundException("Source unit was not found in the unit table");

            if (!UnitList.TryGetValue(targetID, out Unit target))
                throw new UnitNotFoundException("Target unit was not found in the unit table");

            return Convert(amount, source, target);
        }

        /// <summary>Convert an amount in one unit to its equivalent in another unit</summary>
        /// <param name="amount">Amount in the source unit</param>
        /// <param name="source">source unit</param>
        /// <param name="target">target unit</param>
        /// <exception cref="InvalidUnitException">
        /// If source or target unit are malformed or incompatible
        /// </exception>
        /// <returns>Equivalent amount in the target unit</returns>
        private Fraction Convert(Fraction amount, Unit source, Unit target)
        {
            if (source == null || target == null) throw new ArgumentNullException();

            if (source.U_Ratio <= 0F) throw new InvalidUnitException("Source unit does not have a valid ratio");
            if (target.U_Ratio <= 0F) throw new InvalidUnitException("Target unit does not have a valid ratio");

            if (source.U_TypeCode != target.U_TypeCode)
                throw new InvalidUnitException(
                    String.Format(
                        "Source and Target unit types are incompatible: {0}->{1}",
                        source.U_TypeCode.GetString(),
                        target.U_TypeCode.GetString()
                        )
                    );

            return amount * (source.U_Ratio / target.U_Ratio);
        }


        /// <summary>
        /// Gets a string representation of an amount in the specified system,
        /// or the source system if no conversion exists or <see cref="Unit.System.Any"/> is specified
        /// </summary>
        /// <param name="amount">Amount in the source unit</param>
        /// <param name="sourceID">ID of the source unit</param>
        /// <param name="targetSystem">Desired system for the output</param>
        /// <exception cref="UnitNotFoundException">If the given source unit does not exist</exception>
        /// <returns>A string containing an amount with units</returns>
        public string GetString(Fraction amount, int sourceID, Unit.System targetSystem)
        {
            if (!UnitList.TryGetValue(sourceID, out Unit sourceUnit))
                throw new UnitNotFoundException("Source unit was not found in the unit table");

            return GetString(amount, sourceUnit, targetSystem);
        }

        /// <summary>
        /// Gets a string representation of an amount in the specified system,
        /// or the source system if no conversion exists or <see cref="Unit.System.Any"/> is specified
        /// </summary>
        /// <param name="amount">Amount in the source unit</param>
        /// <param name="sourceUnit">Source unit</param>
        /// <param name="targetSystem">Desired system for the output</param>
        /// <returns>A string containing an amount with units</returns>
        public string GetString(Fraction amount, Unit sourceUnit, Unit.System targetSystem)
        {
            if (targetSystem == Unit.System.Any)
            {
                targetSystem = sourceUnit.U_System;
            }

            switch (targetSystem)
            {
                case Unit.System.Customary:
                    return GetUSString(amount, sourceUnit);
                case Unit.System.Metric:
                    return GetMetricString(amount, sourceUnit);
                default:
                    if (sourceUnit.U_System == Unit.System.Customary)
                        return GetUSString(amount, sourceUnit);
                    else
                        return GetMetricString(amount, sourceUnit);
            }
        }

        /// <summary>Attempt to get a string representation of an amount converted to US units</summary>
        /// <param name="amount">Amount in source unit</param>
        /// <param name="sourceID">ID of source unit</param>
        /// <returns>A string containing an amount and unit in US units if possible, otherwise the source unit</returns>
        /// <exception cref="UnitNotFoundException">If the given source unit does not exist</exception>
        public string GetUSString(Fraction amount, int sourceID)
        {
            if (!UnitList.TryGetValue(sourceID, out Unit sourceRow)) throw new UnitNotFoundException("Source unit was not found in the unit table");

            return GetUSString(amount, sourceRow);
        }

        /// <summary>Attempt to get a string representation of an amount converted to US units</summary>
        /// <param name="amount">Amount in source unit</param>
        /// <param name="sourceRow">Source unit</param>
        /// <returns>A string containing an amount and unit in US units if possible, otherwise the source unit</returns>
        public string GetUSString(Fraction amount, Unit sourceRow)
        {
            string outputStr = null;

            if (sourceRow.U_System == Unit.System.Customary)
            {
                // already in US units
                outputStr = amount.ToString() + " " + sourceRow.U_Abbreviation;
                return outputStr;
            }

            //var rows = _UnitsTable.Select("U_System = 'USC' AND U_Typecode = " + sourceRow.U_Typecode + " OR U_ID = " + sourceRow.U_ID, "U_Ratio ASC");
            var rows = UnitList
                .Where(
                    p => (p.Value.U_System == Unit.System.Customary &&
                          p.Value.U_TypeCode == sourceRow.U_TypeCode) ||
                          p.Key == sourceRow.ID
                )
                .Select(p => p.Value)
                .OrderBy(p => p.U_Ratio)
                .ToList();

            if (rows.Count <= 1)
            {
                return amount.ToString() + " " + sourceRow.U_Abbreviation;
            }
            else
            {
                int sourceIndex = -1;
                int i = 0;

                foreach (Unit row in rows)
                {
                    if (row.U_ID == sourceRow.U_ID) sourceIndex = i;
                    else i++;
                }

                if (sourceIndex == -1)
                {
                    // oh no
                }
                else
                {
                    Unit target = null;
                    int targetIndex = 0;

                    if (sourceIndex == 0)
                    {
                        // no smaller, use larger
                        targetIndex = 1;
                    }
                    else if (sourceIndex > 0 && sourceIndex < rows.Count)
                    {
                        // use smaller
                        targetIndex = sourceIndex - 1;
                    }

                    target = rows[targetIndex];

                    Fraction newAmount = Convert(amount, sourceRow, target);
                    Fraction fPart = newAmount.FractionPart;

                    string targetAbbrev = target.U_Abbreviation;

                    if (fPart > 0)
                    {
                        // not exact

                        bool smallerUnitFound = false;
                        Fraction subAmount = Fraction.Zero;
                        string subTargetAbbrev = null;

                        int startIndex = Math.Min(sourceIndex, targetIndex) - 1;
                        startIndex = (startIndex >= 0) ? startIndex : 0;

                        for (int j = startIndex; j >= 0 && !smallerUnitFound; j--)
                        {
                            // traverse back down the list to find the sub unit
                            if (rows[j] is Unit subTarget)
                            {
                                try
                                {
                                    subAmount = Convert(fPart, target, subTarget);

                                    if (subAmount > 1)
                                    {
                                        smallerUnitFound = true;
                                        subTargetAbbrev = subTarget.U_Abbreviation;
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                        }

                        

                        if (smallerUnitFound)
                        {
                            subAmount = RoundFractionToClosest(subAmount, "1/8", "1/3");

                            outputStr = String.Format(
                                "{0} {1}, {2} {3}",
                                newAmount.IntegerPart,
                                targetAbbrev,
                                subAmount.Simplify(),
                                subTargetAbbrev);
                        }
                        else
                        {
                            newAmount = RoundFractionToClosest(newAmount, "1/8", "1/3");

                            outputStr = String.Format(
                                "{0} {1}",
                                newAmount.Simplify(),
                                targetAbbrev);
                        }
                    }
                    else
                    {
                        // no fractional part
                        newAmount = RoundFractionToClosest(newAmount, "1/8", "1/3");

                        outputStr = String.Format(
                            "{0} {1}",
                            newAmount.Round("1/8").Simplify(),
                            targetAbbrev);
                    }
                    
                }
            }
            

            return outputStr;
        }

        private static Fraction RoundFractionToClosest(Fraction amount, Fraction precisionA, Fraction precisionB)
        {
            Fraction roundedA = amount.Round(precisionA);
            Fraction roundedB = amount.Round(precisionB);

            if (Math.Abs((roundedA - amount).ApproximateValue) < Math.Abs((roundedB - amount).ApproximateValue))
                return roundedA;
            else
                return roundedB;
        }

        /// <summary>Attempt to get a string representation of an amount converted to metric units</summary>
        /// <param name="amount">Amount in source unit</param>
        /// <param name="sourceID">ID of source unit</param>
        /// <returns>A string containing an amount and unit in metric units if possible, otherwise the source unit</returns>
        /// <exception cref="UnitNotFoundException">If the given source unit does not exist</exception>
        public string GetMetricString(Fraction amount, int sourceID)
        {
            if (!UnitList.TryGetValue(sourceID, out Unit sourceRow)) throw new UnitNotFoundException("Source unit was not found in the unit table");

            return GetMetricString(amount, sourceRow);
        }

        /// <summary>Attempt to get a string representation of an amount converted to metric units</summary>
        /// <param name="amount">Amount in source unit</param>
        /// <param name="sourceRow">Source unit</param>
        /// <returns>A string containing an amount and unit in metric units if possible, otherwise the source unit</returns>
        public string GetMetricString(Fraction amount, Unit sourceRow)
        {
            string outputStr = null;

            if (sourceRow.U_System == Unit.System.Metric)
            {
                // already in metric units
                outputStr = Math.Round(amount.ApproximateValue, 3).ToString("0.###") + " " + sourceRow.U_Abbreviation;
                return outputStr;
            }

            //var rows = _UnitsTable.Select("U_System = 'MET' AND U_Typecode = " + sourceRow.U_Typecode + " OR U_ID = " + sourceRow.U_ID, "U_Ratio ASC");
            var rows = UnitList
                .Where(
                    p => (p.Value.U_System == Unit.System.Metric &&
                          p.Value.U_TypeCode == sourceRow.U_TypeCode) ||
                          p.Key == sourceRow.ID
                )
                .Select(p => p.Value)
                .OrderBy(p => p.U_Ratio)
                .ToList();

            if (rows.Count <= 1)
            {
                outputStr = Math.Round(amount.ApproximateValue, 3).ToString("0.###");
            }
            else
            {
                int sourceIndex = -1;
                int i = 0;

                foreach (Unit unit in rows)
                {
                    if (unit.U_ID == sourceRow.U_ID) sourceIndex = i;
                    else i++;
                }

                if (sourceIndex == -1)
                {
                    // oh no
                }
                else
                {
                    Unit target = null;

                    if (sourceIndex == 0)
                    {
                        // no smaller, use larger
                        target = rows[1];
                    }
                    else if (sourceIndex > 0 && sourceIndex < rows.Count)
                    {
                        // use smaller
                        target = rows[sourceIndex - 1];
                    }

                    Fraction newAmount = Convert(amount, sourceRow, target);

                    outputStr = Math.Round(newAmount.ApproximateValue, 3).ToString("0.###") + " " + target.U_Abbreviation;
                }
            }

            return outputStr;
        }

        /// <summary>Refresh the contents of the Units table from the database</summary>
        public void UpdateUnitsTable()
        {
            IEnumerable<Unit> units = unitsAdapter.SelectAll();

            UnitNameMap = units.ToDictionary(unit => unit.U_Name, unit => unit);
            UnitAbbrevMap = units.ToDictionary(unit => unit.U_Abbreviation, unit => unit);
            UnitList = units.ToDictionary(unit => unit.ID, unit => unit);
        }

        /// <summary>Represents an error that occurs when a requested unit does not exist</summary>
        public class UnitNotFoundException : Exception
        {
            /// <summary>Create a new instance of the exception with the specified message</summary>
            /// <param name="message"></param>
            public UnitNotFoundException(string message) : base(message)
            {

            }
        }

        /// <summary>Represents an error that occurs when a given unit cannot be used in an operation</summary>
        public class InvalidUnitException : Exception
        {
            /// <summary>Create a new instance of the exception with the specified message</summary>
            /// <param name="message"></param>
            public InvalidUnitException(string message) : base(message)
            {

            }
        }

        /// <summary>Convert a decimal number to fractional form, optionally rounding to nearest 1/8</summary>
        /// <param name="amount"></param>
        /// <param name="roundToEigth">If true, output will be rounded to the nearest 1/8 (0.125)</param>
        /// <returns>
        /// A fractional representation of the input or the input rounded to 3 decimal places
        /// if the fractional part could not be mapped to a power of 2 between -1 and -3
        /// </returns>
        public static string FormatAsFraction(decimal amount, bool roundToEigth = false)
        {
            var sb = new StringBuilder();

            // whole part
            if (amount > 1)
            {
                sb.Append((int)amount + " ");
                amount = amount % 1;
            }

            decimal rounded = (roundToEigth) ? decimal.Round(amount * 8) / 8 : decimal.Round(amount, 3);

            switch (rounded)
            {
                case 0.500M:
                    sb.Append("1/2");
                    break;

                // quarters
                case 0.250M:
                    sb.Append("1/4");
                    break;

                case 0.750M:
                    sb.Append("3/4");
                    break;

                // thirds
                case 0.333M:
                    sb.Append("1/3");
                    break;

                case 0.667M:
                    sb.Append("2/3");
                    break;


                // eigths
                case 0.125M:
                case 0.375M:
                case 0.625M:
                case 0.875M:
                    sb.AppendFormat("{0}/8", (int)(rounded / 0.125M));
                    break;


                case 0.000M:
                    if (sb.Length >= 1) sb.Remove(sb.Length - 1, 1);
                    break;

                default:
                    if (sb.Length >= 1) sb.Remove(sb.Length - 1, 1);
                    sb.Append(rounded.ToString(".###"));
                    break;
            }

            return sb.ToString();
        }

        /// <summary>Convert a string representation of a fraction to its numeric equivalent</summary>
        /// <param name="inputStr">String containing the fraction to be parsed</param>
        /// <returns>The decimal equivalent of the input, or 0 if the input was not a valid fraction</returns>
        public static decimal ParseAmount(string inputStr)
        {
            decimal result = 0.0M;
            string[] parts;

            inputStr = inputStr.Trim();
            if (inputStr.Contains(' '))
            {
                parts = inputStr.Split(' ');
                bool good = int.TryParse(parts[0], out int wholePart);
                if (good) result += wholePart;

                inputStr = parts[1];
            }

            parts = inputStr.Split('/');
            if (parts.Length > 1)
            {
                try
                {
                    decimal numerator = decimal.Parse(parts[0]);
                    decimal denominator = decimal.Parse(parts[1]);
                    result += numerator / denominator;
                }
                catch { }
            }
            else
            {
                if (decimal.TryParse(inputStr, out decimal parsed)) result += parsed;
            }

            return result;
        }

        /// <summary>Try to find a unit in the table with a name or abbreviation matching the given string</summary>
        /// <param name="input"></param>
        /// <returns>The matching Unit if one was found, or null</returns>
        public Unit FindUnit(string input)
        {
            input = input.Trim();

            if (UnitAbbrevMap.TryGetValue(input, out Unit fromAbbrev)) return fromAbbrev;
            if (UnitNameMap.TryGetValue(input, out Unit fromName)) return fromName;

            return null;
        }

        /// <summary>Attempt to parse a string into an amount and a unit</summary>
        /// <param name="input">String containing an amount of a unit</param>
        /// <param name="unitID">Variable to hold the ID of the parsed unit</param>
        /// <param name="amount">Variable to hold the parsed amount</param>
        /// <returns>True if the string was successfully parsed</returns>
        public bool TryParseUnitString(string input, out int unitID, out decimal amount)
        {
            unitID = 0;
            amount = 0.0M;

            const string pattern = @"((?:\d*\.)?\d+|\d+\/\d+)\s+(\w+)";
            // group 1: match integer, fraction, or decimal
            // group 2: match word

            var match = Regex.Match(input, pattern);
            if (match.Success && match.Groups.Count == 2)
            {
                amount = ParseAmount(match.Groups[0].Value);
                var unit = FindUnit(match.Groups[1].Value);

                if (unit != null)
                {
                    unitID = unit.U_ID;
                    return true;
                }
                else return false;
            }
            else return false;
        }
    }
}
