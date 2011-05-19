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

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions
{
    public abstract class QueryComponent
    {

        public abstract StringBuilder ToSqlString();
    }

    public class QueryGenerator : QueryComponent
    {
        List<QueryComponent> _QueryComponents;


        public QueryGenerator()
        {
            _QueryComponents = new List<QueryComponent>();
        }

        public void AddQueryComponent(QueryComponent component)
        {
            _QueryComponents.Add(component);
        }

        
        public void ReplacePlaceHolders(QueryGenerator generator)
        {    
            for (int i = 0; i < _QueryComponents.Count; i++)
            {
                if (_QueryComponents[i] is PlaceHolderComponent)
                {
                    _QueryComponents[i] = generator;
                }

                //Diese Variante endet sehr bald einen Stackoverflow --> keine Rekursion
                //else if (_QueryComponents[i] is QueryGenerator)
                //{
                //    ((QueryGenerator)_QueryComponents[i]).ReplacePlaceHolders(generator);
                //}
            }

            //Iteratives Vorgehen vermeidet den StachOverflow
            List<QueryComponent> tmp = new List<QueryComponent>();
            foreach (QueryComponent c in _QueryComponents)
            {
                if (c is QueryGenerator)
                {
                    tmp.AddRange(((QueryGenerator)c)._QueryComponents);
                } 
                else 
                {
                    tmp.Add(c);
                }
            }

            _QueryComponents = tmp;
        }

        public override StringBuilder ToSqlString()
        {
            StringBuilder ret = new StringBuilder();
            foreach (QueryComponent c in _QueryComponents)
            {
                ret.Append(c.ToSqlString());
            }

            return ret;
        }
    }

    public class SqlStringComponent : QueryComponent
    {
        private StringBuilder _SqlString;

        public SqlStringComponent(String sqlString)
        {
            _SqlString = new StringBuilder();
            _SqlString.Append(sqlString);
        }
        
        public SqlStringComponent(StringBuilder sqlString)
        {
            _SqlString = sqlString;
        }

        public override StringBuilder ToSqlString()
        {
            return _SqlString;
        }
    }

    public class PlaceHolderComponent : QueryComponent
    {


        public override StringBuilder ToSqlString()
        {
            throw new InvalidOperationException("PlaceHolderComponent cannot be transformed to a StringBuilder");
        }
    }
}
