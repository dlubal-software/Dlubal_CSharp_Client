using Dlubal.WS.Common.Dialogs;
using System.Collections.Generic;
using System.Reflection;

namespace Dlubal.WS.Clients.DotNetClientTest.Dialogs
{
    public class TreeItemMethodInfo : CustomTreeItem<MethodInfo>
    {
        public TreeItemMethodInfo(string name) : base(name)
        {
        }

        internal override void GetContentOfCheckedItems(List<MethodInfo> itemContentList)
        {
            if (IsChecked == false)
            {
                return;
            }

            if (Items == null)
            {
                if (Content != null)
                {
                    itemContentList.Add(Content);
                }

                return;
            }

            foreach (var item in Items)
            {
                item.GetContentOfCheckedItems(itemContentList);
            }
        }
    }
}