using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WF.Player.Providers
{
    /// <summary>
    /// A source of CartridgeTags.
    /// </summary>
    public interface IProvider : IEnumerable, IEnumerable<CartridgeTag>
    {
        
    }
}
