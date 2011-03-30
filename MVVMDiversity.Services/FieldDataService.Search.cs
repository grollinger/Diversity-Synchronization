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
            public Search(FieldDataService owner)
            {
                _owner = owner;
            }

            public BackgroundOperation executeSearch(SearchSpecification search, int currentProjectID, Action<IList<ISerializableObject>> finishedCallback)
            {
                if (search == null)
                    throw new ArgumentNullException("search");
                

                _progress = BackgroundOperation.newInterruptable();
                _progress.IsProgressIndeterminate = true;
                _progress.ProgressDescriptionID = "Services_FieldData_Querying";
                    
                _configuredSearch = search;
                _currentProjectID = currentProjectID;
                

                new Action(asyncQuery).BeginInvoke(
                    (res) =>
                    {
                        if (finishedCallback != null)
                            finishedCallback(_queryResult);
                    },null);


                
                return _progress;
            }

            private void asyncQuery()
            {                
                

                if (_owner.Connections != null)
                {
                    var repo = _owner.Connections.Repository;
                    if (repo != null)
                    {
                        repo.Progress = new ProgressInterval(_progress, 100f, 1);
                        _queryResult = repo.Connector.LoadList(_configuredSearch.ObjectType, GetQueryRestriction());
                    }
                    else
                        _Log.Error("Repository N/A");
                }
                else
                    _Log.Error("ConnectionsProvider N/A");
            }


            private SearchSpecification _configuredSearch;
            private int _currentProjectID;
            private BackgroundOperation _progress;
           
            private IList<ISerializableObject> _queryResult;       

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
