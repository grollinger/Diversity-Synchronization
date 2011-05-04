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
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;

namespace UBT.AI4.Bio.DivMobi.ListSynchronization
{
    //Diese KLasse entält die zu synchronisierenden KLssen in geordeter Reihenfolge. Abhängige Klasse haben stets höhere Indizes, als die KLassen von denen sie abhängen.
    //Zunächst werden spezilisierte Listen für Upload und Download vorgegeben. Im weiteren Projektverlauf sollen diese durch einen generischen Listengenerator ersetzt werden.
    public static class LookupSynchronizationInformation
    {
        

     

        public static List<Type> downloadDefinitionsList()
        {
            //Diese Liste enthält alle KLassen, die beim Download der definierenden KLassen auf das Mobiltelefon benötigt werden.

            List<Type> classes = new List<Type>();
            classes.Add(typeof(Analysis));
            classes.Add(typeof(AnalysisResult));
            classes.Add(typeof(Property));
            classes.Add(typeof(CollTaxonomicGroup_Enum));
            classes.Add(typeof(LocalisationSystem));
            classes.Add(typeof(CollCircumstances_Enum));
            classes.Add(typeof(CollUnitRelationType_Enum));
            classes.Add(typeof(CollIdentificationCategory_Enum)); //evtl. durch default "determination" ersetzen
            classes.Add(typeof(CollIdentificationQualifier_Enum)); //Muss bei Synchronisation auskommentiert werden, wegen leeren Eintrag als Schlüssel
            classes.Add(typeof(AnalysisTaxonomicGroup));
            classes.Add(typeof(CollEventImageType_Enum));//durch defaultwerte ersetzbar
            classes.Add(typeof(CollSpecimenImageType_Enum));//durch defaultwerte ersetzbar
            //Todo ColleventaeriesImageType->Konsistenz
            return classes;
        }

        public static List<Type> getFieldDataList()
        {       
            //Gibt an welche Klassen beim Download aus DiversityCollection beachtet werden müssen.
            List<Type> classes = new List<Type>();

            //Definierende KLassen übernehmen
            classes.Add(typeof(Analysis));
            classes.Add(typeof(AnalysisResult));
            classes.Add(typeof(Property));
            classes.Add(typeof(CollTaxonomicGroup_Enum));
            classes.Add(typeof(LocalisationSystem));
            classes.Add(typeof(CollCircumstances_Enum));
            classes.Add(typeof(CollUnitRelationType_Enum));
            classes.Add(typeof(CollIdentificationCategory_Enum)); //evtl. durch default "determination" ersetzen
            classes.Add(typeof(CollIdentificationQualifier_Enum)); //Muss bei Synchronisation auskommentiert werden, wegen leeren Eintrag als Schlüssel
            classes.Add(typeof(AnalysisTaxonomicGroup));
            //Todo AnalysisResult
            classes.Add(typeof(CollEventImageType_Enum));//durch defaultwerte ersetzbar
            classes.Add(typeof(CollSpecimenImageType_Enum));//durch defaultwerte ersetzbar
            //Todo ColleventseriesImageType->Konsistenz

            classes.Add(typeof(CollectionEventSeries));
            classes.Add(typeof(CollectionEvent));
            classes.Add(typeof(CollectionEventLocalisation));
            classes.Add(typeof(CollectionEventProperty));
            classes.Add(typeof(CollectionSpecimen));
            classes.Add(typeof(CollectionProject));
            classes.Add(typeof(CollectionAgent));
            classes.Add(typeof(IdentificationUnit));
            classes.Add(typeof(Identification));
            classes.Add(typeof(IdentificationUnitAnalysis));
            //Todo EventSeriesImage?
            classes.Add(typeof(CollectionEventImage));
            classes.Add(typeof(CollectionSpecimenImage));
            classes.Add(typeof(IdentificationUnitGeoAnalysis));
          
            return classes;
        }

        public static List<Type> getImageClasses()
        {
            List<Type> classes = new List<Type>();
            //Todo EventSeriesImage?
            classes.Add(typeof(CollectionEventImage));
            classes.Add(typeof(CollectionSpecimenImage));
            return classes;
        }

        public static List<Type> buildHierarchy(List<Type> unordered)
        {
            //Todo Implementierung: Soll Eine Abähigkeitshierrchy direkt aus Analyse der DataItems erstellen.
            List<Type> ordered = new List<Type>();
            return ordered;
        }

        public static Dictionary<Type,String> getReflexiveReferences()
        {
            //Gibt an welche Klassen sich selbst referenzieren und in welchen Feldern der Fremdschlüssel gespeichert ist.
            //Es muss auch eine Variante geben, die sich das aus den DataItems aufbaut.
            Dictionary<Type,String> reflex =new Dictionary<Type,String>();
            reflex.Add(typeof(Analysis),"_AnalysisParentID");
            reflex.Add(typeof(CollCircumstances_Enum), "_ParentCode");
            //CollectionEventSeries, soll von DivMobi nicht reflexiv unterstützt werden.
            reflex.Add(typeof(CollTaxonomicGroup_Enum),"_ParentCode");
            reflex.Add(typeof(CollUnitRelationType_Enum), "_ParentCode");
            reflex.Add(typeof(IdentificationUnit), "_RelatedUnitID");
            reflex.Add(typeof(LocalisationSystem), "_LocalisationSystemParentID");
            reflex.Add(typeof(Property), "_PropertyParentID");
            reflex.Add(typeof(CollSpecimenImageType_Enum), "_ParentCode");
            reflex.Add(typeof(CollEventImageType_Enum), "_ParentCode");
            return reflex;
        }

        public static Dictionary<Type, String> getReflexiveIDFields()
        {
            //Gibt an in welchem Feld die ID der refernzierten Klasse steht.
            Dictionary<Type, String> id = new Dictionary<Type, String>();
            id.Add(typeof(Analysis), "_AnalysisID");
            id.Add(typeof(CollCircumstances_Enum), "_Code");
            id.Add(typeof(CollTaxonomicGroup_Enum), "_Code");
            id.Add(typeof(CollUnitRelationType_Enum), "_Code");
            id.Add(typeof(IdentificationUnit), "_IdentificationUnitID");
            id.Add(typeof(LocalisationSystem), "_LocalisationSystemID");
            id.Add(typeof(Property), "_PropertyID");
            id.Add(typeof(CollSpecimenImageType_Enum), "_Code");
            id.Add(typeof(CollEventImageType_Enum), "_Code");
            return id;
        }

        public static Dictionary<Type, String> getAutoIncFields()
        {
            //gibt an welche KLassen ein AutoinCfeld haben und wie es heißt.
            Dictionary<Type, String> autoinc = new Dictionary<Type, String>();
            autoinc.Add(typeof(Analysis), "_AnalysisID");
            autoinc.Add(typeof(CollectionEvent), "_CollectionEventID");
            autoinc.Add(typeof(CollectionEventSeries), "_SeriesID");
            autoinc.Add(typeof(CollectionSpecimen), "_CollectionSpecimenID");
            autoinc.Add(typeof(IdentificationUnit), "_IdentificationUnitID");
            return autoinc;
        }

        public static Dictionary<String,Type> getDeterminingFields(Type t)
        {
            //Gibt die Felder der Fremdschlüssel bei der Synchronisatoin an, die aufgrund unterschiedlicher Zählweisen in Repository und Mobilgerät angepasst werden müssen.
            Dictionary<String, Type> determiningFields = new Dictionary<String,Type>();
            
            if(t.Equals(typeof(Analysis)))
            {
                determiningFields.Add("_AnalysisParentID",typeof(Analysis));
            }

            if (t.Equals(typeof(AnalysisResult)))
            {
                determiningFields.Add("_AnalysisID", typeof(Analysis));
            }

            if (t.Equals(typeof(AnalysisTaxonomicGroup)))
            {
                determiningFields.Add("_AnalysisID", typeof(Analysis));
            }
            if (t.Equals(typeof(CollectionAgent)))
            {
                determiningFields.Add( "_CollectionSpecimenID",typeof(CollectionSpecimen));
            }

            if (t.Equals(typeof(CollectionEvent)))
            {
                determiningFields.Add("_SeriesID", typeof(CollectionEventSeries));
            }

            if (t.Equals(typeof(CollectionEventImage)))
            {
                determiningFields.Add("_CollectionEventID", typeof(CollectionEvent));
            }
            if (t.Equals(typeof(CollectionEventLocalisation)))
            {
                determiningFields.Add("_CollectionEventID", typeof(CollectionEvent));
            }
            if (t.Equals(typeof(CollectionEventProperty)))
            {
                determiningFields.Add("_CollectionEventID", typeof(CollectionEvent));
            }

            if (t.Equals(typeof(CollectionProject)))
            {
                determiningFields.Add("_CollectionSpecimenID", typeof(CollectionSpecimen));
            }
            if (t.Equals(typeof(CollectionSpecimen)))
            {
                determiningFields.Add("_CollectionEventID", typeof(CollectionEvent));
            }

            if (t.Equals(typeof(CollectionSpecimenImage)))
            {
                determiningFields.Add("_CollectionSpecimenID", typeof(CollectionSpecimen));
                determiningFields.Add("_IdentificationUnitID", typeof(IdentificationUnit));
            }
            if (t.Equals(typeof(Identification)))
            {
                determiningFields.Add("_CollectionSpecimenID", typeof(CollectionSpecimen));
                determiningFields.Add("_IdentificationUnitID", typeof(IdentificationUnit));
            }
            if (t.Equals(typeof(IdentificationUnit)))
            {
                determiningFields.Add("_CollectionSpecimenID", typeof(CollectionSpecimen));
                determiningFields.Add("_RelatedUnitID", typeof(IdentificationUnit));
            }
            if(t.Equals(typeof(IdentificationUnitAnalysis)))
            {
                determiningFields.Add("_IdentificationUnitID", typeof(IdentificationUnit));
                determiningFields.Add("_CollectionSpecimenID", typeof(CollectionSpecimen));
                determiningFields.Add("_AnalysisID", typeof(Analysis));
            }
            if (t.Equals(typeof(IdentificationUnitGeoAnalysis)))
            {
                determiningFields.Add("_IdentificationUnitID", typeof(IdentificationUnit));
                determiningFields.Add("_CollectionSpecimenID", typeof(CollectionSpecimen));
            }
            return determiningFields;
        }

    }
}
