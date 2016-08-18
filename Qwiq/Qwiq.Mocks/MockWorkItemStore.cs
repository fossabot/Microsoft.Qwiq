using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Mocks
{
    public class MockWorkItemStore : IWorkItemStore
    {
        private readonly IEnumerable<IWorkItemLinkInfo> _links;

        private readonly IList<IWorkItem> _workItems;

        private readonly IDictionary<int, IWorkItem> _lookup;

        public MockWorkItemStore()
            : this(new MockTfsTeamProjectCollection(), new MockProject())
        {
        }

        public MockWorkItemStore(ITfsTeamProjectCollection teamProjectCollection, IProject project)
        {
            TeamProjectCollection = teamProjectCollection;
            Projects = new[] { project };

            _workItems = new List<IWorkItem>();

            foreach (var wit in project.WorkItemTypes)
            {
                var wi = new MockWorkItem { Id = _workItems.Count + 1, Type = wit };
                _workItems.Add(wi);
            }

            _lookup = _workItems.ToDictionary(k => k.Id, e => e);
        }

        public MockWorkItemStore(IEnumerable<IWorkItem> workItems)
            : this()
        {
            _workItems = new List<IWorkItem>(workItems);
            _lookup = _workItems.ToDictionary(k => k.Id, e => e);
        }

        public MockWorkItemStore(IEnumerable<IWorkItem> workItems, IEnumerable<IWorkItemLinkInfo> links )
            :this(workItems)
        {
            _links = links;
        }

        public MockWorkItemStore(ITfsTeamProjectCollection teamProjectCollection, IEnumerable<IWorkItem> workItems)
            : this(teamProjectCollection, new MockProject())
        {
            _workItems = new List<IWorkItem>(workItems);
            _lookup = _workItems.ToDictionary(k => k.Id, e => e);
        }

        public IEnumerable<IProject> Projects { get; set; }

        public ITfsTeamProjectCollection TeamProjectCollection { get; set; }

        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes { get { return CoreLinkTypeReferenceNames.All.Select(s => new MockWorkItemLinkType(s)); } }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            if (_workItems == null) throw new InvalidOperationException("Class must be initialized with set of IWorkItems.");

            Trace.TraceInformation("Querying for work items " + wiql);

            return _workItems;
        }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            if (_workItems == null) throw new InvalidOperationException("Class must be initialized with set of IWorkItems.");

            var h = new HashSet<int>(ids);

            Trace.TraceInformation("Querying for IDs " + string.Join(", ", h));

            return _lookup
                    .Where(p => h.Contains(p.Key))
                    .Select(p => p.Value)
                    .ToList();
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            return Query(new[] { id }).SingleOrDefault();
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            Trace.TraceInformation("Querying for links " + wiql);

            return _links;
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}