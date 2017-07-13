﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localization.Xliff.OM.Core;

namespace XliffLib.Model
{
    public class PropertyGroup : PropertyContainer
    {
        public PropertyGroup(string name): base(name)
        {
            Containers = new List<PropertyContainer>();
        }
        public IList<PropertyContainer> Containers { get; private set; }

        public override TranslationContainer ToXliff(IdCounter counter)
        {
			var id = "g" + (counter.GetNextGroupId());
			var xliffGroup = new Group(id)
			{
				Name = this.Name
			};
            foreach (var container in Containers)
            {
				var xliffContainer = container.ToXliff(counter);
				xliffGroup.Containers.Add(xliffContainer);
            }
            return xliffGroup;
        }
    }
}
