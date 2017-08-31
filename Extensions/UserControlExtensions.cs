using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Nido.Common.BackEnd;

namespace Nido.Common.Web
{
    public interface INidoMasterPage
    {
        void DisplayError(GenericResponse response);
    }

    public static class PageExtensions
    {
        public static int ConvertToInt<T>(this Page obj, string i)
         where T : System.Web.UI.MasterPage, INidoMasterPage
        {
            try
            {
                return Convert.ToInt32(i);
            }
            catch
            {
                ((T)obj.Master).DisplayError(new GenericResponse(false, new string[] { "Data Related error occurred" }));
                return -1;
            }
        }

        public static void DisplayError(this Page obj, GenericResponse response)
        {
            ((INidoMasterPage)obj.Master).DisplayError(response);
        }
    }

    public static class UserControlExtensions
    {
        public static int ConvertToInt<T>(this UserControl obj, string i)
            where T : System.Web.UI.MasterPage, INidoMasterPage
        {
            try
            {
                return Convert.ToInt32(i);
            }
            catch
            {
                ((T)obj.Page.Master).DisplayError(new GenericResponse(false, new string[] { "Data Related error occurred" }));
                return -1;
            }
        }

        public static int ConvertToInt(this UserControl obj, string i)
        {
            try
            {
                return Convert.ToInt32(i);
            }
            catch
            {
                ((INidoMasterPage)obj.Page.Master).DisplayError(new GenericResponse(false, new string[] { "Data Related error occurred" }));
                return -1;
            }
        }

        public static decimal ConvertToDecimal(this UserControl obj, string d)
        {
            try
            {
                return Convert.ToDecimal(d);
            }
            catch
            {
                ((INidoMasterPage)obj.Page.Master).DisplayError(new GenericResponse(false, new string[] { "Data Related error occurred" }));
                return -1;
            }
        }

        public static DateTime ConvertToDateTme(this UserControl obj, string dateTime)
        {
            try
            {
                return Convert.ToDateTime(dateTime);
            }
            catch
            {
                ((INidoMasterPage)obj.Page.Master).DisplayError(new GenericResponse(false, new string[] { "Data Related error occurred" }));
                return DateTime.Now;
            }
        }

        public static void DisplayError(this UserControl obj, GenericResponse response)
        {
            ((INidoMasterPage)obj.Page.Master).DisplayError(response);
        }
    }
}

