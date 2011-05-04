using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Interface;
using MVVMDiversity.Model;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using System.Threading;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace MVVMDiversity.DesignServices
{
    public class FieldData : IFieldDataService
    {
        public class ISOMock : ISerializableObject
        {

            public DateTime LogTime
            {
                get
                {
                    return DateTime.Now;
                }
                set
                {
                    
                }
            }


            public Guid Rowguid { get; set; }

            public ISOMock()
            {
                Rowguid = Guid.NewGuid();
            }
            
        }

        public IEnumerable<Model.SearchSpecification> SearchTypes
        {
            get
            {
                return new List<SearchSpecification>()
                {
                    //Collection Event Series                
                    new SearchSpecification("SearchType_CES",null,new List<Restriction>()
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
                    new SearchSpecification("SearchType_IU",null,new List<Restriction>()
                    {
                        //Last Identification
                        new TextRestriction("Restriction_LastIdent","_LastIdentificationCache",false),
                        //Taxonomic Group
                        new TextRestriction("Restriction_TaxGroup","_TaxonomicGroup",true),
                        //Unit Description
                        new TextRestriction("Restriction_UnitDesc","_UnitDescription",true){IsEnabled = true},
                        //Log Updated Between
                        new DateRangeRestriction("Restriction_LogUpdated","_LogUpdatedWhen"),
                    })
                };
            }
        }

        
        public BackgroundOperation uploadData(string userNr, int projectID)
        {
            var b = BackgroundOperation.newUninterruptable();
            return b;
        }

        public BackgroundOperation downloadData(IList<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject> selection)
        {
            var b = BackgroundOperation.newUninterruptable();
            return b;
        }

        public BackgroundOperation executeSearch(SearchSpecification search, int currentProjectID, Action<IList<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject>> finishedCallback)
        {
            new Action(() =>
            {
                Thread.Sleep(1000);
                var ces = new CollectionEventSeries()
                {
                    DateEnd = DateTime.Now.AddDays(1),
                    DateStart = DateTime.Now,
                    Description = "Collection Event Series Test",
                    Geography = "Geography?",
                    LogTime = DateTime.Now,
                    Rowguid = Guid.NewGuid(),
                    SeriesCode = "seriesCode",
                };
                finishedCallback(new List<ISerializableObject>()
            {
                ces
            });
            }).BeginInvoke(null, null);
            return BackgroundOperation.newUninterruptable();
        }


        public BackgroundOperation uploadData(string userNr, int projectID, Action finishedCallback)
        {
            return BackgroundOperation.newUninterruptable();

        }

        public BackgroundOperation downloadData(IList<ISerializableObject> selection, Action finishedCallback)
        {
            throw new NotImplementedException();
        }

        public bool tryCleaningSyncTable()
        {
            return true;
        }
    }
}
