
namespace Bska.Client.UI.Helper
{
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.ViewModels;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class RecursiveCallHelper
    {
        public string GetHirecharyDesignNodeIds(EmployeeDesign item)
        {
            string _nodeIds = "";

            if (item.ParentNode != null)
            {
                _nodeIds += this.GetHirecharyDesignNodeIds(item.ParentNode);
                _nodeIds += ",";
            }

            _nodeIds += item.BuidldingDesignId;

            return _nodeIds;
        }

        public StuffTreeViewModel GetStuffLastTreeParent(StuffTreeViewModel parent)
        {
            StuffTreeViewModel currentParent = null;

            if (parent.Parent != null)
            {
                currentParent = this.GetStuffLastTreeParent(parent.Parent);
            }
            else
            {
                currentParent = parent;
            }
            return currentParent;
        }


        public Boolean stuffParentRecovery(Stuff parent,Stuff current)
        {
            bool isChild = false;
            if (parent.Equals(current))
            {
                isChild = true;
            }
            else if (parent.Parent != null)
            {
                isChild = this.stuffParentRecovery(parent.Parent,current);
            }
            return isChild;
        }

        public HashSet<int> getStuffPerfecs(Stuff current,HashSet<int> nos)
        {
            HashSet<int> orgIds = nos;
            if (current.OrganizationPefectStuffs.Count>0)
            {
                current.OrganizationPefectStuffs.ForEach(op =>
                {
                    orgIds.Add(op.BuidldingDesignId);
                });
            }

            if (current.Parent != null)
            {
                this.getStuffPerfecs(current.Parent,orgIds);
            }
            return orgIds;
        }
    }
}
