//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Georg Rollinger
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
using MVVMDiversity.Model;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using log4net;

namespace MVVMDiversity.Services
{
	public partial class FieldDataService
	{
        private class Search
        {
            private ILog _Log = LogManager.GetLogger(typeof(Search));

            private FieldDataService _owner;
            private AsyncOperationInstance<IList<ISerializableObject>> _operation;

            private SearchSpecification _configuredSearch;
            private int _currentProjectID;
            private IList<ISerializableObject> _queryResult; 

            public Search(FieldDataService owner, AsyncOperationInstance<IList<ISerializableObject>> op)
            {
                _owner = owner;
                _operation = op;
            }

            public void executeSearch(SearchSpecification search, int currentProjectID)
            {
                if (search == null)
                    throw new ArgumentNullException("search");
                
                _operation.IsProgressIndeterminate = true;
                _operation.StatusDescription = "Services_FieldData_Querying";
                    
                _configuredSearch = search;
                _currentProjectID = currentProjectID;
                

                new Action(asyncQuery).BeginInvoke(null,null);               
            }

            private void asyncQuery()
            {                
                

                if (_owner.Connections != null)
                {
                    var repo = _owner.Connections.Repository;
                    if (repo != null)
                    {
                        repo.Progress = new ProgressInterval(_operation, 100f, 1);
                        _queryResult = repo.Connector.LoadList(_configuredSearch.ObjectType, GetQueryRestriction());
                        _operation.success(_queryResult);
                    }
                    else
                        _Log.Error("Repository N/A");
                }
                else
                    _Log.Error("ConnectionsProvider N/A");

                _operation.failure("Services_FieldData_Error_MissingServices","");
            }


                  

            private static IList<SearchSpecification> _sTypes = new List<SearchSpecification>()
                {
                    //Collection Event Series
                    new SearchSpecification("SearchType_CES",typeof(CollectionEventSeries),new List<Restriction>()
                    {
                        //Series Code
                        new TextRestriction("Restriction_SeriesCode","_SeriesCode",false),
                        //Description
                        new TextRestriction("Restriction_Description","_Description",false),
                        //Start Date Range
                        new DateRangeRestriction("Restriction_StartDate","_DateStart"),
                        //End Date Range
                        new DateRangeRestriction("Restriction_EndDate","_DateEnd"),
                    }),
                    //Identification Unit
                    new SearchSpecification("SearchType_IU",typeof(IdentificationUnit),new List<Restriction>()
                    {
                        //Last Identification
                        new TextRestriction("Restriction_LastIdent","_LastIdentificationCache",false),
                        //Taxonomic Group
                        new TextRestriction("Restriction_TaxGroup","_TaxonomicGroup",true),
                        //Unit Description
                        new TextRestriction("Restriction_UnitDesc","_UnitDescription",true),
                        //Log Updated Between
                        new DateRangeRestriction("Restriction_LogUpdated","_LogUpdatedWhen"),
                    }),
                };
            public static IList<SearchSpecification> SearchTypes
            {
                get
                {
                    return new List<SearchSpecification>(_sTypes);
                }
            }

            private IRestriction GetQueryRestriction()
            {
                IRestriction projectRestriction = RestrictionFactory.Eq(typeof(CollectionProject), "_ProjectID", _currentProjectID);
                IRestriction selectionRestrictions = null;
                foreach (var restrictionSpecification in _configuredSearch.Restrictions)
                {
                    if (restrictionSpecification == null || !restrictionSpecification.IsEnabled)
                        continue;

                    IRestriction currRes = null;
                    if (restrictionSpecification is TextRestriction)
                        currRes = fromTextRestriction(restrictionSpecification as TextRestriction);
                    else if (restrictionSpecification is DateRangeRestriction)
                        currRes = fromDateRangeRestriction(restrictionSpecification as DateRangeRestriction);

                    if (currRes != null)
                    {
                        if (selectionRestrictions != null)
                            selectionRestrictions = RestrictionFactory.And().Add(selectionRestrictions).Add(currRes);
                        else
                            selectionRestrictions = currRes;
                    }
                }
                if (selectionRestrictions != null)
                    return RestrictionFactory.And().Add(projectRestriction).Add(selectionRestrictions);
                else
                    return projectRestriction;
            }

            private IRestriction fromTextRestriction(TextRestriction res)
            {
                string value = res.Value ?? "";

                if (res.ExactMatch)
                    return RestrictionFactory.Eq(_configuredSearch.ObjectType, res.Property, value );
                else
                    return RestrictionFactory.Like(_configuredSearch.ObjectType, res.Property, string.Format("%{0}%", value.Trim('%')));
            }

            private IRestriction fromDateRangeRestriction(DateRangeRestriction res)
            {
                return RestrictionFactory.Btw(_configuredSearch.ObjectType, res.Property, res.StartDate, res.EndDate);
            }
        }
	}
}
