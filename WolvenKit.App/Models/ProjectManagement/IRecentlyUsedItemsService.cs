using System.Collections.Generic;
using DynamicData;

namespace WolvenKit.App.Models.ProjectManagement;

public interface IRecentlyUsedItemsService
{
    public IObservableCache<RecentlyUsedItemModel, string> Items { get; }

    public List<RecentlyUsedItemModel> PinnedItems { get; }

    void AddItem(RecentlyUsedItemModel recentlyUsedItemModel);
    void RemoveItem(RecentlyUsedItemModel itemModel);
    void PinItem(string key);
    void UnpinItem(string key);
}
