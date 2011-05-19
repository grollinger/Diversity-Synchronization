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
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util
{
    public interface IAdvanceProgress
    {
        /// <summary>
        /// Advances the operation counter by one step.
        /// </summary>        
        void advance();
        /// <summary>
        /// Advances the operation counter by <value>steps</value> steps.
        /// </summary> 
        void advance(int steps);
        
        /// <summary>
        /// Represents the number of steps you expect this operation to take.
        /// Used to calculate the progress withing this operation.
        /// </summary>
        int InternalTotal { get; set; }

        /// <summary>
        /// Whether the user has requested to abort the current operation
        /// </summary>
        bool IsCancelRequested { get; }
    }
}
