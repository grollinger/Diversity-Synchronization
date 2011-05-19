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

namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class TableAttribute : Attribute, ITarget
    {
        private string _tableMapping = null;
        private string _target = AttributeConstants.DEFAULT_TARGET;

        public TableAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableAttribute"/> class.
        /// </summary>
        /// <param name="tableMapping">The name of the table in the data base.</param>
        public TableAttribute(string tableMapping)
        {
            TableMapping = tableMapping;
        }

        public string TableMapping { get { return this._tableMapping; } protected set { this._tableMapping = value; } }
        public string Target { get { return _target; } set { _target = value; } }
    }
}
