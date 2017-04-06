using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.Qwiq.Mocks
{
    [Serializable]
    public class MockWorkItem : WorkItem, IWorkItem
    {
        private IFieldCollection _fields;

        internal bool PartialOpenWasCalled;

        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See a constructor that takes IWorkItemType and fields.")]
        public MockWorkItem()
            : this("Mock")
        {
        }

        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See a constructor that takes IWorkItemType and fields.")]
        public MockWorkItem(string workItemType = null)
            : this(workItemType, null)
        {
        }

        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See a constructor that takes IWorkItemType and fields.")]
        public MockWorkItem(IDictionary<string, object> fields)
            : this((string)null, fields)
        {
        }

        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See a constructor that takes IWorkItemType and fields.")]
        public MockWorkItem(string workItemType = null, IDictionary<string, object> fields = null)
            : this(
                new MockWorkItemType(
                    workItemType ?? "Mock",
                    CoreFieldDefinitions.All.Union(
                        fields?.Keys.Select(MockFieldDefinition.Create)
                        ?? Enumerable.Empty<IFieldDefinition>())),
                fields)
        {
        }

        public MockWorkItem(IWorkItemType type, IDictionary<string, object> fields = null)
            : this(
                type,
                fields?.Select(p => new MockField(type.FieldDefinitions[p.Key], p.Value, p.Value))
                ?? Enumerable.Empty<IField>())
        {
        }

        public MockWorkItem(IWorkItemType type, IEnumerable<IField> fields)
            : base(type)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));

            foreach (var field in fields)
            {
                var f = Fields[field.ReferenceName ?? field.Name];
                f.Value = field.Value;
                f.OriginalValue = field.OriginalValue;
            }

            Links = new MockLinkCollection();
            Uri = new Uri($"vstfs:///WorkItemTracking/WorkItem/{Id}");
            Url = Uri.ToString();
            Revisions = Enumerable.Empty<IRevision>();
            ApplyRules();
        }

        public string ReproSteps
        {
            get => GetValue<string>("Repro Steps");
            set
            {
                SetValue("Repro Steps", value);
                SetValue("Microsoft.VSTS.TCM.ReproSteps", value);
            }
        }

        public new DateTime? ChangedDate
        {
            get => base.ChangedDate;
            set => this[CoreFieldRefNames.ChangedDate] = value;
        }

        public sealed override IFieldCollection Fields => _fields
                                                          ?? (_fields = new MockFieldCollection(this, Type.FieldDefinitions));

        public new int Id
        {
            get => base.Id;
            set => this[CoreFieldRefNames.Id] = value;
        }

        public override bool IsDirty
        {
            get
            {
                return Fields.Any(p => p.IsDirty);
            }
        }

        public override string Keywords
        {
            get => GetValue<string>(WorkItemFields.Keywords);
            set => SetValue(WorkItemFields.Keywords, value);
        }

        public new ICollection<ILink> Links { get; set; }

        public new int RelatedLinkCount => Links.OfType<IRelatedLink>().Count();

        public override IEnumerable<IRevision> Revisions { get; }

        public override Uri Uri { get; }

        public override string Url { get; }

        public override void ApplyRules(bool doNotUpdateChangedBy = false)
        {
        }

        public override void Close()
        {
        }

        public override IWorkItem Copy()
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0;

                var newItem = (MockWorkItem)formatter.Deserialize(stream);
                newItem.Id = 0;

                var link = newItem.CreateRelatedLink(this);
                newItem.Links.Add(link);

                newItem.ApplyRules();

                return newItem;
            }
        }

        public override bool IsValid()
        {
            return Validate() == null;
        }

        public override void Open()
        {
        }

        public override void PartialOpen()
        {
            PartialOpenWasCalled = true;
        }

        public override void Reset()
        {
        }

        public override void Save()
        {
        }

        public override void Save(SaveFlags flags)
        {
        }

        public override IEnumerable<IField> Validate()
        {
            var invalidFields = Fields.Where(p => !p.IsValid).Select(p => p).ToArray();
            return invalidFields.Any() ? invalidFields : null;
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future version. See CreateRelatedLink(IWorkItemLinkTypeEnd, IWorkItem) instead.")]
        public IRelatedLink CreateRelatedLink(IWorkItem target)
        {
            return CreateRelatedLink(
                new MockWorkItemStore().WorkItemLinkTypes
                                       .Single(s => s.ReferenceName == CoreLinkTypeReferenceNames.Related)
                                       .ForwardEnd,
                target);
        }

        protected override object GetValue(string field)
        {
            return Fields[field].Value;
        }

        protected override void SetValue(string field, object value)
        {
            Fields[field].Value = value;
        }
    }
}