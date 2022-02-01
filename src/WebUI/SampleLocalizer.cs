using Syncfusion.Blazor;

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
            return WASM.Client.Resources.SfResources.ResourceManager;
        }
    }
}
