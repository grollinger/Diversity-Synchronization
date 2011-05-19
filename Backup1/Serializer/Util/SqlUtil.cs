//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Tobias Schneider, Lehrstuhl Angewndte Informatik IV, Universität Bayreuth
//
//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License along
//with this program; if not, write to the Free Software Foundation, Inc.,
//51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//#######################################################################
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util
{
    public class SqlUtil
    {
        public static Object SqlConvert(Object o)
        {
            Object ret = null;

            if (o == null)
            {
                ret = "NULL";
            }
            else if (o is DateTime)
            {
                StringBuilder tmp = new StringBuilder();
                DateTime d = (DateTime)o;

                //FIXME das muss verbessert werden ...
                if (DateTime.Equals(d, DateTime.MinValue))
                {
                    ret = "NULL";
                }
                else
                {
                    //Aufgrund unterschiedlicher Datumsformate in den Datenbanken wird das Datum zunächst in das OLE Automationdate als Standard konvertiert.
                    //Da dieses allerdings den 30.12.1899 als Referenz hat und sich Datumsangaben in Datenbanken auf den 1.1.1900 beziehen, wird das Datum um 2 Tage korrigiert.
                    //Damit die Zahl als double und nicht als String interpretiert wird muss auf das ' verzichtet werden.
                    tmp.Append((d.ToOADate()-2).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    //tmp.Append("TO_DATETIME(");
                    
                    //OriginalVersion***
                    //tmp.Append("'").Append(
                    //    d.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append("'");//Übertragung nach CE benötigt yyyy-MM-dd nach München benutzt yyyy-dd-MM
                    //bis hier****

                    //tmp.Append(", '	YYYY-MM-DD HH:MM:SS')");
                    //, System.Globalization.CultureInfo.CreateSpecificCulture("en-US")

                    ret = tmp.ToString();
                }
            }
            else if (o is int || o is float || o is double || o is short || o is int? || o is float? || o is double? || o is short?)
            {
                String s = o.ToString();
                ret = s.Replace(",", ".");             
            }
            else
            {
                StringBuilder tmp = new StringBuilder();
                tmp.Append("'").Append(o.ToString().Replace("'", "''")).Append("'");
                ret = tmp.ToString();
            }
            return ret;
        }
    }
}
