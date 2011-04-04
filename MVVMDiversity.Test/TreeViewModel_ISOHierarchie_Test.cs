using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializable;
using Moq;
using MVVMDiversity.Interface;
using Xunit;
using MVVMDiversity.ViewModel;

namespace MVVMDiversity.Test
{
    public class TreeViewModel_ISOHierarchie_Test
    {
        private Mock<ISerializableObject> rootMock, childMock, gc1Mock,gc2Mock;
        private const string rootName = "Root", childName = "Child", gcName = "GrandChild";
        private Mock<IISOViewModel> rootVMMock,childVMMock,gc1VMMock,gc2VMMock;
        private Mock<IISOViewModelStore> storeMock;

        private TreeViewModel tree;
        private INodeViewModel root, child, grand;

        public TreeViewModel_ISOHierarchie_Test()
        {
            rootMock = new Mock<ISerializableObject>();
            childMock = new Mock<ISerializableObject>();
            var childGUID = Guid.NewGuid();
            childMock.Setup(c => c.Rowguid).Returns(childGUID);

            gc1Mock = new Mock<ISerializableObject>();
            gc2Mock = new Mock<ISerializableObject>();

            rootVMMock = new Mock<IISOViewModel>();
            rootVMMock.Setup(v => v.Parent).Returns(() => null);
            rootVMMock.Setup(v => v.ISO).Returns(rootMock.Object);
            rootVMMock.Setup(v => v.Name).Returns(rootName);
            rootVMMock.Setup(v => v.Rowguid).Returns(Guid.NewGuid());

            
            
            childVMMock = new Mock<IISOViewModel>();
            childVMMock.Setup(v => v.Parent).Returns(rootMock.Object);
            childVMMock.Setup(v => v.Children).Returns(enumerableChildren());
            childVMMock.Setup(v => v.ISO).Returns(childMock.Object);
            childVMMock.Setup(v => v.Name).Returns(childName);
            childVMMock.Setup(v => v.Rowguid).Returns(childGUID);


            gc1VMMock = new Mock<IISOViewModel>();
            gc1VMMock.Setup(v => v.Parent).Returns(childMock.Object);
            gc1VMMock.Setup(v => v.Children).Returns(Enumerable.Empty<ISerializableObject>());
            gc1VMMock.Setup(v => v.Name).Returns(gcName);
            gc1VMMock.Setup(v => v.ISO).Returns(gc1Mock.Object);
            gc1VMMock.Setup(v => v.Rowguid).Returns(Guid.NewGuid());

            gc2VMMock = new Mock<IISOViewModel>();
            gc2VMMock.Setup(v => v.Parent).Returns(childMock.Object);
            gc2VMMock.Setup(v => v.Children).Returns(Enumerable.Empty<ISerializableObject>());
            gc2VMMock.Setup(v => v.ISO).Returns(gc2Mock.Object);
            gc2VMMock.Setup(v => v.Name).Returns(gcName);
            gc2VMMock.Setup(v => v.Rowguid).Returns(Guid.NewGuid());

            storeMock = new Mock<IISOViewModelStore>();
            storeMock.Setup(s => s.addOrRetrieveVMForISO(rootMock.Object)).Returns(rootVMMock.Object);
            storeMock.Setup(s => s.addOrRetrieveVMForISO(childMock.Object)).Returns(childVMMock.Object);
            storeMock.Setup(s => s.addOrRetrieveVMForISO(gc1Mock.Object)).Returns(gc1VMMock.Object);
            storeMock.Setup(s => s.addOrRetrieveVMForISO(gc2Mock.Object)).Returns(gc2VMMock.Object);

            tree = new TreeViewModel(storeMock.Object);
            tree.addGenerator(childVMMock.Object);
            root = tree.Roots.First();
            child = root.Children.First();
        }

        private IEnumerable<ISerializableObject> enumerableChildren()
        {
            return new List<ISerializableObject>() { gc1Mock.Object, gc2Mock.Object };
        }

        [Fact]
        public void root_VM_should_be_in_roots()
        {
            Assert.Equal(1, tree.Roots.Count());
            
            Assert.Equal(rootName, root.Name);
        }

        [Fact]
        public void child_should_be_its_only_child()
        {            
            Assert.Equal(1, root.Children.Count());            
            
            Assert.Equal(childName, child.Name);
        }

        [Fact]
        public void children_of_root_shouldnt_be_evaluated()
        {
            rootVMMock.Verify(v => v.Children, Times.Never());
        }

        [Fact]
        public void children_of_generator_should_be_evaluated_when_expanded()
        {
            childVMMock.Verify(v => v.Children, Times.Never());

            child.IsExpanded = true;

            childVMMock.Verify(v => v.Children, Times.AtLeastOnce());

            Assert.Equal(2, child.Children.Count());
            grand = child.Children.First();

            Assert.Equal(gcName, grand.Name);
        }

        [Fact]
        public void selection_should_contain_all_ISOs()
        {
            var expected = new List<ISerializableObject> () { rootMock.Object, childMock.Object, gc1Mock.Object, gc2Mock.Object};
            var actual = tree.buildSelection();

            foreach (var exp in expected)
                Assert.Contains(exp,actual);
        }

        [Fact]
        public void removing_the_generator_should_empty_the_selection()
        {
            tree.removeGenerator(childVMMock.Object);

            Assert.Empty(tree.Roots);

            Assert.Empty(tree.buildSelection());
        }

        [Fact]
        public void setting_truncate_should_only_clear_the_children()
        {
            child.IsExpanded = true;
            tree.TruncateDataItems = true;
            Assert.Equal(1, tree.Roots.Count());
            Assert.Contains(root, tree.Roots);

            Assert.Equal(1, root.Children.Count());
            Assert.Contains(child, root.Children);

            Assert.Empty(child.Children);
        }

        [Fact]
        public void setting_truncate_should_truncate_selection()
        {
            child.IsExpanded = true;
            tree.TruncateDataItems = true;
            var sel = tree.buildSelection();

            Assert.Equal(2, sel.Count());
            Assert.Contains(rootMock.Object, sel);
            Assert.Contains(childMock.Object, sel);
        }

        [Fact]
        public void resetting_truncate_should_restore_expanded_children()
        {
            child.IsExpanded = true;
            tree.TruncateDataItems = true;
            tree.TruncateDataItems = false;

            Assert.Equal(1, tree.Roots.Count());
            Assert.Contains(root, tree.Roots);

            Assert.Equal(1, root.Children.Count());
            Assert.Contains(child, root.Children);

            Assert.NotEmpty(child.Children);
        }

        
        
    }
}
