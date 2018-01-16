using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox3
{
    public class UnitManager
    {
        private CookbookModel GlobalCookbookAdapter { get { return App.GlobalCookbookModel; } }
        private CookbookDataSet.UnitsDataTable _UnitsTable;
        public CookbookDataSet.UnitsDataTable UnitsTable => _UnitsTable;

        public UnitManager()
        {
            _UnitsTable = new CookbookDataSet.UnitsDataTable();
        }

        public decimal Convert(decimal amount, int sourceID, int targetID)
        {
            var sourceRow = _UnitsTable.FindByU_ID(sourceID);
            var targetRow = _UnitsTable.FindByU_ID(targetID);

            if (sourceRow == null) throw new UnitNotFoundException("Source unit was not found in the unit table");
            if (targetRow == null) throw new UnitNotFoundException("Target unit was not found in the unit table");

            return Convert(amount, sourceRow, targetRow);
        }

        private decimal Convert(decimal amount, CookbookDataSet.UnitsRow sourceRow, CookbookDataSet.UnitsRow targetRow)
        {
            if (sourceRow == null || targetRow == null) throw new ArgumentNullException();

            if (sourceRow.IsU_RatioNull()) throw new InvalidUnitException("Source unit does not have a valid ratio");
            if (targetRow.IsU_RatioNull()) throw new InvalidUnitException("Target unit does not have a valid ratio");

            if (sourceRow.IsU_TypecodeNull() || targetRow.IsU_TypecodeNull() || sourceRow.U_Typecode != targetRow.U_Typecode)
                throw new InvalidUnitException("Source and Target unit types are incompatible: " + sourceRow.U_Typecode + "->" + targetRow.U_Typecode);

            return System.Convert.ToDecimal(sourceRow.U_Ratio / targetRow.U_Ratio) * amount;
        }

        public string GetUSString(decimal amount, int sourceID)
        {
            var sourceRow = _UnitsTable.FindByU_ID(sourceID);
            if (sourceRow == null) throw new UnitNotFoundException("Source unit was not found in the unit table");

            return GetUSString(amount, sourceRow);
        }

        public string GetUSString(decimal amount, CookbookDataSet.UnitsRow sourceRow)
        {
            string outputStr = null;

            if (!sourceRow.IsU_SystemNull() && sourceRow.U_System.Equals("USC"))
            {
                // already in US units
                outputStr = FormatAsFraction(amount);
                if (!sourceRow.IsU_AbbrevNull()) outputStr += " " + sourceRow.U_Abbrev;
                return outputStr;
            }

            var rows = _UnitsTable.Select("U_System = 'USC' AND U_Typecode = " + sourceRow.U_Typecode + " OR U_ID = " + sourceRow.U_ID, "U_Ratio ASC");

            if (rows.Length <= 1)
            {
                return FormatAsFraction(amount) + " " + ((sourceRow.IsU_AbbrevNull()) ? null : sourceRow.U_Abbrev);
            }
            else
            {
                int sourceIndex = -1;
                int i = 0;

                foreach (CookbookDataSet.UnitsRow row in rows)
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
                    CookbookDataSet.UnitsRow target = null;

                    if (sourceIndex == 0)
                    {
                        // no smaller, use larger
                        target = rows[1] as CookbookDataSet.UnitsRow;
                    }
                    else if (sourceIndex > 0 && sourceIndex < rows.Length)
                    {
                        // use smaller
                        target = rows[sourceIndex - 1] as CookbookDataSet.UnitsRow;
                    }

                    decimal newAmount = Convert(amount, sourceRow, target);
                    decimal fPart = newAmount % 1;

                    string targetAbbrev = (target.IsU_AbbrevNull()) ? null : target.U_Abbrev;

                    if (fPart > 0)
                    {
                        // not exact

                        bool smallerUnitFound = false;
                        decimal subAmount = 0.0M;
                        string subTargetAbbrev = null;

                        for (int j = sourceIndex; j >= 0 && !smallerUnitFound; j--)
                        {
                            // traverse back down the list to find the sub unit
                            if (rows[j] is CookbookDataSet.UnitsRow subTarget)
                            {
                                try
                                {
                                    subAmount = Convert(fPart, target, subTarget);

                                    if (subAmount > 1)
                                    {
                                        smallerUnitFound = true;
                                        subTargetAbbrev = (subTarget.IsU_AbbrevNull()) ? null : subTarget.U_Abbrev;
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
            var sourceRow = _UnitsTable.FindByU_ID(sourceID);
            if (sourceRow == null) throw new UnitNotFoundException("Source unit was not found in the unit table");

            return GetMetricString(amount, sourceRow);
        }

        public string GetMetricString(decimal amount, CookbookDataSet.UnitsRow sourceRow)
        {
            string outputStr = null;

            if (!sourceRow.IsU_SystemNull() && sourceRow.U_System.Equals("MET"))
            {
                // already in metric units
                outputStr = decimal.Round(amount, 3).ToString("0.###");
                if (!sourceRow.IsU_AbbrevNull()) outputStr += " " + sourceRow.U_Abbrev;
                return outputStr;
            }

            var rows = _UnitsTable.Select("U_System = 'MET' AND U_Typecode = " + sourceRow.U_Typecode + " OR U_ID = " + sourceRow.U_ID, "U_Ratio ASC");

            if (rows.Length <= 1)
            {
                outputStr = decimal.Round(amount, 3).ToString("0.###");
            }
            else
            {
                int sourceIndex = -1;
                int i = 0;

                foreach (CookbookDataSet.UnitsRow row in rows)
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
                    CookbookDataSet.UnitsRow target = null;

                    if (sourceIndex == 0)
                    {
                        // no smaller, use larger
                        target = rows[1] as CookbookDataSet.UnitsRow;
                    }
                    else if (sourceIndex > 0 && sourceIndex < rows.Length)
                    {
                        // use smaller
                        target = rows[sourceIndex - 1] as CookbookDataSet.UnitsRow;
                    }

                    decimal newAmount = Convert(amount, sourceRow, target);

                    outputStr = decimal.Round(newAmount, 3).ToString("0.###");
                    if (!target.IsU_AbbrevNull()) outputStr += " " + target.U_Abbrev;
                }
            }

            return outputStr;
        }

        public void UpdateUnitsTable()
        {
            GlobalCookbookAdapter.UnitsTableAdapter.Fill(_UnitsTable);
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

        public static decimal FormatAsDecimal(string inputStr)
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
    }
}
