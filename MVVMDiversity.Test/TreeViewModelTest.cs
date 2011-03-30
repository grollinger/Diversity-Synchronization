using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.ViewModel;
using Xunit;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using Moq;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using MVVMDiversity.Interface;

namespace MVVMDiversity.Test
{
    public class TreeViewModelTest
    {
        private const string _rootISOName = "RootISO";

        public TreeViewModelTest()
        {        
            //Mock ISO
            rootISOMock = new Mock<ISerializableObject>();
            rootISOMock.Setup(o=>o.Rowguid).Returns(Guid.NewGuid());
            _rootISO = rootISOMock.Object;

            //VM returned for the above ISO
            rootVMMock = new Mock<IISOViewModel>();            
            rootVMMock.Setup(o => o.Parent).Returns(()=>null);
            rootVMMock.Setup(o => o.Name).Returns(_rootISOName).Verifiable();
            rootVMMock.Setup(o => o.Icon).Returns(ISOIcon.Agent).Verifiable();
            rootVMMock.Setup(o => o.Rowguid).Returns(_rootISO.Rowguid);            

            storeMock = new Mock<IISOViewModelStore>();
            storeMock.Setup(s => s.addOrRetrieveVMForISO(_rootISO)).Returns(rootVMMock.Object).Verifiable();
            _store = storeMock.Object;

             _tree = new TreeViewModel(_store);
        }

        private TreeViewModel _tree;

        private ISerializableObject _rootISO;

        private IISOViewModelStore _store;

        private IISOViewModel _childVM;

        private Mock<ISerializableObject> rootISOMock;
        private Mock<IISOViewModel> rootVMMock;
        private Mock<IISOViewModelStore> storeMock;

        [Fact]
        public void new_tree_should_have_no_roots()
        {
            Assert.Empty(_tree.Roots);
        }

        [Fact]
        public void should_contain_added_root()
        {
            _tree.addGenerator(rootVMMock.Object);

            Assert.NotEmpty(_tree.Roots);
            var firstRoot = _tree.Roots.First();
            Assert.Equal(ISOIcon.Agent, firstRoot.Icon);
            Assert.Equal(_rootISOName, firstRoot.Name);           

            
        }

       

    }
}
