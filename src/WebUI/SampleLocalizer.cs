using Syncfusion.Blazor;
using WASM.Shared.Localization;

namespace WebUI;

public class SampleLocalizer : ISyncfusionStringLocalizer
{
    public string GetText(string key)
    {
        return this.ResourceManager.GetString(key);
    }

    public System.Resources.ResourceManager ResourceManager
    {
        get
        {
            return ResourceSyncfusion.ResourceManager;
        }
    }
}
