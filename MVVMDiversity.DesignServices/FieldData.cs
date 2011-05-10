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
     

        /*public AsyncOperationInstance executeSearch(SearchSpecification search, int currentProjectID, Action<IList<UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable.ISerializableObject>> finishedCallback)
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
            return AsyncOperationInstance.newUninterruptable();
        }


        public AsyncOperationInstance uploadData(string userNr, int projectID, Action finishedCallback)
        {
            return AsyncOperationInstance.newUninterruptable();

        }*/        

        public bool tryCleaningSyncTable()
        {
            return true;
        }


        public AsyncOperationInstance uploadData(string userNr, int projectID, AsyncOperationFinishedHandler finishedCallBack)
        {
            throw new NotImplementedException();
        }

        public event AsyncOperationFinishedHandler UploadFinished;

        public AsyncOperationInstance downloadData(IList<ISerializableObject> selection, AsyncOperationFinishedHandler finishedCallBack)
        {
            throw new NotImplementedException();
        }

        public event AsyncOperationFinishedHandler DownloadFinished;

        public AsyncOperationInstance startSearch(SearchSpecification search, int currentProjectID)
        {
            throw new NotImplementedException();
        }

        public event AsyncOperationFinishedHandler<IList<ISerializableObject>> SearchFinished;


        public AsyncOperationInstance uploadData(string userNr, int projectID)
        {
            throw new NotImplementedException();
        }

        public AsyncOperationInstance downloadData(IList<ISerializableObject> selection)
        {
            throw new NotImplementedException();
        }

        AsyncOperation<IList<ISerializableObject>> IFieldDataService.startSearch(SearchSpecification search, int currentProjectID)
        {
            throw new NotImplementedException();
        }
    }
}
