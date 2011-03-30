using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;


namespace MVVMDiversity.ViewModel
{
    public enum ISOIcon
    {
        /// <summary>
        /// Unknown symbol (usually this refers to a purely white rectangle).
        /// </summary>
        //Unknown = 0,

        //Event = 1,
        //EventGrey = 2,

        //SiteProperty = 3,
        //SitePropertyGrey = 4,

        //EventSeries = 5,
        //EventSeriesGrey = 6,

        //Location = 7,
        //LocationGrey = 8,

        //Agent = 9,
        //AgentGrey = 10,

        //Specimen = 11,
        //SpecimenGrey = 12,
        //SpecimenRed = 38,

        //Tree = 13,
        //TreeGrey = 14,

        //Branch = 15,
        //BranchGrey = 16,

        //Plant = 17,
        //PlantGrey = 18,

        //Other = 19,
        //OtherGrey = 20,

        //Identification = 21,
        //IdentificationGrey = 22,



        //Analysis = 23,
        //AnalysisGrey = 24,

        //Foto = 25,
        //FotoGrey = 26,

        //Image = 27,
        //ImageGrey = 28,

        //Alga = 29,
        //Assel = 30,
        //Bacterium = 31,
        //Bird = 32,
        //Bryophyte = 33,
        //Fish = 34,
        //Fungus = 35,
        //Insect = 36,
        //Lichen = 37,
        //Mammal = 38,
        //Mollusc = 39,
        //Myxomycete = 40,
        //Virus = 41,
        //Geography = 43,
        //GeographyGrey = 44,
        //GPS = 45,
        //GPSGrey = 46
        Unknown = 0,

        Event = 1,
        EventGrey = 2,

        SiteProperty = 3, //TODO Missing?
        SitePropertyGrey = 4, //TODO Missing?

        EventSeries = 5,
        EventSeriesGrey = 6,

        Location = 7, //TODO =Localisation?
        LocationGrey = 8,

        Agent = 9,
        AgentGrey = 10,

        Specimen = 11,
        SpecimenGrey = 12,
        SpecimenRed = 38,

        Tree = 13,
        TreeGrey = 14,

        Branch = 15,
        BranchGrey = 16,

        Plant = 17,
        PlantGrey = 18,

        Other = 19,
        OtherGrey = 20,

        Analysis = 21,
        AnalysisGrey = 22,

        Foto = 23,
        FotoGrey = 24,

        Alge = 25,
        Assel = 26,
        Bacterium = 27,
        Bird = 28,
        Bryophyt = 29,
        Fish = 30,
        Fungus = 31,
        Insect = 32,
        Lichen = 33,
        Mammal = 34,
        Mollusc = 35,
        Myxomycete = 36,
        Virus = 37,

        GeoAnalysis = 39,
        GeoAnalysisGrey = 40,
        GPS = 41,
        GPSGrey = 42,
        Home=43,

        Location0 = 44,
        Location4 = 45,
        Location5 = 46,
        Location6 = 47,
        
    }
}
