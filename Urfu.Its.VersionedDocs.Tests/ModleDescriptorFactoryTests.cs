using System;
using System.Collections.Generic;
using Castle.Core.Internal;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Urfu.Its.VersionedDocs.Core;
using Urfu.Its.VersionedDocs.Documents.Shared.ContentBuilders;
using Urfu.Its.VersionedDocs.Services;

namespace Urfu.Its.VersionedDocs.Tests
{
    [TestClass]
    public class ModleDescriptorFactoryTests
    {
        [TestMethod]
        public void ShouldBuildValidDescriptorFromTypeWithPrimitives()
        {
            var factory = new AllPropertiesAreBlocksDescriptorFactory<PrimitivesTestModel>();

            var actualDescriptor = factory.CreateDocumentDescriptor();

            var expectedDescriptor = new VersionedDocumentDescriptor
            {
                Blocks =
                {
                    new VersionedDocumentBlockDescriptor(nameof(PrimitivesTestModel.StringProp), VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null),
                    new VersionedDocumentBlockDescriptor(nameof(PrimitivesTestModel.IntegerProp), VersionedDocumentBlockItemKind.Integer),
                    new VersionedDocumentBlockDescriptor(nameof(PrimitivesTestModel.NullableIntegerProp), VersionedDocumentBlockItemKind.Integer | VersionedDocumentBlockItemKind.Null),
                    new VersionedDocumentBlockDescriptor(nameof(PrimitivesTestModel.NumberProp), VersionedDocumentBlockItemKind.Number),
                    new VersionedDocumentBlockDescriptor(nameof(PrimitivesTestModel.NullableNumberProp), VersionedDocumentBlockItemKind.Number | VersionedDocumentBlockItemKind.Null),
                    new VersionedDocumentBlockDescriptor(nameof(PrimitivesTestModel.BooleanProp), VersionedDocumentBlockItemKind.Boolean),
                    new VersionedDocumentBlockDescriptor(nameof(PrimitivesTestModel.NullableBooleanProp), VersionedDocumentBlockItemKind.Boolean | VersionedDocumentBlockItemKind.Null)
                }                
            };

            foreach (var block in expectedDescriptor.Blocks) block.DefaultContentBuilderType = factory.DefaultContentBuilderType;
            //    expectedDescriptor.Blocks.
            //    ForEach(d=>d.DefaultContentBuilderType = factory.DefaultContentBuilderType);

            actualDescriptor.ShouldBeEquivalentTo(expectedDescriptor);
        }

        [TestMethod]
        public void ShouldBuildValidDescriptorFromTypeWithArrayPropertiesOfPrimitives()
        {
            var factory = new AllPropertiesAreBlocksDescriptorFactory<ArrayPropertiesWithPrimitivesTestModel>();
            
            var actualDescriptor = factory.CreateDocumentDescriptor();

            var expectedDescriptor = new VersionedDocumentDescriptor
            {
                Blocks =
                {                    
                    new VersionedDocumentBlockDescriptor(nameof(ArrayPropertiesWithPrimitivesTestModel.StringArray), new VersionedDocumentBlockItemDescriptor(VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null)),
                    new VersionedDocumentBlockDescriptor(nameof(ArrayPropertiesWithPrimitivesTestModel.IntegerArray), new VersionedDocumentBlockItemDescriptor(VersionedDocumentBlockItemKind.Integer)),
                    new VersionedDocumentBlockDescriptor(nameof(ArrayPropertiesWithPrimitivesTestModel.NullableIntegerArray), new VersionedDocumentBlockItemDescriptor(VersionedDocumentBlockItemKind.Integer | VersionedDocumentBlockItemKind.Null)),
                    new VersionedDocumentBlockDescriptor(nameof(ArrayPropertiesWithPrimitivesTestModel.NumberArray), new VersionedDocumentBlockItemDescriptor(VersionedDocumentBlockItemKind.Number)),
                    new VersionedDocumentBlockDescriptor(nameof(ArrayPropertiesWithPrimitivesTestModel.NullableNumberArray), new VersionedDocumentBlockItemDescriptor(VersionedDocumentBlockItemKind.Number | VersionedDocumentBlockItemKind.Null)),
                    new VersionedDocumentBlockDescriptor(nameof(ArrayPropertiesWithPrimitivesTestModel.BooleanArray), new VersionedDocumentBlockItemDescriptor(VersionedDocumentBlockItemKind.Boolean)),
                    new VersionedDocumentBlockDescriptor(nameof(ArrayPropertiesWithPrimitivesTestModel.NullableBooleanArray), new VersionedDocumentBlockItemDescriptor(VersionedDocumentBlockItemKind.Boolean | VersionedDocumentBlockItemKind.Null))
                }
            };

            actualDescriptor.ShouldBeEquivalentTo(expectedDescriptor, o=>o.Excluding(s=>s.SelectedMemberInfo.Name == nameof(IVersionedDocumentBlockItemDescriptor.DefaultContentBuilderType)));
        }

        [TestMethod]
        public void ShouldBuildValidDescriptorFromTypeWithArrayPropertiesOfObjects()
        {
            var factory = new AllPropertiesAreBlocksDescriptorFactory<ArrayPropertiesOfObjectsTestModel>();
            
            var actualDescriptor = factory.CreateDocumentDescriptor();

            var expectedDescriptor = new VersionedDocumentDescriptor(
                new[] {
                    new VersionedDocumentBlockDescriptor(nameof(ArrayPropertiesOfObjectsTestModel.ObjectArray), 
                        new VersionedDocumentBlockItemDescriptor(new [] {
                            new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.StringProp), VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null),
                            new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.IntegerProp), VersionedDocumentBlockItemKind.Integer),
                            new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.NullableIntegerProp), VersionedDocumentBlockItemKind.Integer | VersionedDocumentBlockItemKind.Null),
                            new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.NumberProp), VersionedDocumentBlockItemKind.Number),
                            new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.NullableNumberProp), VersionedDocumentBlockItemKind.Number | VersionedDocumentBlockItemKind.Null),
                            new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.BooleanProp), VersionedDocumentBlockItemKind.Boolean),
                            new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.NullableBooleanProp), VersionedDocumentBlockItemKind.Boolean | VersionedDocumentBlockItemKind.Null)
                        }))
                });

            actualDescriptor.ShouldBeEquivalentTo(expectedDescriptor, o => o.Excluding(s => s.SelectedMemberInfo.Name == nameof(IVersionedDocumentBlockItemDescriptor.DefaultContentBuilderType)));            
        }

        [TestMethod]
        public void ShouldBuildValidDescriptorFromTypeWithObjectProperty()
        {
            var factory = new AllPropertiesAreBlocksDescriptorFactory<ObjectPropertiesTestModel>();
            
            var actualDescriptor = factory.CreateDocumentDescriptor();

            var expectedDescriptor = new VersionedDocumentDescriptor(new []
            {
                new VersionedDocumentBlockDescriptor(nameof(ObjectPropertiesTestModel.ObjectProp), new []
                {
                    new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.StringProp), VersionedDocumentBlockItemKind.String | VersionedDocumentBlockItemKind.Null),
                    new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.IntegerProp), VersionedDocumentBlockItemKind.Integer),
                    new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.NullableIntegerProp), VersionedDocumentBlockItemKind.Integer | VersionedDocumentBlockItemKind.Null),
                    new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.NumberProp), VersionedDocumentBlockItemKind.Number),
                    new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.NullableNumberProp), VersionedDocumentBlockItemKind.Number | VersionedDocumentBlockItemKind.Null),
                    new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.BooleanProp), VersionedDocumentBlockItemKind.Boolean),
                    new VersionedDocumentBlockItemDescriptor(nameof(PrimitivesTestModel.NullableBooleanProp), VersionedDocumentBlockItemKind.Boolean | VersionedDocumentBlockItemKind.Null)
                })
                {
                    DefaultContentBuilderType = factory.DefaultContentBuilderType
                }
            });

            actualDescriptor.ShouldBeEquivalentTo(expectedDescriptor, o => o.Excluding(s => s.SelectedMemberInfo.Name == nameof(IVersionedDocumentBlockItemDescriptor.DefaultContentBuilderType)));
        }
    }

    class ObjectPropertiesTestModel
    {
        public PrimitivesTestModel ObjectProp { get; set; }
    }

    class ArrayPropertiesOfObjectsTestModel
    {
        public ICollection<PrimitivesTestModel> ObjectArray { get; set; }
    }

    class ArrayPropertiesWithPrimitivesTestModel
    {
        public ICollection<string> StringArray { get; set; }
        public ICollection<int> IntegerArray { get; set; }
        public ICollection<int?> NullableIntegerArray { get; set; }
        public ICollection<decimal> NumberArray { get; set; }
        public ICollection<decimal?> NullableNumberArray { get; set; }
        public ICollection<bool> BooleanArray { get; set; }
        public ICollection<bool?> NullableBooleanArray { get; set; }
    }

    class PrimitivesTestModel
    {
        public string StringProp { get; set; }
        public int IntegerProp { get; set; }
        public int? NullableIntegerProp { get; set; }
        public decimal NumberProp { get; set; }
        public decimal? NullableNumberProp { get; set; }
        public bool BooleanProp { get; set; }
        public bool? NullableBooleanProp { get; set; }        
    }
}