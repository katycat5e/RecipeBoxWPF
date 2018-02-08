using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using RecipeBox3.SQLiteModel.Adapters;
using RecipeBox3.SQLiteModel.Data;

namespace RecipeBox3
{
    public class UnitManager : DependencyObject
    {
        private UnitsAdapter unitsAdapter = new UnitsAdapter();


        public Dictionary<int, Unit> UnitList
        {
            get { return (Dictionary<int, Unit>)GetValue(UnitListProperty); }
            set { SetValue(UnitListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitListProperty =
            DependencyProperty.Register("UnitList", typeof(Dictionary<int, Unit>), typeof(UnitManager), new PropertyMetadata(null));



        public UnitManager()
        {
            
        }

        public decimal Convert(decimal amount, int sourceID, int targetID)
        {
            if (!UnitList.TryGetValue(sourceID, out Unit source))
                throw new UnitNotFoundException("Source unit was not found in the unit table");

            if (!UnitList.TryGetValue(targetID, out Unit target))
                throw new UnitNotFoundException("Target unit was not found in the unit table");

            return Convert(amount, source, target);
        }

        private decimal Convert(decimal amount, Unit source, Unit target)
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

            return System.Convert.ToDecimal(source.U_Ratio / target.U_Ratio) * amount;
        }

        public string GetString(decimal amount, int sourceID, Unit.System targetSystem)
        {
            if (!UnitList.TryGetValue(sourceID, out Unit sourceUnit))
                throw new UnitNotFoundException("Source unit was not found in the unit table");

            return GetString(amount, sourceUnit, targetSystem);
        }

        public string GetString(decimal amount, Unit sourceUnit, Unit.System targetSystem)
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

        public string GetUSString(decimal amount, int sourceID)
        {
            if (!UnitList.TryGetValue(sourceID, out Unit sourceRow)) throw new UnitNotFoundException("Source unit was not found in the unit table");

            return GetUSString(amount, sourceRow);
        }

        public string GetUSString(decimal amount, Unit sourceRow)
        {
            string outputStr = null;

            if (sourceRow.U_System == Unit.System.Customary)
            {
                // already in US units
                outputStr = FormatAsFraction(amount) + " " + sourceRow.U_Abbreviation;
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
                .ToList();

            if (rows.Count <= 1)
            {
                return FormatAsFraction(amount) + " " + sourceRow.U_Abbreviation;
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

                    decimal newAmount = Convert(amount, sourceRow, target);
                    decimal fPart = newAmount % 1;

                    string targetAbbrev = target.U_Abbreviation;

                    if (fPart > 0)
                    {
                        // not exact

                        bool smallerUnitFound = false;
                        decimal subAmount = 0.0M;
                        string subTargetAbbrev = null;

                        for (int j = sourceIndex; j >= 0 && !smallerUnitFound; j--)
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
                            outputStr = String.Format("{0} {1}, {2} {3}", decimal.Round(newAmount), targetAbbrev, FormatAsFraction(subAmount, true), subTargetAbbrev);
                        }
                        else
                        {
                            outputStr = String.Format("{0} {1}", FormatAsFraction(newAmount, true), targetAbbrev);
                        }
                    }
                    else
                    {
                        // no fractional part
                        outputStr = String.Format("{0} {1}", newAmount, targetAbbrev);
                    }
                    
                }
            }
            

            return outputStr;
        }

        public string GetMetricString(decimal amount, int sourceID)
        {
            if (!UnitList.TryGetValue(sourceID, out Unit sourceRow)) throw new UnitNotFoundException("Source unit was not found in the unit table");

            return GetMetricString(amount, sourceRow);
        }

        public string GetMetricString(decimal amount, Unit sourceRow)
        {
            string outputStr = null;

            if (sourceRow.U_System == Unit.System.Metric)
            {
                // already in metric units
                outputStr = decimal.Round(amount, 3).ToString("0.###") + " " + sourceRow.U_Abbreviation;
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
                .ToList();

            if (rows.Count <= 1)
            {
                outputStr = decimal.Round(amount, 3).ToString("0.###");
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

                    decimal newAmount = Convert(amount, sourceRow, target);

                    outputStr = decimal.Round(newAmount, 3).ToString("0.###") + " " + target.U_Abbreviation;
                }
            }

            return outputStr;
        }

        public void UpdateUnitsTable()
        {
            UnitList = unitsAdapter.SelectAll().ToDictionary(p => p.ID, p => p);
        }

        public class UnitNotFoundException : Exception
        {
            public UnitNotFoundException(string message) : base(message)
            {

            }
        }

        public class InvalidUnitException : Exception
        {
            public InvalidUnitException(string message) : base(message)
            {

            }
        }

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

        public static decimal ParseFraction(string inputStr)
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

        public Unit FindUnit(string input)
        {
            input = input.Trim();

            foreach (Unit unit in UnitList.Values)
            {
                if (input == unit.U_Name || input == unit.U_Plural || input == unit.U_Abbreviation)
                {
                    return unit;
                }
            }

            return null;
        }

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
                amount = ParseFraction(match.Groups[0].Value);
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
