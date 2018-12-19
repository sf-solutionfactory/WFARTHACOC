using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFARTHA.Services
{
    public class FormatosC
    {
        public decimal toNum(string numR, string miles, string decimales)
        {
            string num = numR;
            if (num != "" && num != null)
            {
                num = num.Replace("$", "");
                num = num.Replace("%", "");
                num = num.Replace(miles, "");
                num = num.Replace(decimales, ".");
            }
            else
            {
                num = "0.00";
            }

            return Convert.ToDecimal(num);
        }

        public string toShow(decimal? num2, string decimales)
        {
            decimal num = 0;
            if(num2 == null)
            {
                num = 0;
            }
            else
            {
                num = Convert.ToDecimal(num2);
            }

            string regresa = num.ToString("N2");
            string[] separa = regresa.Split('.');

            if (regresa != null | regresa != "")
            {
                if (decimales == ".")
                {
                    regresa = separa[0].Replace(".", ",");
                    regresa = "$ " + regresa + decimales + separa[1];
                }
                else if (decimales == ",")
                {
                    regresa = separa[0].Replace(",", ".");
                    regresa = "$ " + regresa + decimales + separa[1];
                }
            }
            else
            {
                regresa = "$ 0" + decimales + "00";
            }

            return regresa;
        }

        public string toShowPorc(decimal num, string decimales)
        {
            string regresa = num.ToString("N2");
            string[] separa = regresa.Split('.');

            if (regresa != null | regresa != "")
            {
                if (decimales == ".")
                {
                    regresa = separa[0].Replace(".", ",");
                    regresa = regresa + decimales + separa[1] + "%";
                }
                else if (decimales == ",")
                {
                    regresa = separa[0].Replace(",", ".");
                    regresa = regresa + decimales + separa[1] + "%";
                }
            }
            else
            {
                regresa = "$ 0" + decimales + "00";
            }

            return regresa;
        }

        public string toShowNum(decimal num, string decimales)
        {
            string regresa = num.ToString("N2");
            string[] separa = regresa.Split('.');

            if (regresa != null | regresa != "")
            {
                if (decimales == ".")
                {
                    regresa = separa[0].Replace(".", ",");
                    regresa = regresa + decimales + separa[1];
                }
                else if (decimales == ",")
                {
                    regresa = separa[0].Replace(",", ".");
                    regresa = regresa + decimales + separa[1];
                }
            }
            else
            {
                regresa = "$ 0" + decimales + "00";
            }

            return regresa;
        }
    }
}