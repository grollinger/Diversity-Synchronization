using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;

namespace MVVMDiversity.ViewModel
{
	public partial class ISOViewModel
	{
        private class AgentVM : ISOViewModel
        {
            public AgentVM(CollectionAgent a)
                : base (a)
            {

            }

            private CollectionAgent Agent { get { return ISO as CollectionAgent; } }

            public override ISerializableObject Parent
            {
                get { return null; }
            }

            public override IEnumerable<ISerializableObject> Properties
            {
                get { return null; }
            }

            public override IEnumerable<ISerializableObject> Children
            {
                get { return null; }
            }

            protected override string getName()
            {
                if (Agent != null)
                {
                    return Agent.CollectorsName;
                }
                else
                    return "No Agent";

            }

            protected override Enums.ISOIcon getIcon()
            {
                return Enums.ISOIcon.Agent;
            }
        }
	}
}
