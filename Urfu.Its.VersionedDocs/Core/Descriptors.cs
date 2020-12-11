using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Urfu.Its.VersionedDocs.Core
{
    public class VersionedDocumentDescriptor : IVersionedDocumentBlockEnumerableItem
    {
        public VersionedDocumentDescriptor(IEnumerable<VersionedDocumentBlockDescriptor> blocks) : this()
        {
            foreach (var block in blocks)
            {
                Blocks.Add(block);
            }
        }

        public VersionedDocumentDescriptor()
        {
            Blocks = new VersionedDocumentItemCollection<VersionedDocumentBlockDescriptor>(this);
        }

        public ICollection<VersionedDocumentBlockDescriptor> Blocks { get; }

        public IEnumerable<IVersionedDocumentBlockItemDescriptor> EnumerateItems()
        {
            foreach (var blockDescriptor in Blocks)
            {
                yield return blockDescriptor;
            }
        }
    }

    public class VersionedDocumentSectionDescriptor
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }        
    }

    public class VersionedDocumentBlockDescriptor : VersionedDocumentBlockItemDescriptorBase
    {
        public VersionedDocumentBlockDescriptor(string name, VersionedDocumentBlockItemKind kind) : base(name, kind)
        {

        }

        public VersionedDocumentBlockDescriptor(string name, IEnumerable<VersionedDocumentBlockItemDescriptor> properties) : base(name, properties)
        {

        }

        public VersionedDocumentBlockDescriptor(string name, VersionedDocumentBlockItemDescriptor itemDescriptor) : base(name, itemDescriptor)
        {

        }

        [JsonIgnore]
        public Type NonVersionedDataLoaderType { get; set; }

        [JsonIgnore]
        public Type ProcessorType { get; set; }

        public ICollection<string> DependentBlocks { get; set; } = new List<string>();        
    }

    public class VersionedDocumentBlockItemDescriptor : VersionedDocumentBlockItemDescriptorBase
    {
        public VersionedDocumentBlockItemDescriptor(VersionedDocumentBlockItemKind kind) : base(kind)
        {
            
        }

        public VersionedDocumentBlockItemDescriptor(IEnumerable<VersionedDocumentBlockItemDescriptor> properties) : base(properties)
        {
            
        }

        public VersionedDocumentBlockItemDescriptor(string name, IEnumerable<VersionedDocumentBlockItemDescriptor> properties) : base(name, properties)
        {

        }

        public VersionedDocumentBlockItemDescriptor(string name, VersionedDocumentBlockItemKind kind) : base(name, kind)
        {
            
        }

        public VersionedDocumentBlockItemDescriptor(string name, VersionedDocumentBlockItemDescriptor itemDescriptor) : base(name, itemDescriptor)
        {
            
        }
    }

    public abstract class VersionedDocumentBlockItemDescriptorBase : IVersionedDocumentBlockItemDescriptor
    {
        protected VersionedDocumentBlockItemDescriptorBase()
        {
            Properties = new VersionedDocumentItemCollection<VersionedDocumentBlockItemDescriptor>(this);
        }

        protected VersionedDocumentBlockItemDescriptorBase(IEnumerable<VersionedDocumentBlockItemDescriptor> properties) : this()
        {
            Kind = VersionedDocumentBlockItemKind.Object;
            foreach (var property in properties)
                Properties.Add(property);
        }

        protected VersionedDocumentBlockItemDescriptorBase(string name, IEnumerable<VersionedDocumentBlockItemDescriptor> properties) : this(properties)
        {
            Name = name;
        }

        protected VersionedDocumentBlockItemDescriptorBase(string name, VersionedDocumentBlockItemDescriptor itemDescriptor) : this()
        {
            Name = name;
            Kind = VersionedDocumentBlockItemKind.Array;
            Items = itemDescriptor;
        }

        protected VersionedDocumentBlockItemDescriptorBase(string name, VersionedDocumentBlockItemKind kind) : this()
        {
            Kind = kind;
            Name = name;
        }

        protected VersionedDocumentBlockItemDescriptorBase(VersionedDocumentBlockItemKind kind) : this()
        {
            Kind = kind;            
        }

        /// <summary>
        /// Название блока данных. Должно быть уникальным в пределах одного документа.
        /// </summary>
        public string Name { get; set; }

        public VersionedDocumentBlockItemKind Kind { get; set; }

        /// <summary>
        /// Дескрипторы свойств. Используются только если <see cref="Kind"/>.HasFlag(<see cref="VersionedDocumentBlockItemKind.Object"/>)
        /// </summary>
        public ICollection<VersionedDocumentBlockItemDescriptor> Properties { get; }

        /// <summary>
        /// Дескриптор элемента массива. Используется только если <see cref="Kind"/>.HasFlag(<see cref="VersionedDocumentBlockItemKind.Array"/>)
        /// </summary>
        public VersionedDocumentBlockItemDescriptor Items { get; set; }

        /// <summary>
        /// Используется при обновлении схем. Процессор дефолтных данных. typeof(void) - если не нужно использовать процессор.
        /// </summary>
        [JsonIgnore]
        public Type DefaultContentBuilderType { get; set; }

        /// <summary>
        /// Используется при обновлении схем. Отсюда берутся дефолтные данные для новых элементов блока
        /// </summary>
        [JsonIgnore]
        public string DefaultContent { get; set; }

        bool IEquatable<IVersionedDocumentBlockItemDescriptor>.Equals(IVersionedDocumentBlockItemDescriptor other)
        {
            return string.Equals(Name, other.Name) && Kind == other.Kind && Equals(Properties, other.Properties) && Equals(Items, other.Items);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return ((IEquatable<IVersionedDocumentBlockItemDescriptor>)this).Equals((IVersionedDocumentBlockItemDescriptor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Kind;
                hashCode = (hashCode * 397) ^ (Properties != null ? Properties.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Items != null ? Items.GetHashCode() : 0);
                return hashCode;
            }
        }

        public IEnumerable<IVersionedDocumentBlockItemDescriptor> EnumerateItems()
        {
            if (Kind.HasFlag(VersionedDocumentBlockItemKind.Array))
                yield return Items;
            else if(Kind.HasFlag(VersionedDocumentBlockItemKind.Object))
            {
                foreach (var property in Properties)
                    yield return property;
            }            
        }
    }

    public interface IVersionedDocumentBlockItemDescriptor : IEquatable<IVersionedDocumentBlockItemDescriptor>, IVersionedDocumentBlockEnumerableItem
    {
        string Name { get; }
        VersionedDocumentBlockItemKind Kind { get; set; }
        ICollection<VersionedDocumentBlockItemDescriptor> Properties { get; }
        VersionedDocumentBlockItemDescriptor Items { get; set; }

        /// <summary>
        /// Используется при обновлении схем. Отсюда берутся дефолтные данные для новых элементов блока или самих блоков
        /// </summary>
        Type DefaultContentBuilderType { get; set; }

        string DefaultContent { get; set; }
    }

    public interface IVersionedDocumentBlockEnumerableItem
    {
        IEnumerable<IVersionedDocumentBlockItemDescriptor> EnumerateItems();
    }

    /// <summary>
    /// Поддерживаемые движком виды блоков данных и содержимого блоков. Значения взяты из JSchemaType для прямого каста.
    /// </summary>
    [Flags]
    public enum VersionedDocumentBlockItemKind
    {
        String = 1,
        Number = 2,
        Integer = 4,
        Boolean = 8,
        Object = 16,
        Array = 32,
        Null = 64,
    }
}